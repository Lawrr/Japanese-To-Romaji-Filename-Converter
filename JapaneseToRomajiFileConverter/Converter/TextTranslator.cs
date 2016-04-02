using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace JapaneseToRomajiFileConverter.Converter {
    public class TextTranslator {

        public const string LanguagePair = "ja|en";

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
            // Normalize
            inText = inText.Normalize(NormalizationForm.FormKC);

            // Check if already romanized
            if (IsRomanized(inText)) return inText;

            // Split the text into separate sequential sections and translate each section
            // 1. Romanized - Don't translate
            // 2. Katakana - Translate to output language
            // 3. Hiragana / Kanji - Translate to phonetic
            List<TextSection> textSections = GetTextSections(inText);

            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;

            string outText = "";

            foreach (TextSection textSection in textSections) {
                switch (textSection.Type) {
                    case SectionType.HiraganaKanji: {
                        // Get phoentic text
                        string url = string.Format("https://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}",
                                                   textSection.Text, LanguagePair);
                        HtmlDocument doc = new HtmlWeb().Load(url);
                        string phoneticText = doc.GetElementbyId("src-translit").InnerText;
                        outText += phoneticText;
                        break;
                    }

                    case SectionType.Katakana: {
                        // Get translated text
                        string url = string.Format("https://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}",
                                                   textSection.Text, LanguagePair);
                        HtmlDocument doc = new HtmlWeb().Load(url);
                        string translatedText = doc.GetElementbyId("result_box").InnerText;
                        outText += translatedText;
                        break;
                    }

                    case SectionType.Romanized:
                    default: {
                        // Leave as is
                        outText += textSection.Text;
                        break;
                    }
                }
            }

            // Trim leading/trailing whitespace
            outText = outText.Trim();

            return outText;
        }

        // Loop through characters in a string and split them into sequential sections
        // eg. "Cake 01. ヴァンパイア雪降る夜"
        // => ["Cake 01. ", "ヴァンパイア", "雪降る夜"]
        private static List<TextSection> GetTextSections(string inText) {
            List<TextSection> textSections = new List<TextSection>();

            SectionType prevSectionType = SectionType.Romanized;
            SectionType currSectionType = prevSectionType;

            TextSection currSection = new TextSection(currSectionType, "");

            foreach (char c in inText) {
                string cs = c.ToString();

                if (IsHiragana(cs) || IsKanji(cs)) {
                    // Hiragana / Kanji
                    currSectionType = SectionType.HiraganaKanji;
                } else if (IsKatakana(cs)) {
                    // Katakana
                    currSectionType = SectionType.Katakana;
                } else {
                    // Romanized or other
                    currSectionType = SectionType.Romanized;
                }

                // Check if there is a new section
                if (prevSectionType == currSectionType) {
                    // Same section
                    currSection.Text += cs;
                } else {
                    // New section
                    prevSectionType = currSectionType;

                    // Add section to section list if there is text in it
                    if (!string.IsNullOrEmpty(currSection.Text)) {
                        textSections.Add(currSection);
                    }

                    // Create new section
                    currSection = new TextSection(currSectionType, cs);
                }
            }

            // Add last section to the list
            if (!string.IsNullOrEmpty(currSection.Text)) {
                textSections.Add(currSection);
            }

            return textSections;
        }

        public static bool IsRomanized(string text) {
            return text.Where(c => c >= LatinMin && c <= LatinMax).Count() == text.Length;
        }

        public static bool IsHiragana(string text) {
            return text.Where(c => c >= HiraganaMin && c <= HiraganaMax).Count() == text.Length;
        }

        public static bool IsKatakana(string text) {
            return text.Where(c => c >= KatakanaMin && c <= KatakanaMax).Count() == text.Length;
        }

        public static bool IsKanji(string text) {
            return text.Where(c => c >= KanjiMin && c <= KanjiMax).Count() == text.Length;
        }

    }
}
