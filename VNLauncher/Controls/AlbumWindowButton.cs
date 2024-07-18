#pragma warning disable IDE0049
#pragma warning disable CS8618


using FontAwesome.WPF;
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

namespace VNLauncher.Controls
{
    public class AlbumWindowButton : Control
    {
        public static readonly DependencyProperty AlbumWindowButtonIconProperty =
            DependencyProperty.Register("AlbumWindowButtonIcon", typeof(FontAwesomeIcon), typeof(AlbumWindowButton));

        public static readonly RoutedEvent AlbumWindowButtonCoreClickEvent =
            EventManager.RegisterRoutedEvent("AlbumWindowButtonCoreClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AlbumWindowButton));

        private LocalColorAcquirer resource;
        private AlbumWindowButtonCore coreButton;
        private Border mainBorder;
        public FontAwesomeIcon AlbumWindowButtonIcon
        {
            get
            {
                return (FontAwesomeIcon)GetValue(AlbumWindowButtonIconProperty);
            }
            set
            {
                SetValue(AlbumWindowButtonIconProperty, value);
            }
        }

        public event RoutedEventHandler AlbumWindowButtonCoreClick
        {
            add
            {
                AddHandler(AlbumWindowButtonCoreClickEvent, value);
            }
            remove
            {
                RemoveHandler(AlbumWindowButtonCoreClickEvent, value);
            }
        }

        static AlbumWindowButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AlbumWindowButton), new FrameworkPropertyMetadata(typeof(AlbumWindowButton)));
        }
        public AlbumWindowButton()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {
                coreButton!.Visibility = Visibility.Visible;
            };
            MouseLeave += (sender, e) =>
            {
                coreButton!.Visibility = Visibility.Hidden;
            };
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            coreButton = (Template.FindName("coreButton", this) as AlbumWindowButtonCore)!;
            mainBorder = (Template.FindName("mainBorder", this) as Border)!;
            coreButton.Useable = true;
            coreButton.Click += AlbumWindowButtonCore_Click;
            if (coreButton.AlbumWindowButtonCoreIcon == FontAwesomeIcon.ChevronLeft)
            {
                coreButton.SetValue(Canvas.LeftProperty, 30D);
            }
            else
            {
                coreButton.SetValue(Canvas.RightProperty, 30D);
            }
        }
        public Boolean Usable
        {
            get
            {
                return coreButton.Useable;
            }
            set
            {
                coreButton.Useable = value;
            }
        }

        private void AlbumWindowButtonCore_Click(Object sender, RoutedEventArgs e)
        {
            if (coreButton.Useable)
            {
                RaiseEvent(new RoutedEventArgs(AlbumWindowButtonCoreClickEvent));
            }
        }
    }
}
