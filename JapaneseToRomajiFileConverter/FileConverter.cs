using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JapaneseToRomajiFileConverter {
    public class FileConverter {

        public event EventHandler<ProgressEventArgs> Progress;

        private List<string> Files;

        public FileConverter(List<string> files) {
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
                string newFileName = Translator.Translate(fileName).Trim();
                title = Translator.Translate(title).Trim();
                performers = Translator.Translate(performers).Trim();
                albumArtists = Translator.Translate(albumArtists).Trim();
                album = Translator.Translate(album).Trim();

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
                OnProgressEvent(ProgressEvent.Converted, filePath, newFilePath);
            }
            OnProgressEvent(ProgressEvent.Completed);
        }

        private void OnProgressEvent(ProgressEvent type, string oldFilePath = null, string newFilePath = null) {
            Progress(this, new ProgressEventArgs(type, oldFilePath, newFilePath));
        }

    }

    public enum ProgressEvent {
        Converted,
        Completed
    }

    public class ProgressEventArgs : EventArgs {

        public ProgressEvent Type;
        public string OldFilePath;
        public string NewFilePath;
        
        public ProgressEventArgs(ProgressEvent type, string oldFilePath = null, string newFilePath = null) {
            Type = type;
            OldFilePath = oldFilePath;
            NewFilePath = newFilePath;
        }

    }

}
