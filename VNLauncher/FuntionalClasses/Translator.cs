#pragma warning disable IDE0049

using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace VNLauncher.FuntionalClasses
{
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
    public class GPTTranslator
    {
        private String url;
        private HttpClient httpClient;
        private String apiKey = "sk-mhpEcarQrnAhAPReNM9aF9gEMamN5BFObjeGjslah6ytNeZ9";
        private String userAgent = "Apifox/1.0.0 (https://apifox.com)";

        private Conversation conversation;


        public GPTTranslator()
        {
            url = "https://api.chatanywhere.com.cn/v1/chat/completions";
            httpClient = new HttpClient();
            conversation = new Conversation(10);
        }
        public void RemoveLast()
        {
            conversation.History.RemoveAt(conversation.History.Count - 1);
        }
        public async Task<String> Translate(String jp)
        {

            List<Dictionary<String, String>> text = new List<Dictionary<String, String>>();
            jp = TextModifier.Modify(jp);
            conversation.Add(jp);
            text.Add(new Dictionary<String, String>
                {
                    { "role", "user" },
                    { "content", "阅读下面一段日文对话或独白：\n："+conversation.ToString()+"结合上文翻译对话中的最后一句："+conversation.LastSentence+"\n为简体中文。" +
                    "注意，对话来源于OCR光学识别，因此可能有形近字错误。"},
                });

            Dictionary<String, Object> requestData = new Dictionary<String, Object>
                {
                    { "model", "gpt-3.5-turbo-0125" },
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
    public class LocalTranslator
    {
        private String url;
        private HttpClient httpClient;
        List<Dictionary<String, String>> history = new List<Dictionary<String, String>>();
        public LocalTranslator()
        {
            url = "http://127.0.0.1:5000/v1/chat/completions";
            httpClient = new HttpClient();
        }
        public async Task<String> Translate(String jp)
        {
            jp = TextModifier.Modify(jp);
            history.Add(
                new Dictionary<String, String>
            {
                { "role", "user" },
                { "content", "将这段文本直接翻译成中文，不要进行任何额外的格式修改，如果遇到大量语气词，请直接将语气词保留，注意连接上下文，这里是你需要翻译的文本：{"+jp+"}" }
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
            if (history.Count >= 22)
            {
                history.RemoveAt(0);
                history.RemoveAt(0);
            }
            return assistantMessage;
        }
    }
}
