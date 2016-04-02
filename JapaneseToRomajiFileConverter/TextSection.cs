namespace JapaneseToRomajiFileConverter {
    public class TextSection {

        public SectionType Type { get; private set; }
        public string Text { get; set; }

        public TextSection(SectionType type, string text) {
            Type = type;
            Text = text;
        }

    }

    public enum SectionType {
        Romanized,
        HiraganaKanji,
        Katakana
    }

}