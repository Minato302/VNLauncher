#pragma warning disable IDE0049

using FontAwesome.WPF;
using System.Windows;
using System.Windows.Controls;
using VNLauncher.FuntionalClasses;

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
        public MainWindowGameInfo()
        {

        }
    }
}
