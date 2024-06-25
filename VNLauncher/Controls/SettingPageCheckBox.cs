﻿#pragma warning disable IDE0049
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
    public class SettingPageCheckBox : Control
    {
        public static readonly DependencyProperty SettingPageCheckBoxTextProperty =
            DependencyProperty.Register("SettingPageCheckBoxText", typeof(String), typeof(SettingPageCheckBox));

        public String SettingPageCheckBoxText
        {
            get
            {
                return (String)GetValue(SettingPageCheckBoxTextProperty);
            }
            set
            {
                SetValue(SettingPageCheckBoxTextProperty, value);
            }
        }
        static SettingPageCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SettingPageCheckBox), new FrameworkPropertyMetadata(typeof(SettingPageCheckBox)));
        }
    }
}