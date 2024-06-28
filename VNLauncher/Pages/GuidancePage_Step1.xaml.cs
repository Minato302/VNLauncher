#pragma warning disable IDE0049

using System.Windows.Controls;
using VNLauncher.FuntionalClasses;
using VNLauncher.Windows;

namespace VNLauncher.Pages
{
    public partial class GuidancePage_Step1 : Page
    {
        GlobalHook hook;
        GuidancePageParameter parameter;
        WindowInfo? gameWindowInfo;
        public GuidancePage_Step1(GuidancePageParameter parameter)
        {
            gameWindowInfo = null;
            InitializeComponent();
            windowTitleBlock.Text = "未选择窗口";
            parameter.BaseWindow.Title = "添加游戏_步骤1/3";
            hook = new GlobalHook(new List<(String, Action)> { ("键盘按键回车", GetWindow) });
            this.parameter = parameter;
            if (parameter.GameInfo.IsAutoStart)
            {
                mainTipsTextBlock.Text = "    系统已启动游戏，请等待启动完成后确保游戏窗口可见，将鼠标移动到游戏窗口内部，之后按下回车键以捕获游戏窗口，并确认下方的窗口名称正确。注意：如果此时游戏是全屏模式启动的，请先切换为窗口模式。";
            }
            else
            {
                mainTipsTextBlock.Text = "    您选择手动启动，请打开游戏窗口并确保游戏窗口可见，将鼠标移动到游戏窗口内部，之后按下回车键以捕获游戏窗口，并确认下方的窗口名称正确。注意：如果这是一个独占全屏的窗口，请先在游戏内修改其为窗口模式。";
            }
        }
        private void GetWindow()
        {
            WindowInfo? info = WindowsHandler.GetWindow();
            if (info != null)
            {
                windowTitleBlock.Text = ((WindowInfo)info).Title;
                gameWindowInfo = info;
            }
            else
            {
                gameWindowInfo = null;
                windowTitleBlock.Text = "未选择窗口";
            }
        }

        private void NextStepButton_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            if (gameWindowInfo != null)
            {
                parameter.GameInfo.GameWindow = ((WindowInfo)gameWindowInfo).Hwnd;
                parameter.GameInfo.GameWindowTitle = ((WindowInfo)gameWindowInfo).Title;
                parameter.GameInfo.GameWindowClass = ((WindowInfo)gameWindowInfo).ClassName;
                parameter.BaseWindow.Handoff(new GuidancePage_Step2(parameter), parameter.BaseWindow);

            }
            else
            {
                errorTipsTextBlock.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void CloseButton_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            parameter.BaseWindow.Close();
        }

        private void Page_Unloaded(Object sender, System.Windows.RoutedEventArgs e)
        {
            hook.UninstallHook();
        }

        private void BackButton_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            AddGameWindow addGameWindow = new AddGameWindow(parameter.MainWindow, parameter.GameInfo.GameName, parameter.GameInfo.GameExePath);
            parameter.BaseWindow.GoAway(addGameWindow);
        }
    }
}
