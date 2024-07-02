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
using VNLauncher.FunctionalClasses;
using System.IO;

namespace VNLauncher.Pages
{
    public partial class BaiduTranslateSettingPage : Page
    {
        private FileManager fileManager;
        public BaiduTranslateSettingPage()
        {
            InitializeComponent();
            fileManager = new FileManager();
            enabledCheckBox.SetChangedAction(() =>
            {
                apiKeyTextBox.IsEnabled = true;
                secretKeyTextBox.IsEnabled = true;
            }, () =>
            {
                apiKeyTextBox.IsEnabled = false;
                secretKeyTextBox.IsEnabled = false;
            });
        }
        private void Page_Loaded(Object sender, RoutedEventArgs e)
        {
            String jsonContent = File.ReadAllText(fileManager.UserDataJsonPath);
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            enabledCheckBox.IsChecked = responseData.baiduTranslate.enabled;
            apiKeyTextBox.Text = responseData.baiduTranslate.apiKey;
            secretKeyTextBox.Text = responseData.baiduTranslate.secretKey;
        }
        private void SaveButton_Click(Object sender, RoutedEventArgs e)
        {
            String jsonContent = File.ReadAllText(fileManager.UserDataJsonPath);
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            responseData.baiduTranslate.enabled = enabledCheckBox.IsChecked;
            responseData.baiduTranslate.apiKey = apiKeyTextBox.Text;
            responseData.baiduTranslate.secretKey = secretKeyTextBox.Text;
            File.WriteAllText(fileManager.UserDataJsonPath, Newtonsoft.Json.JsonConvert.SerializeObject(responseData, Newtonsoft.Json.Formatting.Indented));
        }

    }
}
