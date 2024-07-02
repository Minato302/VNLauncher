#pragma warning disable IDE0049

using System.Windows;
using System.Windows.Controls;
using VNLauncher.Controls;
using VNLauncher.FunctionalClasses;
using VNLauncher.Windows;

namespace VNLauncher.Pages
{
    public partial class GuidancePage_Step3 : Page
    {
        private GuidancePageParameter parameter;
        private TransparentWindow tWindow;
        private GlobalHook hook;
        private FileManager fileManager;
        public GuidancePage_Step3(GuidancePageParameter parameter)
        {
            this.parameter = parameter;
            fileManager = new FileManager();
            parameter.BaseWindow.Title = "添加游戏_步骤3/3";
            tWindow = new TransparentWindow(parameter.GameInfo.GameWindow);
            InitializeComponent();
            hook = new GlobalHook(new List<(String, Action)> { ("键盘按键↑", UpKeyPressed), ("键盘按键←", LeftKeyPressed), ("键盘按键↓", DownKeyPressed), ("键盘按键→", RightKeyPressed) });
        }
        private void Page_Initialized(Object sender, EventArgs e)
        {
            tWindow.Show();
        }

        private void EndButton_Click(Object sender, RoutedEventArgs e)
        {
            fileManager.CreateNewGameToFile(parameter.GameInfo.GameName, parameter.GameInfo.GameExePath, parameter.GameInfo.GameCover,
                parameter.GameInfo.IsAutoStart, parameter.GameInfo.IsWindowShot, parameter.GameInfo.GameWindowTitle, parameter.GameInfo.GameWindowClass, tWindow.GameCaptionLocation);
            parameter.MainWindow.gameListStackPanel.Children.Add(new MainWindowGameButton(parameter.GameInfo.GameName, parameter.MainWindow));
            Page_Unloaded(sender, e);
            parameter.BaseWindow.GoAway(null);
            WindowsHandler.Close(parameter.GameInfo.GameWindow);
        }
        private void LeftKeyPressed()
        {
            if (adjustSideButton.Side == GuidancePageAdjustSideButton.AdjustSide.RightDown)
            {
                tWindow.RightSideLeftMove();
            }
            else
            {
                tWindow.LeftSideLeftMove();
            }
        }
        private void RightKeyPressed()
        {
            if (adjustSideButton.Side == GuidancePageAdjustSideButton.AdjustSide.RightDown)
            {
                tWindow.RightSideRightMove();
            }
            else
            {
                tWindow.LeftSideRightMove();
            }
        }
        private void UpKeyPressed()
        {
            if (adjustSideButton.Side == GuidancePageAdjustSideButton.AdjustSide.RightDown)
            {
                tWindow.DownSideUpMove();
            }
            else
            {
                tWindow.UpSideUpMove();
            }
        }
        private void DownKeyPressed()
        {
            if (adjustSideButton.Side == GuidancePageAdjustSideButton.AdjustSide.RightDown)
            {
                tWindow.DownSideDownMove();
            }
            else
            {
                tWindow.UpSideDownMove();
            }
        }

        private void CloseButton_Click(Object sender, RoutedEventArgs e)
        {
            parameter.BaseWindow.Close();
        }

        private void BackButton_Click(Object sender, RoutedEventArgs e)
        {
            parameter.BaseWindow.Title = "添加游戏_步骤2/3";
            parameter.BaseWindow.Handoff(new GuidancePage_Step2(parameter), parameter.BaseWindow);
        }

        private void Page_Unloaded(Object sender, RoutedEventArgs e)
        {
            hook.UninstallHook();
            tWindow.Close();
        }
    }
}
