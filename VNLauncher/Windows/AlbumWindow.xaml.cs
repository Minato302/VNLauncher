#pragma warning disable IDE0049

using FontAwesome.WPF;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Windows
{
    public partial class AlbumWindow : Window
    {
        private Game game;
        private List<String> captureNames;
        private FileManager fileManager;
        private Int32 index;
        public AlbumWindow(Game game)
        {
            InitializeComponent();
            index = 0;
            this.game = game;
            captureNames = new List<String>();
            fileManager = new FileManager();
            captureNames = Directory.GetFiles(fileManager.GetGameCapturesPath(game.Name)!).ToList();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            gameNameTextBlock.Text = game.Name;
            gameIconImage.Source = ImageHandler.GetImage(fileManager.GetGameIconPath(game.Name)!);
            SetButtonFunction();
            LoadImage();
        }
        private void SetButtonFunction()
        {
            if (captureNames.Count == 1)
            {
                goPreviewImageButton.Usable = false;
                goNextImageButton.Usable = false;
            }
            else
            {
                if (index == 0)
                {
                    goPreviewImageButton.Usable = false;
                    goNextImageButton.Usable = true;
                }
                else if (index == captureNames.Count - 1)
                {
                    goPreviewImageButton.Usable = true;
                    goNextImageButton.Usable = false;
                }
                else
                {
                    goPreviewImageButton.Usable = true;
                    goNextImageButton.Usable = true;
                }
            }
        }
        private void LoadImage()
        {
            copyTipsBlock.Visibility = Visibility.Hidden;
            if (captureNames.Count == 0)
            {
                noImageTips1.Visibility = Visibility.Visible;
                noImageTips2.Visibility = Visibility.Visible;
                deleteImageButton.Visibility = Visibility.Hidden;
                copyImageButton.Visibility = Visibility.Hidden;
                image.Visibility = Visibility.Hidden;
                timeTextBlock.Visibility = Visibility.Hidden;
                goPreviewImageButton.Usable = false;
                goNextImageButton.Usable = false;
            }
            else
            {
                SetButtonFunction();
                image.Source = ImageHandler.GetImage(captureNames[index]);
                String year = Path.GetFileName(captureNames[index])[0..4];
                String month = Path.GetFileName(captureNames[index])[4..6];
                String day = Path.GetFileName(captureNames[index])[6..8];
                String hour = Path.GetFileName(captureNames[index])[8..10];
                String minute = Path.GetFileName(captureNames[index])[10..12];



                timeTextBlock.Text = year + "年" + month + "月" + day + "日" + " " + hour + ":" + minute;
            }
        }

        private void CloseButton_Click(Object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(Object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void GoPreviewImageButtonCore_Click(Object sender, RoutedEventArgs e)
        {
            index--;
            LoadImage();
        }

        private void GoNextImageButtonCore_Click(Object sender, RoutedEventArgs e)
        {
            index++;
            LoadImage();
        }

        private void DeleteImageButton_Click(Object sender, RoutedEventArgs e)
        {
            DeleteCapture();
        }

        private void CopyImageButton_Click(Object sender, RoutedEventArgs e)
        {
            BitmapImage capture = ImageHandler.GetImage(captureNames[index]);
            Clipboard.SetImage(capture);
            copyTipsBlock.Visibility = Visibility.Visible;
        }
        private void DeleteCapture()
        {

            if (index == captureNames.Count - 1)
            {
                File.Delete(captureNames[index]);
                captureNames = Directory.GetFiles(fileManager.GetGameCapturesPath(game.Name)!).ToList();
                index--;
                LoadImage();
            }
            else
            {
                File.Delete(captureNames[index]);
                captureNames = Directory.GetFiles(fileManager.GetGameCapturesPath(game.Name)!).ToList();
                LoadImage();
            }
        }
        void MouseDragMove(Object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
