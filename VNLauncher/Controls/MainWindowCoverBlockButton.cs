#pragma warning disable IDE0049

using FontAwesome.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VNLauncher.FuntionalClasses;

namespace VNLauncher.Controls
{
    public class MainWindowCoverBlockButton : Button
    {
        static MainWindowCoverBlockButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindowCoverBlockButton), new FrameworkPropertyMetadata(typeof(MainWindowCoverBlockButton)));
        }
        private LocalColorAcquirer resource;

        public static readonly DependencyProperty MainWindowCoverBlockButtonTextProperty =
         DependencyProperty.Register("MainWindowCoverBlockButtonText", typeof(String), typeof(MainWindowCoverBlockButton));

        public String MainWindowCoverBlockButtonText
        {
            get
            {
                return (String)GetValue(MainWindowCoverBlockButtonTextProperty);
            }
            set
            {
                SetValue(MainWindowCoverBlockButtonTextProperty, value);
            }
        }
        public MainWindowCoverBlockButton()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {
                Cursor = Cursors.Hand;
                Border border = (Template.FindName("mainBorder", this) as Border)!;
                border.Background = resource.GetColor("mainWindowCoverBlockColor_ButtonMouseEnter") as Brush;

                TextBlock textblock = (Template.FindName("itemTextBlock", this) as TextBlock)!;
                textblock.Foreground = resource.GetColor("iconColor") as Brush;
            };
            MouseLeave += (sender, e) =>
            {
                Cursor = Cursors.Arrow;
                Border border = (Template.FindName("mainBorder", this) as Border)!;
                border.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));

                TextBlock textblock = (Template.FindName("itemTextBlock", this) as TextBlock)!;
                textblock.Foreground = resource.GetColor("mainWindowCoverBlockColor_ButtonText") as Brush;
            };
        }
    }
}
