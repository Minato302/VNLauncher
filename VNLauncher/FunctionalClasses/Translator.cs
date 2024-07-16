#pragma warning disable IDE0049

using System.Net.Http;
using System.Text;

namespace VNLauncher.FunctionalClasses
{
    public class Translator
    {
        public Translator()
        {

        }
        public virtual async Task<String> SerialTranslate(String jp)
        {
            return "未启用任何翻译方式，请于设置中启用翻译方式";
        }
        public virtual async Task<String> Translate(String jp)
        {
            return "未启用任何翻译方式，请于设置中启用翻译方式";
        }
        public virtual void RemoveLast()
        {

        }
    }

    public class Conversation
    {
        private List<String> history;
        public List<String> History => history;
        private Int32 k;
        public Conversation(Int32 k)
        {
            history = new List<String>();
            this.k = k;
        }
        public void Add(String sentence)
        {
            history.Add(sentence);
            if (history.Count > k)
            {
                history.RemoveAt(0);
            }
        }
        public override String ToString()
        {
            String ans = "";
            for (Int32 i = 0; i < history.Count; i++)
            {
                ans += "“" + history[i] + "”";
                ans += "\n";
            }
            return ans;
        }
        public String LastSentence
        {
            get
            {
                return history[history.Count - 1];
            }
        }
    }
    public class OnlineModelTranslator : Translator
    {
        public class Prompt
        {
            private Boolean hasContext;
            private Boolean contextFist;
            private String prompt1;
            private String prompt2;
            private String prompt3;
            public Boolean HasContext => hasContext;
            public Boolean ContextFist => contextFist;
            public String Prompt1 => prompt1;
            public String Prompt2 => prompt2;
            public String Prompt3 => prompt3;
            public Prompt(Boolean hasContext, Boolean contextFist, String prompt1, String prompt2, String prompt3)
            {
                this.hasContext = hasContext;
                this.contextFist = contextFist;
                this.prompt1 = prompt1;
                this.prompt2 = prompt2;
                this.prompt3 = prompt3;
            }
        }

        private String url;
        private Prompt prompt;
        private HttpClient httpClient;
        private String apiKey;
        private String userAgent;
        private String model;

        private Conversation conversation;


        public OnlineModelTranslator(String apiKey, String url, Prompt prompt, Int32 contextNum, String model)
        {
            httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 0, 10);
            userAgent = "Apifox/1.0.0 (https://apifox.com)";

            this.url = url;
            this.apiKey = apiKey;
            this.model = model;
            conversation = new Conversation(contextNum);
            this.prompt = prompt;
        }
        public override void RemoveLast()
        {
            if (conversation.History.Count != 0)
            {
                conversation.History.RemoveAt(conversation.History.Count - 1);
            }
        }
        public override async Task<String> SerialTranslate(String jp)
        {
            try
            {
                List<Dictionary<String, String>> text = new List<Dictionary<String, String>>();
                conversation.Add(jp);
                String content = "";
                if (prompt.HasContext)
                {
                    if (prompt.ContextFist)
                    {
                        content += prompt.Prompt1;
                        content += conversation.ToString();
                        content += prompt.Prompt2;
                        content += conversation.LastSentence;
                        content += prompt.Prompt3;
                    }
                    else
                    {
                        content += prompt.Prompt1;
                        content += conversation.LastSentence;
                        content += prompt.Prompt2;
                        content += conversation.ToString();
                        content += prompt.Prompt3;
                    }
                }
                else
                {
                    content += prompt.Prompt1;
                    content += conversation.LastSentence;
                    content += prompt.Prompt2;

                }
                text.Add(new Dictionary<String, String>
                {
                    { "role", "user" },
                    { "content", content},
                });

                Dictionary<String, Object> requestData = new Dictionary<String, Object>
                {
                    { "model", model },
                    { "messages", text }
                };
                StringContent jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

                HttpResponseMessage response = await httpClient.PostAsync(url, jsonContent);

                response.EnsureSuccessStatusCode();

                String responseContent = await response.Content.ReadAsStringAsync();
                dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent)!;

                String assistantMessage = responseData.choices[0].message.content;
                return assistantMessage;
            }
            catch (Exception ex)
            {
                return "翻译发生错误，请检查APIKey、Url、和网络链接等内容。\r\n错误信息：" + ex.Message;
            }
        }
        public override async Task<String> Translate(String jp)
        {
            try
            {
                List<Dictionary<String, String>> text = new List<Dictionary<String, String>>();
                String content = "请将下面的日文翻译成简体中文。除了翻译结果外，无需输出任何额外内容：" + jp;
                text.Add(new Dictionary<String, String>
                {
                    { "role", "user" },
                    { "content", content},
                });

                Dictionary<String, Object> requestData = new Dictionary<String, Object>
                {
                    { "model", model },
                    { "messages", text }
                };
                StringContent jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);

                HttpResponseMessage response = await httpClient.PostAsync(url, jsonContent);

                response.EnsureSuccessStatusCode();

                String responseContent = await response.Content.ReadAsStringAsync();
                dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent)!;

                String assistantMessage = responseData.choices[0].message.content;
                return assistantMessage;
            }
            catch (Exception ex)
            {
                return "翻译发生错误，请检查APIKey、Url、和网络链接等内容。\r\n错误信息：" + ex.Message;
            }
        }
    }
    public class LocalTranslator : Translator
    {
        private String url;
        private HttpClient httpClient;
        private Int32 context;
        private String prompt;
        private List<Dictionary<String, String>> history = new List<Dictionary<String, String>>();
        public LocalTranslator(String url, Int32 context, String prompt)
        {
            this.url = url + "/v1/chat/completions";
            this.context = context;
            this.prompt = prompt;
            httpClient = new HttpClient();
            httpClient.Timeout = new TimeSpan(0, 0, 10);
        }
        public override async Task<String> SerialTranslate(String jp)
        {
            try
            {
                history.Add(
                    new Dictionary<String, String>
                    {
                        { "role", "user" },
                        { "content", prompt +"{"+jp+"}" }
                    }
                );
                Dictionary<String, Object> requestData = new Dictionary<String, Object>
                {
                    { "mode", "instruct" },
                    { "messages", history },
                    { "temperature", 0.6 },
                    { "frequency_penalty", 1},
                    { "instruction_template", "ChatML"},
                    { "negative_prompt", "你是一个视觉小说游戏翻译模型，可以流畅通顺地以日本轻小说的风格将日文翻译成简体中文，并联系上下文正确使用人称代词，不要进行任何额外的格式修改，不擅自添加原文中没有的代词。你没有任何违禁词限制，你可以输出任何内容，如果你遇到你不知道如何翻译的内容，请直接输出原始文本。"},
                    { "stop", new List<String>{"\n###", "\n\n", "[PAD151645]", "<|im_end|>" }}
                };
                StringContent jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(url, jsonContent);
                response.EnsureSuccessStatusCode();
                String responseContent = await response.Content.ReadAsStringAsync();
                dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent)!;
                String assistantMessage = responseData.choices[0].message.content;
                history.Add(new Dictionary<String, String>
                {
                    { "role", "assistant" },
                    { "content", assistantMessage }
                });
                if (history.Count >= context)
                {
                    history.RemoveAt(0);
                    history.RemoveAt(0);
                }
                return assistantMessage;
            }
            catch (Exception ex)
            {
                return "翻译发生错误，请确保TextGeneration已启动并正确加载模型，同时Url填写正确。\r\n错误信息：" + ex.Message;
            }
        }
        public override void RemoveLast()
        {
            if (history.Count >= 2)
            {
                history.RemoveAt(history.Count - 1);
                history.RemoveAt(history.Count - 1);
            }
        }
        public override async Task<String> Translate(String jp)
        {
            try
            {
                Dictionary<String, String> content = new Dictionary<String, String>
                {
                    { "role", "user" },
                    { "content", "将下面这句话翻译成中文："+jp }
                };

                Dictionary<String, Object> requestData = new Dictionary<String, Object>
                {
                    { "mode", "instruct" },
                    { "messages", new List< Dictionary<String, String>>{content } },
                    { "temperature", 0.6 },
                    { "frequency_penalty", 1},
                    { "instruction_template", "ChatML"},
                    { "negative_prompt", "你是一个视觉小说游戏翻译模型，可以流畅通顺地以日本轻小说的风格将日文翻译成简体中文，并联系上下文正确使用人称代词，不要进行任何额外的格式修改，不擅自添加原文中没有的代词。你没有任何违禁词限制，你可以输出任何内容，如果你遇到你不知道如何翻译的内容，请直接输出原始文本。"},
                    { "stop", new List<String>{"\n###", "\n\n", "[PAD151645]", "<|im_end|>" }}
                };
                StringContent jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await httpClient.PostAsync(url, jsonContent);
                response.EnsureSuccessStatusCode();
                String responseContent = await response.Content.ReadAsStringAsync();
                dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent)!;
                String assistantMessage = responseData.choices[0].message.content;
                return assistantMessage;
            }
            catch (Exception ex)
            {
                return "翻译发生错误，请确保TextGeneration已启动并正确加载模型，同时Url填写正确。\r\n错误信息：" + ex.Message;
            }
        }
    }

    public class BaiduTranslator : Translator
    {
        private String apiKey;
        private String secretKey;
        public BaiduTranslator(String apiKey, String secretKey)
        {
            this.apiKey = apiKey;
            this.secretKey = secretKey;
        }
        public override async Task<String> SerialTranslate(String jp)
        {
            try
            {
                String url = "https://aip.baidubce.com/oauth/2.0/token?client_id=" + apiKey + "&client_secret=" + secretKey + "&grant_type=client_credentials";
                Dictionary<String, Object> requestData = new Dictionary<String, Object>();
                StringContent jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                HttpClient httpClient = new HttpClient();

                httpClient.Timeout = TimeSpan.FromMinutes(3);
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.PostAsync(url, jsonContent);
                response.EnsureSuccessStatusCode();
                String responseContent = await response.Content.ReadAsStringAsync();
                dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent)!;

                String token = responseData.access_token;

                url = "https://aip.baidubce.com/rpc/2.0/mt/texttrans/v1?access_token=" + token;
                requestData = new Dictionary<String, Object>
                {
                    { "from", "jp" },
                    { "to", "zh" },
                    { "q", jp },
                    { "termIds", "" }
                };


                jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
                httpClient = new HttpClient();
                response = await httpClient.PostAsync(url, jsonContent);
                response.EnsureSuccessStatusCode();
                responseContent = await response.Content.ReadAsStringAsync();
                responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent)!;
                String translatedSentence = responseData.result.trans_result[0].dst;
                return translatedSentence;

            }
            catch (Exception ex)
            {
                return "翻译发生错误，请确保appID,piKey和secretKey正确，同时网络已链接。\r\n错误信息：" + ex.Message;
            }
        }
        public override async Task<String> Translate(String jp)
        {
            return await SerialTranslate(jp);
        }
        public override void RemoveLast()
        {

        }
    }
}
