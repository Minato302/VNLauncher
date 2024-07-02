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
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Controls
{
    public class MainWindowCoverBlockButton : Button
    {
        static MainWindowCoverBlockButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindowCoverBlockButton), new FrameworkPropertyMetadata(typeof(MainWindowCoverBlockButton)));
        }

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
        private LocalColorAcquirer resource;
        private TextBlock itemTextBlock;
        private Border mainBorder;
        public MainWindowCoverBlockButton()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {
                Cursor = Cursors.Hand;
                mainBorder!.Background = resource.GetColor("mainWindowCoverBlockColor_ButtonMouseEnter") as Brush;
                itemTextBlock!.Foreground = resource.GetColor("iconColor") as Brush;
            };
            MouseLeave += (sender, e) =>
            {
                Cursor = Cursors.Arrow;
                mainBorder!.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                itemTextBlock!.Foreground = resource.GetColor("mainWindowCoverBlockColor_ButtonText") as Brush;
            };
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            itemTextBlock = (Template.FindName("itemTextBlock", this) as TextBlock)!;
            mainBorder = (Template.FindName("mainBorder", this) as Border)!;
        }
    }
}
