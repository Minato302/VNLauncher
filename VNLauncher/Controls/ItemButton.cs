﻿#pragma warning disable IDE0049
#pragma warning disable CS8618

using FontAwesome.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class ItemButton : Button
    {
        static ItemButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ItemButton), new FrameworkPropertyMetadata(typeof(ItemButton)));
        }
        public static readonly DependencyProperty ItemButtonIconProperty =
          DependencyProperty.Register("ItemButtonIcon", typeof(FontAwesomeIcon), typeof(ItemButton));
        private LocalColorAcquirer resource;
        private Border mainBorder;

        public FontAwesomeIcon ItemButtonIcon
        {
            get
            {
                return (FontAwesomeIcon)GetValue(ItemButtonIconProperty);
            }
            set
            {
                SetValue(ItemButtonIconProperty, value);
            }
        }
        public ItemButton()
        {
            resource = new LocalColorAcquirer();
            MouseEnter += (sender, e) =>
            {
                Cursor = Cursors.Hand;
                mainBorder!.Background = resource.GetColor("itemButtonColor_MouseEnter");
            };
            MouseLeave += (sender, e) =>
            {
                Cursor = Cursors.Arrow;
                mainBorder!.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            };
            PreviewMouseLeftButtonDown += (sender, e) =>
            {
                mainBorder!.Background = resource.GetColor("signColor");
            };
            PreviewMouseLeftButtonUp += (sender, e) =>
            {
                mainBorder!.Background = resource.GetColor("itemButtonColor_MouseEnter");
            };
        }
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            mainBorder = (Template.FindName("mainBorder", this) as Border)!;
        }
    }
}

