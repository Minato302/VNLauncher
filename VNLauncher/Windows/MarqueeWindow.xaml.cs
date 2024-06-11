#pragma warning disable IDE0049
#pragma warning disable CS8618

using OpenCvSharp.Extensions;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using VNLauncher.FuntionalClasses;

namespace VNLauncher.Windows
{
    public partial class MarqueeWindow : System.Windows.Window
    {
        private IntPtr gameWindowHwnd;
        private Game game;
        private System.Timers.Timer windowMonitoringTimer;
        private System.Timers.Timer tWindowTimer;
        private GlobalHook hook;
        private LocalOCR scanner;
        private TransparentWindow tWindow;
        private FileManager fileManager;


        public MarqueeWindow(IntPtr gameWindowHwnd, Game game)
        {
            InitializeComponent();
            this.gameWindowHwnd = gameWindowHwnd;
            this.game = game;
            fileManager = new FileManager();
            windowMonitoringTimer = new System.Timers.Timer(1000);
            windowMonitoringTimer.Elapsed += OnTimedEvent;
            windowMonitoringTimer.AutoReset = true;
            windowMonitoringTimer.Enabled = true;
            hook = new GlobalHook(new List<(String, Action)>
            {
                ("鼠标左键", WaitAndScan), ("键盘按键←", LeftKeyDown), ("键盘按键→", RightKeyDown), ("键盘按键↑", UpKeyDown),
                ("键盘按键↓", DownKeyDown),("键盘按键Tab",TabKeyDown),("键盘按键R",Retranslate),("键盘按键P", ScreenShot)
            });
            tWindowTimer = new System.Timers.Timer(2000);
            tWindowTimer.Elapsed += (sender, e) =>
            {
                Dispatcher.Invoke(() =>
                {
                    tWindowTimer.Stop();
                    tWindow!.Visibility = Visibility.Hidden;
                });
            };
            windowMonitoringTimer.AutoReset = true;

            scanner = new LocalOCR();

            tWindow = new TransparentWindow(gameWindowHwnd, game.Loaction);
            tWindow.Show();
            tWindow.Visibility = Visibility.Hidden;
            translator = new ChatGPTTranslator();

        }
        void MouseDragMove(Object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MarqueeCloseButton_Click(Object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
        }

        private void TranslateButton_Click(Object sender, RoutedEventArgs e)
        {
            translateButton.Turn();
        }
        private Rectangle gameWindowLocation;
        private void OnTimedEvent(Object? sender, System.Timers.ElapsedEventArgs e)
        {
            WindowInfo? window = WindowsHandler.GetWindow(game.WindowTitle, game.WindowClass);
            if (window == null)
            {
                Dispatcher.Invoke(() =>
                {
                    Topmost = true;
                    game.EndGame();
                    tWindow.Close();
                    windowMonitoringTimer.Stop();
                    Close();

                });
            }
            else
            {
                if (gameWindowLocation != ((WindowInfo)window).Bounds)
                {
                    Dispatcher.Invoke(() =>
                    {
                        tWindow.Visibility = Visibility.Visible;
                        gameWindowLocation = ((WindowInfo)window).Bounds;
                    });
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        tWindow.Visibility = Visibility.Hidden;
                        gameWindowLocation = ((WindowInfo)window).Bounds;
                    });
                }
            }
        }
        private ChatGPTTranslator translator;
        // 在类级别声明一个 CancellationTokenSource 变量
        private CancellationTokenSource tokenSource;

        private async void WaitAndScan()
        {
            if (translateButton.IsTranslating)
            {
                WordBlock? b = null;

                // 取消上一个任务
                tokenSource?.Cancel();

                // 创建新的 CancellationTokenSource
                tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Waiting);
                });

                try
                {
                    await Task.Run(async () =>
                    {
                        while (true)
                        {
                            // 检查是否请求取消任务
                            token.ThrowIfCancellationRequested();

                            Bitmap lastCapture = WindowCapture.Shot(gameWindowHwnd,
                                game.IsWindowShot ? WindowCapture.CaptureMode.Window : WindowCapture.CaptureMode.FullScreenCut,
                                game.Loaction);
                            List<Int32> historySimilarities = new List<Int32>();

                            for (Int32 i = 0; i < 15; i++)
                            {
                                token.ThrowIfCancellationRequested();

                                await Task.Delay(200);
                                Bitmap capture = WindowCapture.Shot(gameWindowHwnd,
                                    game.IsWindowShot ? WindowCapture.CaptureMode.Window : WindowCapture.CaptureMode.FullScreenCut,
                                    game.Loaction);
                                //          Int32 similarity = ImageHandler.GetSimilarity(BitmapConverter.ToMat(lastCapture), BitmapConverter.ToMat(capture));
                                
                                Int32 similarity = ImageHandler.CalculateWhiteIntersections(BitmapConverter.ToMat(capture));

                                Int32 count = 0;

                                foreach (Int32 historySimilarity in historySimilarities)
                                {
                                    if (similarity != 0)
                                    {
                                        if (historySimilarity >= similarity)
                                        {
                                            count++;
                                        }
                                    }
                                }

                                if (count < 2)
                                {
                                    historySimilarities.Add(similarity);
                                    lastCapture = new Bitmap(capture);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Identifying);
                            });

                            List<WordBlock> dialog = scanner.Scan(lastCapture);
                            b = WordBlock.Splicing(dialog);

                            if (b.Words.Length != 0)
                            {
                                break;
                            }
                        }
                    }, token);

                    if (b != null)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            marqueeTextBlock.Text = b.Words;
                            stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Translating);
                            //   Translate(b.Words);
                        });
                    }
                }
                catch (OperationCanceledException)
                {

                }
            }
        }

        private async void Retranslate()
        {
            WordBlock b = null;
            Bitmap capture = WindowCapture.Shot(gameWindowHwnd,
                          game.IsWindowShot ? WindowCapture.CaptureMode.Window : WindowCapture.CaptureMode.FullScreenCut,
                          game.Loaction);
            await Task.Run(async () =>
            {
                List<WordBlock> dialog = scanner.Scan(capture);
                Application.Current.Dispatcher.Invoke(() =>
                 {
                     stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Identifying);
                     b = WordBlock.Splicing(dialog);
                 });
            });
            if (b != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Translating);
                    marqueeTextBlock.Text = b.Words;
                    translator.RemoveLast();
                    Translate(b.Words);
                });
            }

        }
        private void ScreenShot()
        {
            String soundPath = AppDomain.CurrentDomain.BaseDirectory + "Resources\\ScreenshotSound.mp3";
            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.Open(new Uri(soundPath));
            mediaPlayer.Play();

            String time = DateTime.Now.ToString("yyMMddHHmmssffffff");
            Bitmap capture = WindowCapture.Shot(gameWindowHwnd,
                          game.IsWindowShot ? WindowCapture.CaptureMode.Window : WindowCapture.CaptureMode.FullScreenCut,
                          game.Loaction);
            fileManager.SaveCapture(game.Name, capture, time);

        }
        private async void Translate(String jpContent)
        {
            String cnContent = await translator.Translate(jpContent);
            marqueeTextBlock.Text += '\n';
            marqueeTextBlock.Text += cnContent;
            stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Over);
        }
        private void TabKeyDown()
        {
            translateButton.Turn();
            tokenSource?.Cancel();
            if (translateButton.IsTranslating)
            {
                stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Over);
            }
            else
            {
                stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Closed);
            }
        }

        public void LeftKeyDown()
        {
            if (adjustSideButton.Side == Controls.MarqueeAdjustSideButton.AdjustSide.LeftUp)
            {
                game.Loaction.LeftSideLeftMove();
                tWindow.LeftSideLeftMove();
            }
            else
            {
                game.Loaction.RightSideLeftMove();
                tWindow.RightSideLeftMove();
            }
            if (tWindow.Visibility == Visibility.Hidden)
            {
                tWindow.Visibility = Visibility.Visible;
                tWindowTimer.Start();
            }
            else
            {
                tWindowTimer.Stop();
                tWindowTimer.Start();
            }
        }
        public void RightKeyDown()
        {
            if (adjustSideButton.Side == Controls.MarqueeAdjustSideButton.AdjustSide.LeftUp)
            {
                game.Loaction.LeftSideRightMove();
                tWindow.LeftSideRightMove();
            }
            else
            {
                game.Loaction.RightSideRightMove();
                tWindow.RightSideRightMove();
            }
            if (tWindow.Visibility == Visibility.Hidden)
            {
                tWindow.Visibility = Visibility.Visible;
                tWindowTimer.Start();
            }
            else
            {
                tWindowTimer.Stop();
                tWindowTimer.Start();
            }
        }
        public void UpKeyDown()
        {
            if (adjustSideButton.Side == Controls.MarqueeAdjustSideButton.AdjustSide.LeftUp)
            {
                game.Loaction.UpSideUpMove();
                tWindow.UpSideUpMove();
            }
            else
            {
                game.Loaction.DownSideUpMove();
                tWindow.DownSideUpMove();
            }
            if (tWindow.Visibility == Visibility.Hidden)
            {
                tWindow.Visibility = Visibility.Visible;
                tWindowTimer.Start();
            }
            else
            {
                tWindowTimer.Stop();
                tWindowTimer.Start();
            }
        }
        public void DownKeyDown()
        {
            if (adjustSideButton.Side == Controls.MarqueeAdjustSideButton.AdjustSide.LeftUp)
            {
                game.Loaction.UpSideDownMove();
                tWindow.UpSideDownMove();
            }
            else
            {
                game.Loaction.DownSideDownMove();
                tWindow.DownSideDownMove();
            }
            if (tWindow.Visibility == Visibility.Hidden)
            {
                tWindow.Visibility = Visibility.Visible;
                tWindowTimer.Start();
            }
            else
            {
                tWindowTimer.Stop();
                tWindowTimer.Start();
            }
        }

        private void Window_Unloaded(Object sender, RoutedEventArgs e)
        {
            hook.UninstallHook();
        }
    }
}
