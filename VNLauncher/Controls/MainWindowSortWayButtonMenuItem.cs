#pragma warning disable IDE0049

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VNLauncher.FuntionalClasses;

namespace VNLauncher.Controls
{
    public class MainWindowSortWayButtonMenuItem : Button
    {
        static MainWindowSortWayButtonMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindowSortWayButtonMenuItem), new FrameworkPropertyMetadata(typeof(MainWindowSortWayButtonMenuItem)));
        }
        public static readonly DependencyProperty SortWayProperty =
            DependencyProperty.Register("SortWay", typeof(String), typeof(MainWindowSortWayButtonMenuItem));

        public String SortWay
        {
            get
            {
                return (String)GetValue(SortWayProperty);
            }
            set
            {
                SetValue(SortWayProperty, value);
            }
        }

        private LocalColorAcquirer resource;
        public MainWindowSortWayButtonMenuItem()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {

                Border border = (Template.FindName("mainBorder", this) as Border)!;
                border.Background = resource.GetColor("signColor") as Brush;


            };
            MouseLeave += (sender, e) =>
            {
                Border border = (Template.FindName("mainBorder", this) as Border)!;
                border.Background = resource.GetColor("mainWindowSortWayButtonMenuColor") as Brush;
            };

        }
    }

}
