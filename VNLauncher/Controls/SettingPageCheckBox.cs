#pragma warning disable IDE0049
#pragma warning disable CS8618

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VNLauncher.Controls
{
    public class SettingPageCheckBox : Control
    {
        public static readonly DependencyProperty SettingPageCheckBoxTextProperty =
            DependencyProperty.Register("SettingPageCheckBoxText", typeof(String), typeof(SettingPageCheckBox));
        public String SettingPageCheckBoxText
        {
            get
            {
                return (String)GetValue(SettingPageCheckBoxTextProperty);
            }
            set
            {
                SetValue(SettingPageCheckBoxTextProperty, value);
            }
        }
        private Boolean isChecked;
        public Boolean IsChecked
        {
            get
            {
                starTextBlock.Visibility = Visibility.Hidden;
                return isChecked;
            }
            set
            {
                isChecked = value;
                if(isChecked)
                {
                    changedAction_ToChecked?.Invoke();
                    isCheckedRect.Visibility = Visibility.Visible;
                }
                else
                {
                    changedAction_ToUnchecked?.Invoke();
                    isCheckedRect.Visibility = Visibility.Hidden;
                }

            }
        }
        private Rectangle isCheckedRect;
        private Border mainBorder;
        private TextBlock starTextBlock;
        private Action changedAction_ToChecked;
        private Action changedAction_ToUnchecked;
        static SettingPageCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingPageCheckBox), new FrameworkPropertyMetadata(typeof(SettingPageCheckBox)));
        }
        public void SetChangedAction(Action changedAction_ToChecked, Action changedAction_ToUnchecked)
        {
            this.changedAction_ToChecked = changedAction_ToChecked;
            this.changedAction_ToUnchecked = changedAction_ToUnchecked;
        }
        public SettingPageCheckBox()
        {
            isChecked = false;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            isCheckedRect = (Template.FindName("isCheckedRect", this) as Rectangle)!;
            mainBorder = (Template.FindName("mainBorder", this) as Border)!;
            starTextBlock = (Template.FindName("starTextBlock", this) as TextBlock)!;
            mainBorder.MouseLeftButtonUp += (s, e) =>
            {
                starTextBlock.Visibility = Visibility.Visible;
                if (!isChecked)
                {
                    isChecked = true;
                    changedAction_ToChecked?.Invoke();
                    isCheckedRect.Visibility = Visibility.Visible;
                }
                else
                {
                    isChecked = false;
                    changedAction_ToUnchecked?.Invoke();
                    isCheckedRect.Visibility = Visibility.Hidden;
                }
            };
        }    
    }
}
