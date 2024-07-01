#pragma warning disable IDE0049

using FontAwesome.WPF;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VNLauncher.Controls;
using VNLauncher.FuntionalClasses;

namespace VNLauncher.Windows
{
    public partial class MainWindow : Window
    {
        private FileManager fileManager;
        public MainWindow()
        {
            InitializeComponent();
            fileManager = new FileManager();
        }

        public UIElementCollection GameListButtons
        {
            get
            {
                return gameListStackPanel.Children;
            }
        }
        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            List<(String, String)> gameInfo = fileManager.GetAllGameInfoFromFile();
            foreach ((String, String) game in gameInfo)
            {
                gameListStackPanel.Children.Add(new MainWindowGameButton(game.Item1, this));
            }
        }
        void MouseDragMove(Object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                maximizeButton.TitleBarButtonIcon = FontAwesomeIcon.SquareOutline;
                Top = 0;
            }
            DragMove();
            searchWayPopup.IsOpen = false;
        }

        private void MinimizeButton_Click(Object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            searchWayPopup.IsOpen = false;
        }

        private void MaximizeButton_Click(Object sender, RoutedEventArgs e)
        {
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            if (WindowState == WindowState.Maximized)
            {
                maximizeButton.TitleBarButtonIcon = FontAwesomeIcon.WindowRestore;
            }
            else
            {
                maximizeButton.TitleBarButtonIcon = FontAwesomeIcon.SquareOutline;
            }

            searchWayPopup.IsOpen = false;
        }

        private void CloseButton_Click(Object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SortWayButton_Click(Object sender, RoutedEventArgs e)
        {
            searchWayPopup.IsOpen = true;
        }

        private void Window_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            searchWayPopup.IsOpen = false;
        }

        private void Window_MouseRightButtonDown(Object sender, MouseButtonEventArgs e)
        {
            searchWayPopup.IsOpen = false;
        }

        private void LastOpenedTimeItem_Click(Object sender, RoutedEventArgs e)
        {

        }

        private void LastJoinedTimeItem_Click(Object sender, RoutedEventArgs e)
        {

        }

        private void PlayedTimeItem_Click(Object sender, RoutedEventArgs e)
        {

        }

        private void AddGameButton_Click(Object sender, RoutedEventArgs e)
        {
            AddGameWindow addGameWindow = new AddGameWindow(this);
            addGameWindow.ShowDialog();
        }

        private void GameStartButton_Click(Object sender, RoutedEventArgs e)
        {
            foreach (MainWindowGameButton gameButton in gameListStackPanel.Children)
            {
                if (gameButton.IsSelected)
                {
                    StartGameTipsWindow tipsWindow = new StartGameTipsWindow(gameButton.Game, this);
                    tipsWindow.Show();
                }
            }
        }
        public void UpdateSelectedGameInfo()
        {
            foreach (MainWindowGameButton gameButton in gameListStackPanel.Children)
            {
                if (gameButton.IsSelected)
                {
                    gameButton.UpdateInfo();
                }
            }
        }

        private void SettingButton_Click(Object sender, RoutedEventArgs e)
        {
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.ShowDialog();
        }
    }
}