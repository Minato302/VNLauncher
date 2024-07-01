#pragma warning disable IDE0049

using System.Net.Http;
using System.Text;

namespace VNLauncher.FuntionalClasses
{
    public abstract class Translator
    {
        public abstract Task<String> Translate(String jp);
        public abstract void RemoveLast();
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
    public class OnlineModelTranslator :Translator
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
            userAgent = "Apifox/1.0.0 (https://apifox.com)";

            this.url = url;
            this.apiKey = apiKey;
            this.model = model;
            conversation = new Conversation(contextNum);
            this.prompt = prompt;
        }
        public override void RemoveLast()
        {
            conversation.History.RemoveAt(conversation.History.Count - 1);
        }
        public override async Task<String> Translate(String jp)
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
    }
    public class LocalTranslator :Translator
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
        }
        public override async Task<String> Translate(String jp)
        {
            history.Add(
                new Dictionary<String, String>
            {
                { "role", "user" },
                { "content", prompt +"{"+jp+"}" }
            });
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
        public override void RemoveLast()
        {
            history.RemoveAt(history.Count - 1);
            history.RemoveAt(history.Count - 1);
        }
    }
}
