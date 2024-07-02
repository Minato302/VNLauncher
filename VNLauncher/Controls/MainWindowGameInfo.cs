#pragma warning disable IDE0049
#pragma warning disable CS8618

using FontAwesome.WPF;
using System.Windows;
using System.Windows.Controls;
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Controls
{
    public class MainWindowGameInfo : Control
    {
        static MainWindowGameInfo()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindowGameInfo), new FrameworkPropertyMetadata(typeof(MainWindowGameInfo)));
        }
        public static readonly DependencyProperty MainWindowGameInfoIconProperty =
                   DependencyProperty.Register("MainWindowGameInfoIcon", typeof(FontAwesomeIcon), typeof(MainWindowGameInfo));
        private TextBlock valueTextBlock;

        public FontAwesomeIcon MainWindowGameInfoIcon
        {
            get
            {
                return (FontAwesomeIcon)GetValue(MainWindowGameInfoIconProperty);
            }
            set
            {
                SetValue(MainWindowGameInfoIconProperty, value);
            }
        }

        public static readonly DependencyProperty MainWindowGameInfoItemProperty =
           DependencyProperty.Register("MainWindowGameInfoItem", typeof(String), typeof(MainWindowGameInfo));

        public String MainWindowGameInfoItem
        {
            get
            {
                return (String)GetValue(MainWindowGameInfoItemProperty);
            }
            set
            {
                SetValue(MainWindowGameInfoItemProperty, value);
            }
        }

        public static readonly DependencyProperty MainWindowGameInfoValueProperty =
             DependencyProperty.Register("MainWindowGameInfoValue", typeof(String), typeof(MainWindowGameInfo));

        public String MainWindowGameInfoValue
        {
            get
            {
                return (String)GetValue(MainWindowGameInfoValueProperty);
            }
            set
            {
                SetValue(MainWindowGameInfoValueProperty, value);
            }
        }
        public void SetInfo(String info)
        {
            valueTextBlock.Text = info;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            valueTextBlock = (Template.FindName("valueTextBlock", this) as TextBlock)!;

        }
        public MainWindowGameInfo()
        {

        }
    }
}
