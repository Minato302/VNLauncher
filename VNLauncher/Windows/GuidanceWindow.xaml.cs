#pragma warning disable IDE0049

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using VNLauncher.FunctionalClasses;

namespace VNLauncher.Windows
{
    public partial class GuidanceWindow : Window
    {
        private const Double MarginFromLeft = 50;
        private Boolean isAutoStart;
        private String gameName;
        private String gameExePath;
        private MainWindow mainWindow;
        public GuidanceWindow(MainWindow mainWindow, String gameName,String gameExePath, Boolean isAutoStart)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.isAutoStart = isAutoStart;
            this.gameName = gameName;
            this.gameExePath = gameExePath;

        }
        private void GuidanceWindow_Loaded(Object sender, RoutedEventArgs e)
        {
            GuidancePageParameter parameter = new GuidancePageParameter();
            AddGameInfo gameInfo = new AddGameInfo();
            gameInfo.GameName = gameName;
            gameInfo.GameExePath = gameExePath;
            gameInfo.IsAutoStart = isAutoStart;
            parameter.GameInfo = gameInfo;
            parameter.MainFrame = mainFrame;
            parameter.BaseWindow = this;
            parameter.MainWindow = mainWindow;
            mainFrame.Navigate(new Pages.GuidancePage_Step1(parameter));
            ShowUp();
        }
        public void ShowUp()
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation opacityAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.4)));
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));
            DoubleAnimation leftAnimation = new DoubleAnimation(-Width, MarginFromLeft, new Duration(TimeSpan.FromSeconds(0.4)));
            Storyboard.SetTarget(leftAnimation, this);
            Storyboard.SetTargetProperty(leftAnimation, new PropertyPath("(Canvas.Left)"));
            storyboard.Children.Add(opacityAnimation);
            storyboard.Children.Add(leftAnimation);
            BeginStoryboard(storyboard);
        }

        public void GoAway(Window? endWindow)
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation opacityAnimation = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.4)));
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(Window.OpacityProperty));
            DoubleAnimation leftAnimation = new DoubleAnimation(MarginFromLeft, -Width, new Duration(TimeSpan.FromSeconds(0.4)));
            Storyboard.SetTarget(leftAnimation, this);
            Storyboard.SetTargetProperty(leftAnimation, new PropertyPath("(Canvas.Left)"));
            storyboard.Children.Add(opacityAnimation);
            storyboard.Children.Add(leftAnimation);
            if (endWindow != null)
            {
                storyboard.Completed += (s, e) =>
                {
                    Close();
                    endWindow.Show();
                };
            }
            else
            {
                storyboard.Completed += (s, e) =>
                {
                    Close();
                };
            }
            BeginStoryboard(storyboard);
        }
        public void Handoff(Page targetPage, GuidanceWindow baseWindow)
        {
            Storyboard goAwayStoryboard = new Storyboard();
            DoubleAnimation opacityToInvisible = new DoubleAnimation(1, 0, new Duration(TimeSpan.FromSeconds(0.4)));
            Storyboard.SetTargetProperty(opacityToInvisible, new PropertyPath(Window.OpacityProperty));
            DoubleAnimation moveLeft = new DoubleAnimation(MarginFromLeft, -Width, new Duration(TimeSpan.FromSeconds(0.4)));
            Storyboard.SetTarget(moveLeft, this);
            Storyboard.SetTargetProperty(moveLeft, new PropertyPath("(Canvas.Left)"));
            goAwayStoryboard.Children.Add(opacityToInvisible);
            goAwayStoryboard.Children.Add(moveLeft);
            goAwayStoryboard.Completed += (s, e) =>
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(0.4);
                timer.Tick += (s2, e2) =>
                {
                    timer.Stop();
                    mainFrame.Navigate(targetPage);
                    baseWindow.Topmost = true;
                    Storyboard comeBackStoryboard = new Storyboard();
                    DoubleAnimation opacityToVisible = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromSeconds(0.4)));
                    Storyboard.SetTargetProperty(opacityToVisible, new PropertyPath(Window.OpacityProperty));
                    DoubleAnimation moveRight = new DoubleAnimation(-Width, MarginFromLeft, new Duration(TimeSpan.FromSeconds(0.4)));
                    Storyboard.SetTarget(moveRight, this);
                    Storyboard.SetTargetProperty(moveRight, new PropertyPath("(Canvas.Left)"));
                    comeBackStoryboard.Children.Add(opacityToVisible);
                    comeBackStoryboard.Children.Add(moveRight);
                    BeginStoryboard(comeBackStoryboard);
                };
                timer.Start();
            };
            BeginStoryboard(goAwayStoryboard);
        }

    }
}

