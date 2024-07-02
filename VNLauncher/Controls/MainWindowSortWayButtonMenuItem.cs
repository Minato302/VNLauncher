#pragma warning disable IDE0049
#pragma warning disable CS8618

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VNLauncher.FunctionalClasses;

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
        private Border mainBorder;
        public MainWindowSortWayButtonMenuItem()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {

                mainBorder!.Background = resource.GetColor("signColor") as Brush;


            };
            MouseLeave += (sender, e) =>
            {
                mainBorder!.Background = resource.GetColor("mainWindowSortWayButtonMenuColor") as Brush;
            };

        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainBorder = (Template.FindName("mainBorder", this) as Border)!;
        }

    }

}
