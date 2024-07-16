#pragma warning disable IDE0049

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VNLauncher.Windows
{
    public partial class SemitransparentWindow : Window
    {
        private Point? firstPoint;
        private MarqueeWindow marquee;
        public SemitransparentWindow(MarqueeWindow marquee)
        {
            InitializeComponent();
            Width = SystemParameters.PrimaryScreenWidth;
            Height = SystemParameters.PrimaryScreenHeight;
            Left = 0;
            Top = 0;
            firstPoint = null;
            this.marquee = marquee;
        }

        private void Window_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            if (firstPoint == null)
            {
                firstPoint = e.GetPosition(this);
            }
            else
            {
                (Point, Point) box = GetTransparentBox((Point)firstPoint, e.GetPosition(this));
                Close();
                marquee.BoxTranslate(new System.Drawing.Point(Convert.ToInt32(box.Item1.X), Convert.ToInt32(box.Item1.Y)),
                    new System.Drawing.Point(Convert.ToInt32(box.Item2.X), Convert.ToInt32(box.Item2.Y)));
            }
        }

        private void Window_PreviewMouseMove(Object sender, MouseEventArgs e)
        {
            if (firstPoint != null)
            {
                GetTransparentBox((Point)firstPoint, e.GetPosition(this));
            }
        }
        private (Point, Point) GetTransparentBox(Point firstPoint, Point secondPoint)
        {
            Background = new SolidColorBrush(Color.FromArgb(1, 0, 0, 0));


            Point leftUp = new Point(firstPoint.X < secondPoint.X ? firstPoint.X : secondPoint.X, firstPoint.Y < secondPoint.Y ? firstPoint.Y : secondPoint.Y);
            Point rightDown = new Point(firstPoint.X > secondPoint.X ? firstPoint.X : secondPoint.X, firstPoint.Y > secondPoint.Y ? firstPoint.Y : secondPoint.Y);

            upSemiTransparentGrid.SetValue(Canvas.LeftProperty, 0D);
            upSemiTransparentGrid.SetValue(Canvas.TopProperty, 0D);
            upSemiTransparentGrid.Height = leftUp.Y;
            upSemiTransparentGrid.Width = Width;

            leftSemiTransparentGrid.SetValue(Canvas.LeftProperty, 0D);
            leftSemiTransparentGrid.SetValue(Canvas.TopProperty, Convert.ToDouble(leftUp.Y));
            leftSemiTransparentGrid.Height = rightDown.Y - leftUp.Y;
            leftSemiTransparentGrid.Width = leftUp.X;

            downSemiTransparentGrid.SetValue(Canvas.LeftProperty, 0D);
            downSemiTransparentGrid.SetValue(Canvas.TopProperty, rightDown.Y);
            downSemiTransparentGrid.Height = Height - rightDown.Y;
            downSemiTransparentGrid.Width = Width;


            rightSemiTransparentGrid.SetValue(Canvas.LeftProperty, rightDown.X);
            rightSemiTransparentGrid.SetValue(Canvas.TopProperty, Convert.ToDouble(leftUp.Y));
            rightSemiTransparentGrid.Height = rightDown.Y - leftUp.Y;
            rightSemiTransparentGrid.Width = Width - rightDown.X;

            return (leftUp, rightDown);
        }
    }
}
