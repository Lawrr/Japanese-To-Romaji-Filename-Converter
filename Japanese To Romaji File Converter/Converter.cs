using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace JapaneseToRomajiFileConverter {
    public class Converter {

        public static string[] StartSrcSplit = new string[] { "<div id=src-translit class=translit dir=ltr style=\"text-align:;display:block\">" };
        public static string[] EndSrcSplit = new string[] { "</div>" };
        public static string LanguagePair = "ja|en";
        public static char MapChar = '`';

        // TODO store in better data structure?
        // Unicode ranges for each set
        public static int RomajiMin = 0x0020;
        public static int RomajiMax = 0x007E;
        public static int HiraganaMin = 0x3040;
        public static int HiraganaMax = 0x309F;
        public static int KatakanaMin = 0x30A0;
        public static int KatakanaMax = 0x30FF;
        public static int KanjiMin = 0x4E00;
        public static int KanjiMax = 0x9FBF;
        // Covers Basic Latin, Latin-1 Supplement, Extended A, Extended B
        public static int LatinMin = 0x0000;
        public static int LatinMax = 0x024F;


        public event EventHandler<ProgressEventArgs> Progress;

        private List<string> Files;

        public Converter(List<string> files) {
            Files = files;
        }

        public void Convert() {
            char sep = ':';

            foreach (string filePath in Files) {
                if (!File.Exists(filePath)) {
                    // TODO Error
                    continue;
                }

                // Get file details
                string directoryPath = Path.GetDirectoryName(filePath);
                string extension = Path.GetExtension(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                // Get tags
                TagLib.File tagFile = TagLib.File.Create(filePath);
                string title = tagFile.Tag.Title;
                string performers = String.Join(sep.ToString(), tagFile.Tag.Performers);
                string albumArtists = String.Join(sep.ToString(), tagFile.Tag.AlbumArtists);
                string album = tagFile.Tag.Album;

                // Translate
                string newFileName = Translate(fileName).Trim();
                title = Translate(title).Trim();
                performers = Translate(performers).Trim();
                albumArtists = Translate(albumArtists).Trim();
                album = Translate(album).Trim();

                // Set new tags
                tagFile.Tag.Title = title;
                tagFile.Tag.Performers = performers.Split(sep).Select(item => item.Trim()).ToArray();
                tagFile.Tag.AlbumArtists = albumArtists.Split(sep).Select(item => item.Trim()).ToArray();
                tagFile.Tag.Album = album;
                tagFile.Save();

                // Move file to new path
                string newFilePath = directoryPath + Path.DirectorySeparatorChar + newFileName + extension;
                // TODO exception
                File.Move(filePath, newFilePath);

                // Update progress
                OnProgressEvent(ProgressEvent.Converted, fileName + extension, newFileName + extension);
            }
            OnProgressEvent(ProgressEvent.Completed);
        }

        private static bool IsRomanized(string text) {
            return text.Where(c => c >= LatinMin && c <= LatinMax).Count() == text.Length;
        }

        private static string Translate(string inText) {
            // Check null
            if (inText == null) return "";

            // Check if already romanized
            if (IsRomanized(inText)) return inText;

            // Map english characters to substitutes
            Tuple<string, List<char>> charMap = MapChars(inText);
            inText = charMap.Item1;

            // Get translation
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            string url = String.Format("https://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}",
                                        inText, LanguagePair);
            string src = webClient.DownloadString(url);

            // Get translation from source code between two strings
            string outText = src.Split(StartSrcSplit, StringSplitOptions.None).Last()
                                       .Split(EndSrcSplit, StringSplitOptions.None).First();

            // Decode html encodings
            outText = WebUtility.HtmlDecode(outText);

            // Unmap english characters back from substitutes
            outText = UnmapChars(outText, charMap.Item2);

            return outText;
        }

        private static Tuple<string, List<char>> MapChars(string text) {
            // Replace characters with sub chars for the translation
            List<char> mapChars = new List<char>();
            StringBuilder mapText = new StringBuilder(text);

            // Loop through each character and map english with $MapChar
            for (int i = 0; i < text.Length; i++) {
                char currChar = text[i];
                if (IsRomanized(currChar.ToString())) {
                    mapChars.Add(currChar);
                    mapText[i] = MapChar;
                } else if (currChar == MapChar) {
                    mapChars.Add(currChar);
                }
            }

            Console.WriteLine("Map");
            Console.WriteLine(text);
            Console.WriteLine(mapText.ToString());

            return Tuple.Create(mapText.ToString(), mapChars);
        }

        private static string UnmapChars(string text, List<char> mapChars) {
            StringBuilder unmapText = new StringBuilder(text);

            // Loop through each character and unmap mapped chars
            int mapIndex = 0;
            for (int i = 0; i < text.Length; i++) {
                if (text[i] == MapChar) {
                    unmapText[i] = mapChars[mapIndex++];
                }
            }

            Console.WriteLine("UnMap");
            Console.WriteLine(text);
            Console.WriteLine(unmapText.ToString());
            return unmapText.ToString();
        }

        private void OnProgressEvent(ProgressEvent type, string oldFileName = null, string newFileName = null) {
            Progress(this, new ProgressEventArgs(type, oldFileName, newFileName));
        }

    }

    public enum ProgressEvent {
        Converted,
        Completed
    }

    public class ProgressEventArgs : EventArgs {

        public ProgressEvent Type;
        public string OldFileName;
        public string NewFileName;
        
        public ProgressEventArgs(ProgressEvent type, string oldFileName = null, string newFileName = null) {
            Type = type;
            OldFileName = oldFileName;
            NewFileName = newFileName;
        }

    }

}
