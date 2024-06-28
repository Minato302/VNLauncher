#pragma warning disable IDE0049

using System.IO;
using System.Windows;
using System.Windows.Controls;
using VNLauncher.FuntionalClasses;
using System.Text;

namespace VNLauncher.Pages
{
    public partial class GPTTranslateSettingPage : Page
    {
        private FileManager fileManager;
        public GPTTranslateSettingPage()
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
            gptTranslateQuestionButton.SetTips("支持GPT翻译，您可以选择直接使用GPT或各种GPT API的转发商（如ChatAnywhere）进行翻译。直接使用OpenAI的API时，对应的Url为\"https://api.openai.com/v1/chat/completions\"，使用转发商API时，可以查阅对应网站的Url。");
            promptQuestionButton.SetTips("由于GPT模型可能会出现将上下文全部翻译或者没有阅读上下文就翻译的情况，因此在这里的提示词需要同时加入上下文和需要翻译句子的内容。您可以自由选择上下文和当前句在" +
                "提示词中的位置，其中//{上下文}//表示在该处插入上下文，//{当前句}//表示在该处插入当前句。其中//{当前句}//应当有且仅有一处，//{上下文}//至多有一处。");
        }

        private void Page_Loaded(Object sender, RoutedEventArgs e)
        {
            String jsonContent = File.ReadAllText(fileManager.UserDataJsonPath);
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            enabledCheckBox.IsChecked = responseData.gptTranslate.enabled;
            urlTextBox.Text = responseData.gptTranslate.url;
            apiKeyTextBox.Text = responseData.gptTranslate.apiKey;
            modelTextBox.Text = responseData.gptTranslate.model;
            contextTextBox.Text = responseData.gptTranslate.context;
            String prompt = responseData.gptTranslate.prompt.prompt1;
            Boolean contextFirst = responseData.gptTranslate.prompt.contextFirst;
            Boolean hasContext = responseData.gptTranslate.prompt.hasContext;
            if (contextFirst)
            {
                prompt += "//{上下文}//";
                prompt += responseData.gptTranslate.prompt.prompt2;
                prompt += "//{当前句}//";
                prompt += responseData.gptTranslate.prompt.prompt3;


            }
            else
            {
                prompt += "//{当前句}//";
                prompt += responseData.gptTranslate.prompt.prompt2;
                if (hasContext)
                {
                    prompt += "//{上下文}//";
                }
                prompt += responseData.gptTranslate.prompt.prompt3;
            }
            promptTextBox.Text = prompt;
        }

        private void SaveButton_Click(Object sender, RoutedEventArgs e)
        {
            String jsonContent = File.ReadAllText(fileManager.UserDataJsonPath);
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            responseData.gptTranslate.enabled = enabledCheckBox.IsChecked;
            responseData.gptTranslate.url = urlTextBox.Text;
            responseData.gptTranslate.apiKey = apiKeyTextBox.Text;
            responseData.gptTranslate.model = modelTextBox.Text;
            responseData.gptTranslate.context = contextTextBox.Text;
            try
            {
                if (promptTextBox.Text.IndexOf("//{当前句}//") == -1)
                {
                    throw new Exception();
                }
                else if (promptTextBox.Text.IndexOf("//{上下文}//") == -1)
                {
                    responseData.gptTranslate.prompt.prompt1 = promptTextBox.Text.Split("//{当前句}//")[0];
                    responseData.gptTranslate.prompt.prompt2 = promptTextBox.Text.Split("//{当前句}//")[1];
                    responseData.gptTranslate.prompt.prompt3 = "";
                    responseData.gptTranslate.prompt.hasContext = false;
                }
                else if (promptTextBox.Text.IndexOf("//{上下文}//") < promptTextBox.Text.IndexOf("//{当前句}//"))
                {
                    responseData.gptTranslate.prompt.prompt1 = promptTextBox.Text.Split("//{上下文}//")[0];
                    responseData.gptTranslate.prompt.prompt2 = promptTextBox.Text.Split("//")[2];
                    responseData.gptTranslate.prompt.prompt3 = promptTextBox.Text.Split("//{当前句}//")[1];
                    responseData.gptTranslate.prompt.hasContext = true;
                    responseData.gptTranslate.prompt.contextFirst = true;
                }
                else
                {
                    responseData.gptTranslate.prompt.prompt1 = promptTextBox.Text.Split("//{当前句}//")[0];
                    responseData.gptTranslate.prompt.prompt2 = promptTextBox.Text.Split("//")[2];
                    responseData.gptTranslate.prompt.prompt3 = promptTextBox.Text.Split("//{上下文}//")[1];
                    responseData.gptTranslate.prompt.hasContext = true;
                    responseData.gptTranslate.prompt.contextFirst = false;
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
            gptTranslateQuestionButton.ClosePopup();
            promptQuestionButton.ClosePopup();
        }

        private void MainCanvas_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            gptTranslateQuestionButton.ClosePopup();
            promptQuestionButton.ClosePopup();
        }
    }
}
