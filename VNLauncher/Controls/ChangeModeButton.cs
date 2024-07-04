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
using VNLauncher.FunctionalClasses;
using FontAwesome.WPF;
using System.Net.Sockets;

namespace VNLauncher.Controls
{
    public class ChangeModeButton : Control
    {
        public enum Mode
        {
            Mode1,
            Mode2
        }
        private LocalColorAcquirer resource;
        private Mode selectedMode;
        private Border mainBorder;
        private FontAwesome.WPF.FontAwesome modeIcon;
        private TextBlock modeTextBlock;

        public Mode SelectedMode => selectedMode;
        static ChangeModeButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ChangeModeButton), new FrameworkPropertyMetadata(typeof(ChangeModeButton)));
        }

        public static readonly DependencyProperty ChangeModeButtonMode1IconProperty =
                 DependencyProperty.Register("ChangeModeButtonMode1Icon", typeof(FontAwesomeIcon), typeof(ChangeModeButton));

        public FontAwesomeIcon ChangeModeButtonMode1Icon
        {
            get
            {
                return (FontAwesomeIcon)GetValue(ChangeModeButtonMode1IconProperty);
            }
            set
            {
                SetValue(ChangeModeButtonMode1IconProperty, value);
            }
        }

        public static readonly DependencyProperty ChangeModeButtonMode1StringProperty =
                 DependencyProperty.Register("ChangeModeButtonMode1String", typeof(String), typeof(ChangeModeButton));

        public String ChangeModeButtonMode1String
        {
            get
            {
                return (String)GetValue(ChangeModeButtonMode1StringProperty);
            }
            set
            {
                SetValue(ChangeModeButtonMode1StringProperty, value);
            }
        }

        public static readonly DependencyProperty ChangeModeButtonMode2IconProperty =
                 DependencyProperty.Register("ChangeModeButtonMode2Icon", typeof(FontAwesomeIcon), typeof(ChangeModeButton));

        public FontAwesomeIcon ChangeModeButtonMode2Icon
        {
            get
            {
                return (FontAwesomeIcon)GetValue(ChangeModeButtonMode2IconProperty);
            }
            set
            {
                SetValue(ChangeModeButtonMode2IconProperty, value);
            }
        }

        public static readonly DependencyProperty ChangeModeButtonMode2StringProperty =
                 DependencyProperty.Register("ChangeModeButtonMode2String", typeof(String), typeof(ChangeModeButton));

        public String ChangeModeButtonMode2String
        {
            get
            {
                return (String)GetValue(ChangeModeButtonMode2StringProperty);
            }
            set
            {
                SetValue(ChangeModeButtonMode2StringProperty, value);
            }
        }

        public ChangeModeButton()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (e, sender) =>
            {
                Cursor = Cursors.Hand;
                mainBorder!.Background = resource.GetColor("itemButtonColor_MouseEnter");
            };
            MouseLeave += (e, sender) =>
            {
                Cursor = Cursors.Arrow;
     
                mainBorder!.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            };
            PreviewMouseLeftButtonDown += ChangeStartWay;
            PreviewMouseLeftButtonDown += (e, sender) =>
            {
                mainBorder!.Background = resource.GetColor("signColor");
            };
            PreviewMouseLeftButtonUp += (e, sender) =>
            {
                mainBorder!.Background = resource.GetColor("itemButtonColor_MouseEnter");
            };
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            selectedMode = Mode.Mode1;
            modeIcon = (Template.FindName("modeIcon", this) as FontAwesome.WPF.FontAwesome)!;
            modeIcon.Icon = ChangeModeButtonMode1Icon;
            modeTextBlock = (Template.FindName("modeTextBlock", this) as TextBlock)!;
            modeTextBlock.Text = ChangeModeButtonMode1String;
            mainBorder = (Template.FindName("mainBorder", this) as Border)!;
        }
        public void ChangeStartWay(Object sender, MouseButtonEventArgs e)
        {
            if (selectedMode == Mode.Mode1)
            {
                selectedMode = Mode.Mode2;
                modeIcon.Icon = ChangeModeButtonMode2Icon;
                modeTextBlock.Text = ChangeModeButtonMode2String;
            }
            else if (selectedMode == Mode.Mode2)
            {
                selectedMode = Mode.Mode1;
                modeIcon.Icon = ChangeModeButtonMode1Icon;
                modeTextBlock.Text = ChangeModeButtonMode1String;
            }
        }
    }
}
