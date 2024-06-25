#pragma warning disable IDE0049

using System.Text;

namespace VNLauncher.FuntionalClasses
{
    public class TextModifier
    {
        private readonly static HashSet<Char> EmphasisWords = new HashSet<Char> { 'カ', '力', 'タ', '夕', 'ニ', '二' };
        private static Boolean IsKatakana(Char ch)
        {
            return '\u30A0' <= ch && ch <= '\u30FF';
        }
        public static Boolean IsJapaneseWord(Char ch)
        {
            Int32 down = Convert.ToInt32("0800", 16);
            Int32 up = Convert.ToInt32("9fa5", 16);
            if (down <= Convert.ToInt32(ch) && Convert.ToInt32(ch) <= up)
            {
                return true;
            }
            return false;
        }
        public static String Modify(String text)
        {
            StringBuilder sb = new StringBuilder(text);
            for (Int32 i = 1; i < text.Length - 1; i++)
            {
                if (text[i] == '力' && IsKatakana(text[i + 1]))
                {
                    sb[i] = 'カ';
                }
                if (text[i] == 'カ' && (!IsKatakana(text[i + 1])) && (!IsKatakana(text[i - 1])))
                {
                    sb[i] = '力';
                }
                if (text[i] == '夕' && (IsKatakana(text[i - 1]) || IsKatakana(text[i + 1])))
                {
                    sb[i] = 'タ';
                }
                if (text[i] == 'タ' && (!IsKatakana(text[i - 1]) && !IsKatakana(text[i + 1])))
                {
                    sb[i] = '夕';
                }
                if (text[i] == '二' && (IsKatakana(text[i - 1]) || IsKatakana(text[i + 1])))
                {
                    sb[i] = 'ニ';
                }
                if (text[i] == 'ニ' && (!IsKatakana(text[i - 1]) && !IsKatakana(text[i + 1])))
                {
                    sb[i] = '二';
                }
            }
            if (text.Contains("一一"))
            {
                Int32 i = text.IndexOf("一一");
                sb[i] = '—';
                sb[i + 1] = '-';
            }
            return sb.ToString();
        }
    }
}
