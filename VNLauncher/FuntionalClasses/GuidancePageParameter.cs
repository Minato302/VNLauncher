#pragma warning disable IDE0049

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VNLauncher.Windows;

namespace VNLauncher.FuntionalClasses
{
    public class GuidancePageParameter
    {
        private Frame mainFrame;
        private Boolean isAutoStart;
        private AddGameInfo gameInfo;
        private GuidanceWindow baseWindow;
        private MainWindow mainWindow;

        public Frame MainFrame
        {
            get
            {
                return mainFrame;
            }
            set
            {
                mainFrame = value;
            }
        }
        public Boolean IsAutoStart
        {
            get
            {
                return isAutoStart;
            }
            set
            {
                isAutoStart = value;
            }
        }
        public AddGameInfo GameInfo
        {
            get
            {
                return gameInfo;
            }
            set
            {
                gameInfo = value;
            }
        }
        public GuidanceWindow BaseWindow
        {
            get
            {
                return baseWindow;
            }
            set
            {
                baseWindow = value;
            }
        }
        public MainWindow MainWindow
        {
            get
            {
                return mainWindow;
            }
            set
            {
                mainWindow = value;
            }
        }
    }
    public class AddGameInfo
    {
        private Boolean isAutoStart;
        private Boolean isWindowShot;
        private String gameWindowClass;
        private String gameName;
        private String gameExePath;
        private String gameWindowTitle;
        private IntPtr gameWindow;
        private Bitmap gameCover;

        public Boolean IsAutoStart
        {
            get
            {
                return isAutoStart;
            }
            set
            {
                isAutoStart = value;
            }
        }
        public String GameWindowClass
        {
            get
            {
                return gameWindowClass;
            }
            set
            {
                gameWindowClass = value;
            }
        }
        public String GameName
        {
            get
            {
                return gameName;
            }
            set
            {
                gameName = value;
            }
        }
        public String GameExePath
        {
            get
            {
                return gameExePath;
            }
            set
            {
                gameExePath = value;
            }
        }
        public IntPtr GameWindow
        {
            get
            {
                return gameWindow;
            }
            set
            {
                gameWindow = value;
            }
        }
        public String GameWindowTitle
        {
            get
            {
                return gameWindowTitle;
            }
            set
            {
                gameWindowTitle = value;
            }
        }
        public Bitmap GameCover
        {
            get
            {
                return gameCover;
            }
            set
            {
                gameCover = value;
            }
        }
        public Boolean IsWindowShot
        {
            get
            {
                return isWindowShot;
            }
            set
            {
                isWindowShot = value;
            }
        }
    }
}
