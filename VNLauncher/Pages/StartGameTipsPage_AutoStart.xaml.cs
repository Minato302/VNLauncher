#pragma warning disable IDE0049

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

namespace VNLauncher.Pages
{

    public partial class StartGameTipsPage_AutoStart : Page
    {
        private StartGameTipsWindow baseWindow;
        public StartGameTipsPage_AutoStart(StartGameTipsWindow baseWindow)
        {
            InitializeComponent();
            this.baseWindow = baseWindow;
        }

        private void CloseButton_Click(Object sender, RoutedEventArgs e)
        {
            baseWindow.Game.EndGame();
            baseWindow.GameProcess?.Kill();
            baseWindow.Close();
        }
    }
}
