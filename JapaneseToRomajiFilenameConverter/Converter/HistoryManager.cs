using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace JapaneseToRomajiFilenameConverter.Converter {
    public class HistoryManager {

        public const string DefaultFilePath = "conversion-history.xml";

        private string FilePath;

        public HistoryManager(string filePath = DefaultFilePath) {
            FilePath = filePath;
        }

        public void SaveConversion(ConversionItem item) {
            // Get xml file or create root element if the file does not exist/cannot load
            XDocument doc;
            try {
                doc = XDocument.Load(FilePath);
            } catch (Exception) {
                doc = new XDocument();
                doc.Add(new XElement("Conversions"));
                doc.Save(FilePath);
            }

            // Get root element and add new conversion
            XElement root = doc.Root;
            XElement itemElement = new XElement("Item");
            using (XmlWriter writer = itemElement.CreateWriter()) {
                // New file path
                writer.WriteElementString("Path", item.NewData.FilePath);

                // Old data
                writer.WriteStartElement("Data");
                WriteConversionData(writer, item.OldData);
                writer.WriteEndElement();
            }

            // Add and save new item
            root.Add(itemElement);
            root.Save(FilePath);
        }

        private void WriteConversionData(XmlWriter writer, ConversionData data) {
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

        public void RemoveConversion(ConversionItem item) {
            XDocument doc = XDocument.Load(FilePath);
            IEnumerable<XElement> query = from node in doc.Descendants("Item")
                                          let path = node.Descendants("Path").First()
                                          where path != null && path.Value.Equals(item.NewData.FilePath)
                                          select node;
            query.ToList().ForEach(n => n.Remove());
            doc.Save(FilePath);
        }

        public static List<ConversionItem> GetConversions(string dataFilePath = DefaultFilePath) {
            List<ConversionItem> items = new List<ConversionItem>();

            if (!File.Exists(dataFilePath)) return items;

            XmlReaderSettings readerSettings = new XmlReaderSettings {
                ConformanceLevel = ConformanceLevel.Auto,
                IgnoreWhitespace = true
            };

            using (StreamReader stream = new StreamReader(dataFilePath, Encoding.UTF8)) {
                using (XmlReader reader = XmlReader.Create(stream, readerSettings)) {
                    reader.MoveToContent();
                    while (!reader.EOF) {
                        if (reader.NodeType == XmlNodeType.Element && reader.Name == "Item") {
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

        private static ConversionItem ParseConversionItem(XElement element) {
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
