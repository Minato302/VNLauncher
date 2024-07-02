#pragma warning disable IDE0049
#pragma warning disable CS8618

using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Controls
{
    public class MarqueeRadioButton : Control
    {
        static MarqueeRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarqueeRadioButton), new FrameworkPropertyMetadata(typeof(MarqueeRadioButton)));
        }
        private Ellipse isCheckedCircle;
        private Border mainBorder;
        private Border boxBorder;
        private TextBlock checkNameTextBlock;
        private List<MarqueeRadioButton> groupFriends;
        private LocalColorAcquirer resource;
        private Action changedAction;
        public Boolean IsChecked
        {
            get
            {
                ApplyTemplate();
                return isCheckedCircle.Visibility == Visibility.Visible;
            }
        }
        public void SetUnenabled()
        {
            ApplyTemplate();
            IsEnabled = false;
            boxBorder.BorderBrush = resource.GetColor("marqueeButtonColor_Text_Unenabled") as System.Windows.Media.Brush;
            checkNameTextBlock.Foreground = resource.GetColor("marqueeButtonColor_Text_Unenabled") as System.Windows.Media.Brush;
        }
        public void SetChecked()
        {
            ApplyTemplate();
            isCheckedCircle.Visibility = Visibility.Visible;
            foreach (MarqueeRadioButton radioButton in groupFriends)
            {
                radioButton.isCheckedCircle.Visibility = Visibility.Hidden;

            }
        }
        public static readonly DependencyProperty MarqueeRadioButtonTextProperty =
             DependencyProperty.Register("MarqueeRadioButtonText", typeof(String), typeof(MarqueeRadioButton));
        public String MarqueeRadioButtonText
        {
            get
            {
                return (String)GetValue(MarqueeRadioButtonTextProperty);
            }
            set
            {
                SetValue(MarqueeRadioButtonTextProperty, value);
            }
        }
        public MarqueeRadioButton()
        {
            groupFriends = new List<MarqueeRadioButton>();
            resource = new LocalColorAcquirer();
        }
        static public void SetGroup(List<MarqueeRadioButton> group, Action changedAction, MarqueeRadioButton radioButton)
        {
            foreach (MarqueeRadioButton rb in group)
            {
                rb.groupFriends = new List<MarqueeRadioButton>(group);
                rb.groupFriends.Remove(rb);
                rb.ApplyTemplate();
                rb.changedAction = changedAction;
            }
            radioButton.SetChecked();
        }
        static public void SetGroup(List<MarqueeRadioButton> group, Action changedAction)
        {
            foreach (MarqueeRadioButton rb in group)
            {
                rb.groupFriends = new List<MarqueeRadioButton>(group);
                rb.groupFriends.Remove(rb);
                rb.ApplyTemplate();
                rb.changedAction = changedAction;
            }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            isCheckedCircle = (Template.FindName("isCheckedCircle", this) as Ellipse)!;
            mainBorder = (Template.FindName("mainBorder", this) as Border)!;
            boxBorder = (Template.FindName("boxBorder", this) as Border)!;
            checkNameTextBlock = (Template.FindName("checkNameTextBlock", this) as TextBlock)!;
            mainBorder.MouseLeftButtonUp += (s, e) =>
            {
                isCheckedCircle.Visibility = Visibility.Visible;
                foreach (MarqueeRadioButton radioButton in groupFriends)
                {
                    radioButton.isCheckedCircle.Visibility = Visibility.Hidden;

                }
                changedAction?.Invoke();
            };

            if (!IsEnabled)
            {
                boxBorder.BorderBrush = resource.GetColor("marqueeButtonColor_Text_Unenabled") as System.Windows.Media.Brush;
                checkNameTextBlock.Foreground = resource.GetColor("marqueeButtonColor_Text_Unenabled") as System.Windows.Media.Brush;
            }
        }
    }
}
