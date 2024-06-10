#pragma warning disable IDE0049
#pragma warning disable CS8618

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
        private TextBlock mainTextBlock;
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
                mainTextBlock!.Foreground = resource.GetColor("signColor") as Brush;
            };
            MouseLeave += (sender, e) =>
            {
                mainTextBlock!.Foreground = resource.GetColor("iconColor") as Brush;
            };
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainTextBlock = (Template.FindName("mainTextBlock", this) as TextBlock)!;
        }
    }
}
