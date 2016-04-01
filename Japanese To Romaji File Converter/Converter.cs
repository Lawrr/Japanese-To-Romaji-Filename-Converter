using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Japanese_To_Romaji_File_Converter {
    public class Converter {

        public event EventHandler<ProgressEventArgs> Progress;

        private List<string> Files;

        public Converter(List<string> files) {
            Files = files;
        }

        public void Convert() {
            string[] startSplit = new string[] { "<div id=src-translit class=translit dir=ltr style=\"text-align:;display:block\">" };
            string[] endSplit = new string[] { "</div>" };
            string langPair = "ja|en";

            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;

            foreach (string filePath in Files) {
                // Get file details
                string directoryPath = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string extension = Path.GetExtension(filePath);

                // Get translation
                string url = String.Format("https://www.google.com/translate_t?hl=en&ie=UTF8&text={0}&langpair={1}", fileName, langPair);
                string source = webClient.DownloadString(url);
                string translation = source.Split(startSplit, StringSplitOptions.None).Last().Split(endSplit, StringSplitOptions.None).First();

                // Move file to new path
                string newFilePath = directoryPath + Path.DirectorySeparatorChar + translation + extension;
                File.Move(filePath, newFilePath);

                // Update tags
                TagLib.File file = TagLib.File.Create(newFilePath);
                file.Tag.Title = translation;
                file.Save();

                // Update progress
                OnProgressUpdate(ProgressUpdate.Converted, filePath, translation + extension);
            }
            OnProgressUpdate(ProgressUpdate.Completed);
        }

        private void OnProgressUpdate(ProgressUpdate type, string oldFileName = null, string newFileName = null) {
            Progress(this, new ProgressEventArgs(type, oldFileName, newFileName));
        }

    }

    public enum ProgressUpdate {
        Converted,
        Completed
    }

    public class ProgressEventArgs : EventArgs {

        public ProgressUpdate Type;
        public string OldFileName;
        public string NewFileName;
        
        public ProgressEventArgs(ProgressUpdate type, string oldFileName = null, string newFileName = null) {
            Type = type;
            OldFileName = oldFileName;
            NewFileName = newFileName;
        }

    }

}
