#pragma warning disable IDE0049

using System.Diagnostics;
using System.Windows;
using VNLauncher.FunctionalClasses;
using VNLauncher.Pages;
using VNLauncher.Windows;

namespace VNLauncher
{
    public partial class StartGameTipsWindow : Window
    {
        private System.Timers.Timer timer;
        private Game game;
        private Process gameProcess = null;
        public Process GameProcess => gameProcess;
        public Game Game => game;
        private MainWindow mainWindow;
        public StartGameTipsWindow(Game game, MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            this.game = game;
            StartGameTipsPage_AutoStart autoStartPage = new StartGameTipsPage_AutoStart(this);
            if (game.IsAutoStart)
            {
                mainFrame.Navigate(autoStartPage);
            }
            else
            {
                mainFrame.Navigate(new StartGameTipsPage_ManualStart(this));
            }
            try
            {
                gameProcess = game.StartGame();
            }
            catch (Exception)
            {
                autoStartPage.mainTipsTextBlock.Text = "系统无法启动游戏，游戏路径可能被更改，请将游戏文件重新移回源路径处或重新加入该游戏";
            }
            timer.Enabled = true;
        }
        private void OnTimedEvent(Object? sender, System.Timers.ElapsedEventArgs e)
        {
            WindowInfo? window = WindowsHandler.GetWindow(game.WindowTitle, game.WindowClass);
            if (window != null)
            {
                Dispatcher.Invoke(() =>
                {
                    timer.Stop();
                    timer.Dispose();
                    MarqueeWindow marquee = new MarqueeWindow(((WindowInfo)window).Hwnd, game, mainWindow);
                    mainWindow.SetStartButton(true);
                    marquee.Show();
                    Close();
                });
            }
        }
    }
}
