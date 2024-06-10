#pragma warning disable IDE0049

using System.Windows;
using System.IO;
using System.Windows.Controls;
using VNLauncher.FuntionalClasses;
using System.Windows.Media.Imaging;

namespace VNLauncher.Controls
{
    public class MainWindowDailyCaputreDisplay : Control
    {
        private List<String> imageNames;
        private FileManager fileManager;
        private String gameName;
        static MainWindowDailyCaputreDisplay()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindowDailyCaputreDisplay), new FrameworkPropertyMetadata(typeof(MainWindowDailyCaputreDisplay)));
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (imageNames.Count == 0)
            {
                return;
            }
            String year = imageNames[0].Substring(0, 4);
            String month = imageNames[0].Substring(4, 2);
            String day = imageNames[0].Substring(6, 2);

            String nowDate = DateTime.Now.ToShortDateString();

            TextBlock dateDisplay = (Template.FindName("dateDisplayTextBlock", this) as TextBlock)!;

            if (nowDate.Substring(0, 4) == year)
            {
                dateDisplay.Text = "";
                if (month[0] == '0')
                {
                    dateDisplay.Text += month[1] + "月";
                }
                else
                {
                    dateDisplay.Text += month + "月";
                }
                if (day[0] == '0')
                {
                    dateDisplay.Text += day[1] + "月";
                }
                else
                {
                    dateDisplay.Text += day + "月";
                }
            }
            else
            {
                dateDisplay.Text = year + "年";
                if (month[0] == '0')
                {
                    dateDisplay.Text += month[1] + "月";
                }
                else
                {
                    dateDisplay.Text += month + "月";
                }
                if (day[0] == '0')
                {
                    dateDisplay.Text += day[1] + "月";
                }
                else
                {
                    dateDisplay.Text += day + "月";
                }
            }

            StackPanel panel1 = (Template.FindName("displayCapturePanel_Left", this) as StackPanel)!;
            StackPanel panel2 = (Template.FindName("displayCapturePanel_Right", this) as StackPanel)!;
            for (Int32 i = 0; i < imageNames.Count; i++)
            {
                if (i % 2 == 0)
                {
                    panel1.Children.Add(CreateImageByName(imageNames[i]));
                }
                else
                {
                    panel2.Children.Add(CreateImageByName(imageNames[i]));
                }
            }
        }
        public MainWindowDailyCaputreDisplay(List<String> imageNames, String gameName)
        {
            this.imageNames = imageNames;
            fileManager = new FileManager();
            this.gameName = gameName;
        }

        private Image CreateImageByName(String name)
        {
            String path = Path.Combine(fileManager.GetGameCapturesPath(gameName)!, name);
            BitmapImage bitmap = new BitmapImage(new Uri(path));
            Image image = new Image();
            image.Source = bitmap;
            image.Margin = new Thickness(20, 0, 0, 20);
            return image;
        }
    }
}
