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
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void BilibiliLink_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://space.bilibili.com/23411401",
                    UseShellExecute = true,
                    Verb = "open",
                });
            }
            catch (Exception)
            {

            }
        }

        private void GithubLink_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://github.com/Minato302",
                    UseShellExecute = true,
                    Verb = "open",
                });
            }
            catch (Exception)
            {

            }
        }
    }
}
