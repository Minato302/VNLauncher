#pragma warning disable IDE0049

using FontAwesome.WPF;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VNLauncher.Controls;
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Windows
{
    public partial class MainWindow : System.Windows.Window
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

            try
            {
                List<String> names = new List<String>(File.ReadAllLines(fileManager.GamesOrderPath));
                if (names.Count != gameInfo.Count)
                {
                    throw new Exception();
                }
                foreach (String name in names)
                {
                    Boolean hasName = false;
                    foreach ((String, String) info in gameInfo)
                    {
                        if (info.Item1 == name)
                        {
                            hasName = true;
                        }
                    }
                    if (!hasName)
                    {
                        throw new Exception();
                    }
                }
                foreach (String name in names)
                {
                    MainWindowGameButton gameButton = new MainWindowGameButton(name);
                    gameButton.SetSelectedAction(SelectGameButton);
                    gameListStackPanel.Children.Add(gameButton);
                }
            }
            catch (Exception)
            {
                gameListStackPanel.Children.Clear();
                foreach ((String, String) game in gameInfo)
                {
                    MainWindowGameButton gameButton = new MainWindowGameButton(game.Item1);
                    gameButton.SetSelectedAction(SelectGameButton);
                    gameListStackPanel.Children.Add(gameButton);
                }
            }
            if (gameListStackPanel.Children.Count > 0)
            {
                ((MainWindowGameButton)gameListStackPanel.Children[0]).BeingSelected();
            }
            else
            {
                mainRightGird_GameInfo.Visibility = Visibility.Hidden;
                mainRightGrid_Picture.Visibility = Visibility.Hidden;
                noGameTipsCanvas.Visibility = Visibility.Visible;
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
            searchWayPopup.IsOpen = false;
            WindowState = WindowState.Minimized;
            searchWayPopup.IsOpen = false;
        }

        private void MaximizeButton_Click(Object sender, RoutedEventArgs e)
        {
            searchWayPopup.IsOpen = false;
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
            searchWayPopup.IsOpen = false;

            List<MainWindowGameButton> gameButtons = new List<MainWindowGameButton>();
            foreach (MainWindowGameButton button in gameListStackPanel.Children)
            {
                gameButtons.Add(button);
            }
            gameButtons.Sort((MainWindowGameButton b1, MainWindowGameButton b2) =>
            {
                return b2.Game.LastStartTime.CompareTo(b1.Game.LastStartTime);
            });
            gameListStackPanel.Children.Clear();
            foreach (MainWindowGameButton button in gameButtons)
            {
                gameListStackPanel.Children.Add(button);
            }
        }

        private void LastJoinedTimeItem_Click(Object sender, RoutedEventArgs e)
        {
            searchWayPopup.IsOpen = false;

            List<MainWindowGameButton> gameButtons = new List<MainWindowGameButton>();
            foreach (MainWindowGameButton button in gameListStackPanel.Children)
            {
                gameButtons.Add(button);
            }
            gameButtons.Sort((MainWindowGameButton b1, MainWindowGameButton b2) =>
            {
                return b2.Game.JoinTime.CompareTo(b1.Game.JoinTime);
            });
            gameListStackPanel.Children.Clear();
            foreach (MainWindowGameButton button in gameButtons)
            {
                gameListStackPanel.Children.Add(button);
            }
        }

        private void PlayedTimeItem_Click(Object sender, RoutedEventArgs e)
        {
            searchWayPopup.IsOpen = false;

            List<MainWindowGameButton> gameButtons = new List<MainWindowGameButton>();
            foreach (MainWindowGameButton button in gameListStackPanel.Children)
            {
                gameButtons.Add(button);
            }
            gameButtons.Sort((MainWindowGameButton b1, MainWindowGameButton b2) =>
            {
                return b2.Game.PlayTimeMinute.CompareTo(b1.Game.PlayTimeMinute);
            });
            gameListStackPanel.Children.Clear();
            foreach (MainWindowGameButton button in gameButtons)
            {
                gameListStackPanel.Children.Add(button);
            }
        }

        private void AddGameButton_Click(Object sender, RoutedEventArgs e)
        {
            searchWayPopup.IsOpen = false;
            AddGameWindow addGameWindow = new AddGameWindow(this);
            addGameWindow.ShowDialog();
        }

        private void GameStartButton_Click(Object sender, RoutedEventArgs e)
        {
            searchWayPopup.IsOpen = false;
            if (MainWindowGameButton.SelectedGameButton != null)
            {
                StartGameTipsWindow tipsWindow = new StartGameTipsWindow(MainWindowGameButton.SelectedGameButton.Game, this);
                tipsWindow.Show();
            }
        }

        private void SettingButton_Click(Object sender, RoutedEventArgs e)
        {
            if (!SettingWindow.IsShowing)
            {
                SettingWindow settingWindow = new SettingWindow();
                settingWindow.Show();
            }
        }
        private void Window_KeyDown(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                e.Handled = true;
            }
        }

        private void OpenFileButton_Click(Object sender, RoutedEventArgs e)
        {
            if (MainWindowGameButton.SelectedGameButton != null)
            {
                String folderPath = Path.GetDirectoryName(MainWindowGameButton.SelectedGameButton.Game.ExePath)!;
                Process.Start("explorer.exe", folderPath);
            }
        }
        public void SetStartButton(Boolean toRunning)
        {
            if (toRunning)
            {
                gameStartButton.StartRunning("游戏中");
            }
            else
            {
                gameStartButton.StopRunning("开始游戏");
            }
        }

        private void SelectGameButton()
        {
            GameButtonsStateReset();
            UpdateGameInfo(MainWindowGameButton.SelectedGameButton!.Game);
            coverImage.Source = ImageHandler.GetImage(fileManager.GetGameCoverPath(MainWindowGameButton.SelectedGameButton!.Game.Name)!);
        }
        private void GameButtonsStateReset()
        {
            foreach (MainWindowGameButton gameButton in gameListStackPanel.Children)
            {
                gameButton.RelieveSelected();
            }
        }
        public void UpdateGameInfo(Game game)
        {
            captureDisplayPanel.Children.Clear();
            String[] captureNames = Directory.GetFiles(fileManager.GetGameCapturesPath(game.Name)!);
            Random rand = new Random();
            Int32 index = rand.Next(captureNames.Length + 1);
            if (index == captureNames.Length)
            {
                coverBlock.MainWindowCoverBlockImage = ImageHandler.GetImage(fileManager.GetGameCoverPath(game.Name)!);
            }
            else
            {
                coverBlock.MainWindowCoverBlockImage = ImageHandler.GetImage(captureNames[index]);
            }
            coverBlock.SetImageCount(captureNames.Length);
            List<List<String>> groups = fileManager.GroupCaptureNamesByPrefix(game.Name).Values.ToList();
            groups.Sort((List<String> x, List<String> y) =>
            {
                return -Convert.ToInt64(x[0][..8]).CompareTo(Convert.ToInt64(y[0][..8]));
            });
            foreach (List<String> group in groups)
            {
                captureDisplayPanel.Children.Add(new MainWindowDailyCaputreDisplay(group, game.Name));
            }
            if (game.PlayTimeMinute >= 120)
            {
                gameTotalTimeInfo.SetInfo((game.PlayTimeMinute / 60.0).ToString("0.0") + "小时");
            }
            else
            {
                gameTotalTimeInfo.SetInfo(game.PlayTimeMinute.ToString() + "分钟");
            }

            gameLastStartTimeInfo.SetInfo(game.LastStartTime.ToString("d"));
        }

        public void AddGame(String gameName)
        {
            MainWindowGameButton gameButton = new MainWindowGameButton(gameName);
            gameButton.SetSelectedAction(SelectGameButton);
            gameListStackPanel.Children.Add(gameButton);
            gameButton.BeingSelected();

            if (gameListStackPanel.Children.Count == 1)
            {
                mainRightGird_GameInfo.Visibility = Visibility.Visible;
                mainRightGrid_Picture.Visibility = Visibility.Visible;
                noGameTipsCanvas.Visibility = Visibility.Hidden;
            }
        }
        public void RemoveSelectedGame()
        {
            List<MainWindowGameButton> oldGames = new List<MainWindowGameButton>();
            foreach (MainWindowGameButton gameButton in GameListButtons)
            {
                oldGames.Add(gameButton);
            }
            gameListStackPanel.Children.Clear();
            foreach (MainWindowGameButton gameButton in oldGames)
            {
                if (gameButton != MainWindowGameButton.SelectedGameButton)
                {
                    gameListStackPanel.Children.Add(gameButton);
                }
            }
            Directory.Delete(fileManager.GetGameFolderPath(MainWindowGameButton.SelectedGameButton!.Game.Name)!, true);

            if (gameListStackPanel.Children.Count != 0)
            {
                ((MainWindowGameButton)gameListStackPanel.Children[0]).BeingSelected();
            }
            else
            {
                mainRightGird_GameInfo.Visibility = Visibility.Hidden;
                mainRightGrid_Picture.Visibility = Visibility.Hidden;
                noGameTipsCanvas.Visibility = Visibility.Visible;
            }

        }

        private void RemoveGameButton_Click(Object sender, RoutedEventArgs e)
        {
            RemoveGameConfirmWindow confirmWindow = new RemoveGameConfirmWindow(this);
            confirmWindow.ShowDialog();
        }


        private void Window_Closing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            List<String> names = new List<String>();
            foreach (MainWindowGameButton gameButton in gameListStackPanel.Children)
            {
                names.Add(gameButton.Game.Name);
            }
            File.WriteAllLines(fileManager.GamesOrderPath, names);
        }

        private void CoverBlockSeeCapturesButton_Click(Object sender, RoutedEventArgs e)
        {
            Game selectedGame = MainWindowGameButton.SelectedGameButton!.Game;
            String[] captureNames = Directory.GetFiles(fileManager.GetGameCapturesPath(selectedGame.Name)!);
            Random rand = new Random();
            Int32 index = rand.Next(captureNames.Length + 1);
            if (index == captureNames.Length)
            {
                coverBlock.MainWindowCoverBlockImage = ImageHandler.GetImage(fileManager.GetGameCoverPath(selectedGame.Name)!);
            }
            else
            {
                coverBlock.MainWindowCoverBlockImage = ImageHandler.GetImage(captureNames[index]);
            }
        }

        private void CoverBlockChangeCoverButton_Click(Object sender, RoutedEventArgs e)
        {
            Game selectedGame = MainWindowGameButton.SelectedGameButton!.Game;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择封面";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png, *.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
            if (openFileDialog.ShowDialog() == true)
            {
                String selectedImagePath = openFileDialog.FileName;
                BitmapImage bitmap = ImageHandler.GetImage(selectedImagePath);
                coverImage.Source = bitmap;
                BitmapEncoder encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(bitmap));

                FileStream fileStream = new FileStream(fileManager.GetGameCoverPath(selectedGame.Name)!, System.IO.FileMode.Create);
                encoder.Save(fileStream);
            }
        }

        private void OpenAlbumButton_Click(Object sender, RoutedEventArgs e)
        {
            Game selectedGame = MainWindowGameButton.SelectedGameButton!.Game;
            AlbumWindow albumWindow = new AlbumWindow(selectedGame);
            albumWindow.ShowDialog();
            UpdateGameInfo(selectedGame);

        }

        private void SearchGameTextBox_TextChanged(Object sender, TextChangedEventArgs e)
        {
            if (searchGameTextBox.Text == "")
            {
                cancelSearchButton.Visibility = Visibility.Hidden;
                searchGameButton.Visibility = Visibility.Visible;
                foreach (MainWindowGameButton gameButton in gameListStackPanel.Children)
                {
                    gameButton.Visibility = Visibility.Visible;
                }
            }
            else
            {
                cancelSearchButton.Visibility = Visibility.Visible;
                searchGameButton.Visibility = Visibility.Hidden;
                foreach (MainWindowGameButton gameButton in gameListStackPanel.Children)
                {
                    if (gameButton.Game.Name.Contains(searchGameTextBox.Text))
                    {
                        gameButton.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        gameButton.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void CancelSearchButton_Click(Object sender, RoutedEventArgs e)
        {
            searchGameTextBox.Text = "";
            cancelSearchButton.Visibility = Visibility.Hidden;
            searchGameButton.Visibility = Visibility.Visible;
            foreach (MainWindowGameButton gameButton in gameListStackPanel.Children)
            {
                gameButton.Visibility = Visibility.Visible;
            }
        }
    }
}