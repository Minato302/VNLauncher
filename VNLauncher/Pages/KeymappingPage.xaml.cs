#pragma warning disable IDE0049

using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VNLauncher.FunctionalClasses;

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
                ["translateSwitch"] = translateSwitchItem.KeyName,
                ["showMarquee"] = showMarqueeItem.KeyName,
                ["screenShot"] = screenShotItem.KeyName,
                ["retranslate"] = retranslateItem.KeyName,
                ["captureSideUpMove"] = captureSideUpMoveItem.KeyName,
                ["captureSideDownMove"] = captureSideDownMoveItem.KeyName,
                ["captureSideLeftMove"] = captureSideLeftMoveItem.KeyName,
                ["captureSideRightMove"] = captureSideRightMoveItem.KeyName,
                ["boxSelectAndTranslate"] = boxSelectAndTranslateItem.KeyName
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
            String screenshot = responseData.keyMapping.screenShot;
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
            String boxSelectAndTranslate = responseData.keyMapping.boxSelectAndTranslate;
            boxSelectAndTranslateItem.SetKey(boxSelectAndTranslate);
        }
    }
}
