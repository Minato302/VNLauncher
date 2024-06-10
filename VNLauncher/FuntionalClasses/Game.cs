#pragma warning disable IDE0049

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Diagnostics;

namespace VNLauncher.FuntionalClasses
{
    public class Game
    {
        public class CaptionLocation
        {
            private Double leftRate;
            private Double rightRate;
            private Double upRate;
            private Double downRate;
            private Double captionHorizontalRate;
            private Double captionVerticalRate;
            public Double LeftRate => leftRate;
            public Double RightRate => rightRate;
            public Double UpRate => upRate;
            public Double DownRate => downRate;
            public Double CaptionHorizontalRate => captionHorizontalRate;
            public Double CaptionVerticalRate => captionVerticalRate;

            public static readonly Double AdjustPace = 0.05;

            public CaptionLocation(Double leftRate, Double rightRate, Double upRate, Double downRate, Double captionHorizontalRate, Double captionVerticalRate)
            {
                this.leftRate = leftRate;
                this.rightRate = rightRate;
                this.upRate = upRate;
                this.downRate = downRate;
                this.captionHorizontalRate = captionHorizontalRate;
                this.captionVerticalRate = captionVerticalRate;
            }
            public void RightSideLeftMove()
            {
                if (captionHorizontalRate >= 0.001 + AdjustPace)
                {
                    rightRate += AdjustPace;
                    captionHorizontalRate -= AdjustPace;
                }
            }
            public void RightSideRightMove()
            {
                if (rightRate >= 0.001 + AdjustPace)
                {
                    rightRate -= AdjustPace;      
                    captionHorizontalRate += AdjustPace;
                }
            }
            public void LeftSideLeftMove()
            {
                if (leftRate >= 0.001 + AdjustPace)
                {
                    leftRate -= AdjustPace;      
                    captionHorizontalRate += AdjustPace;
                }
            }
            public void LeftSideRightMove()
            {
                if (captionHorizontalRate >= 0.001 + AdjustPace)
                {
                    leftRate += AdjustPace;
                    captionHorizontalRate -= AdjustPace;
                }
            }
            public void UpSideUpMove()
            {
                if (upRate >= 0.001 + AdjustPace)
                {
                    upRate -= AdjustPace;
                    captionVerticalRate += AdjustPace;
                }
            }
            public void UpSideDownMove()
            {
                if (captionVerticalRate >= 0.001 + AdjustPace)
                {
                    upRate += AdjustPace;
                    captionVerticalRate -= AdjustPace;
                }
            }
            public void DownSideDownMove()
            {
                if (downRate >= 0.001 + AdjustPace)
                {
                    downRate -= AdjustPace;
                    captionVerticalRate += AdjustPace;
                }
            }
            public void DownSideUpMove()
            {
                if (captionVerticalRate >= 0.001 + AdjustPace)
                {
                    downRate += AdjustPace;
                    captionVerticalRate -= AdjustPace;
                }
            }

        }
        private String name;
        public String Name => name;
        private String exePath;
        public String ExePath => exePath;

        private CaptionLocation location;
        public CaptionLocation Loaction => location;

        private Int32 playTimeMinute;
        private Boolean isAutoStart;
        public Boolean IsAutoStart => isAutoStart;
        private Boolean isWindowShot;
        public Boolean IsWindowShot => isWindowShot;
        private String windowTitle;
        public String WindowTitle => windowTitle;
        private String windowClass;
        public String WindowClass => windowClass;
        private DateTime lastStartTime;
        private System.Timers.Timer timer;

        public Game(String name, FileManager reader)
        {
            this.name = name;
            String gameDataPath = reader.GetGameDataPath(name)!;
            String jsonContent = File.ReadAllText(gameDataPath);
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            exePath = responseData.path;
            playTimeMinute = responseData.playTimeMinute;
            isAutoStart = responseData.isAutoStart;
            isWindowShot = responseData.isWindowShot;
            windowTitle = responseData.windowTitle;
            windowClass = responseData.windowClass;
            lastStartTime = responseData.lastStartTime;
            Double d1 = responseData.gameCaptionLocation.leftRate;
            Double d2 = responseData.gameCaptionLocation.rightRate;
            Double d3 = responseData.gameCaptionLocation.upRate;
            Double d4 = responseData.gameCaptionLocation.downRate;
            Double d5 = responseData.gameCaptionLocation.captionHorizontalRate;
            Double d6 = responseData.gameCaptionLocation.captionVerticalRate;

            location = new CaptionLocation(d1, d2, d3, d4, d5, d6);

            timer = new System.Timers.Timer(60000);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = false;
        }
        public void UpdateToFile()
        {
            FileManager writer = new FileManager();
            String gameDatePath = writer.GetGameDataPath(name)!;
            File.Delete(gameDatePath);
            JObject gameData = new JObject();
            JObject captionLoaction = new JObject();

            DateTime date = DateTime.Now;
            gameData["path"] = exePath;
            gameData["windowTitle"] = windowTitle;
            gameData["windowClass"] = windowClass;
            gameData["isAutoStart"] = isAutoStart;
            gameData["isWindowShot"] = isWindowShot;
            gameData["playTimeMinute"] = playTimeMinute;
            gameData["lastStartTime"] = date;

            captionLoaction["leftRate"] = location.LeftRate;
            captionLoaction["rightRate"] = location.RightRate;
            captionLoaction["upRate"] = location.UpRate;
            captionLoaction["downRate"] = location.DownRate;
            captionLoaction["captionHorizontalRate"] = location.CaptionHorizontalRate;
            captionLoaction["captionVerticalRate"] = location.CaptionVerticalRate;

            gameData["gameCaptionLocation"] = captionLoaction;
            File.WriteAllText(gameDatePath, gameData.ToString());
        }
        public void StartGame()
        {
            if(isAutoStart)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = exePath;
                startInfo.WindowStyle = ProcessWindowStyle.Normal;
                Process.Start(startInfo);
            }
            lastStartTime = DateTime.Now;
            timer.Enabled = true;
        }
        public void EndGame()
        {
            timer.Stop();
            UpdateToFile();
        }
        private void OnTimedEvent(Object? sender, System.Timers.ElapsedEventArgs e)
        {
            playTimeMinute++;
        }
    }
}
