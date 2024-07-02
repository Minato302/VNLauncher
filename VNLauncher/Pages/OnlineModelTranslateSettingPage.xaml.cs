#pragma warning disable IDE0049

using System.IO;
using System.Windows;
using System.Windows.Controls;
using VNLauncher.FunctionalClasses;
using System.Text;

namespace VNLauncher.Pages
{
    public partial class OnlineModelTranslateSettingPage : Page
    {
        private FileManager fileManager;
        public OnlineModelTranslateSettingPage()
        {
            InitializeComponent();
            fileManager = new FileManager();
            enabledCheckBox.SetChangedAction(() =>
            {
                urlTextBox.IsEnabled = true;
                apiKeyTextBox.IsEnabled = true;
                modelTextBox.IsEnabled = true;
                contextTextBox.IsEnabled = true;
                promptTextBox.IsEnabled = true;

            }, () =>
            {
                urlTextBox.IsEnabled = false;
                apiKeyTextBox.IsEnabled = false;
                modelTextBox.IsEnabled = false;
                contextTextBox.IsEnabled = false;
                promptTextBox.IsEnabled = false;
            });
            onlineTranslateQuestionButton.SetTips("支持ChatGPT、DeepSeek等在线模型翻译。您可以选择直接使用GPT或各种GPT API的转发商（如ChatAnywhere，具体可以看链接1）进行翻译，也可以选择类似GPT的大语言模型进行翻译。直接使用OpenAI的API时，对应的Url为\"https://api.openai.com/v1/chat/completions\"，使用转发商API和其他大语言模型时，可以查阅对应网站的Url。使用其他大语言模型时，需要确认其可以翻译日语，且调用方法和OpenAI的类似，且仅需要APIKey而不是APIKey+SecretKey，推荐使用DeepSeek，具体可以看链接2");
            onlineTranslateQuestionButton.ReplaceTextWithHyperlink("链接1", "https://chatanywhere.apifox.cn/doc-2694962", "这里");
            onlineTranslateQuestionButton.ReplaceTextWithHyperlink("链接2", "https://platform.deepseek.com/api-docs/zh-cn/", "这里");

            promptQuestionButton.SetTips("由于大语言模型可能会出现将上下文全部翻译或者没有阅读上下文就翻译的情况，因此在这里的提示词需要同时加入上下文和需要翻译句子的内容。您可以自由选择上下文和当前句在" +
                "提示词中的位置，其中//{上下文}//表示在该处插入上下文，//{当前句}//表示在该处插入当前句。其中//{当前句}//应当有且仅有一处，//{上下文}//至多有一处。");
        }

        private void Page_Loaded(Object sender, RoutedEventArgs e)
        {
            String jsonContent = File.ReadAllText(fileManager.UserDataJsonPath);
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            enabledCheckBox.IsChecked = responseData.onlineModelTranslate.enabled;
            urlTextBox.Text = responseData.onlineModelTranslate.url;
            apiKeyTextBox.Text = responseData.onlineModelTranslate.apiKey;
            modelTextBox.Text = responseData.onlineModelTranslate.model;
            contextTextBox.Text = responseData.onlineModelTranslate.context;
            String prompt = responseData.onlineModelTranslate.prompt.prompt1;
            Boolean contextFirst = responseData.onlineModelTranslate.prompt.contextFirst;
            Boolean hasContext = responseData.onlineModelTranslate.prompt.hasContext;
            if (contextFirst)
            {
                prompt += "//{上下文}//";
                prompt += responseData.onlineModelTranslate.prompt.prompt2;
                prompt += "//{当前句}//";
                prompt += responseData.onlineModelTranslate.prompt.prompt3;


            }
            else
            {
                prompt += "//{当前句}//";
                prompt += responseData.onlineModelTranslate.prompt.prompt2;
                if (hasContext)
                {
                    prompt += "//{上下文}//";
                }
                prompt += responseData.onlineModelTranslate.prompt.prompt3;
            }
            promptTextBox.Text = prompt;
        }

        private void SaveButton_Click(Object sender, RoutedEventArgs e)
        {
            String jsonContent = File.ReadAllText(fileManager.UserDataJsonPath);
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            responseData.onlineModelTranslate.enabled = enabledCheckBox.IsChecked;
            responseData.onlineModelTranslate.url = urlTextBox.Text;
            responseData.onlineModelTranslate.apiKey = apiKeyTextBox.Text;
            responseData.onlineModelTranslate.model = modelTextBox.Text;
            responseData.onlineModelTranslate.context = contextTextBox.Text;
            try
            {
                if (promptTextBox.Text.IndexOf("//{当前句}//") == -1)
                {
                    throw new Exception();
                }
                else if (promptTextBox.Text.IndexOf("//{上下文}//") == -1)
                {
                    responseData.onlineModelTranslate.prompt.prompt1 = promptTextBox.Text.Split("//{当前句}//")[0];
                    responseData.onlineModelTranslate.prompt.prompt2 = promptTextBox.Text.Split("//{当前句}//")[1];
                    responseData.onlineModelTranslate.prompt.prompt3 = "";
                    responseData.onlineModelTranslate.prompt.hasContext = false;
                }
                else if (promptTextBox.Text.IndexOf("//{上下文}//") < promptTextBox.Text.IndexOf("//{当前句}//"))
                {
                    responseData.onlineModelTranslate.prompt.prompt1 = promptTextBox.Text.Split("//{上下文}//")[0];
                    responseData.onlineModelTranslate.prompt.prompt2 = promptTextBox.Text.Split("//")[2];
                    responseData.onlineModelTranslate.prompt.prompt3 = promptTextBox.Text.Split("//{当前句}//")[1];
                    responseData.onlineModelTranslate.prompt.hasContext = true;
                    responseData.onlineModelTranslate.prompt.contextFirst = true;
                }
                else
                {
                    responseData.onlineModelTranslate.prompt.prompt1 = promptTextBox.Text.Split("//{当前句}//")[0];
                    responseData.onlineModelTranslate.prompt.prompt2 = promptTextBox.Text.Split("//")[2];
                    responseData.onlineModelTranslate.prompt.prompt3 = promptTextBox.Text.Split("//{上下文}//")[1];
                    responseData.onlineModelTranslate.prompt.hasContext = true;
                    responseData.onlineModelTranslate.prompt.contextFirst = false;
                }
            }
            catch (Exception)
            {
                tipsTextBlock.Visibility = Visibility.Visible;
                promptTextBox.SaveFailed();
                return;
            }
            File.WriteAllText(fileManager.UserDataJsonPath, Newtonsoft.Json.JsonConvert.SerializeObject(responseData, Newtonsoft.Json.Formatting.Indented));
            tipsTextBlock.Visibility = Visibility.Hidden;

        }

        private void MainCanvas_MouseLeftButtonDown(Object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            onlineTranslateQuestionButton.ClosePopup();
            promptQuestionButton.ClosePopup();
        }

        private void MainCanvas_MouseRightButtonDown(Object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            onlineTranslateQuestionButton.ClosePopup();
            promptQuestionButton.ClosePopup();
        }
    }
}
