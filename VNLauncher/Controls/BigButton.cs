﻿#pragma warning disable IDE0049
#pragma warning disable CS8618

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Controls
{
    public class BigButton : Button
    {
        private Boolean isRunning;
        private Border mainBorder;
        private TextBlock functionTextBlock;
        static BigButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BigButton), new FrameworkPropertyMetadata(typeof(BigButton)));
        }
        public static readonly DependencyProperty BigButtonTextProperty =
              DependencyProperty.Register("BigButtonText", typeof(String), typeof(BigButton));
        public String BigButtonText
        {
            get { return (String)GetValue(BigButtonTextProperty); }
            set { SetValue(BigButtonTextProperty, value); }
        }
        private LocalColorAcquirer resource;
        public BigButton()
        {
            isRunning = false;
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {
                if (!isRunning)
                {
                    Cursor = Cursors.Hand;
                    mainBorder!.Background = resource.GetColor("signColor_Dark");
                }
            };
            MouseLeave += (sender, e) =>
            {
                if (!isRunning)
                {
                    Cursor = Cursors.Arrow;
                    mainBorder!.Background = resource.GetColor("signColor");
                }
            };
            PreviewMouseLeftButtonDown += (sender, e) =>
            {
                if (!isRunning)
                {
                    mainBorder!.Background = resource.GetColor("signColor_Light");
                }
            };
            PreviewMouseLeftButtonUp += (sender, e) =>
            {
                if (!isRunning)
                {
                    mainBorder!.Background = resource.GetColor("signColor_Dark");
                }
            };
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainBorder = (Template.FindName("mainBorder", this) as Border)!;
            functionTextBlock = (Template.FindName("funtionTextBlock", this) as TextBlock)!;

        }
        public void StartRunning(String runningInfo)
        {
            ApplyTemplate();
            isRunning = true;
            mainBorder.Background = resource.GetColor("bigButtonColor_Running");
            IsEnabled = false;
            functionTextBlock.Text = runningInfo;
        }
        public void StopRunning(String originInfo)
        {
            ApplyTemplate();
            isRunning = false;
            mainBorder.Background = resource.GetColor("signColor");
            IsEnabled = true;
            functionTextBlock.Text = originInfo;
        }
    }
}
