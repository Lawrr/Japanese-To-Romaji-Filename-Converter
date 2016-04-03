using System;
using HtmlAgilityPack;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Globalization;

namespace JapaneseToRomajiFileConverter.Converter {
    public class TextToken {

        public TokenType Type { get; private set; }
        public string Text { get; set; }
        public string Prefix { get; set; }
        public string Translation { get; set; }

        public TextToken(TokenType type, string text = "", string prefix = "") {
            Type = type;
            Text = text;
            Prefix = prefix;
        }

        public void Translate(List<string> particles, string languagePair = TextTranslator.LanguagePair) {
            switch (Type) {
                case TokenType.HiraganaKanji: {
                    // Get phoentic text
                    string url = TextTranslator.GetTranslatorUrl(Text, languagePair);
                    HtmlDocument doc = new HtmlWeb().Load(url);
                    string phoneticText = doc.GetElementbyId("src-translit").InnerText;
                    Translation = FormatTranslation(phoneticText, particles);
                    break;
                }

                case TokenType.Katakana: {
                    // Get translated text
                    string url = TextTranslator.GetTranslatorUrl(Text, languagePair);
                    HtmlDocument doc = new HtmlWeb().Load(url);
                    string translatedText = doc.GetElementbyId("result_box").InnerText;
                    Translation = FormatTranslation(translatedText, particles);
                    break;
                }

                case TokenType.Romanized:
                default: {
                    Translation = Prefix + Text;
                    break;
                }
            }
        }

        private string FormatTranslation(string translatedText, List<string> particles) {
            // Add prefixes, trim whitespace, and capitalise words
            string outText = "";
            switch (Type) {
                case TokenType.HiraganaKanji:
                    translatedText = new CultureInfo("en").TextInfo.ToTitleCase(translatedText);
                    foreach (string particle in particles) {
                        translatedText = Regex.Replace(translatedText,
                                                       @"\b" + particle + @"\b",
                                                       particle,
                                                       RegexOptions.IgnoreCase);
                    }
                    outText = Prefix + translatedText.Trim();
                    break;

                case TokenType.Katakana:
                    translatedText = new CultureInfo("en").TextInfo.ToTitleCase(translatedText);
                    outText = Prefix + translatedText.Trim();
                    break;

                case TokenType.Romanized:
                default:
                    outText = Prefix + translatedText;
                    break;
            }

            return outText;
        }
    }

    public enum TokenType {
        Romanized,
        HiraganaKanji,
        Katakana
    }

}