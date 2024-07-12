#pragma warning disable IDE0049

using OpenCvSharp;
using OpenCvSharp.Extensions;
using Sdcb.PaddleInference;
using Sdcb.PaddleOCR;
using Sdcb.PaddleOCR.Models;
using Sdcb.PaddleOCR.Models.Local;
using System.Net.Http;
using System.Text;

namespace VNLauncher.FunctionalClasses
{
    public class WordBlock
    {
        private Rect block;

        private String words;
        public Rect Block => block;
        public String Words => words;
        public WordBlock(Rect block, String words)
        {
            this.block = block;
            this.words = words;

        }
        public static WordBlock Merge(WordBlock r1, WordBlock r2, Boolean horizontal)
        {
            Point[] points = new Point[4];
            points[0] = new Point(Math.Min(r1.block.X, r2.block.X), Math.Min(r1.block.Y, r2.block.Y));
            points[1] = new Point(Math.Max(r1.block.X + r1.block.Width, r2.block.X + r2.block.Width), Math.Min(r1.block.Y, r2.block.Y));
            points[2] = new Point(Math.Max(r1.block.X + r1.block.Width, r2.block.X + r2.block.Width), Math.Max(r1.block.Y + r1.block.Height, r2.block.Y + r2.block.Height));
            points[3] = new Point(Math.Min(r1.block.X, r2.block.X), Math.Max(r1.block.Y + r1.block.Height, r2.block.Y + r2.block.Height));

            RotatedRect minRect = Cv2.MinAreaRect(points);
            if (horizontal)
            {
                if (r1.block.Left < r2.block.Left)
                {
                    return new WordBlock(minRect.BoundingRect(), r1.words + r2.words);
                }
                else
                {
                    return new WordBlock(minRect.BoundingRect(), r2.words + r1.words);
                }
            }
            else
            {
                if (r1.block.Top < r2.block.Top)
                {
                    return new WordBlock(minRect.BoundingRect(), r1.words + r2.words);
                }
                else
                {
                    return new WordBlock(minRect.BoundingRect(), r2.words + r1.words);
                }
            }

        }
        public Boolean IsVaildBlock(Int32 minWidth, Int32 minHeight)
        {
            if (block.Height < minHeight || block.Width < minWidth)
            {
                return false;
            }
            Int32 down = Convert.ToInt32("0800", 16);
            Int32 up = Convert.ToInt32("9fa5", 16);
            foreach (Char ch in words)
            {
                if (TextModifier.IsJapaneseWord(ch))
                {
                    return true;
                }
            }
            return false;
        }
        private static Boolean InSameLine(WordBlock b1, WordBlock b2, Double d)
        {
            return Math.Abs((b1.block.Top + b1.block.Bottom) / 2 - (b2.block.Top + b2.block.Bottom) / 2) <= d;
        }
        public static WordBlock Splicing(List<WordBlock> wordBlocks)
        {
            Double minDistance = 0;
            foreach (var wordBlock in wordBlocks)
            {
                minDistance += wordBlock.block.Height;
            }
            minDistance /= wordBlocks.Count * 2;
            List<List<WordBlock>> res = new List<List<WordBlock>>();
            foreach (WordBlock wordBlock in wordBlocks)
            {

                Int32 ind;
                for (ind = 0; ind < res.Count; ind++)
                {
                    if (InSameLine(wordBlock, res[ind][0], minDistance))
                    {
                        res[ind].Add(wordBlock);
                        break;
                    }
                }
                if (ind == res.Count)
                {
                    res.Add(new List<WordBlock> { wordBlock });
                }
            }
            if (res.Count != 0)
            {
                WordBlock temp;
                foreach (var combined in res)
                {
                    combined.Sort((WordBlock b1, WordBlock b2) =>
                    {
                        if (b1.block.Left < b2.block.Left)
                        {
                            return 1;
                        }
                        else if (b1.block.Left > b2.block.Left)
                        {
                            return -1;
                        }
                        else return 0;
                    });
                    temp = new WordBlock(combined[0].block, combined[0].words);
                    for (int i = 1; i < combined.Count; i++)
                    {
                        temp = Merge(temp, combined[i], true);
                    }
                    combined[0] = temp;
                }
                temp = new WordBlock(res[0][0].block, res[0][0].words);
                for (int i = 1; i < res.Count; i++)
                {
                    temp = Merge(temp, res[i][0], false);
                }
                res[0][0] = temp;
            }
            if (res.Count == 0)
            {
                return new WordBlock(new Rect(), "");
            }
            return res[0][0];

        }
    }
    public abstract class OCR
    {
        public class OCRResult
        {
            private Boolean hasContent;
            private String? resultText;
            public OCRResult(Boolean hasContent, String? resultText = null)
            {
                this.hasContent = hasContent;
                this.resultText = resultText;
            }
            public Boolean HasContent => hasContent;
            public String ResultText => resultText!;
        }
        public abstract Task<OCRResult> Scan(System.Drawing.Bitmap pic);
    }
    public class LocalOCR : OCR
    {
        private FullOcrModel model;
        private PaddleOcrAll all;

        public LocalOCR(Boolean isV4Model, Boolean usingGPU)
        {
            if (isV4Model)
            {
                model = LocalFullModels.JapanV4;
            }
            else
            {
                model = LocalFullModels.JapanV3;
            }
            if (usingGPU)
            {
                all = new PaddleOcrAll(model, PaddleDevice.Gpu())
                {
                    AllowRotateDetection = false,
                    Enable180Classification = false,
                };
            }
            else
            {
                all = new PaddleOcrAll(model, PaddleDevice.Mkldnn())
                {
                    AllowRotateDetection = false,
                    Enable180Classification = false,
                };
            }
        }
        public async override Task<OCRResult> Scan(System.Drawing.Bitmap pic)
        {
            List<WordBlock> blocks = new List<WordBlock>();
            Mat src = BitmapConverter.ToMat(pic);
            Mat mat = new Mat();
            Cv2.CvtColor(src, mat, ColorConversionCodes.BGRA2BGR);
            PaddleOcrResult result = all.Run(mat);
            foreach (PaddleOcrResultRegion region in result.Regions)
            {
                if (region.Score >= 0.8)
                {
                    blocks.Add(new WordBlock(region.Rect.BoundingRect(), region.Text));
                }
            }
            for (Int32 i = 0; i < blocks.Count; i++)
            {
                if (!blocks[i].IsVaildBlock(10, 10))
                {
                    blocks.Remove(blocks[i]);
                    i--;
                }
            }
            mat.Dispose();
            src.Dispose();
            if (blocks.Count > 0)
            {
                return new OCRResult(true, WordBlock.Splicing(blocks).Words);
            }
            else
            {
                return new OCRResult(false);
            }
        }
    }
    public class BaiduOCR : OCR
    {
        private String apiKey;
        private String secretKey;
        public BaiduOCR(String apiKey, String secretKey)
        {
            this.apiKey = apiKey;
            this.secretKey = secretKey;
        }
        public async override Task<OCRResult> Scan(System.Drawing.Bitmap pic)
        {
            String token = await GetToken();

            String url = "https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic?access_token=" + token;
            String base64 = BitmapToByte(pic);
            Dictionary<String, String> requestData = new Dictionary<String, String>
            {
                { "image", base64 },
                { "language_type", "JAP"}
            };
            FormUrlEncodedContent formContent = new FormUrlEncodedContent(requestData);
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsync(url, formContent);
            response.EnsureSuccessStatusCode();
            String responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseData = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent)!;
            Int32 lineCount = responseData.words_result_num;
            String result = "";
            for (Int32 i = 0; i < lineCount; i++)
            {
                 result += responseData.words_result[i].words;
            }
          

            if (responseContent != "")
            {
                return new OCRResult(true, result);
            }
            else
            {
                return new OCRResult(false);
            }

        }
        private String BitmapToByte(System.Drawing.Bitmap bitmap)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            ms.Seek(0, System.IO.SeekOrigin.Begin);
            Byte[] bytes = new Byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length);
            ms.Dispose();
            return Convert.ToBase64String(bytes);
        }
        private async Task<String> GetToken()
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
            return token;
        }

    }
}
