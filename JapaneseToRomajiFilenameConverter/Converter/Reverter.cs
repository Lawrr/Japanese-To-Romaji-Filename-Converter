using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace JapaneseToRomajiFileConverter.Converter {
    public class Reverter {

        public const string DefaultDataFilePath = "conversion-history.xml";

        private string DataFilePath;

        public Reverter(string dataFilePath = DefaultDataFilePath) {
            DataFilePath = dataFilePath;
        }

        public void SaveConversion(ConversionItem item) {
            using (Stream stream = File.Open(DataFilePath, FileMode.Append)) {
                using (XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8)) {
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 4;

                    // Start item
                    writer.WriteStartElement("ConversionItem");

                    // New file path
                    writer.WriteElementString("Path", item.NewData.FilePath);

                    // Old data
                    writer.WriteStartElement("Data");
                    WriteConversionData(writer, item.OldData);
                    writer.WriteEndElement();

                    // End item
                    writer.WriteEndElement();

                    writer.WriteWhitespace(Environment.NewLine);
                }
            }
        }

        private void WriteConversionData(XmlTextWriter writer, ConversionData data) {
            writer.WriteElementString("Path", data.FilePath);
            writer.WriteElementString("Title", data.Title);
            writer.WriteElementString("Album", data.Album);
            writer.WriteStartElement("Artists");
            foreach (string s in data.Performers) {
                writer.WriteElementString("Artist", s);
            }
            writer.WriteEndElement();
            writer.WriteStartElement("AlbumAritsts");
            foreach (string s in data.AlbumArtists) {
                writer.WriteElementString("Artist", s);
            }
            writer.WriteEndElement();
        }

        public List<ConversionItem> GetConversions() {
            List<ConversionItem> items = new List<ConversionItem>();

            if (!File.Exists(DataFilePath)) return items;

            XmlReaderSettings readerSettings = new XmlReaderSettings {
                ConformanceLevel = ConformanceLevel.Auto,
                IgnoreWhitespace = true
            };

            using (StreamReader stream = new StreamReader(DataFilePath, Encoding.UTF8)) {
                using (XmlReader reader = XmlReader.Create(stream, readerSettings)) {
                    reader.MoveToContent();
                    while (!reader.EOF) {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "ConversionItem") {
                            XElement element = (XElement) XNode.ReadFrom(reader);
                            ConversionItem item = ParseConversionItem(element);
                            items.Add(item);
                        } else {
                            reader.Read();
                        }
                    }
                }
            }

            return items;
        }

        private ConversionItem ParseConversionItem(XElement element) {
            ConversionData oldData = new ConversionData();
            ConversionData newData = new ConversionData();

            foreach (XElement child in element.Elements()) {
                switch (child.Name.LocalName) {
                    case "Path":
                        newData.FilePath = child.Value;
                        break;

                    case "Data":
                        foreach (XElement dataChild in child.Elements()) {
                            switch (dataChild.Name.LocalName) {
                                case "Path":
                                    oldData.FilePath = dataChild.Value;
                                    break;

                                case "Title":
                                    oldData.Title = dataChild.Value;
                                    break;

                                case "Album":
                                    oldData.Album = dataChild.Value;
                                    break;

                                case "Artists": {
                                    string[] artists = new string[dataChild.Elements().Count()];
                                    int index = 0;
                                    foreach (XElement artistChild in dataChild.Elements()) {
                                        artists[index++] = artistChild.Value;
                                    }
                                    oldData.Performers = artists;
                                    break;
                                }

                                case "AlbumArtists": {
                                    string[] artists = new string[dataChild.Elements().Count()];
                                    int index = 0;
                                    foreach (XElement artistChild in dataChild.Elements()) {
                                        artists[index++] = artistChild.Value;
                                    }
                                    oldData.AlbumArtists = artists;
                                    break;
                                }
                            }
                        }
                        break;
                }
            }

            return new ConversionItem(oldData, newData);
        }
    }
}
