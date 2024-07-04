#pragma warning disable IDE0049

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Controls
{
    public class GuidancePageAdjustSideButton : Button
    {
        public enum AdjustSide
        {
            LeftUp,
            RightDown
        }
        private LocalColorAcquirer resource;
        private AdjustSide side;

        private Border mainBorder_LeftUp;
        private Border mainBorder_RightDown;
        private TextBlock adjustSideTextBlock;
        static GuidancePageAdjustSideButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GuidancePageAdjustSideButton), new FrameworkPropertyMetadata(typeof(GuidancePageAdjustSideButton)));
        }
        public AdjustSide Side => side;
        public GuidancePageAdjustSideButton()
        {
            side = AdjustSide.LeftUp;

            resource = new LocalColorAcquirer();
            MouseEnter += (e, sender) =>
            {
                Cursor = Cursors.Hand;
                mainBorder_LeftUp!.Background = resource.GetColor("itemButtonColor_MouseEnter");
            };
            MouseLeave += (e, sender) =>
            {
                Cursor = Cursors.Arrow;
                mainBorder_LeftUp!.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            };

            PreviewMouseLeftButtonUp += ChangeAdjustSide;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainBorder_LeftUp = (Template.FindName("mainBorder_LeftUp", this) as Border)!;
            mainBorder_RightDown = (Template.FindName("mainBorder_RightDown", this) as Border)!;
            adjustSideTextBlock = (Template.FindName("adjustSideTextBlock", this) as TextBlock)!;
        }
        public void ChangeAdjustSide(Object sender, MouseButtonEventArgs e)
        {
            if (side == AdjustSide.LeftUp)
            {
                side = AdjustSide.RightDown;
                mainBorder_LeftUp.BorderThickness = new Thickness(1, 1, 0, 0);
                mainBorder_LeftUp.BorderBrush = resource.GetColor("iconColor");

                mainBorder_RightDown.BorderThickness = new Thickness(0, 0, 3.5, 3.5);
                mainBorder_RightDown.BorderBrush = resource.GetColor("signColor");

                adjustSideTextBlock.Text = "右下";
            }
            else
            {
                side = AdjustSide.LeftUp;

                mainBorder_RightDown.BorderThickness = new Thickness(0, 0, 1, 1);
                mainBorder_RightDown.BorderBrush = resource.GetColor("iconColor");

                mainBorder_LeftUp.BorderThickness = new Thickness(3.5, 3.5, 0, 0);
                mainBorder_LeftUp.BorderBrush = resource.GetColor("signColor");

                adjustSideTextBlock.Text = "左上";
            }
        }
    }
}
