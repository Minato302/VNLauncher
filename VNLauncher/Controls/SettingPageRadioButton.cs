#pragma warning disable IDE0049
#pragma warning disable CS8618

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

namespace VNLauncher.Controls
{
    public class SettingPageRadioButton : Control
    {
        private Ellipse isCheckedCircle;
        private Border mainBorder;
        private List<SettingPageRadioButton> groupFriends;
        public Boolean IsChecked=> isCheckedCircle.Visibility == Visibility.Visible;
        static SettingPageRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingPageRadioButton), new FrameworkPropertyMetadata(typeof(SettingPageRadioButton)));
        }
        public static readonly DependencyProperty SettingPageRadioButtonTextProperty =
             DependencyProperty.Register("SettingPageRadioButtonText", typeof(String), typeof(SettingPageRadioButton));
        public String SettingPageRadioButtonText
        {
            get
            {
                return (String)GetValue(SettingPageRadioButtonTextProperty);
            }
            set
            {
                SetValue(SettingPageRadioButtonTextProperty, value);
            }
        }
        public SettingPageRadioButton()
        {
            groupFriends = new List<SettingPageRadioButton>();
        }
        static public void SetGroup(List<SettingPageRadioButton> group, SettingPageRadioButton checkedButton)
        {
            foreach(SettingPageRadioButton radioButton in group)
            {
                radioButton.groupFriends = new List<SettingPageRadioButton>(group);
                radioButton.groupFriends.Remove(radioButton);
            }
            checkedButton.isCheckedCircle.Visibility = Visibility.Visible;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            isCheckedCircle = (Template.FindName("isCheckedCircle", this) as Ellipse)!;
            mainBorder = (Template.FindName("mainBorder", this) as Border)!;
            mainBorder.MouseLeftButtonUp += (s, e) =>
            {
                isCheckedCircle.Visibility = Visibility.Visible;
                foreach(SettingPageRadioButton radioButton in groupFriends)
                {
                    radioButton.isCheckedCircle.Visibility = Visibility.Hidden;
                }
            };
        }
    }
}
