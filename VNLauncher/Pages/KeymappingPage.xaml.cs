#pragma warning disable IDE0049

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VNLauncher.FuntionalClasses;
using Newtonsoft.Json.Linq;
using System.IO;   

namespace VNLauncher.Pages
{
    public partial class KeymappingPage : Page
    {
        private FileManager fileManager;
        public KeymappingPage()
        {
            InitializeComponent();
            NavigationCommands.BrowseBack.InputGestures.Clear();
            fileManager = new FileManager();
        }

        private void SaveButton_Click(Object sender, RoutedEventArgs e)
        {
            JObject keyMapping = new JObject
            {
                ["translateSwitch"] = translateSwitchItem.GetKeyName(),
                ["showMarquee"] = showMarqueeItem.GetKeyName(),
                ["screenshot"] = screenShotItem.GetKeyName(),
                ["retranslate"] = retranslateItem.GetKeyName(),
                ["captureSideUpMove"] = captureSideUpMoveItem.GetKeyName(),
                ["captureSideDownMove"] = captureSideDownMoveItem.GetKeyName(),
                ["captureSideLeftMove"] = captureSideLeftMoveItem.GetKeyName(),
                ["captureSideRightMove"] = captureSideRightMoveItem.GetKeyName()
            };
            String jsonContent = File.ReadAllText(fileManager.UserDataJsonPath);
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            responseData.keyMapping = keyMapping;
            File.WriteAllText(fileManager.UserDataJsonPath, responseData.ToString());

        }

        private void Page_Loaded(Object sender, RoutedEventArgs e)
        {
            String jsonContent = File.ReadAllText(fileManager.UserDataJsonPath);
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            String tanslateSwitch = responseData.keyMapping.translateSwitch;
            translateSwitchItem.SetKey(tanslateSwitch);
            String showMarquee = responseData.keyMapping.showMarquee;
            showMarqueeItem.SetKey(showMarquee);
            String screenshot = responseData.keyMapping.screenshot;
            screenShotItem.SetKey(screenshot);
            String retranslate = responseData.keyMapping.retranslate;
            retranslateItem.SetKey(retranslate);
            String captureSideUpMove = responseData.keyMapping.captureSideUpMove;
            captureSideUpMoveItem.SetKey(captureSideUpMove);
            String captureSideDownMove = responseData.keyMapping.captureSideDownMove;
            captureSideDownMoveItem.SetKey(captureSideDownMove);
            String captureSideLeftMove = responseData.keyMapping.captureSideLeftMove;
            captureSideLeftMoveItem.SetKey(captureSideLeftMove);
            String captureSideRightMove = responseData.keyMapping.captureSideRightMove;
            captureSideRightMoveItem.SetKey(captureSideRightMove);
        }
    }
}
