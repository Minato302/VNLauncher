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
    /// <summary>
    /// StartGameTipsPage_AutoStart.xaml 的交互逻辑
    /// </summary>
    public partial class StartGameTipsPage_AutoStart : Page
    {
        private StartGameTipsWindow baseWindow;
        public StartGameTipsPage_AutoStart(StartGameTipsWindow baseWindow)
        {
            InitializeComponent();
            this.baseWindow = baseWindow;
        }
    }
}
