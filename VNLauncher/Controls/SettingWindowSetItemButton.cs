#pragma warning disable IDE0049
#pragma warning disable CS8618

using FontAwesome.WPF;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VNLauncher.FuntionalClasses;

namespace VNLauncher.Controls
{
    public class SettingWindowSetItemButton : Button
    {
        private Border mainBorder;
        private LocalColorAcquirer resource;
        private Boolean isSelected;
        public Boolean IsSelected => isSelected;
        static SettingWindowSetItemButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingWindowSetItemButton), new FrameworkPropertyMetadata(typeof(SettingWindowSetItemButton)));
        }
        public static readonly DependencyProperty SettingWindowSetItemButtonIconProperty =
        DependencyProperty.Register("SettingWindowSetItemButtonIcon", typeof(FontAwesomeIcon), typeof(SettingWindowSetItemButton));
        public static readonly DependencyProperty SettingWindowSetItemButtonTextProperty =
                DependencyProperty.Register("SettingWindowSetItemButtonText", typeof(String), typeof(SettingWindowSetItemButton));
        public FontAwesomeIcon SettingWindowSetItemButtonIcon
        {
            get
            {
                return (FontAwesomeIcon)GetValue(SettingWindowSetItemButtonIconProperty);
            }
            set
            {
                SetValue(SettingWindowSetItemButtonIconProperty, value);
            }
        }
        public String SettingWindowSetItemButtonText
        {
            get
            {
                return (String)GetValue(SettingWindowSetItemButtonTextProperty);
            }
            set
            {
                SetValue(SettingWindowSetItemButtonTextProperty, value);
            }
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainBorder = (Template.FindName("mainBorder", this) as Border)!;
        }
        public SettingWindowSetItemButton()
        {
            resource = new LocalColorAcquirer();
            isSelected = false;
            MouseEnter += (sender, e) =>
            {
                if (!isSelected)
                {
                    mainBorder!.Background = resource.GetColor("mainWindowGameButtonColor_MouseEnter") as Brush;
                }
            };
            MouseLeave += (sender, e) =>
            {
                if (!isSelected)
                {
                    mainBorder!.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                }
            };
        }
        public void RelieveSelected()
        {
            isSelected = false;
            mainBorder.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }
        public void BeingSelected()
        {
            isSelected = true;
            mainBorder!.Background = resource.GetColor("settingWindowSetItemColor_Selected") as Brush;
        }
    }
}
