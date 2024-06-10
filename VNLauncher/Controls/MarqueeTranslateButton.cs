#pragma warning disable IDE0049

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
using FontAwesome.WPF;
using VNLauncher.FuntionalClasses;

namespace VNLauncher.Controls
{
    public class MarqueeTranslateButton : Button
    {
        private LocalColorAcquirer resource;
        static MarqueeTranslateButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarqueeTranslateButton), new FrameworkPropertyMetadata(typeof(MarqueeTranslateButton)));
        }
        public MarqueeTranslateButton()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {
                FontAwesome.WPF.FontAwesome font = (Template.FindName("mainFont", this) as FontAwesome.WPF.FontAwesome)!;
                font.Foreground = resource.GetColor("marqueeTranslateButtonColor_MouseEnter") as Brush;

            };
            MouseLeave += (sender, e) =>
            {
                FontAwesome.WPF.FontAwesome font = (Template.FindName("mainFont", this) as FontAwesome.WPF.FontAwesome)!;
                font.Foreground = resource.GetColor("marqueeTranslateButtonColor") as Brush;
            };
        }
        public void Turn()
        {
            FontAwesome.WPF.FontAwesome font = (Template.FindName("mainFont", this) as FontAwesome.WPF.FontAwesome)!;
            if (font.Icon == FontAwesomeIcon.StopCircleOutline)
            {
                font.Icon = FontAwesomeIcon.PlayCircleOutline;
            }
            else
            {
                font.Icon = FontAwesomeIcon.StopCircleOutline;
            }
        }
        public Boolean IsTranslating
        {
            get
            {
                FontAwesome.WPF.FontAwesome font = (Template.FindName("mainFont", this) as FontAwesome.WPF.FontAwesome)!;
                return font.Icon == FontAwesomeIcon.StopCircleOutline;
            }
        }
    }
}
