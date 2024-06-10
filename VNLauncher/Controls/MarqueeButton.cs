#pragma warning disable IDE0049

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
    public class MarqueeButton : Control
    {
        private LocalColorAcquirer resource;
        static MarqueeButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarqueeButton), new FrameworkPropertyMetadata(typeof(MarqueeButton)));
        }
        public static readonly DependencyProperty MarqueeButtonTextProperty =
            DependencyProperty.Register("MarqueeButtonText", typeof(String), typeof(MarqueeButton));

        public String MarqueeButtonText
        {
            get
            {
                return (String)GetValue(MarqueeButtonTextProperty);
            }
            set
            {
                SetValue(MarqueeButtonTextProperty, value);
            }
        }
        public MarqueeButton()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {
                Cursor = Cursors.Hand;
                Border border = (Template.FindName("mainBorder", this) as Border)!;
                border.BorderThickness = new Thickness(1);

            };
            MouseLeave += (sender, e) =>
            {
                Cursor = Cursors.Arrow;
                Border border = (Template.FindName("mainBorder", this) as Border)!;
                border.BorderThickness = new Thickness(0);
            };
            PreviewMouseLeftButtonDown += (sender, e) =>
            {
                Border border = (Template.FindName("mainBorder", this) as Border)!;
                border.Background = resource.GetColor("marqueeButtonColor_MouseDown") as Brush;
            };
            PreviewMouseLeftButtonUp += (sender, e) =>
            {
                Border border = (Template.FindName("mainBorder", this) as Border)!;
                border.Background = resource.GetColor("marqueeButtonColor") as Brush;
            };
        }
    }
}
