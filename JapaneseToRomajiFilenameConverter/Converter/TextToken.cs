using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace JapaneseToRomajiFilenameConverter.Converter {
    public class TextToken {

        public TokenType Type { get; private set; }
        public string Text { get; set; }
        public string Prefix { get; set; }

        public static Dictionary<string, string> PunctuationMap { get; private set; } = new Dictionary<string, string>() {
            { "、", "," },
            { "“", " \"" },
            { "”", "\"" }
        };

        public TextToken(TokenType type, string text = "", string prefix = "") {
            Type = type;
            Text = text;
            Prefix = prefix;
        }

        // Loop through characters in a string and split them into sequential tokens
        // eg. "Cake 01. ヴァンパイア雪降る夜"
        // => ["Cake 01. ", "ヴァンパイア", "雪降る夜"]
        public static List<TextToken> GetTextTokens(string inText) {
            List<TextToken> textTokens = new List<TextToken>();

            // Start with arbitrary token type
            TokenType prevCharTokenType = TokenType.Latin;
            TokenType currCharTokenType = prevCharTokenType;

            TextToken currToken = new TextToken(currCharTokenType);

            foreach (char c in inText) {
                string cs = c.ToString();

                if (TextTranslator.IsProlongedChar(c)) {
                    // Special condition for prolonged sound character
                    currCharTokenType = prevCharTokenType;
                } else if (TextTranslator.IsHiragana(cs) || TextTranslator.IsKanji(cs)) {
                    // Hiragana / Kanji
                    currCharTokenType = TokenType.HiraganaKanji;
                } else if (TextTranslator.IsKatakana(cs)) {
                    // Katakana
                    currCharTokenType = TokenType.Katakana;
                } else {
                    // Latin or other
                    currCharTokenType = TokenType.Latin;
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

        public static bool HasPrefix(TokenType prevType, TokenType currType, char prevLastChar, char currFirstChar) {
            bool hasPrefix = char.IsPunctuation(currFirstChar);

            switch (currType) {
                // =========================================================================
                // Current: Latin
                // =========================================================================
                case TokenType.Latin:
                    // ==============================
                    // Previous: HiraganaKanji / Katakana
                    // ==============================
                    if (!char.IsWhiteSpace(currFirstChar) && !char.IsPunctuation(currFirstChar)) {
                        hasPrefix = true;
                    }

                    // Some other characters which override above
                    switch (currFirstChar) {
                        case '(':
                        case '&':
                            hasPrefix = true;
                            break;

                        case '、':
                        case ',':
                        case '“':
                        case '”':
                        case '"':
                        case ')':
                        case '」':
                        case '~':
                        case '-':
                        case '!':
                            hasPrefix = false;
                            break;
                    }
                    break;

                // =========================================================================
                // Current: HiraganaKanji
                // =========================================================================
                case TokenType.HiraganaKanji:
                    switch (prevType) {
                        // ==============================
                        // Previous: Latin
                        // ==============================
                        case TokenType.Latin:
                            if (!char.IsWhiteSpace(prevLastChar)) {
                                hasPrefix = true;
                            }

                            // Some other characters which override above
                            switch (prevLastChar) {
                                case '(':
                                case '「':
                                case '"':
                                case '“':
                                case '~':
                                case '-':
                                    hasPrefix = false;
                                    break;
                            }
                            break;

                        // ==============================
                        // Previous: Katakana
                        // ==============================
                        case TokenType.Katakana:
                            hasPrefix = true;
                            break;
                    }
                    break;

                // =========================================================================
                // Current: Katakana
                // =========================================================================
                case TokenType.Katakana:
                    switch (prevType) {
                        // ==============================
                        // Previous: Latin
                        // ==============================
                        case TokenType.Latin:
                            if (!char.IsWhiteSpace(prevLastChar)) {
                                hasPrefix = true;
                            }

                            // Some other characters which override above
                            switch (prevLastChar) {
                                case '(':
                                case '「':
                                case '"':
                                case '“':
                                case '~':
                                case '-':
                                    hasPrefix = false;
                                    break;
                            }
                            break;

                        // ==============================
                        // Previous: HirganaKanji
                        // ==============================
                        case TokenType.HiraganaKanji:
                            hasPrefix = true;
                            break;
                    }
                    break;
            }

            return hasPrefix;
        }

        public static string GetTokenPrefix(TokenType prevType, TokenType currType,
                                            char prevLastChar, char currFirstChar) {
            string prefix = "";

            if (HasPrefix(prevType, currType, prevLastChar, currFirstChar)) {
                prefix = " ";
            }

            return prefix;
        }

        // 1. Latin - Don't translate
        // 2. Katakana - Translate to output language
        // 3. Hiragana / Kanji - Translate to phonetic
        public string Translate(List<string> maps = null,
                                List<string> particles = null,
                                string languagePair = TextTranslator.LanguagePair) {
            string translation = "";

            switch (Type) {
                case TokenType.HiraganaKanji: {
                    // Get phoentic text
                    string url = TextTranslator.GetTranslatorUrl(Text, languagePair);
                    HtmlDocument doc = new HtmlWeb().Load(url);
                    string phoneticText = WebUtility.HtmlDecode(doc.GetElementbyId("src-translit").InnerText);
                    translation = FormatTranslation(phoneticText, maps, particles);
                    break;
                }

                case TokenType.Katakana: {
                    // Get translated text
                    string url = TextTranslator.GetTranslatorUrl(Text, languagePair);
                    HtmlDocument doc = new HtmlWeb().Load(url);
                    string translatedText = WebUtility.HtmlDecode(doc.GetElementbyId("result_box").InnerText);
                    translation = FormatTranslation(translatedText, maps, particles);
                    break;
                }

                case TokenType.Latin:
                default: {
                    translation = FormatTranslation(Text, maps, particles);
                    break;
                }
            }

            return translation;
        }

        private string FormatTranslation(string translatedText,
                                         List<string> maps = null,
                                         List<string> particles = null) {
            // Add prefixes, trim whitespace, and capitalise words, etc.
            string outText = "";
            switch (Type) {
                case TokenType.HiraganaKanji:
                case TokenType.Katakana:
                    // Maps
                    translatedText = TextTranslator.MapPhrases(translatedText, maps);

                    // Capitalise
                    translatedText = new CultureInfo("en").TextInfo.ToTitleCase(translatedText);

                    // Decapitalise particles
                    translatedText = TextTranslator.LowercaseParticles(translatedText, particles);
                    
                    // Trim and join
                    outText = Prefix + translatedText.Trim();
                    break;

                case TokenType.Latin:
                default:
                    // Replace japanese punctuation
                    foreach (string s in PunctuationMap.Keys) {
                        string sVal;
                        if (PunctuationMap.TryGetValue(s, out sVal)) {
                            translatedText = translatedText.Replace(s, sVal);
                        }
                    }

                    // Join
                    outText = Prefix + translatedText;
                    break;
            }

            return outText;
        }
    }

    public enum TokenType {
        Latin,
        HiraganaKanji,
        Katakana
    }

}