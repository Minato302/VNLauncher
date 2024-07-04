#pragma warning disable IDE0049
#pragma warning disable CS8618

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Controls
{
    public class KeyMappingItem : Control
    {
        private TextBlock keyTextBlock;
        private TextBlock starTextBlock;
        private Border keyTextBorder;
        private LocalColorAcquirer resource;
        private GlobalKeyReader keyReader;

        public static readonly DependencyProperty KeyMappingItemFunctionTextProperty =
            DependencyProperty.Register("KeyMappingItemFunctionText", typeof(String), typeof(KeyMappingItem));

        public String KeyMappingItemFunctionText
        {
            get
            {
                return (String)GetValue(KeyMappingItemFunctionTextProperty);
            }
            set
            {
                SetValue(KeyMappingItemFunctionTextProperty, value);
            }
        }
        static KeyMappingItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyMappingItem), new FrameworkPropertyMetadata(typeof(KeyMappingItem)));
        }
        public KeyMappingItem()
        {
            resource = new LocalColorAcquirer();
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            keyTextBlock = (Template.FindName("keyTextBlock", this) as TextBlock)!;
            keyTextBorder = (Template.FindName("keyTextBorder", this) as Border)!;
            starTextBlock = (Template.FindName("starTextBlock", this) as TextBlock)!;
            keyTextBlock.MouseEnter += (sender, e) =>
            {
                Cursor = Cursors.Hand;
            };
            keyTextBlock.MouseLeave += (sender, e) =>
            {
                Cursor = Cursors.Arrow;
            };
            keyTextBorder.MouseLeftButtonUp += (s, e) =>
            {
                WaitForSettingKey();
            };
        }
        public void WaitForSettingKey()
        {
            keyReader = new GlobalKeyReader((inputKey) =>
            {
                SetKey(inputKey);
                starTextBlock.Visibility = Visibility.Visible;
                keyReader.UninstallHook();
            });
            Thread.Sleep(100);
            keyReader.ReadAKey();
            keyTextBlock.Text = "               ";
            keyTextBorder!.Background = resource.GetColor("itemButtonColor_MouseEnter");
        }
        public void SetKey(InputKey key)
        {
            ApplyTemplate();
            keyTextBlock.Text = key.Name;
            keyTextBorder.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        }
        public void SetKey(String keyName)
        {
            InputConverter converter = new InputConverter();
            SetKey(converter.ConvertToInputKey(keyName));
        }
        public String KeyName
        {
            get
            {
                starTextBlock.Visibility = Visibility.Hidden;
                return keyTextBlock.Text; 
            }
        }

    }
}
