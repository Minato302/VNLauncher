#pragma warning disable IDE0049

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using FontAwesome.WPF;
using VNLauncher.FuntionalClasses;

namespace VNLauncher.Controls
{
    public class QuestionButton : Button
    {
        private String tips;
        private LocalColorAcquirer resource;
        static QuestionButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuestionButton), new FrameworkPropertyMetadata(typeof(QuestionButton)));
        }
        public QuestionButton()
        {
            tips = "";
            resource = new LocalColorAcquirer();
            PreviewMouseLeftButtonDown += (e, sender) =>
            {
                Popup popup = (Template.FindName("tipsPopup", this) as Popup)!;
                popup.IsOpen = true;
                TextBlock textBlock = (Template.FindName("tipsTextBlock", this) as TextBlock)!;
                textBlock.Text = tips;
                FontAwesome.WPF.FontAwesome icon = (Template.FindName("questionIcon", this) as FontAwesome.WPF.FontAwesome)!;
                icon.Foreground = resource.GetColor("signColor") as System.Windows.Media.Brush;
                
                
            };
            MouseEnter += (e, sender) =>
            {
                Cursor= Cursors.Hand;
            };
            MouseLeave += (e, sender) =>
            {
                Cursor = Cursors.Arrow;
            };
        }
        public void ClosePopup()
        {
            Popup popup = (Template.FindName("tipsPopup", this) as Popup)!;
            popup.IsOpen = false;
            FontAwesome.WPF.FontAwesome icon = (Template.FindName("questionIcon", this) as FontAwesome.WPF.FontAwesome)!;
            icon.Foreground = resource.GetColor("iconColor") as System.Windows.Media.Brush;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        public void SetTips(String tips)
        {
            this.tips = tips;
        }
    }
}
