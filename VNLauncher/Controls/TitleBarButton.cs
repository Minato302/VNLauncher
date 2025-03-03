﻿#pragma warning disable IDE0049
#pragma warning disable CS8618

using FontAwesome.WPF;
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
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Controls
{
    public class TitleBarButton : Button
    {
        static TitleBarButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitleBarButton), new FrameworkPropertyMetadata(typeof(TitleBarButton)));
        }

        public static readonly DependencyProperty TitleBarButtonIconProperty =
    DependencyProperty.Register("TitleBarButtonIcon", typeof(FontAwesomeIcon), typeof(TitleBarButton));
        private LocalColorAcquirer resource;

        public FontAwesomeIcon TitleBarButtonIcon
        {
            get { return (FontAwesomeIcon)GetValue(TitleBarButtonIconProperty); }
            set { SetValue(TitleBarButtonIconProperty, value); }
        }
        private Border mainBorder;
        public TitleBarButton()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {
                mainBorder!.Background = resource.GetColor("mainWindowTitleBarButtonColor_MouseEnter");
            };
            MouseLeave += (sender, e) =>
            {
                mainBorder!.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            };
            PreviewMouseLeftButtonDown += (sender, e) =>
            {
                mainBorder!.Background = resource.GetColor("signColor");
            };
            PreviewMouseLeftButtonUp += (sender, e) =>
            {
                mainBorder!.Background = resource.GetColor("mainWindowTitleBarButtonColor_MouseEnter");
            };
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainBorder = (Template.FindName("mainBorder", this) as Border)!;
        }
    }
}
