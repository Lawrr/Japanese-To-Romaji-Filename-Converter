namespace JapaneseToRomajiFileConverter.Converter {
    public class TextSection {

        public SectionType Type { get; private set; }
        public string Text { get; set; }
        public string Prefix { get; set; }

        public TextSection(SectionType type, string text = "", string prefix = "") {
            Type = type;
            Text = text;
            Prefix = prefix;
        }

    }

    public enum SectionType {
        Romanized,
        HiraganaKanji,
        Katakana
    }

}