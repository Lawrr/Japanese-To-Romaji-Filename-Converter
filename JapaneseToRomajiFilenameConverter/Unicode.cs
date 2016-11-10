using System.Linq;

namespace JapaneseToRomajiFilenameConverter {
    public static class Unicode {

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

        public static bool IsLatin(string text) {
            return text.Count(c => c >= LatinMin && c <= LatinMax) == text.Length;
        }

        public static bool IsJapanese(string text) {
            return text.Count(c => IsHiragana(c.ToString()) ||
                                   IsKatakana(c.ToString()) ||
                                   IsKanji(c.ToString())) == text.Length;
        }

        public static bool IsHiragana(string text) {
            return text.Count(c => (c >= HiraganaMin && c <= HiraganaMax) ||
                                   c == HirakataProlongedChar) == text.Length;
        }

        public static bool IsKatakana(string text) {
            return text.Count(c => (c >= KatakanaMin && c <= KatakanaMax) ||
                                   c == HirakataProlongedChar) == text.Length;
        }

        public static bool IsKanji(string text) {
            return text.Count(c => c >= KanjiMin && c <= KanjiMax) == text.Length;
        }

        public static bool IsProlongedChar(char c) {
            return c == HirakataProlongedChar;
        }
    }
}
