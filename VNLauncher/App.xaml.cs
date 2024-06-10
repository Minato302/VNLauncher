#pragma warning disable IDE0049

using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace VNLauncher
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
       {
            base.OnStartup(e);

            String appDirectory = AppDomain.CurrentDomain.BaseDirectory;

            String userDataFolder = Path.Combine(appDirectory, "userdata");
            String userDataFile = Path.Combine(userDataFolder, "userdata.json");
            String gamesFolder = Path.Combine(userDataFolder, "games");



            if (!Directory.Exists(userDataFolder))
            {
                Directory.CreateDirectory(userDataFolder);
                Directory.CreateDirectory(gamesFolder);
                DirectoryInfo dirInfo = new DirectoryInfo(userDataFolder);
                dirInfo.Attributes |= FileAttributes.Hidden;

                Directory.CreateDirectory(userDataFolder);

                if (!File.Exists(userDataFile))
                {
                    File.WriteAllText(userDataFile, "{}"); 
                }
                if (!Directory.Exists(gamesFolder))
                {
                    Directory.CreateDirectory(gamesFolder);
                }

            }
        }
    }

}
