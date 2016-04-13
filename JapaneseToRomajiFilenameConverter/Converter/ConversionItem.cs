using System;

namespace JapaneseToRomajiFileConverter.Converter {

    [Serializable]
    public class ConversionItem {

        public ConversionData OldData { get; private set; }
        public ConversionData NewData { get; private set; }

        public ConversionItem(ConversionData oldData, ConversionData newData) {
            OldData = oldData;
            NewData = newData;
        }

        public override string ToString() {
            return OldData.FilePath + " - " + NewData.FilePath;
        }

    }
}
