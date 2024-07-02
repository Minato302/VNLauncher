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
    public class SettingPageTextBox : Control
    {
        private TextBox mainTextBox;
        private TextBlock starTextBlock;
        static SettingPageTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingPageTextBox), new FrameworkPropertyMetadata(typeof(SettingPageTextBox)));
        }
        public static readonly DependencyProperty SettingPageTextBoxTextWrappingProperty =
               DependencyProperty.Register("SettingPageTextBoxTextWrapping", typeof(TextWrapping), typeof(SettingPageTextBox));
        public TextWrapping SettingPageTextBoxTextWrapping
        {
            get
            {
                return (TextWrapping)GetValue(SettingPageTextBoxTextWrappingProperty);
            }
            set
            {
                SetValue(SettingPageTextBoxTextWrappingProperty, value);
            }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainTextBox = (Template.FindName("mainTextBox", this) as TextBox)!;
            starTextBlock = (Template.FindName("starTextBlock", this) as TextBlock)!;
            mainTextBox.TextChanged += (sender, e) => 
            {
                starTextBlock.Visibility = Visibility.Visible;
            };
        }
        public void SaveFailed()
        {
            starTextBlock.Visibility = Visibility.Visible;
        }
        public String Text
        {
            get
            {
                starTextBlock.Visibility = Visibility.Hidden;
                return mainTextBox.Text;
            }
            set
            {
                mainTextBox.Text = value;
                starTextBlock.Visibility = Visibility.Hidden;
            }
        }
    }
}
