using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JapaneseToRomajiFileConverter.Converter {
    public class TextTranslator {

        public const string LanguagePair = "ja|en";

        private const string TranslatorUrl = "https://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}";

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

        public static string GetTranslatorUrl(string text, string languagePair = LanguagePair) {
            return string.Format(TranslatorUrl, text, languagePair);
        }

        public static string Translate(string inText, string languagePair = LanguagePair) {
            // Normalize
            inText = inText.Normalize(NormalizationForm.FormKC);

            // Check if already translated / romanized
            if (IsTranslated(inText)) return inText;

            // Split the text into separate sequential tokens and translate each token
            // 1. Romanized - Don't translate
            // 2. Katakana - Translate to output language
            // 3. Hiragana / Kanji - Translate to phonetic
            List<TextToken> textTokens = TextToken.GetTextTokens(inText);

            // Load particle lists
            string jaParticlesPath = Path.Combine(Particles.DirectoryPath, Particles.Ja);
            List<string> jaParticles = new List<string>(File.ReadAllLines(jaParticlesPath));

            string outText = "";
            foreach (TextToken textToken in textTokens) {
                outText += textToken.Translate(jaParticles);
            }

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

    }
}
