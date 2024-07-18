#pragma warning disable IDE0049

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using VNLauncher.Controls;

namespace VNLauncher.Pages
{
    public partial class StartGameTipsPage_ManualStart : Page
    {
        private StartGameTipsWindow baseWindow;
        public StartGameTipsPage_ManualStart(StartGameTipsWindow baseWindow)
        {
            InitializeComponent();
            this.baseWindow = baseWindow;
        }

        private void CloseButton_Click(Object sender, RoutedEventArgs e)
        {
            baseWindow.Close();
        }

        private void OpenGameFileButton_Click(Object sender, RoutedEventArgs e)
        {
            String folderPath = System.IO.Path.GetDirectoryName(baseWindow.Game.ExePath)!;
            Process.Start("explorer.exe", folderPath);
        }
    }
}
