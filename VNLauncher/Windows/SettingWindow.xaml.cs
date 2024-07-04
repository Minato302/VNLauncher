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
            foreach (SettingWindowSetItemButton button in settingItemStackPanel.Children)
            {
                button.RelieveSelected();
            }
            ocrSettingButton.BeingSelected();
            settingInfoFrame.Navigate(new Pages.OCRSettingPage());
        }

        private void KeymappingButton_Click(Object sender, RoutedEventArgs e)
        {

            foreach (SettingWindowSetItemButton button in settingItemStackPanel.Children)
            {
                button.RelieveSelected();
            }
            keymappingButton.BeingSelected();
            settingInfoFrame.Navigate(new Pages.KeymappingPage());
        }

        private void BaiduTranslateSettingButton_Click(Object sender, RoutedEventArgs e)
        {
            foreach (SettingWindowSetItemButton button in settingItemStackPanel.Children)
            {
                button.RelieveSelected();
            }
            baiduTranslateSettingButton.BeingSelected();
            settingInfoFrame.Navigate(new Pages.BaiduTranslateSettingPage());
        }

        private void OnlineModelTranslateSettingButton_Click(Object sender, RoutedEventArgs e)
        {
            foreach (SettingWindowSetItemButton button in settingItemStackPanel.Children)
            {
                button.RelieveSelected();
            }
            onlineModelTranslateSettingButton.BeingSelected();
            settingInfoFrame.Navigate(new Pages.OnlineModelTranslateSettingPage());
        }

        private void LocalTranslateSettingButton_Click(Object sender, RoutedEventArgs e)
        {
            foreach (SettingWindowSetItemButton button in settingItemStackPanel.Children)
            {
                button.RelieveSelected();
            }
            settingInfoFrame.Navigate(new Pages.LocalTranslateSettingPage());
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

        private void MouseDragMove(Object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            keymappingButton.BeingSelected();
            settingInfoFrame.Navigate(new Pages.KeymappingPage());
        }
    }
}
