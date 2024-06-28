#pragma warning disable IDE0049
#pragma warning disable CS8618

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using VNLauncher.FuntionalClasses;

namespace VNLauncher.Controls
{
    public class QuestionButton : Button
    {
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
            resource = new LocalColorAcquirer();
            PreviewMouseLeftButtonDown += (e, sender) =>
            {
                tipsPopUp!.IsOpen = true;
                questionIcon!.Foreground = resource.GetColor("signColor") as System.Windows.Media.Brush;
            };
            MouseEnter += (e, sender) =>
            {
                Cursor = Cursors.Hand;
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
            ApplyTemplate();
            tipsTextBlock.Text = tips;
        }
        public void ReplaceTextWithHyperlink(String searchString, String url, String replaceString)
        {
            ApplyTemplate();
            String text = tipsTextBlock.Text;

            var inlines = new List<Inline>(tipsTextBlock.Inlines);
            tipsTextBlock.Inlines.Clear();
            foreach (Inline inline in inlines)
            {
                if (inline is Run run)
                {
                    String inlineText = run.Text;
                    Int32 index = inlineText.IndexOf(searchString);
                    if (index >= 0)
                    {
                        if (index > 0)
                        {
                            tipsTextBlock.Inlines.Add(new Run(inlineText.Substring(0, index)));
                        }
                        Hyperlink hyperlink = new Hyperlink(new Run(replaceString))
                        {
                            NavigateUri = new Uri(url),
                            Foreground = resource.GetColor("iconColor_White") as System.Windows.Media.Brush,
                            TextDecorations = TextDecorations.Underline
                        };

                        hyperlink.RequestNavigate += (sender, e) =>
                        {
                            e.Handled = true;
                            ClosePopup();
                            try
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = e.Uri.AbsoluteUri,
                                    UseShellExecute = true,
                                    Verb = "open",
                                });
                            }
                            catch (Exception)
                            {
                            }
                        };

                        tipsTextBlock.Inlines.Add(hyperlink);

                        if (index + searchString.Length < inlineText.Length)
                        {
                            tipsTextBlock.Inlines.Add(new Run(inlineText.Substring(index + searchString.Length)));
                        }
                    }
                    else
                    {
                        tipsTextBlock.Inlines.Add(inline);
                    }
                }
                else
                {
                    tipsTextBlock.Inlines.Add(inline);
                }
            }
        }



    }
}
