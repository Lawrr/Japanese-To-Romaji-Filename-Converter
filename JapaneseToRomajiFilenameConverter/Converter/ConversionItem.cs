using System.IO;

namespace JapaneseToRomajiFileConverter.Converter {

    public class ConversionItem {

        public ConversionData OldData { get; }
        public ConversionData NewData { get; }

        public ConversionItem(ConversionData oldData, ConversionData newData) {
            OldData = oldData;
            NewData = newData;
        }

        public override string ToString() {
            return NewData.FilePath + " - " + Path.GetFileName(OldData.FilePath);
        }

    }
}
