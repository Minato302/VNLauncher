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
        private MainWindow mainWindow;


        public MarqueeWindow(IntPtr gameWindowHwnd, Game game, MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
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
            cTranslator = new GPTTranslator();
            lTranslator = new LocalTranslator();

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
        private GPTTranslator cTranslator;
        private LocalTranslator lTranslator;

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

                            Bitmap lastCapture = GetGameShot(game.Loaction);
                            List<Int32> historySimilarities = new List<Int32>();

                            for (Int32 i = 0; i < 15; i++)
                            {
                                token.ThrowIfCancellationRequested();

                                await Task.Delay(200);
                                Bitmap capture = GetGameShot(game.Loaction);
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
                        });
                        Translate(b.Words);
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
            Bitmap capture = GetGameShot(game.Loaction);
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
                    cTranslator.RemoveLast();
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

            String time = DateTime.Now.ToString("yyyyMMddHHmmssffffff");

            Bitmap capture = GetGameShot();
            fileManager.SaveCapture(game.Name, capture, time);
            ScreenShotWindow sWindow = new ScreenShotWindow(ImageHandler.ConvertBitmapToBitmapSource(capture));
            sWindow.Show();
        }
        private Bitmap GetGameShot()
        {
            Bitmap capture;
            if (!game.IsWindowShot)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Visibility = Visibility.Hidden;
                });
                capture = WindowCapture.Shot(gameWindowHwnd, WindowCapture.CaptureMode.FullScreenCut);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Visibility = Visibility.Visible;
                });
            }
            else
            {
                capture = WindowCapture.Shot(gameWindowHwnd, WindowCapture.CaptureMode.Window);
            }
            return capture;
        }
        private Bitmap GetGameShot(Game.CaptionLocation location)
        {
            Bitmap capture;
            if (!game.IsWindowShot)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Visibility = Visibility.Hidden;
                });
                capture = WindowCapture.Shot(gameWindowHwnd, WindowCapture.CaptureMode.FullScreenCut, location);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Visibility = Visibility.Visible;
                });
            }
            else
            {
                capture = WindowCapture.Shot(gameWindowHwnd, WindowCapture.CaptureMode.Window, location);
            }
            return capture;
        }
        private async void Translate(String jpContent)
        {
            String cnContent = await cTranslator.Translate(jpContent);
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
            mainWindow.UpdateSelectedGameInfo();
            hook.UninstallHook();
        }
    }
}
