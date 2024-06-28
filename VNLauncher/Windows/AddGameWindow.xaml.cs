#pragma warning disable IDE0049

using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VNLauncher.Controls;
using VNLauncher.FuntionalClasses;

namespace VNLauncher.Windows
{
    public partial class AddGameWindow : Window
    {
        private MainWindow mainWindow;
        private FileManager fileManager;
        
        public AddGameWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            fileManager = new FileManager();
           
            
            this.mainWindow = mainWindow;
            startWayQuestionButton.SetTips("    使用自动启动时，系统将尝试打开游戏的exe文件，在大部分情况下可以正常打开，适用于不需要转区运行的较新的游戏。\n" +
                "    而手动启动针对那些需要转区或系统无法直接打开的游戏，每次启动时需要您自己从文件夹中打开游戏，系统将寻找对应的游戏窗口并进行翻译。\n" +
                "    建议您先使用自动启动，如果发现无法启动、需要转区等问题导致系统无法启动后，再尝试使用手动启动。");
            
        }
        public AddGameWindow(MainWindow mainWindow,String gameName, String gamePath)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            fileManager = new FileManager();
            setGameNameTextBox.Text = gameName;
            gamePathTextBox.Text = gamePath;
        }
        private void OpenFileDialogButton_Click(Object sender, RoutedEventArgs e)
        {

            OpenFileDialog open = new OpenFileDialog();
            String gamePath = "";
            open.Filter = "Executable Files (*.exe)|*.exe";
            open.Title = "请选择游戏.exe路径";
            if (open.ShowDialog() == true)
            {
                gamePath = open.FileName;
            }
            gamePathTextBox.Text = gamePath;
            setGameNameTextBox.Text = Path.GetFileNameWithoutExtension(gamePath);
        }

        private void CloseButton_Click(Object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void InitialSetupButton_Click(Object sender, RoutedEventArgs e)
        {
            Process? gameProcess = null;
            try
            {
                if (gamePathTextBox.Text == "")
                {
                    throw new Exception("请选择游戏路径！");
                }
                if (setGameNameTextBox.Text == "")
                {
                    throw new Exception("请输入游戏名！");
                }
                foreach (MainWindowGameButton button in mainWindow.GameListButtons)
                {
                    if (setGameNameTextBox.Text == button.Game.Name)
                    {
                        throw new Exception("游戏库内已有同名游戏！");
                    }
                    if (gamePathTextBox.Text == button.Game.ExePath)
                    {
                        throw new Exception("游戏库内已有同路径游戏！");
                    }
                }
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = gamePathTextBox.Text;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;

                if (startModeCheckBox.SelectedMode == ChangeModeButton.Mode.Mode1)
                {
                    gameProcess = Process.Start(startInfo);
                    gameProcess!.WaitForInputIdle();
                    Thread.Sleep(1000);
                    IntPtr hWnd = gameProcess.MainWindowHandle;
                    if(hWnd == IntPtr.Zero)
                    {
                        throw new Exception("系统无法启动游戏，请检查路径或选择手动启动！");
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Source == "System.Diagnostics.Process")
                {
                    exceptionShowingTextBlock.Text = "系统无法启动游戏，请检查路径或选择手动启动";
                    return;
                }
                else
                {
                    exceptionShowingTextBlock.Text = ex.Message;
                    return;
                }
            }
            GuidanceWindow guidanceWindow = new GuidanceWindow(mainWindow, setGameNameTextBox.Text, gamePathTextBox.Text,startModeCheckBox.SelectedMode == ChangeModeButton.Mode.Mode1);
            guidanceWindow.Show();
            Close();
        }

        private void MainCanvas_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            startWayQuestionButton.ClosePopup();
        }

        private void MainCanvas_MouseRightButtonDown(Object sender, MouseButtonEventArgs e)
        {
            startWayQuestionButton.ClosePopup();
        }
    }
}
