#pragma warning disable IDE0049

using FontAwesome.WPF;
using System.Windows;
using System.Windows.Controls;

namespace VNLauncher.Controls
{
    public class MarqueeStateInfo : Control
    {
        public enum State
        {
            Closed,
            Waiting,
            Identifying,
            Translating,
            Over
        }
        private State state;
        private TextBlock stateTextBlock;
        private FontAwesome.WPF.FontAwesome stateIcon;
        static MarqueeStateInfo()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarqueeStateInfo), new FrameworkPropertyMetadata(typeof(MarqueeStateInfo)));
        }
        public MarqueeStateInfo()
        {
            state = State.Closed;
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            stateTextBlock = (GetTemplateChild("stateTextBlock") as TextBlock)!;
            stateIcon = (GetTemplateChild("stateIcon") as FontAwesome.WPF.FontAwesome)!;
        }
        public void ChangeState(State nextState)
        {
            OnApplyTemplate();
            switch (nextState)
            {
                case State.Closed:
                    stateIcon.Icon = FontAwesomeIcon.Ban;
                    stateTextBlock.Text = "已关闭";
                    break;
                case State.Waiting:
                    stateIcon.Icon = FontAwesomeIcon.Spinner;
                    stateTextBlock.Text = "等待中...";
                    break;
                case State.Identifying:
                    stateIcon.Icon = FontAwesomeIcon.Search;
                    stateTextBlock.Text = "识别中...";
                    break;
                case State.Translating:
                    stateIcon.Icon = FontAwesomeIcon.Language;
                    stateTextBlock.Text = "翻译中...";
                    break;
                case State.Over:
                    stateIcon.Icon = FontAwesomeIcon.Check;
                    stateTextBlock.Text = "翻译完成";
                    break;

            }

        }
    }
}
