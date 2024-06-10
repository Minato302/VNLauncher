#pragma warning disable IDE0049

using System.Reflection.Metadata;
using System.Windows;
using VNLauncher.FuntionalClasses;
using VNLauncher.Pages;
using VNLauncher.Windows;

namespace VNLauncher
{
    public partial class StartGameTipsWindow : Window
    {
        private System.Timers.Timer timer;
        private Game game;
        public StartGameTipsWindow(Game game)
        {
            InitializeComponent();
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            this.game = game;
            if(game.IsAutoStart)
            {
                mainFrame.Navigate(new StartGameTipsPage_AutoStart(this));
            }
            else
            {
                mainFrame.Navigate(new StartGameTipsPage_ManualStart(this));
            }
            game.StartGame();
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
                    MarqueeWindow marquee = new MarqueeWindow(((WindowInfo)window).Hwnd, game);
                    marquee.Show();
                    Close();
                });
            }
        }
    }
}
