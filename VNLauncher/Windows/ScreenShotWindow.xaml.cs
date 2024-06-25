
#pragma warning disable IDE0049

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace VNLauncher.Windows
{
    public partial class ScreenShotWindow : Window
    {
        public ScreenShotWindow(BitmapSource image)
        {
            InitializeComponent();
            captureImage.Source = image;
        }
        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            Double screenWidth = SystemParameters.PrimaryScreenWidth;
            Double screenHeight = SystemParameters.PrimaryScreenHeight;
            Left = screenWidth - Width - 10;
            Top = screenHeight;
            var moveUpAnimation = new DoubleAnimation
            {
                From = screenHeight,
                To = screenHeight - Height,
                Duration = TimeSpan.FromSeconds(0.4)
            };
            var moveDownAnimation = new DoubleAnimation
            {
                From = screenHeight - Height,
                To = screenHeight,
                Duration = TimeSpan.FromSeconds(0.4),
                BeginTime = TimeSpan.FromSeconds(3) 
            };
            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(moveUpAnimation);
            storyboard.Children.Add(moveDownAnimation);
            storyboard.Completed += (sender, e) =>
            {
                Close();
            };
            Storyboard.SetTarget(moveUpAnimation, this);
            Storyboard.SetTargetProperty(moveUpAnimation, new PropertyPath(Window.TopProperty));
            Storyboard.SetTarget(moveDownAnimation, this);
            Storyboard.SetTargetProperty(moveDownAnimation, new PropertyPath(Window.TopProperty));
            storyboard.Begin();
        }
    }
}
