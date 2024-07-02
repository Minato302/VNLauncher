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
        private TextBlock mainTextBlock;

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
                mainTextBlock!.Foreground = resource.GetColor("marqueeTranslateButtonColor_MouseEnter") as Brush;
            };
            MouseLeave += (sender, e) =>
            {
                mainTextBlock!.Foreground = resource.GetColor("marqueeTranslateButtonColor") as Brush;
            };
            PreviewMouseUp += (sender, e) =>
            {
                Turn();
            };
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainTextBlock = (Template.FindName("mainTextBlock", this) as TextBlock)!;
        }
        public void Turn()
        {
            if (mainTextBlock.Text == "↖")
            {
                mainTextBlock.Text = "↘";
                side = AdjustSide.RightDown;
            }
            else
            {
                mainTextBlock.Text = "↖";
                side = AdjustSide.LeftUp;
            }
        }
    }
}
