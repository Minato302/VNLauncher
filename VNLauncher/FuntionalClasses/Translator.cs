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
    public class ChatGPTTranslator
    {
        private String url;
        private HttpClient httpClient;
        private String apiKey = "sk-mhpEcarQrnAhAPReNM9aF9gEMamN5BFObjeGjslah6ytNeZ9";
        private String userAgent = "Apifox/1.0.0 (https://apifox.com)";

        private Conversation conversation;


        public ChatGPTTranslator()
        {
            url = "https://api.chatanywhere.com.cn/v1/chat/completions";
            httpClient = new HttpClient();
            conversation = new Conversation(10);
        }
        public async Task<String> Translate(String jp)
        {

            List<Dictionary<String, String>> text = new List<Dictionary<String, String>>();
            if (jp.Contains("一一"))
            {
                Int32 i = jp.IndexOf("一一");
                StringBuilder sb = new StringBuilder(jp);
                sb[i] = '—';
                sb[i + 1] = '-';
                jp = sb.ToString();

            }
            conversation.Add(jp);
            text.Add(new Dictionary<String, String>
                {
                    { "role", "user" },
                    { "content", "阅读下面一段日文对话：\n："+conversation.ToString()+"结合上文翻译对话中的最后一句："+conversation.LastSentence+"\n为简体中文。" +
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
}
