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
using VNLauncher.FuntionalClasses;

namespace VNLauncher.Controls
{
    public class MainWindowCoverBlock : Control
    {
        static MainWindowCoverBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MainWindowCoverBlock), new FrameworkPropertyMetadata(typeof(MainWindowCoverBlock)));
        }

        public static readonly DependencyProperty MainWindowCoverBlockImageProperty =
                  DependencyProperty.Register("MainWindowCoverBlockImage", typeof(ImageSource), typeof(MainWindowCoverBlock));
        public static readonly DependencyProperty MainWindowCoverBlockImageCountProperty =
          DependencyProperty.Register("MainWindowCoverBlockImageCount", typeof(String), typeof(MainWindowCoverBlock));
        public TextBlock imageCountTextBlock;
        public ImageSource MainWindowCoverBlockImage
        {
            get
            {
                return (ImageSource)GetValue(MainWindowCoverBlockImageProperty);
            }
            set
            {
                SetValue(MainWindowCoverBlockImageProperty, value);
            }
        }
        public String MainWindowCoverBlockImageCount
        {
            get
            {
                return (String)GetValue(MainWindowCoverBlockImageCountProperty);
            }
            set
            {
                SetValue(MainWindowCoverBlockImageCountProperty, value);
            }
        }
        public void SetImageCount(Int32 count)
        {
            if(count==0)
            {
                imageCountTextBlock.Text = "";
            }
            else
            {
                imageCountTextBlock.Text = "+" + count.ToString();
            }
        }
        public MainWindowCoverBlock()
        {

        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            imageCountTextBlock = (Template.FindName("imageCountTextBlock", this) as TextBlock)!;
        }
    }
}
