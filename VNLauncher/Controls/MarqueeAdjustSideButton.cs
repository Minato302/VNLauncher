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

    public class MarqueeAdjustSideButton : Button
    {
        public enum AdjustSide
        {
            LeftUp,
            RightDown
        }
        private LocalColorAcquirer resource;
        private AdjustSide side;
        public AdjustSide Side => side;
        static MarqueeAdjustSideButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarqueeAdjustSideButton), new FrameworkPropertyMetadata(typeof(MarqueeAdjustSideButton)));
        }
        public MarqueeAdjustSideButton()
        {
            resource = new LocalColorAcquirer();
            side = AdjustSide.LeftUp;
            MouseEnter += (sender, e) =>
            {
                TextBlock textBox = (Template.FindName("mainTextBlock", this) as TextBlock)!;
                textBox.Foreground = resource.GetColor("marqueeTranslateButtonColor_MouseEnter") as Brush;

            };
            MouseLeave += (sender, e) =>
            {
                TextBlock textBox = (Template.FindName("mainTextBlock", this) as TextBlock)!;
                textBox.Foreground = resource.GetColor("marqueeTranslateButtonColor") as Brush;
            };
            PreviewMouseUp += (sender, e) =>
            {
                Turn();
            };
        }

        public void Turn()
        {
            TextBlock font = (Template.FindName("mainTextBlock", this) as TextBlock)!;
            if (font.Text == "↖")
            {
                font.Text = "↘";
                side = AdjustSide.RightDown;
            }
            else
            {
                font.Text = "↖";
                side = AdjustSide.LeftUp;
            }
        }
    }
}
