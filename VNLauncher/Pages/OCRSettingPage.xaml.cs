#pragma warning disable IDE0049

using System.Windows.Controls;
using VNLauncher.FunctionalClasses;
using VNLauncher.Controls;
using System.IO;    


namespace VNLauncher.Pages
{
    public partial class OCRSettingPage : Page
    {
        private FileManager fileManager;
        public OCRSettingPage()
        {
            InitializeComponent();
            onlineOCREnabledCheckBox.SetChangedAction(() =>
            {
                apiKeyTextBox.IsEnabled = true;
                secretKeyTextBox.IsEnabled = true;
            }, () =>
                 {
                     apiKeyTextBox.IsEnabled = false;
                     secretKeyTextBox.IsEnabled = false;
                 });
            fileManager= new FileManager();
            localOCRQuestionButton.SetTips("本地OCR有V3和V4两个模型，V3正确率约在97%左右，V4可以达到99.4%左右。V3模型只需要CPU就可以运行，V4建议使用GPU（A卡N卡均可）");
        }

        private void Page_Loaded(Object sender, System.Windows.RoutedEventArgs e)
        {
            String jsonContent = File.ReadAllText(fileManager.UserDataJsonPath);
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            Boolean isV4Model= responseData.ocr.localOCR.isV4Model;
            Boolean usingGPU = responseData.ocr.localOCR.usingGPU;
            SettingPageRadioButton.SetGroup(new List<SettingPageRadioButton> { modelV3RadioButton, modelV4RadioButton }, isV4Model ? modelV4RadioButton : modelV3RadioButton);
            SettingPageRadioButton.SetGroup(new List<SettingPageRadioButton> { usingCPURadioButton, usingGPURadioButton }, usingGPU ? usingGPURadioButton : usingCPURadioButton);
            onlineOCREnabledCheckBox.IsChecked = responseData.ocr.onlineOCR.enabled;
            apiKeyTextBox.Text = responseData.ocr.onlineOCR.apiKey;
            secretKeyTextBox.Text = responseData.ocr.onlineOCR.secretKey;
        }

        private void SaveButton_Click(Object sender, System.Windows.RoutedEventArgs e)
        {
            //将保存后的结果重新写入文件
            String jsonContent = File.ReadAllText(fileManager.UserDataJsonPath);
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent)!;
            responseData.ocr.localOCR.isV4Model = modelV4RadioButton.IsChecked;
            responseData.ocr.localOCR.usingGPU = usingGPURadioButton.IsChecked;
            responseData.ocr.onlineOCR.enabled = onlineOCREnabledCheckBox.IsChecked;
            responseData.ocr.onlineOCR.apiKey = apiKeyTextBox.Text;
            responseData.ocr.onlineOCR.secretKey = secretKeyTextBox.Text;
            File.WriteAllText(fileManager.UserDataJsonPath, Newtonsoft.Json.JsonConvert.SerializeObject(responseData, Newtonsoft.Json.Formatting.Indented));
        }

        private void MainCanvas_MouseLeftButtonDown(Object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            localOCRQuestionButton.ClosePopup();
        }

        private void MainCanvas_MouseRightButtonDown(Object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            localOCRQuestionButton.ClosePopup();
        }
    }
}
