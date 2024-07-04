#pragma warning disable IDE0049
#pragma warning disable CS8618


using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Controls
{
    public class MainWindowGameButton : Button
    {
        static MainWindowGameButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindowGameButton), new FrameworkPropertyMetadata(typeof(MainWindowGameButton)));
        }
        private FileManager fileManager;
        private Windows.MainWindow mainWindow;
        private LocalColorAcquirer resource;
        private Boolean isSelected;
        public Boolean IsSelected => isSelected;

        public static readonly DependencyProperty MainWindowGameButtonTextProperty =
                DependencyProperty.Register("MainWindowGameButtonText", typeof(String), typeof(MainWindowGameButton));
        public static readonly DependencyProperty MainWindowGameButtonItemSourceProperty =
                DependencyProperty.Register("MainWindowGameButtonItemSource", typeof(ImageSource), typeof(MainWindowGameButton));
        private Game game;
        public Game Game => game;
        private Border mainBorder;

        public MainWindowGameButton(String gameName, Windows.MainWindow mainWindow)
        {
            fileManager = new FileManager();
            game = new Game(gameName, fileManager);
            this.mainWindow = mainWindow;
            SetValue(MainWindowGameButtonTextProperty, gameName);
            SetValue(MainWindowGameButtonItemSourceProperty, new BitmapImage(new Uri(fileManager.GetGameIconPath(gameName)!)));
            resource = new LocalColorAcquirer();
            isSelected = false;
            PreviewMouseLeftButtonDown += (sender, e) =>
            {
                foreach (MainWindowGameButton gameButton in mainWindow.gameListStackPanel.Children)
                {
                    gameButton.RelieveSelected();
                }
                BeingSelected();
            };
            MouseEnter += (sender, e) =>
            {
                if (!isSelected)
                {
                    mainBorder!.Background = resource.GetColor("mainWindowGameButtonColor_MouseEnter");
                }
            };
            MouseLeave += (sender, e) =>
            {
                if (!isSelected)
                {
                    mainBorder!.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                }
            };

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainBorder = (Template.FindName("mainBorder", this) as Border)!;
        }
        public String MainWindowGameButtonText
        {
            get
            {
                return (String)GetValue(MainWindowGameButtonTextProperty);
            }
            set
            {
                SetValue(MainWindowGameButtonTextProperty, value);
            }
        }
        public ImageSource MainWindowGameButtonItemSource
        {
            get
            {
                return (ImageSource)GetValue(MainWindowGameButtonItemSourceProperty);
            }
            set
            {
                SetValue(MainWindowGameButtonItemSourceProperty, value);
            }
        }
        public void UpdateInfo()
        {
            mainWindow.captureDisplayPanel.Children.Clear();
            String[] captureNames = Directory.GetFiles(fileManager.GetGameCapturesPath(Game.Name)!);
            Random rand = new Random();
            Int32 index = rand.Next(captureNames.Length + 1);
            if (index == captureNames.Length)
            {
                mainWindow.coverBlock.MainWindowCoverBlockImage = new BitmapImage(new Uri(fileManager.GetGameCoverPath(Game.Name)!));
            }
            else
            {
                mainWindow.coverBlock.MainWindowCoverBlockImage = new BitmapImage(new Uri(captureNames[index]));
            }
            mainWindow.coverBlock.SetImageCount(captureNames.Length);
            List<List<String>> groups = fileManager.GroupCaptureNamesByPrefix(Game.Name).Values.ToList();
            groups.Sort((List<String> x, List<String> y) =>
            {
                return -Convert.ToInt64(x[0][..8]).CompareTo(Convert.ToInt64(y[0][..8]));
            });
            foreach (List<String> group in groups)
            {
                mainWindow.captureDisplayPanel.Children.Add(new MainWindowDailyCaputreDisplay(group, Game.Name));
            }
            if (game.PlayTimeMinute >= 120)
            {
                mainWindow.gameTotalTimeInfo.SetInfo((game.PlayTimeMinute / 60.0).ToString("0.0") + "小时");
            }
            else
            {
                mainWindow.gameTotalTimeInfo.SetInfo(game.PlayTimeMinute.ToString() + "分钟");
            }

            mainWindow.gameLastStartTimeInfo.SetInfo(game.LastStartTime.ToString("d"));
        }
        public void BeingSelected()
        {
            isSelected = true;
            mainBorder.Background = resource.GetColor("mainWindowGameButtonColor_Selected");
            mainWindow.coverPicture.Source = new BitmapImage(new Uri(fileManager.GetGameCoverPath(Game.Name)!));
            UpdateInfo();
        }
        public void RelieveSelected()
        {
            isSelected = false;
            mainWindow.captureDisplayPanel.Children.Clear();
            mainBorder.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }

    }
}
