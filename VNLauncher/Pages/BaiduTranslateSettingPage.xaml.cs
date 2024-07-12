#pragma warning disable IDE0049

using System.IO;
using System.Windows;
using System.Windows.Controls;
using VNLauncher.FunctionalClasses;

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
            baiduTranslateQuestionButton.SetTips("百度翻译质量相对于大模型翻译质量较差，如果硬件不允许跑本地模型时建议使用在线模型。如果一定要使用，请访问链接1创建应用和APIKey。");
            baiduTranslateQuestionButton.ReplaceTextWithHyperlink("链接1", "https://console.bce.baidu.com/ai/#/ai/machinetranslation/overview/index", "这里");

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
