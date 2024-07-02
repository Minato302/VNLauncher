#pragma warning disable IDE0049

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace VNLauncher.FunctionalClasses
{
    public class WindowCapture
    {
        public enum CaptureMode
        {
            Window,
            FullScreenCut
        }


        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern Boolean PrintWindow(IntPtr hwnd, IntPtr hdcBlt, UInt32 nFlags);

        [DllImport("dwmapi.dll")]
        private static extern Int32 DwmGetWindowAttribute(IntPtr hwnd, Int32 dwAttribute, out WindowsHandler.LPRECT pvAttribute, Int32 cbAttribute);

        private static Double GetDpiFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            return g.DpiX / 96.0;
        }
        private static Bitmap CaptureScreen()
        {

            Int32 width = Convert.ToInt32(GetDpiFactor() * SystemParameters.PrimaryScreenWidth);
            Int32 height = Convert.ToInt32(GetDpiFactor() * SystemParameters.PrimaryScreenHeight);

            Bitmap bmpScreenCapture = new Bitmap(width, height);

            Graphics g = Graphics.FromImage(bmpScreenCapture);
            IntPtr hdc = g.GetHdc();
            IntPtr hScreenDC = GetDC(IntPtr.Zero);
            BitBlt(hdc, 0, 0, width, height, hScreenDC, 0, 0, TernaryRasterOperations.SRCCOPY);
            g.ReleaseHdc(hdc);
            ReleaseDC(IntPtr.Zero, hScreenDC);
            return bmpScreenCapture;
        }
        [DllImport("gdi32.dll")]
        private static extern Boolean BitBlt(IntPtr hdcDest, Int32 nXDest, Int32 nYDest, Int32 nWidth, Int32 nHeight, IntPtr hdcSrc, Int32 nXSrc, Int32 nYSrc, TernaryRasterOperations dwRop);

        private enum TernaryRasterOperations : UInt32
        {
            SRCCOPY = 0x00CC0020
        }
        public static Bitmap Shot(IntPtr hWnd, CaptureMode mode)
        {
            Bitmap windowBitmap;
            WindowInfo window = new WindowInfo(hWnd);
            if (window.Bounds.Width == 0 && window.Bounds.Height == 0)
            {
                return new Bitmap(1, 1);
            }
            if (mode == CaptureMode.FullScreenCut)
            {
                Bitmap screenBitmap = CaptureScreen();
                windowBitmap = new Bitmap(window.Bounds.Width, window.Bounds.Height);
                Graphics g = Graphics.FromImage(windowBitmap);
                g.DrawImage(screenBitmap, 0, 0, window.Bounds, GraphicsUnit.Pixel);
                screenBitmap.Dispose();
            }
            else
            {
                windowBitmap = new Bitmap(window.Bounds.Width, window.Bounds.Height);
                Graphics graphics = Graphics.FromImage(windowBitmap);
                IntPtr hdc = graphics.GetHdc();
                PrintWindow(hWnd, hdc, 1);
                graphics.ReleaseHdc(hdc);
                windowBitmap=ImageHandler.ResizeToFullImage(windowBitmap);
            }
            return windowBitmap;
        }
        public static Bitmap Shot(IntPtr hWnd, CaptureMode mode, Game.CaptionLocation location)
        {
            return ImageHandler.CropToCaption(Shot(hWnd, mode), location);
        }

        public static BitmapSource ShotToBitmapSource(IntPtr hWnd, CaptureMode mode)
        {

            Bitmap windowBitmap = Shot(hWnd, mode);
            IntPtr hBitmap = windowBitmap.GetHbitmap();
            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(hBitmap);
            windowBitmap.Dispose();
            return bitmapSource;
        }
        [DllImport("gdi32.dll")]
        private static extern Boolean DeleteObject(IntPtr hObject);


        private static Boolean HasBlankRegion(Bitmap bitmap)
        {
            Int32 checkRegionSize = 10;
            for (Int32 x = bitmap.Width - checkRegionSize; x < bitmap.Width; x++)
            {
                for (Int32 y = bitmap.Height - checkRegionSize; y < bitmap.Height; y++)
                {
                    if (bitmap.GetPixel(x, y).ToArgb() != Color.Empty.ToArgb())
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
