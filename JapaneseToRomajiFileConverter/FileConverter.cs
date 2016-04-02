using System;
using System.Collections.Generic;
using System.IO;

namespace JapaneseToRomajiFileConverter {
    public class FileConverter {

        public event EventHandler<ProgressEventArgs> Progress;

        private List<string> Files;

        public FileConverter(List<string> files) {
            Files = files;
        }

        public void Convert() {
            // Convert each file
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
                string title = tagFile.Tag.Title ?? "";
                string album = tagFile.Tag.Album ?? "";
                string[] performers = tagFile.Tag.Performers ?? new string[] {};
                string[] albumArtists = tagFile.Tag.AlbumArtists ?? new string[] {};

                // Translate
                string newFileName = Translator.Translate(fileName);
                title = Translator.Translate(title);
                album = Translator.Translate(album);

                for (int i = 0; i < performers.Length; i++) {
                    performers[i] = Translator.Translate(performers[i]);
                }
                for (int i = 0; i < albumArtists.Length; i++) {
                    albumArtists[i] = Translator.Translate(albumArtists[i]);
                }

                // Set new tags
                tagFile.Tag.Title = title;
                tagFile.Tag.Album = album;
                tagFile.Tag.Performers = performers;
                tagFile.Tag.AlbumArtists = albumArtists;
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
