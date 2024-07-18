#pragma warning disable IDE0049
#pragma warning disable CS8618

using FontAwesome.WPF;
using System.Windows;
using System.Windows.Controls;
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Controls
{
    public class AlbumWindowButtonCore : Button
    {
        public static readonly DependencyProperty AlbumWindowButtonCoreIconProperty =
                 DependencyProperty.Register("AlbumWindowButtonCoreIcon", typeof(FontAwesomeIcon), typeof(AlbumWindowButtonCore));
        private LocalColorAcquirer resource;
        private FontAwesome.WPF.FontAwesome icon;
        private Boolean usable;
        public Boolean Useable
        {
            get
            {
                return usable;
            }
            set
            {
                usable = value;
            }
        }

        public FontAwesomeIcon AlbumWindowButtonCoreIcon
        {
            get
            {
                return (FontAwesomeIcon)GetValue(AlbumWindowButtonCoreIconProperty);
            }
            set
            {
                SetValue(AlbumWindowButtonCoreIconProperty, value);
            }
        }
        static AlbumWindowButtonCore()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AlbumWindowButtonCore), new FrameworkPropertyMetadata(typeof(AlbumWindowButtonCore)));
        }
        public AlbumWindowButtonCore()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {
                if (usable)
                {
                    icon!.FontSize = 22;
                }
            };
            MouseLeave += (sender, e) =>
            {
                icon!.FontSize = 18;
            };
            PreviewMouseDown += (sender, e) =>
            {
                if (usable)
                {
                    icon!.Foreground = resource.GetColor("signColor");
                }
            };
            PreviewMouseUp += (sender, e) =>
            {
                icon!.Foreground = resource.GetColor("iconColor");
            };
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            icon = (Template.FindName("icon", this) as FontAwesome.WPF.FontAwesome)!;
        }
    }
}
