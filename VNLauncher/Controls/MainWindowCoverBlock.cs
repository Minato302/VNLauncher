#pragma warning disable IDE0049
#pragma warning disable CS8618

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        public static readonly RoutedEvent SeeCapturesButtonClickEvent =
            EventManager.RegisterRoutedEvent("SeeCapturesButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MainWindowCoverBlock));

        public static readonly RoutedEvent ChangeCoverButtonClickEvent =
            EventManager.RegisterRoutedEvent("ChangeCoverButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MainWindowCoverBlock));

        public event RoutedEventHandler SeeCapturesButtonClick
        {
            add { AddHandler(SeeCapturesButtonClickEvent, value); }
            remove { RemoveHandler(SeeCapturesButtonClickEvent, value); }
        }

        public event RoutedEventHandler ChangeCoverButtonClick
        {
            add { AddHandler(ChangeCoverButtonClickEvent, value); }
            remove { RemoveHandler(ChangeCoverButtonClickEvent, value); }
        }

        private TextBlock imageCountTextBlock;
        private Image coverImage;
        private MainWindowCoverBlockButton seeCapturesButton;
        private MainWindowCoverBlockButton changeCoverButton;
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
            if (count == 0)
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
            ApplyTemplate();
            coverImage = (Template.FindName("coverImage", this) as Image)!;
            imageCountTextBlock = (Template.FindName("imageCountTextBlock", this) as TextBlock)!;
            seeCapturesButton = (Template.FindName("seeCapturesButton", this) as MainWindowCoverBlockButton)!;
            changeCoverButton = (Template.FindName("changeCoverButton", this) as MainWindowCoverBlockButton)!;
            seeCapturesButton.Click += SeeCapturesButton_Click;
            changeCoverButton.Click += ChangeCoverButton_Click;
        }

        private void SeeCapturesButton_Click(Object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(SeeCapturesButtonClickEvent));
        }
        private void ChangeCoverButton_Click(Object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ChangeCoverButtonClickEvent));
        }
    }
}
