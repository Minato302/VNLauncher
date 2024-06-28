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
using System.IO;    

namespace VNLauncher.Pages
{
    public partial class LocalTranslateSettingPage : Page
    {
        private FileManager fileManager;
        public LocalTranslateSettingPage()
        {
            InitializeComponent();
            fileManager = new FileManager();
            enabledChechBox.SetChangedAction(() =>
            {
                contextTextBox.IsEnabled = true;
                urlTextBox.IsEnabled = true;
                promptTextBox.IsEnabled = true;
            }, () =>
            {
                contextTextBox.IsEnabled = false;
                urlTextBox.IsEnabled = false;
                promptTextBox.IsEnabled = false;
            });
            localTranslateQuestionButton.SetTips("本地翻译需要使用TextGeneration和Sakura翻译模型，TextGeneration的安装方法可以看链接1，Sakura模型的安装方法可以看链接2。您可以根据自己的硬件" +
                "情况选择合适的模型。若使用本地翻译，则在使用VNTranslator打开游戏之前，需要保证TextGeneration是打开且可以正常工作的。");
            localTranslateQuestionButton.ReplaceTextWithHyperlink("链接1", "https://www.bilibili.com/video/BV1Te411U7me/?spm_id_from=333.999.0.0&vd_source=4ba7d05c4113ac9c7f783198a7d99aa0", "这个视频");
            localTranslateQuestionButton.ReplaceTextWithHyperlink("链接2", "https://www.bilibili.com/video/BV18J4m1Y7Sa/?spm_id_from=333.999.0.0&vd_source=4ba7d05c4113ac9c7f783198a7d99aa0", "这个视频");
            localTranslatePromptQuestionButton.SetTips("由于Sakura模型是以对话形式翻译的，因此自动存储了上下文。这里是每次翻译时的提示词，需要翻译的内容会被添加在后面。");
            localTranslateUrlQuestionButton.SetTips("打开“start_windows.bat”后，等待TextGeneration启动完毕后，可以在“OpenAI-compatible API URL:”后看到url，默认为http://127.0.0.1:5000。");
        }

        private void Page_Loaded(Object sender, RoutedEventArgs e)
        {
            String jsonContent = File.ReadAllText(fileManager.UserDataJsonPath);
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            enabledChechBox.IsChecked = responseData.localTranslate.enabled;
            contextTextBox.Text = responseData.localTranslate.context.ToString();
            urlTextBox.Text = responseData.localTranslate.url.ToString();
            promptTextBox.Text = responseData.localTranslate.prompt;
        }

        private void SaveButton_Click(Object sender, RoutedEventArgs e)
        {
            String jsonContent = File.ReadAllText(fileManager.UserDataJsonPath);
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            responseData.localTranslate.enabled = enabledChechBox.IsChecked;
            responseData.localTranslate.context = contextTextBox.Text;
            responseData.localTranslate.url = urlTextBox.Text;
            responseData.localTranslate.prompt = promptTextBox.Text;
            File.WriteAllText(fileManager.UserDataJsonPath, Newtonsoft.Json.JsonConvert.SerializeObject(responseData, Newtonsoft.Json.Formatting.Indented));
        }

        private void MainCanvas_MouseLeftButtonDown(Object sender, MouseButtonEventArgs e)
        {
            localTranslateQuestionButton.ClosePopup();
            localTranslatePromptQuestionButton.ClosePopup();
            localTranslateUrlQuestionButton.ClosePopup();
        }

        private void MainCanvas_MouseRightButtonDown(Object sender, MouseButtonEventArgs e)
        {
            localTranslateQuestionButton.ClosePopup();
            localTranslatePromptQuestionButton.ClosePopup();
            localTranslateUrlQuestionButton.ClosePopup();
        }
    }
    
}


