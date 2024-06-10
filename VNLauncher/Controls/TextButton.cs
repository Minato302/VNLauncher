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
    public class TextButton : Button
    {
        private LocalColorAcquirer resource;
        static TextButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextButton), new FrameworkPropertyMetadata(typeof(TextButton)));
        }
        public static readonly DependencyProperty TextButtonFontSizeProperty =
            DependencyProperty.Register("TextButtonFontSize", typeof(Double), typeof(TextButton));

        public Double TextButtonFontSize
        {
            get { return (Double)GetValue(TextButtonFontSizeProperty); }
            set { SetValue(TextButtonFontSizeProperty, value); }
        }
        public TextButton()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {
                TextBlock border = (Template.FindName("mainTextBlock", this) as TextBlock)!;
                border.Foreground = resource.GetColor("signColor") as Brush;
            };
            MouseLeave += (sender, e) =>
            {
                TextBlock border = (Template.FindName("mainTextBlock", this) as TextBlock)!;
                border.Foreground = resource.GetColor("iconColor") as Brush;
            };
        }
    }
}
