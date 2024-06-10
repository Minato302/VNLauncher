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
    public class MainWindowControlBarButton : Button
    {
        static MainWindowControlBarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindowControlBarButton), new FrameworkPropertyMetadata(typeof(MainWindowControlBarButton)));
        }
        public static readonly DependencyProperty MainWindowControlBarButtonIconProperty =
            DependencyProperty.Register("MainWindowControlBarButtonIcon", typeof(FontAwesomeIcon), typeof(MainWindowControlBarButton));
        private LocalColorAcquirer resource;

        public FontAwesomeIcon MainWindowControlBarButtonIcon
        {
            get
            {
                return (FontAwesomeIcon)GetValue(MainWindowControlBarButtonIconProperty);
            }
            set
            {
                SetValue(MainWindowControlBarButtonIconProperty, value);
            }
        }
        public MainWindowControlBarButton()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {
                Cursor = Cursors.Hand;
                Border border = (Template.FindName("mainBorder", this) as Border)!;
                border.Background = resource.GetColor("mainWindowControlBarButtonColor_MouseEnter") as Brush;
            };
            MouseLeave += (sender, e) =>
            {
                Cursor = Cursors.Arrow;
                Border border = (Template.FindName("mainBorder", this) as Border)!;
                border.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            };
            PreviewMouseLeftButtonDown += (sender, e) =>
            {
                Border border = (Template.FindName("mainBorder", this) as Border)!;
                border.Background = resource.GetColor("signColor") as Brush;
            };
            PreviewMouseLeftButtonUp += (sender, e) =>
            {
                Border border = (Template.FindName("mainBorder", this) as Border)!;
                border.Background = resource.GetColor("mainWindowControlBarButtonColor_MouseEnter") as Brush;
            };
        }
    }
}
