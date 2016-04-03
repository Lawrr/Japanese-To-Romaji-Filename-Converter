using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System;
using System.Globalization;

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

                        // Add prefix and trim whitespace
                        outText += textSection.Prefix + phoneticText.Trim();
                        break;
                    }

                    case SectionType.Katakana: {
                        // Get translated text
                        string url = string.Format("https://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}",
                                                   textSection.Text, LanguagePair);
                        HtmlDocument doc = new HtmlWeb().Load(url);
                        string translatedText = doc.GetElementbyId("result_box").InnerText;

                        // Add prefix and trim whitespace
                        outText += textSection.Prefix + translatedText.Trim();
                        break;
                    }

                    case SectionType.Romanized:
                    default: {
                        // Add prefix
                        outText += textSection.Prefix + textSection.Text;
                        break;
                    }
                }
            }

            // Trim leading/trailing whitespace
            outText = outText.Trim();

            return outText;
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

        // Loop through characters in a string and split them into sequential sections
        // eg. "Cake 01. ヴァンパイア雪降る夜"
        // => ["Cake 01. ", "ヴァンパイア", "雪降る夜"]
        private static List<TextSection> GetTextSections(string inText) {
            List<TextSection> textSections = new List<TextSection>();

            // Start with arbitrary section type
            SectionType prevCharSectionType = SectionType.Romanized;
            SectionType currCharSectionType = prevCharSectionType;

            TextSection currSection = new TextSection(currCharSectionType);

            foreach (char c in inText) {
                string cs = c.ToString();

                if (IsHiragana(cs) || IsKanji(cs)) {
                    // Hiragana / Kanji
                    currCharSectionType = SectionType.HiraganaKanji;
                } else if (IsKatakana(cs)) {
                    // Katakana
                    currCharSectionType = SectionType.Katakana;
                } else {
                    // Romanized or other
                    currCharSectionType = SectionType.Romanized;
                }

                // Check if there is a new section
                if (prevCharSectionType == currCharSectionType) {
                    // Same section
                    currSection.Text += cs;
                } else {
                    // New section

                    // Modifies the prefix of the section depending on prev/curr sections
                    // eg. Add space before curr section
                    string sectionPrefix = "";

                    if (!string.IsNullOrEmpty(currSection.Text)) {
                        // Add section to section list if there is text in it
                        textSections.Add(currSection);

                        // Get section prefix for new section if previous section was not empty
                        if (textSections.Count > 0) {
                            char prevLastChar = textSections.Last().Text.Last();
                            sectionPrefix = GetSectionPrefix(prevCharSectionType,
                                                             currCharSectionType,
                                                             prevLastChar, c);
                        }
                    }

                    // Create new section
                    currSection = new TextSection(currCharSectionType, cs, sectionPrefix);

                    prevCharSectionType = currCharSectionType;
                }
            }

            // Add last section to the list
            if (!string.IsNullOrEmpty(currSection.Text)) {
                textSections.Add(currSection);
            }

            return textSections;
        }

        private static string GetSectionPrefix(SectionType prevType, SectionType currType,
                                               char prevLastChar, char currFirstChar) {
            string prefix = "";

            switch (currType) {
                // =========================================================================
                // Current: Romanized
                // =========================================================================
                case SectionType.Romanized:
                    switch (prevType) {
                        // ==============================
                        // Previous: HiraganaKanji
                        // ==============================
                        case SectionType.HiraganaKanji:
                            if (!char.IsWhiteSpace(currFirstChar) &&
                                !char.IsPunctuation(currFirstChar) &&
                                currFirstChar != '~') {
                                prefix = " ";
                            }
                            break;

                        // ==============================
                        // Previous: Katakana
                        // ==============================
                        case SectionType.Katakana:
                            if (!char.IsWhiteSpace(currFirstChar) &&
                                !char.IsPunctuation(currFirstChar)) {
                                prefix = " ";
                            }
                            break;
                    }
                    break;

                // =========================================================================
                // Current: HiraganaKanji
                // =========================================================================
                case SectionType.HiraganaKanji:
                    switch (prevType) {
                        // ==============================
                        // Previous: Romanized
                        // ==============================
                        case SectionType.Romanized:
                            if (!char.IsWhiteSpace(prevLastChar) &&
                                prevLastChar != '~') {
                                prefix = " ";
                            }
                            break;

                        // ==============================
                        // Previous: Katakana
                        // ==============================
                        case SectionType.Katakana:
                            prefix = " ";
                            break;
                    }
                    break;

                // =========================================================================
                // Current: Katakana
                // =========================================================================
                case SectionType.Katakana:
                    switch (prevType) {
                        // ==============================
                        // Previous: Romanized
                        // ==============================
                        case SectionType.Romanized:
                            if (!char.IsWhiteSpace(prevLastChar)) {
                                prefix = " ";
                            }
                            break;

                        // ==============================
                        // Previous: HirganaKanji
                        // ==============================
                        case SectionType.HiraganaKanji:
                            prefix = " ";
                            break;
                    }
                    break;
            }

            return prefix;
        }

    }
}
