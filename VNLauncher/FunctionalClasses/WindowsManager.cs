#pragma warning disable IDE0049

using System.Drawing;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;

namespace VNLauncher.FunctionalClasses
{
    public class WindowsHandler
    {
        private static readonly Predicate<WindowInfo> DefaultPredicate = x => x.IsVisible && x.Title.Length > 0;
        public static IReadOnlyList<WindowInfo> FindAll(Predicate<WindowInfo>? match = null)
        {
            List<WindowInfo> windowList = new List<WindowInfo>();
            EnumWindows(OnWindowEnum, 0);
            return windowList.FindAll(match ?? DefaultPredicate);

            Boolean OnWindowEnum(IntPtr hWnd, Int32 lparam)
            {
                if (GetParent(hWnd) == IntPtr.Zero)
                {
                    StringBuilder lpString = new StringBuilder(512);
                    GetClassName(hWnd, lpString, lpString.Capacity);
                    String className = lpString.ToString();
                    StringBuilder lptrString = new StringBuilder(512);
                    GetWindowText(hWnd, lptrString, lptrString.Capacity);
                    String title = lptrString.ToString().Trim();
                    Boolean isVisible = IsWindowVisible(hWnd);
                    LPRECT rect = default;
                    GetWindowRect(hWnd, ref rect);
                    Rectangle bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                    windowList.Add(new WindowInfo(hWnd, className, title, isVisible, bounds));
                }
                return true;
            }

        }
        public static WindowInfo? GetWindow()
        {
            POINT? lpPoint = GlobalHook.GetMousePosition();
            if (lpPoint != null)
            {
                IntPtr hwnd = WindowFromPoint((POINT)lpPoint);
                IntPtr parentHwnd;
                while (true)
                {
                    parentHwnd = GetParent(hwnd);
                    if (parentHwnd == IntPtr.Zero)
                    {
                        break;
                    }
                    hwnd = parentHwnd;
                }

                IReadOnlyList<WindowInfo> windows = FindAll();
                foreach (WindowInfo window in windows)
                {
                    if (window.Hwnd == hwnd)
                    {
                        return window;
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public static WindowInfo? GetWindow(String title, String className)
        {
            IReadOnlyList<WindowInfo> windows = FindAll();
            WindowInfo? gameMainWindow = null;
            List<WindowInfo> gameWindows = new List<WindowInfo>();
            foreach (WindowInfo ninfo in windows)
            {
                if (ninfo.Title == title && ninfo.ClassName == className)
                {
                    gameWindows.Add(ninfo);
                }
            }
            Int32 maxArea = 0;
            foreach (WindowInfo ninfo in gameWindows)
            {
                if (ninfo.Bounds.Width * ninfo.Bounds.Width >= maxArea)
                {
                    gameMainWindow = ninfo;
                    maxArea = ninfo.Bounds.Width * ninfo.Bounds.Height;
                }
            }
            return gameMainWindow;
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

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)] 
        private static extern Boolean MoveWindow(IntPtr hWnd, Int32 X, Int32 Y, Int32 nWidth, Int32 nHeight, Boolean bRepaint);
        [DllImport("user32.dll", SetLastError = true)] 
        private static extern Boolean GetClientRect(IntPtr hWnd, out WindowsHandler.LPRECT lpRect);


        private static Int32 GWL_STYLE = -16;
        private static UInt32 WS_CAPTION = 0x00C00000;
        private static UInt32 WS_THICKFRAME = 0x00040000;
        private static UInt32 WS_SYSMENU = 0x00080000;
        private static UInt32 WS_POPUP = 0x80000000;

        private static UInt32 SWP_NOSIZE = 0x0001;
        private static UInt32 SWP_NOMOVE = 0x0002;
        private static UInt32 SWP_NOZORDER = 0x0004;
        private static UInt32 SWP_FRAMECHANGED = 0x0020;

        private static UInt32 WM_SYSCOMMAND = 0x0112;

        private static Int32 GWL_EXSTYLE = -20;
        private static UInt32 WS_EX_DLGMODALFRAME = 0x00000001;
        private static UInt32 WS_EX_CLIENTEDGE = 0x00000200;
        private static UInt32 WS_EX_STATICEDGE = 0x00020000;
        private static UInt32 WS_EX_WINDOWEDGE = 0x00000100;
        private static UInt32 WS_EX_LAYERED = 0x00080000;

        private const UInt32 SC_MAXIMIZE = 0xF030;
        private const UInt32 SC_RESTORE = 0xF120;

        public static void RemoveUI(IntPtr hWnd)
        {
            IntPtr stylePtr = GetWindowLongPtr(hWnd, GWL_STYLE);
            Int64 style = stylePtr.ToInt64();
            style &= ~WS_CAPTION; style &= ~WS_THICKFRAME; style &= ~WS_SYSMENU;
            SetWindowLongPtr(hWnd, GWL_STYLE, new IntPtr(style));
            IntPtr exStylePtr = GetWindowLongPtr(hWnd, GWL_EXSTYLE);
            Int64 exStyle = exStylePtr.ToInt64();
            exStyle &= ~(WS_EX_DLGMODALFRAME | WS_EX_CLIENTEDGE | WS_EX_STATICEDGE | WS_EX_WINDOWEDGE | WS_EX_LAYERED);
            SetWindowLongPtr(hWnd, GWL_EXSTYLE, new IntPtr(exStyle));
            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED);
            PostMessage(hWnd, WM_SYSCOMMAND, (IntPtr)SC_MAXIMIZE, IntPtr.Zero);
            PostMessage(hWnd, WM_SYSCOMMAND, (IntPtr)SC_RESTORE, IntPtr.Zero);

        }
        public static void RestoreUI(IntPtr hWnd)
        {
            IntPtr stylePtr = GetWindowLongPtr(hWnd, GWL_STYLE);
            Int64 style = stylePtr.ToInt64();
            style |= WS_CAPTION | WS_THICKFRAME | WS_SYSMENU;
            SetWindowLongPtr(hWnd, GWL_STYLE, new IntPtr(style));

            IntPtr exStylePtr = GetWindowLongPtr(hWnd, GWL_EXSTYLE);
            Int64 exStyle = exStylePtr.ToInt64();
            exStyle |= WS_EX_DLGMODALFRAME | WS_EX_CLIENTEDGE | WS_EX_STATICEDGE | WS_EX_WINDOWEDGE | WS_EX_LAYERED;
            SetWindowLongPtr(hWnd, GWL_EXSTYLE, new IntPtr(exStyle));

            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED);
            PostMessage(hWnd, WM_SYSCOMMAND, (IntPtr)SC_MAXIMIZE, IntPtr.Zero);
            PostMessage(hWnd, WM_SYSCOMMAND, (IntPtr)SC_RESTORE, IntPtr.Zero);
        }
        public static void MaximizeAndFullscreen(IntPtr hWnd)
        {
            IntPtr stylePtr = GetWindowLongPtr(hWnd, GWL_STYLE);
            Int64 style = stylePtr.ToInt64();
            style &= ~WS_CAPTION; style &= ~WS_THICKFRAME; style &= ~WS_SYSMENU;
            SetWindowLongPtr(hWnd, GWL_STYLE, new IntPtr(style));
            IntPtr exStylePtr = GetWindowLongPtr(hWnd, GWL_EXSTYLE);
            Int64 exStyle = exStylePtr.ToInt64();
            exStyle &= ~(WS_EX_DLGMODALFRAME | WS_EX_CLIENTEDGE | WS_EX_STATICEDGE | WS_EX_WINDOWEDGE | WS_EX_LAYERED);
            SetWindowLongPtr(hWnd, GWL_EXSTYLE, new IntPtr(exStyle));
            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED);
            PostMessage(hWnd, WM_SYSCOMMAND, (IntPtr)SC_MAXIMIZE, IntPtr.Zero);
        }
        public static void RestoreToNormalWindow(IntPtr hWnd)
        {
            IntPtr stylePtr = GetWindowLongPtr(hWnd, GWL_STYLE);
            Int64 style = stylePtr.ToInt64();
            style |= WS_CAPTION | WS_THICKFRAME | WS_SYSMENU;
            SetWindowLongPtr(hWnd, GWL_STYLE, new IntPtr(style));
            IntPtr exStylePtr = GetWindowLongPtr(hWnd, GWL_EXSTYLE);
            Int64 exStyle = exStylePtr.ToInt64();
            exStyle |= WS_EX_DLGMODALFRAME | WS_EX_CLIENTEDGE | WS_EX_STATICEDGE | WS_EX_WINDOWEDGE | WS_EX_LAYERED;
            SetWindowLongPtr(hWnd, GWL_EXSTYLE, new IntPtr(exStyle));
            SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED);
            PostMessage(hWnd, WM_SYSCOMMAND, (IntPtr)SC_RESTORE, IntPtr.Zero);
        }


        public static void Close(IntPtr hWnd)
        {
            UInt32 WM_CLOSE = 0x0010;
            SendMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
        }

        private delegate Boolean WndEnumProc(IntPtr hWnd, Int32 lParam);
        [DllImport("user32")]
        private static extern IntPtr WindowFromPoint(POINT point);

        [DllImport("user32")]
        private static extern Boolean EnumWindows(WndEnumProc lpEnumFunc, Int32 lParam);

        [DllImport("user32")]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32")]
        private static extern Boolean IsWindowVisible(IntPtr hWnd);

        [DllImport("user32")]
        private static extern Int32 GetWindowText(IntPtr hWnd, StringBuilder lptrString, Int32 nMaxCount);
        [DllImport("user32")]
        private static extern Int32 GetClassName(IntPtr hWnd, StringBuilder lpString, Int32 nMaxCount);

        [DllImport("user32")]
        private static extern void SwitchToThisWindow(IntPtr hWnd, Boolean fAltTab);
        [DllImport("user32")]
        private static extern Boolean GetWindowRect(IntPtr hWnd, ref LPRECT rect);


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public Int32 X;
            public Int32 Y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public readonly struct LPRECT
        {
            public readonly Int32 Left;
            public readonly Int32 Top;
            public readonly Int32 Right;
            public readonly Int32 Bottom;
        }
    }

    public struct WindowInfo
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean GetWindowRect(IntPtr hWnd, ref WindowsHandler.LPRECT lpRect);
        [DllImport("user32")]
        private static extern Int32 GetClassName(IntPtr hWnd, StringBuilder lpString, Int32 nMaxCount);

        [DllImport("user32")]
        private static extern Int32 GetWindowText(IntPtr hWnd, StringBuilder lptrString, Int32 nMaxCount);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern Boolean PrintWindow(IntPtr hwnd, IntPtr hdcBlt, UInt32 nFlags);

        [DllImport("user32.dll")]
        private static extern Boolean IsWindowVisible(IntPtr hWnd);

        public WindowInfo(IntPtr hWnd)
        {
            this.hWnd = hWnd;
            StringBuilder lpString = new StringBuilder(512);
            GetClassName(hWnd, lpString, lpString.Capacity);
            String className = lpString.ToString();
            StringBuilder lptrString = new StringBuilder(512);
            GetWindowText(hWnd, lptrString, lptrString.Capacity);
            String title = lptrString.ToString().Trim();
            Boolean isVisible = IsWindowVisible(hWnd);
            WindowsHandler.LPRECT rect = default;
            GetWindowRect(hWnd, ref rect);
            Rectangle bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

            this.className = className;
            this.title = title;
            this.isVisible = isVisible;
            this.bounds = bounds;

        }
        public WindowInfo(IntPtr hWnd, String className, String title, Boolean isVisible, Rectangle bounds)
        {
            this.hWnd = hWnd;
            this.className = className;
            this.title = title;
            this.isVisible = isVisible;
            this.bounds = bounds;
        }

        private IntPtr hWnd;
        public IntPtr Hwnd
        {
            get
            {
                return hWnd;
            }
        }
        private String className;

        public String ClassName
        {
            get
            {
                return className;
            }
        }
        private String title;
        public String Title
        {
            get
            {
                return title;
            }
        }
        private Boolean isVisible;
        public Boolean IsVisible
        {
            get
            {
                return isVisible;
            }
        }
        private Rectangle bounds;
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
        }
        public Boolean IsMinimized => Bounds.Left <= -30000 && Bounds.Top <= -30000;

    }
}
