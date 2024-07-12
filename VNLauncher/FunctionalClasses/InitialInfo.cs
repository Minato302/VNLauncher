#pragma warning disable IDE0049

using Newtonsoft.Json.Linq;

namespace VNLauncher.FunctionalClasses
{
    public class InitialInfo
    {
        public void WriteInitialInfo(String path)
        {
            JObject initialSettings = new JObject();

            JObject keyMapping = new JObject
            {
                ["translateSwitch"] = "键盘按键Tab",
                ["showMarquee"] = "键盘按键左Shift",
                ["screenShot"] = "键盘按键F12",
                ["retranslate"] = "键盘按键R",
                ["captureSideUpMove"] = "键盘按键↑",
                ["captureSideDownMove"] = "键盘按键↓",
                ["captureSideLeftMove"] = "键盘按键←",
                ["captureSideRightMove"] = "键盘按键→"
            };

            JObject localOCR = new JObject
            {
                ["isV4Model"] = "false",
                ["usingGPU"] = "false"
            };

            JObject onlineOCR = new JObject
            {
                ["enabled"] = false,
                ["apiKey"] = "",
                ["secretKey"] = ""
            };

            JObject OCR = new JObject
            {
                ["localOCR"] = localOCR,
                ["onlineOCR"] = onlineOCR
            };

            JObject baiduTranslate = new JObject
            {
                ["enabled"] = false,
                ["apiKey"] = "",
                ["secretKey"] = ""
            };

            JObject gptPrompt = new JObject
            {
                ["hasContext"] = true,
                ["contextFirst"] = true,
                ["prompt1"] = "阅读下面一段日文对话：\r\n",
                ["prompt2"] = "\r\n翻译其中最后一句：\r\n",
                ["prompt3"] = "\r\n为简体中文。注意，给你的日文句子结果来自OCR光学字符识别，因此可能会有形近字错误。\r\n"
            };

            JObject onlineModelTranslate = new JObject
            {
                ["enabled"] = false,
                ["url"] = "https://api.chatanywhere.com.cn/v1/chat/completions",
                ["apiKey"] = "",
                ["model"] = "gpt-3.5-turbo",
                ["context"] = 10,
                ["prompt"] = gptPrompt
            };

            JObject localTranslate = new JObject
            {
                ["enabled"] = false,
                ["url"] = "http://127.0.0.1:5000",
                ["context"] = 22,
                ["prompt"] = "将这段文本直接翻译成中文，不要进行任何额外的格式修改，如果遇到大量语气词，请直接将语气词保留，注意连接上下文，这里是你需要翻译的文本："
            };

            JObject marquee = new JObject
            {
                ["bilingual"] = true,
                ["isAutoWait"] = true,
                ["backgroundTransparency"] = 0x99,
                ["textTransparency"] = 0xBB,
                ["fontSize"] = 20,
                ["waitTime"] = 20

            };

            initialSettings["keyMapping"] = keyMapping;
            initialSettings["ocr"] = OCR;
            initialSettings["baiduTranslate"] = baiduTranslate;
            initialSettings["onlineModelTranslate"] = onlineModelTranslate;
            initialSettings["localTranslate"] = localTranslate;
            initialSettings["marquee"] = marquee;

            System.IO.File.WriteAllText(path, initialSettings.ToString());
        }
    }
}
