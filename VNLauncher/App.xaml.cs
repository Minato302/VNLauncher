#pragma warning disable IDE0049

using System.Configuration;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using VNLauncher.FunctionalClasses;

namespace VNLauncher
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            FileManager fileManager = new FileManager();

            if (!Directory.Exists(fileManager.UserDataFolderPath))
            {
                Directory.CreateDirectory(fileManager.UserDataFolderPath);
                Directory.CreateDirectory(fileManager.GamesFolderPath);
                DirectoryInfo dirInfo = new DirectoryInfo(fileManager.UserDataFolderPath);
                InitialInfo initialInfo = new InitialInfo();
                initialInfo.WriteInitialInfo(fileManager.UserDataJsonPath);
                File.Create(fileManager.GamesOrderPath);

            }
        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }

}
