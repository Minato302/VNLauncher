#pragma warning disable IDE0049

using OpenCvSharp.Extensions;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using VNLauncher.FuntionalClasses;

namespace VNLauncher.Windows
{
    public partial class MarqueeWindow : Window
    {
        private IntPtr gameWindowHwnd;
        private Game game;
        private System.Timers.Timer windowMonitoringTimer;
        private System.Timers.Timer tWindowTimer;
        private GlobalHook hook;
        private LocalOCR scanner;
        private TransparentWindow tWindow;

        private Thread workingThread;

        public MarqueeWindow(IntPtr gameWindowHwnd, Game game)
        {
            InitializeComponent();
            this.gameWindowHwnd = gameWindowHwnd;
            this.game = game;
            windowMonitoringTimer = new System.Timers.Timer(1000);
            windowMonitoringTimer.Elapsed += OnTimedEvent;
            windowMonitoringTimer.AutoReset = true;
            windowMonitoringTimer.Enabled = true;
            hook = new GlobalHook(new List<(String, Action)>
            {
                ("鼠标左键", WaitAndScan), ("键盘按键←", LeftKeyDown), ("键盘按键→", RightKeyDown), ("键盘按键↑", UpKeyDown),
                ("键盘按键↓", DownKeyDown),("键盘按键Tab",TabKeyDown) });

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
            workingThread = new Thread(() => { });
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
        private CancellationTokenSource tokenSource;
        private ChatGPTTranslator translator;
        private async void WaitAndScan()  
        {
            if (translateButton.IsTranslating)
            {


                tokenSource?.Cancel();
                tokenSource = new CancellationTokenSource();

                WordBlock? b = null;
                CancellationToken token = tokenSource.Token;
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Waiting);
                    });
                    await Task.Run(async () =>
                    {
                        while (true)
                        {
                            if (token.IsCancellationRequested)
                                token.ThrowIfCancellationRequested();

                            Bitmap lastCapture = WindowCapture.Shot(gameWindowHwnd,
                                game.IsWindowShot ? WindowCapture.CaptureMode.Window : WindowCapture.CaptureMode.FullScreenCut,
                                game.Loaction);

                            List<Int32> historySimilarities = new List<Int32>();
                            for (Int32 i = 0; i < 15; i++)
                            {
                                await Task.Delay(300, token);

                                if (token.IsCancellationRequested)
                                {
                                    token.ThrowIfCancellationRequested();
                                }
                                Bitmap capture = WindowCapture.Shot(gameWindowHwnd,
                                    game.IsWindowShot ? WindowCapture.CaptureMode.Window : WindowCapture.CaptureMode.FullScreenCut,
                                    game.Loaction);

                                Int32 similarity = ImageHandler.GetSimilarity(BitmapConverter.ToMat(lastCapture), BitmapConverter.ToMat(capture));

                                Int32 count = 0;
                                foreach (Int32 historySimilarity in historySimilarities)
                                {
                                    if (historySimilarity == similarity)
                                    {
                                        count++;
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
                            stateInfo.ChangeState(Controls.MarqueeStateInfo.State.Translating);
                            marqueeTextBlock.Text = b.Words;
                            Translate(b.Words);
                        });
                    }
                }
                catch (OperationCanceledException)
                {

                }
            }
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
            workingThread.Interrupt();
            if(translateButton.IsTranslating)
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
