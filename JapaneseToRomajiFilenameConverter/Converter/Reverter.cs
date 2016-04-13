using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace JapaneseToRomajiFileConverter.Converter {
    public class Reverter {

        public const string DefaultDataFilePath = "conversion-history.dat";

        private string DataFilePath;

        public Reverter(string dataFilePath = DefaultDataFilePath) {
            DataFilePath = dataFilePath;
        }

        public void SaveConversion(ConversionItem item) {
            using (Stream stream = File.Open(DataFilePath, FileMode.Append)) {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, item);
            }
        }

        public void DumpHistory() {
            if (!File.Exists(DataFilePath)) return;

            List<ConversionItem> items = new List<ConversionItem>();
            using (Stream stream = File.Open(DataFilePath, FileMode.Open)) {
                BinaryFormatter formatter = new BinaryFormatter();
                while (stream.Position != stream.Length) {
                    items.Add((ConversionItem)formatter.Deserialize(stream));
                }
            }
            foreach (ConversionItem i in items) {
                Console.WriteLine(i.ToString());
            }
        }

    }
}
