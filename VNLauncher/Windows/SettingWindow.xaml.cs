#pragma warning disable IDE0049

using System.Reflection.Metadata;
using System.Windows;
using VNLauncher.Controls;

namespace VNLauncher.Windows
{
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();
        }
        private void MinimizeButton_Click(Object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(Object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OcrSettingButton_Click(Object sender, RoutedEventArgs e)
        {

            if (!ocrSettingButton.IsSelected)
            {
                foreach (SettingWindowSetItemButton button in settingItemStackPanel.Children)
                {
                    button.RelieveSelected();
                }
                ocrSettingButton.BeingSelected();
                settingInfoFrame.Navigate(new Pages.OCRSettingPage());
            }
        }

        private void KeymappingButton_Click(Object sender, RoutedEventArgs e)
        {
            if (!keymappingButton.IsSelected)
            {
                foreach (SettingWindowSetItemButton button in settingItemStackPanel.Children)
                {
                    button.RelieveSelected();
                }
                keymappingButton.BeingSelected();
                settingInfoFrame.Navigate(new Pages.OCRSettingPage());
            }
        }

        private void BaiduTranslateSettingButton_Click(Object sender, RoutedEventArgs e)
        {
            foreach (SettingWindowSetItemButton button in settingItemStackPanel.Children)
            {
                button.RelieveSelected();
            }
            baiduTranslateSettingButton.BeingSelected();
        }

        private void GptTranslateSettingButton_Click(Object sender, RoutedEventArgs e)
        {
            foreach (SettingWindowSetItemButton button in settingItemStackPanel.Children)
            {
                button.RelieveSelected();
            }
            gptTranslateSettingButton.BeingSelected();
        }

        private void LocalTranslateSettingButton_Click(Object sender, RoutedEventArgs e)
        {
            foreach (SettingWindowSetItemButton button in settingItemStackPanel.Children)
            {
                button.RelieveSelected();
            }
            localTranslateSettingButton.BeingSelected();
        }

        private void AboutButton_Click(Object sender, RoutedEventArgs e)
        {
            foreach (SettingWindowSetItemButton button in settingItemStackPanel.Children)
            {
                button.RelieveSelected();
            }
            aboutButton.BeingSelected();
        }
    }
}
