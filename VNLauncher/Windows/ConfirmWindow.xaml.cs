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
using System.Windows.Shapes;

namespace VNLauncher.Windows
{
    public partial class ConfirmWindow : Window
    {
        private MainWindow mainWindow;
        public ConfirmWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void CancelButton_Click(Object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ConfirmButton_Click(Object sender, RoutedEventArgs e)
        {
            mainWindow.RemoveSelectedGame();
            Close();
        }
    }
}
