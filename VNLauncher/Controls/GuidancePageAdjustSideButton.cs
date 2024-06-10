#pragma warning disable IDE0049

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VNLauncher.FuntionalClasses;

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
                Border border = (Template.FindName("mainBorder_LeftUp", this) as Border)!;
                border.Background = resource.GetColor("itemButtonColor_MouseEnter") as Brush;
            };
            MouseLeave += (e, sender) =>
            {
                Cursor = Cursors.Arrow;
                Border border = (Template.FindName("mainBorder_LeftUp", this) as Border)!;
                border.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            };

            PreviewMouseLeftButtonUp += ChangeAdjustSide;
        }
        public void ChangeAdjustSide(Object sender, MouseButtonEventArgs e)
        {
            if (side == AdjustSide.LeftUp)
            {
                side = AdjustSide.RightDown;
                Border border = (Template.FindName("mainBorder_LeftUp", this) as Border)!;
                border.BorderThickness = new Thickness(1, 1, 0, 0);
                border.BorderBrush = resource.GetColor("iconColor") as Brush;
                border = (Template.FindName("mainBorder_RightDown", this) as Border)!;
                border.BorderThickness = new Thickness(0, 0, 3.5, 3.5);
                border.BorderBrush = resource.GetColor("signColor") as Brush;

                TextBlock textBlock = (Template.FindName("adjustSideTextBlock", this) as TextBlock)!;
                textBlock.Text = "右下";
            }
            else
            {
                side = AdjustSide.LeftUp;
                Border border = (Template.FindName("mainBorder_RightDown", this) as Border)!;
                border.BorderThickness = new Thickness(0, 0, 1, 1);
                border.BorderBrush = resource.GetColor("iconColor") as Brush;

                border = (Template.FindName("mainBorder_LeftUp", this) as Border)!;
                border.BorderThickness = new Thickness(3.5, 3.5, 0, 0);
                border.BorderBrush = resource.GetColor("signColor") as Brush;

                TextBlock textBlock = (Template.FindName("adjustSideTextBlock", this) as TextBlock)!;
                textBlock.Text = "左上";
            }
        }
    }
}
