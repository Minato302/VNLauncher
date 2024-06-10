#pragma warning disable IDE0049
#pragma warning disable CS8618

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
        private Popup tipsPopUp;
        private TextBlock tipsTextBlock;
        private FontAwesome.WPF.FontAwesome questionIcon;
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
                tipsPopUp!.IsOpen = true;
                tipsTextBlock!.Text = tips;
                questionIcon!.Foreground = resource.GetColor("signColor") as System.Windows.Media.Brush;                
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
            tipsPopUp.IsOpen = false;
            questionIcon.Foreground = resource.GetColor("iconColor") as System.Windows.Media.Brush;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            tipsPopUp = (Template.FindName("tipsPopup", this) as Popup)!;
            tipsTextBlock = (Template.FindName("tipsTextBlock", this) as TextBlock)!;
            questionIcon = (Template.FindName("questionIcon", this) as FontAwesome.WPF.FontAwesome)!;
        }
        public void SetTips(String tips)
        {
            this.tips = tips;
        }
    }
}
