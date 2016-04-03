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

            // Check if already translated / romanized
            if (IsTranslated(inText)) return inText;

            // Split the text into separate sequential tokens and translate each token
            // 1. Romanized - Don't translate
            // 2. Katakana - Translate to output language
            // 3. Hiragana / Kanji - Translate to phonetic
            List<TextToken> textTokens = GetTextTokens(inText);

            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;

            string outText = "";

            foreach (TextToken textToken in textTokens) {
                switch (textToken.Type) {
                    case TokenType.HiraganaKanji: {
                        // Get phoentic text
                        string url = string.Format("https://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}",
                                                   textToken.Text, LanguagePair);
                        HtmlDocument doc = new HtmlWeb().Load(url);
                        string phoneticText = doc.GetElementbyId("src-translit").InnerText;

                        // Add prefix and trim whitespace
                        outText += textToken.Prefix + phoneticText.Trim();
                        break;
                    }

                    case TokenType.Katakana: {
                        // Get translated text
                        string url = string.Format("https://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}",
                                                   textToken.Text, LanguagePair);
                        HtmlDocument doc = new HtmlWeb().Load(url);
                        string translatedText = doc.GetElementbyId("result_box").InnerText;

                        // Add prefix and trim whitespace
                        outText += textToken.Prefix + translatedText.Trim();
                        break;
                    }

                    case TokenType.Romanized:
                    default: {
                        // Add prefix
                        outText += textToken.Prefix + textToken.Text;
                        break;
                    }
                }
            }

            // Trim leading/trailing whitespace
            outText = outText.Trim();

            return outText;
        }

        public static bool IsTranslated(string text) {
            return text.Where(c => IsJapanese(c.ToString())).Count() == 0;
        }

        public static bool IsRomanized(string text) {
            return text.Where(c => c >= LatinMin && c <= LatinMax).Count() == text.Length;
        }

        public static bool IsJapanese(string text) {
            return text.Where(c => IsHiragana(c.ToString()) ||
                                   IsKatakana(c.ToString()) ||
                                   IsKanji(c.ToString())
                             ).Count() == text.Length;
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

        // Loop through characters in a string and split them into sequential tokens
        // eg. "Cake 01. ヴァンパイア雪降る夜"
        // => ["Cake 01. ", "ヴァンパイア", "雪降る夜"]
        private static List<TextToken> GetTextTokens(string inText) {
            List<TextToken> textTokens = new List<TextToken>();

            // Start with arbitrary token type
            TokenType prevCharTokenType = TokenType.Romanized;
            TokenType currCharTokenType = prevCharTokenType;

            TextToken currToken = new TextToken(currCharTokenType);

            foreach (char c in inText) {
                string cs = c.ToString();

                if (IsHiragana(cs) || IsKanji(cs)) {
                    // Hiragana / Kanji
                    currCharTokenType = TokenType.HiraganaKanji;
                } else if (IsKatakana(cs)) {
                    // Katakana
                    currCharTokenType = TokenType.Katakana;
                } else {
                    // Romanized or other
                    currCharTokenType = TokenType.Romanized;
                }

                // Check if there is a new token
                if (prevCharTokenType == currCharTokenType) {
                    // Same token
                    currToken.Text += cs;
                } else {
                    // New token

                    // Modifies the prefix of the token depending on prev/curr tokens
                    // eg. Add space before curr token
                    string tokenPrefix = "";

                    if (!string.IsNullOrEmpty(currToken.Text)) {
                        // Add token to token list if there is text in it
                        textTokens.Add(currToken);

                        // Get token prefix for new token if previous token was not empty
                        if (textTokens.Count > 0) {
                            char prevLastChar = textTokens.Last().Text.Last();
                            tokenPrefix = GetTokenPrefix(prevCharTokenType,
                                                             currCharTokenType,
                                                             prevLastChar, c);
                        }
                    }

                    // Create new token
                    currToken = new TextToken(currCharTokenType, cs, tokenPrefix);

                    prevCharTokenType = currCharTokenType;
                }
            }

            // Add last token to the list
            if (!string.IsNullOrEmpty(currToken.Text)) {
                textTokens.Add(currToken);
            }

            return textTokens;
        }

        private static string GetTokenPrefix(TokenType prevType, TokenType currType,
                                               char prevLastChar, char currFirstChar) {
            string prefix = "";

            switch (currType) {
                // =========================================================================
                // Current: Romanized
                // =========================================================================
                case TokenType.Romanized:
                    switch (prevType) {
                        // ==============================
                        // Previous: HiraganaKanji
                        // ==============================
                        case TokenType.HiraganaKanji:
                            if (!char.IsWhiteSpace(currFirstChar) &&
                                !char.IsPunctuation(currFirstChar) &&
                                currFirstChar != '~') {
                                prefix = " ";
                            }
                            break;

                        // ==============================
                        // Previous: Katakana
                        // ==============================
                        case TokenType.Katakana:
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
                case TokenType.HiraganaKanji:
                    switch (prevType) {
                        // ==============================
                        // Previous: Romanized
                        // ==============================
                        case TokenType.Romanized:
                            if (!char.IsWhiteSpace(prevLastChar) &&
                                prevLastChar != '~') {
                                prefix = " ";
                            }
                            break;

                        // ==============================
                        // Previous: Katakana
                        // ==============================
                        case TokenType.Katakana:
                            prefix = " ";
                            break;
                    }
                    break;

                // =========================================================================
                // Current: Katakana
                // =========================================================================
                case TokenType.Katakana:
                    switch (prevType) {
                        // ==============================
                        // Previous: Romanized
                        // ==============================
                        case TokenType.Romanized:
                            if (!char.IsWhiteSpace(prevLastChar)) {
                                prefix = " ";
                            }
                            break;

                        // ==============================
                        // Previous: HirganaKanji
                        // ==============================
                        case TokenType.HiraganaKanji:
                            prefix = " ";
                            break;
                    }
                    break;
            }

            return prefix;
        }

    }
}
