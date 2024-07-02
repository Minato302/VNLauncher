#pragma warning disable IDE0049

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using VNLauncher.FunctionalClasses;
using static VNLauncher.FunctionalClasses.Game;

namespace VNLauncher.Windows
{

    public partial class TransparentWindow : Window
    {
        private IntPtr hWnd;
        private Double leftRate;
        private Double rightRate;
        private Double upRate;
        private Double downRate;
        private Double captionHorizontalRate;
        private Double captionVerticalRate;

        public CaptionLocation GameCaptionLocation
        {
            get
            {
                return new CaptionLocation(leftRate, rightRate, upRate, downRate, captionHorizontalRate, captionVerticalRate);
            }
        }



        public TransparentWindow(IntPtr hWnd, CaptionLocation? captionLocation = null)
        {
            InitializeComponent();
            this.hWnd = hWnd;
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Left = 0;
            Top = 0;

            WindowsHandler.LPRECT gameWindowLocation;
            GetWindowRect(hWnd, out gameWindowLocation);

            mainCavas.Width = Width;
            mainCavas.Height = Height;

            gameWindowCavas.Width = (gameWindowLocation.Right - gameWindowLocation.Left) / GetDpiFactor();
            gameWindowCavas.Height = (gameWindowLocation.Bottom - gameWindowLocation.Top) / GetDpiFactor();

            gameWindowCavas.SetValue(Canvas.LeftProperty, gameWindowLocation.Left / GetDpiFactor());
            gameWindowCavas.SetValue(Canvas.TopProperty, gameWindowLocation.Top / GetDpiFactor());

            gameWindowGrid.Width = gameWindowCavas.Width;
            gameWindowGrid.Height = gameWindowCavas.Height;
            gameWindowGrid.SetValue(Canvas.LeftProperty, 0D);
            gameWindowGrid.SetValue(Canvas.TopProperty, 0D);

            if (captionLocation == null)
            {
                leftRate = 1;
                rightRate = 1;
                upRate = 4;
                downRate = 1;
                captionHorizontalRate = 4;
                captionVerticalRate = 1;
            }
            else
            {
                CaptionLocation location = (CaptionLocation)captionLocation;
                leftRate = location.LeftRate;
                rightRate = location.RightRate;
                upRate = location.UpRate;
                downRate = location.DownRate;
                captionHorizontalRate = location.CaptionHorizontalRate;
                captionVerticalRate = location.CaptionVerticalRate;
            }

            leftColumn.Width = new GridLength(leftRate, GridUnitType.Star);
            rightColumn.Width = new GridLength(rightRate, GridUnitType.Star);
            captionColumn.Width = new GridLength(captionHorizontalRate, GridUnitType.Star);
            upRow.Height = new GridLength(upRate, GridUnitType.Star);
            downRow.Height = new GridLength(downRate, GridUnitType.Star);
            captionRow.Height = new GridLength(captionVerticalRate, GridUnitType.Star);


            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern Boolean GetWindowRect(IntPtr hWnd, out WindowsHandler.LPRECT lpRect);
        private void CompositionTarget_Rendering(Object? sender, EventArgs e)
        {
            WindowsHandler.LPRECT gameWindowLocation;
            GetWindowRect(hWnd, out gameWindowLocation);

            gameWindowCavas.Width = (gameWindowLocation.Right - gameWindowLocation.Left) / GetDpiFactor();
            gameWindowCavas.Height = (gameWindowLocation.Bottom - gameWindowLocation.Top) / GetDpiFactor();

            gameWindowCavas.SetValue(Canvas.LeftProperty, gameWindowLocation.Left / GetDpiFactor());
            gameWindowCavas.SetValue(Canvas.TopProperty, gameWindowLocation.Top / GetDpiFactor());

            gameWindowGrid.Width = gameWindowCavas.Width;
            gameWindowGrid.Height = gameWindowCavas.Height;

        }
        private static Double GetDpiFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            return g.DpiX / 96.0;
        }

        public void RightSideLeftMove()
        {
            Topmost = true;
            if (captionHorizontalRate >= 0.001 + CaptionLocation.AdjustPace)
            {
                rightRate += CaptionLocation.AdjustPace;
                rightColumn.Width = new GridLength(rightRate, GridUnitType.Star);
                captionHorizontalRate -= CaptionLocation.AdjustPace;
                captionColumn.Width = new GridLength(captionHorizontalRate, GridUnitType.Star);
            }
        }
        public void RightSideRightMove()
        {
            Topmost = true;
            if (rightRate >= 0.001 + CaptionLocation.AdjustPace)
            {
                rightRate -= CaptionLocation.AdjustPace;
                rightColumn.Width = new GridLength(rightRate, GridUnitType.Star);
                captionHorizontalRate += CaptionLocation.AdjustPace;
                captionColumn.Width = new GridLength(captionHorizontalRate, GridUnitType.Star);
            }
        }
        public void LeftSideLeftMove()
        {
            Topmost = true;
            if (leftRate >= 0.001 + CaptionLocation.AdjustPace)
            {
                leftRate -= CaptionLocation.AdjustPace;
                leftColumn.Width = new GridLength(leftRate, GridUnitType.Star);
                captionHorizontalRate += CaptionLocation.AdjustPace;
                captionColumn.Width = new GridLength(captionHorizontalRate, GridUnitType.Star);
            }
        }
        public void LeftSideRightMove()
        {
            Topmost = true;
            if (captionHorizontalRate >= 0.001 + CaptionLocation.AdjustPace)
            {
                leftRate += CaptionLocation.AdjustPace;
                leftColumn.Width = new GridLength(leftRate, GridUnitType.Star);
                captionHorizontalRate -= CaptionLocation.AdjustPace;
                captionColumn.Width = new GridLength(captionHorizontalRate, GridUnitType.Star);
            }
        }
        public void UpSideUpMove()
        {
            Topmost = true;
            if (upRate >= 0.001 + CaptionLocation.AdjustPace)
            {
                upRate -= CaptionLocation.AdjustPace;
                upRow.Height = new GridLength(upRate, GridUnitType.Star);
                captionVerticalRate += CaptionLocation.AdjustPace;
                captionRow.Height = new GridLength(captionVerticalRate, GridUnitType.Star);
            }
        }
        public void UpSideDownMove()
        {
            Topmost = true;
            if (captionVerticalRate >= 0.001 + CaptionLocation.AdjustPace)
            {
                upRate += CaptionLocation.AdjustPace;
                upRow.Height = new GridLength(upRate, GridUnitType.Star);
                captionVerticalRate -= CaptionLocation.AdjustPace;
                captionRow.Height = new GridLength(captionVerticalRate, GridUnitType.Star);
            }
        }
        public void DownSideDownMove()
        {
            Topmost = true;
            if (downRate >= 0.001 + CaptionLocation.AdjustPace)
            {
                downRate -= CaptionLocation.AdjustPace;
                downRow.Height = new GridLength(downRate, GridUnitType.Star);
                captionVerticalRate += CaptionLocation.AdjustPace;
                captionRow.Height = new GridLength(captionVerticalRate, GridUnitType.Star);
            }
        }
        public void DownSideUpMove()
        {
            Topmost = true;
            if (captionVerticalRate >= 0.001 + CaptionLocation.AdjustPace)
            {
                downRate += CaptionLocation.AdjustPace;
                downRow.Height = new GridLength(downRate, GridUnitType.Star);
                captionVerticalRate -= CaptionLocation.AdjustPace;
                captionRow.Height = new GridLength(captionVerticalRate, GridUnitType.Star);
            }
        }
    }
}
