using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace JapaneseToRomajiFileConverter {
    public class Translator {

        public const string StartSrcSplit = "<div id=src-translit class=translit dir=ltr style=\"text-align:;display:block\">";
        public const string EndSrcSplit = "</div>";
        public const string LanguagePair = "ja|en";
        public const char MapChar = '`';

        // TODO store in better data structure?
        // Unicode ranges for each set
        public const int RomajiMin = 0x0020;
        public const int RomajiMax = 0x007E;
        public const int HiraganaMin = 0x3040;
        public const int HiraganaMax = 0x309F;
        public const int KatakanaMin = 0x30A0;
        public const int KatakanaMax = 0x30FF;
        public const int KanjiMin = 0x4E00;
        public const int KanjiMax = 0x9FBF;
        // Covers Basic Latin, Latin-1 Supplement, Extended A, Extended B
        public const int LatinMin = 0x0000;
        public const int LatinMax = 0x024F;

        public static string Translate(string inText, string languagePair = LanguagePair) {
            // Check null
            if (inText == null) return "";

            // Check if already romanized
            if (IsRomanized(inText)) return inText;

            // Map english characters to substitutes
            Tuple<string, List<char>> charMap = MapChars(inText);
            inText = charMap.Item1;

            // Get translation
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            string url = String.Format("https://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}",
                                       inText, LanguagePair);
            string src = webClient.DownloadString(url);

            // Get translation from source code between two strings
            string outText = src.Split(new string[] { StartSrcSplit }, StringSplitOptions.None).Last()
                                .Split(new string[] { EndSrcSplit }, StringSplitOptions.None).First();

            // Decode html encodings
            outText = WebUtility.HtmlDecode(outText);

            // Unmap english characters back from substitutes
            outText = UnmapChars(outText, charMap.Item2);

            return outText;
        }

        public static bool IsRomanized(string text) {
            return text.Where(c => c >= LatinMin && c <= LatinMax).Count() == text.Length;
        }

        private static Tuple<string, List<char>> MapChars(string text) {
            // Replace characters with sub chars for the translation
            List<char> mapChars = new List<char>();
            StringBuilder mapText = new StringBuilder(text);

            // Loop through each character and map english with $MapChar
            for (int i = 0; i < text.Length; i++) {
                char currChar = text[i];
                if (IsRomanized(currChar.ToString())) {
                    mapChars.Add(currChar);
                    mapText[i] = MapChar;
                } else if (currChar == MapChar) {
                    mapChars.Add(currChar);
                }
            }

            Console.WriteLine("Map");
            Console.WriteLine(text);
            Console.WriteLine(mapText.ToString());

            return Tuple.Create(mapText.ToString(), mapChars);
        }

        private static string UnmapChars(string text, List<char> mapChars) {
            StringBuilder unmapText = new StringBuilder(text);

            // Loop through each character and unmap mapped chars
            int mapIndex = 0;
            for (int i = 0; i < text.Length; i++) {
                if (text[i] == MapChar) {
                    unmapText[i] = mapChars[mapIndex++];
                }
            }

            Console.WriteLine("UnMap");
            Console.WriteLine(text);
            Console.WriteLine(unmapText.ToString());
            return unmapText.ToString();
        }

    }
}
