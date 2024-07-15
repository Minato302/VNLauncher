using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VNLauncher.Controls
{

    public class MarqueeSlider : Slider
    {
        static MarqueeSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarqueeSlider), new FrameworkPropertyMetadata(typeof(MarqueeSlider)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var thumb = GetTemplateChild("Thumb") as Thumb;
            if (thumb != null)
            {
                thumb.PreviewMouseLeftButtonDown += Thumb_PreviewMouseLeftButtonDown;
                thumb.PreviewMouseLeftButtonUp += Thumb_PreviewMouseLeftButtonUp;
            }
        }

        private void Thumb_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ApplyTemplate();
            var grip = (sender as Thumb)?.Template.FindName("grip", sender as Thumb) as Border;
            if (grip != null)
            {
                var animation = new ThicknessAnimation
                {
                    To = new Thickness(2),
                    Duration = TimeSpan.FromSeconds(0.1)
                };
                grip.BeginAnimation(Border.BorderThicknessProperty, animation);
            }
        }

        private void Thumb_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ApplyTemplate();
            var grip = (sender as Thumb)?.Template.FindName("grip", sender as Thumb) as Border;
            if (grip != null)
            {
                var animation = new ThicknessAnimation
                {
                    To = new Thickness(0),
                    Duration = TimeSpan.FromSeconds(0.1)
                };
                grip.BeginAnimation(Border.BorderThicknessProperty, animation);
            }
        }
    }
}
