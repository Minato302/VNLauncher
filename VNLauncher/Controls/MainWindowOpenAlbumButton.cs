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
using VNLauncher.FuntionalClasses;

namespace VNLauncher.Controls
{
    public class MainWindowOpenAlbumButton : Button
    {
        static MainWindowOpenAlbumButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindowOpenAlbumButton), new FrameworkPropertyMetadata(typeof(MainWindowOpenAlbumButton)));
        }
        private LocalColorAcquirer resource;
        private Border mainBorder;
        public  MainWindowOpenAlbumButton()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {
                Cursor = Cursors.Hand;
                mainBorder!.Background = resource.GetColor("mainWindowOpenAlbumButtonColor_MouseEnter") as Brush;
            };
            MouseLeave += (sender, e) =>
            {
                Cursor = Cursors.Arrow;
                mainBorder!.Background = resource.GetColor("mainWindowOpenAlbumButtonColor") as Brush;
            };
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainBorder = (Template.FindName("mainBorder", this) as Border)!;
        }
    }
}
