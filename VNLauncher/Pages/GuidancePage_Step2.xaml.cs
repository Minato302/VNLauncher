#pragma warning disable IDE0049

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Pages
{
    public partial class GuidancePage_Step2 : Page
    {
        private GuidancePageParameter parameter;
        private GlobalHook hook;
        private Boolean getCover;
        private Bitmap cover;
        public GuidancePage_Step2(GuidancePageParameter parameter)
        {
            InitializeComponent();
            parameter.BaseWindow.Title = "添加游戏_步骤2/3";
            this.parameter = parameter;
            cover = new Bitmap(1, 1);
            hook = new GlobalHook(new List<(String, Action)> { ("键盘按键回车", GetCover) });
            WindowsHandler.MaximizeAndFullscreen(parameter.GameInfo.GameWindow);

            getCover = false;
        }

        private void CloseButton_Click(Object sender, RoutedEventArgs e)
        {
            hook.UninstallHook();
            parameter.BaseWindow.Close();
        }
        private void GetCover()
        {
            getCover = true;
            if (screenShotModeButton.SelectedMode == Controls.ChangeModeButton.Mode.Mode1)
            {
                BitmapSource image = WindowCapture.ShotToBitmapSource(parameter.GameInfo.GameWindow, WindowCapture.CaptureMode.Window);
                coverImage.Source = image;
                cover = WindowCapture.Shot(parameter.GameInfo.GameWindow, WindowCapture.CaptureMode.Window);
            }
            else
            {
                parameter.BaseWindow.WindowState = WindowState.Minimized;
                BitmapSource image = WindowCapture.ShotToBitmapSource(parameter.GameInfo.GameWindow, WindowCapture.CaptureMode.FullScreenCut);
                coverImage.Source = image;
                cover = WindowCapture.Shot(parameter.GameInfo.GameWindow, WindowCapture.CaptureMode.FullScreenCut);
                parameter.BaseWindow.WindowState = WindowState.Normal;
            }

        }

        private void NextStepButton_Click(Object sender, RoutedEventArgs e)
        {
            if (getCover)
            {
                parameter.GameInfo.IsWindowShot = screenShotModeButton.SelectedMode == Controls.ChangeModeButton.Mode.Mode1;
                parameter.GameInfo.GameCover = cover;
                parameter.BaseWindow.Handoff(new GuidancePage_Step3(parameter), parameter.BaseWindow);
            }
            else
            {
                errorTipsBlock.Visibility = Visibility.Visible;
            }
        }

        private void BackButton_Click(Object sender, RoutedEventArgs e)
        {
            parameter.BaseWindow.Title = "添加游戏_步骤1/3";
            parameter.BaseWindow.Handoff(new GuidancePage_Step1(parameter), parameter.BaseWindow);
        }

        private void Page_Unloaded(Object sender, RoutedEventArgs e)
        {
            hook.UninstallHook();
        }
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, Int32 nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, Int32 nIndex, IntPtr dwNewLong);
        [DllImport("user32.dll")]
        private static extern Boolean SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, Int32 X, Int32 Y, Int32 cx, Int32 cy, UInt32 uFlags);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern Boolean PostMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

    }
}
