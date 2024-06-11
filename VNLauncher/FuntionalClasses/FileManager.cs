#pragma warning disable IDE0049

using Newtonsoft.Json.Linq;
using System.Drawing;
using System.IO;
using VNLauncher.Windows;

namespace VNLauncher.FuntionalClasses
{
    public class FileManager
    {
        private String appDirectory;
        private String userDataFolder;
        private String userDataJson;
        private String gamesFolder;
        public FileManager()
        {
            appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            userDataFolder = Path.Combine(appDirectory, "userdata");
            userDataJson = Path.Combine(userDataFolder, "userdata.json");
            gamesFolder = Path.Combine(userDataFolder, "games");
        }
        public String UserDataJsonPath => userDataJson;

        public String GamesFolderPath => gamesFolder;

        public String? GetGameFolderPath(String gameName)
        {
            String[] gameFolders = Directory.GetDirectories(gamesFolder);
            foreach (String folder in gameFolders)
            {
                String folderName = Path.GetFileName(folder);
                if (String.Equals(folderName, gameName, StringComparison.OrdinalIgnoreCase))
                {
                    return folder;
                }
            }
            return null;
        }
        public String? GetGameDataPath(String gameName)
        {
            String? gameFolderPath = GetGameFolderPath(gameName);
            if (gameFolderPath == null)
            {
                return null;
            }
            else
            {
                return Path.Combine(gameFolderPath, "gamedata.json");
            }
        }
        public String? GetGameIconPath(String gameName)
        {
            String? gameFolderPath = GetGameFolderPath(gameName);
            if (gameFolderPath == null)
            {
                return null;
            }
            else
            {
                return Path.Combine(gameFolderPath, "icon.png");
            }
        }
        public String? GetGameCoverPath(String gameName)
        {
            String? gameFolderPath = GetGameFolderPath(gameName);
            if (gameFolderPath == null)
            {
                return null;
            }
            else
            {
                return Path.Combine(gameFolderPath, "cover.png");
            }
        }
        public String? GetGameCapturesPath(String gameName)
        {
            String? gameFolderPath = GetGameFolderPath(gameName);
            if (gameFolderPath == null)
            {
                return null;
            }
            else
            {
                return Path.Combine(gameFolderPath, "screenshot");
            }
        }
        public void CreateNewGameToFile(String gameName, String gamePath, Bitmap cover, Boolean isAutoStart, Boolean isWindowShot,
            String windowTitle, String windowClass, Game.CaptionLocation gameCaptionLocation)
        {
            String gameFolder = Path.Combine(gamesFolder, gameName);

            Directory.CreateDirectory(gameFolder);
            String gameDataJsonPath = Path.Combine(gameFolder, "gamedata.json");
            String gameIconPath = Path.Combine(gameFolder, "icon.png");
            String ScreenshotPath = Path.Combine(gameFolder, "screenshot");
            Directory.CreateDirectory(ScreenshotPath);

            JObject gameData = new JObject();
            JObject captionLoaction = new JObject();
            DateTime date = DateTime.Now;
            gameData["path"] = gamePath;
            gameData["windowTitle"] = windowTitle;
            gameData["windowClass"] = windowClass;
            gameData["isAutoStart"] = isAutoStart;
            gameData["isWindowShot"] = isWindowShot;
            gameData["playTimeMinute"] = 0;
            gameData["lastStartTime"] = date;

            captionLoaction["leftRate"] = gameCaptionLocation.LeftRate;
            captionLoaction["rightRate"] = gameCaptionLocation.RightRate;
            captionLoaction["upRate"] = gameCaptionLocation.UpRate;
            captionLoaction["downRate"] = gameCaptionLocation.DownRate;
            captionLoaction["captionHorizontalRate"] = gameCaptionLocation.CaptionHorizontalRate;
            captionLoaction["captionVerticalRate"] = gameCaptionLocation.CaptionVerticalRate;

            gameData["gameCaptionLocation"] = captionLoaction;


            File.WriteAllText(gameDataJsonPath, gameData.ToString());
            String coverPath = Path.Combine(gameFolder, "cover.png");
            cover.Save(coverPath);
            Icon.ExtractAssociatedIcon(gamePath)!.ToBitmap().Save(gameIconPath);
        }
        public List<(String, String)> GetAllGameInfoFromFile()
        {
            List<(String, String)> info = new List<(String, String)>();
            String[] gameFolders = Directory.GetDirectories(gamesFolder);
            foreach (String folder in gameFolders)
            {
                String gameName = Path.GetFileName(folder);
                String gameDataFilePath = Path.Combine(folder, "gamedata.json");
                String jsonContent = File.ReadAllText(gameDataFilePath);
                dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
                String gameExePath = responseData.path;
                info.Add((gameName, gameExePath));
            }
            return info;
        }
        public Dictionary<String, List<String>> GroupCaptureNamesByPrefix(String gameName)
        {
            String path = GetGameCapturesPath(gameName)!;
            String[] captureFiles = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            Dictionary<String, List<String>> prefixGroups = new Dictionary<String, List<String>>();
            foreach (String file in captureFiles)
            {
                String fileName = Path.GetFileName(file);
                String prefix = fileName.Substring(0, 8);

                if (!prefixGroups.ContainsKey(prefix))
                {
                    prefixGroups[prefix] = new List<String>();
                }
                prefixGroups[prefix].Add(fileName);
            }
            return prefixGroups;
        }
        public void SaveCapture(String gameName, Bitmap capture, String captureName)
        {
            capture.Save(Path.Combine(GetGameCapturesPath(gameName)!, captureName + ".png"));
        }
    }
}
