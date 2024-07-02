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
using FontAwesome.WPF;
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Controls
{
    public class MarqueeTranslateButton : Button
    {
        private LocalColorAcquirer resource;
        private FontAwesome.WPF.FontAwesome mainFont;
        static MarqueeTranslateButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarqueeTranslateButton), new FrameworkPropertyMetadata(typeof(MarqueeTranslateButton)));
        }
        public MarqueeTranslateButton()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {
                mainFont!.Foreground = resource.GetColor("marqueeTranslateButtonColor_MouseEnter") as Brush;

            };
            MouseLeave += (sender, e) =>
            {
                mainFont!.Foreground = resource.GetColor("marqueeTranslateButtonColor") as Brush;
            };
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainFont = (Template.FindName("mainFont", this) as FontAwesome.WPF.FontAwesome)!;
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
                if (mainFont == null)
                {
                    return false;
                }
                else
                {
                    return mainFont.Icon == FontAwesomeIcon.StopCircleOutline;
                }
            }
        }
    }
}
