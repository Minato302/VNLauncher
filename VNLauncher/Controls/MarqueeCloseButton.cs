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

namespace VNLauncher.Controls
{
    public class MarqueeCloseButton : Button
    {
        static MarqueeCloseButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarqueeCloseButton), new FrameworkPropertyMetadata(typeof(MarqueeCloseButton)));
        }
        private Line l1;
        private Line l2;
        public MarqueeCloseButton()
        {
            MouseEnter += (sender, e) =>
            {
                l1!.StrokeThickness = 2;
                l2!.StrokeThickness = 2;
            };
            MouseLeave += (sender, e) =>
            {
                l1!.StrokeThickness = 1.5;
                l2!.StrokeThickness = 1.5;
            };
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            l1 = (Template.FindName("crossLine1", this) as Line)!;
            l2 = (Template.FindName("crossLine2", this) as Line)!;
        }
    }
}
