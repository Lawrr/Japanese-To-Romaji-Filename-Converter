using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JapaneseToRomajiFilenameConverter.Converter {
    public class TextTranslator {

        public const string LanguagePair = "ja|en";

        private const string TranslatorUrl = "https://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}";

        private static char MapSplitChar = ':';

        // TODO store in better data structure?
        // Unicode ranges for each set
        public const int RomajiMin = 0x0020;
        public const int RomajiMax = 0x007E;
        public const int HiraganaMin = 0x3040;
        public const int HiraganaMax = 0x309F;
        public const int KatakanaMin = 0x30A0;
        public const int KatakanaMax = 0x30FF;
        // ー character present in both hiragana and katakana
        public const int HirakataProlongedChar = 0x30FC;
        public const int KanjiMin = 0x4E00;
        public const int KanjiMax = 0x9FBF;
        // Covers Basic Latin, Latin-1 Supplement, Extended A, Extended B
        public const int LatinMin = 0x0000;
        public const int LatinMax = 0x024F;

        public static string GetTranslatorUrl(string text, string languagePair = LanguagePair) {
            return string.Format(TranslatorUrl, text, languagePair);
        }

        public static string Translate(string inText, string languagePair = LanguagePair) {
            // Check if already translated / romanized
            // TODO check japanese punctuation too
            // if (IsTranslated(inText)) return inText;

            // Split the text into separate sequential tokens and translate each token
            List<TextToken> textTokens = TextToken.GetTextTokens(inText);

            // Load maps and particles lists once
            string hirakanjiMapPath = Path.Combine(Maps.DirectoryPath, Maps.HirakanjiLatn);
            List<string> hirakanjiMaps = new List<string>(File.ReadAllLines(hirakanjiMapPath));

            string hirakanjiParticlesPath = Path.Combine(Particles.DirectoryPath, Particles.HirakanjiLatn);
            List<string> hirakanjiParticles = new List<string>(File.ReadAllLines(hirakanjiParticlesPath));

            string kataMapPath = Path.Combine(Maps.DirectoryPath, Maps.KataEn);
            List<string> kataMaps = new List<string>(File.ReadAllLines(kataMapPath));

            string kataParticlesPath = Path.Combine(Particles.DirectoryPath, Particles.KataEn);
            List<string> kataParticles = new List<string>(File.ReadAllLines(kataParticlesPath));

            // Translate each token
            string outText = "";
            foreach (TextToken textToken in textTokens) {
                switch (textToken.Type) {
                    case TokenType.HiraganaKanji:
                        outText += textToken.Translate(hirakanjiMaps, hirakanjiParticles);
                        break;

                    case TokenType.Katakana:
                        outText += textToken.Translate(kataMaps, kataParticles);
                        break;

                    case TokenType.Latin:
                    default:
                        outText += textToken.Translate();
                        break;
                }
            }

            // Normalize
            outText = outText.Normalize(NormalizationForm.FormKC);

            return outText;
        }

        public static string MapPhrases(string text, List<string> maps) {
            if (maps != null) {
                foreach (string map in maps) {
                    string[] mapStrings = map.Split(MapSplitChar);

                    // Make sure mapping is valid
                    if (map.IndexOf(MapSplitChar) == 0 || (mapStrings.Length != 1 && mapStrings.Length != 2)) continue;

                    text = Regex.Replace(text,
                        mapStrings[0],
                        mapStrings[1],
                        RegexOptions.IgnoreCase);
                }
            }

            return text;
        }

        public static string LowercaseParticles(string text, List<string> particles) {
            if (particles != null) {
                foreach (string particle in particles) {
                    text = Regex.Replace(text,
                        @"\b" + particle + @"\b",
                        particle,
                        RegexOptions.IgnoreCase);
                }
            }

            return text;
        }

        public static bool IsTranslated(string text) {
            return !text.Any(c => IsJapanese(c.ToString()));
        }

        public static bool IsLatin(string text) {
            return text.Count(c => c >= LatinMin && c <= LatinMax) == text.Length;
        }

        public static bool IsJapanese(string text) {
            return text.Count(c => IsHiragana(c.ToString()) ||
                                   IsKatakana(c.ToString()) ||
                                   IsKanji(c.ToString())) == text.Length;
        }

        public static bool IsHiragana(string text) {
            return text.Count(c => (c >= HiraganaMin && c <= HiraganaMax) || c == HirakataProlongedChar) == text.Length;
        }

        public static bool IsKatakana(string text) {
            return text.Count(c => (c >= KatakanaMin && c <= KatakanaMax) || c == HirakataProlongedChar) == text.Length;
        }

        public static bool IsKanji(string text) {
            return text.Count(c => c >= KanjiMin && c <= KanjiMax) == text.Length;
        }

        public static bool IsProlongedChar(char c) {
            return c == HirakataProlongedChar;
        }

    }
}
