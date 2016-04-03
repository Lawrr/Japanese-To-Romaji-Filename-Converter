namespace JapaneseToRomajiFileConverter.Converter {
    public class TextToken {

        public TokenType Type { get; private set; }
        public string Text { get; set; }
        public string Prefix { get; set; }

        public TextToken(TokenType type, string text = "", string prefix = "") {
            Type = type;
            Text = text;
            Prefix = prefix;
        }

    }

    public enum TokenType {
        Romanized,
        HiraganaKanji,
        Katakana
    }

}