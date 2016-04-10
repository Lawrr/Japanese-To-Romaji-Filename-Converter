using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace JapaneseToRomajiFileConverter.Converter {
    public class FileConverter {

        public event EventHandler<ProgressEventArgs> Progress;

        // Maps illegal filename characters to legal ones
        public static Dictionary<string, string> IllegalFilenameMap { get; private set; } = new Dictionary<string, string>() {
            { "\\", "＼" },
            { "/", "／" },
            { ":", "：" },
            { "*", "＊" },
            { "?", "？" },
            { "\"", "”" },
            { "<", "＜" },
            { ">", "＞" },
            { "|", "｜" }
        };

        private List<string> Files;

        public FileConverter(List<string> files) {
            Files = files;
        }

        public void Convert() {
            Convert(CancellationToken.None);
        }

        public void Convert(CancellationToken ct) {
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

                // Check if function has been cancelled if called asynchronously
                if (ct != CancellationToken.None) {
                    ct.ThrowIfCancellationRequested();
                }

                // Translate
                string newFileName = TextTranslator.Translate(fileName);
                title = TextTranslator.Translate(title);
                album = TextTranslator.Translate(album);

                for (int i = 0; i < performers.Length; i++) {
                    performers[i] = TextTranslator.Translate(performers[i]);
                }
                for (int i = 0; i < albumArtists.Length; i++) {
                    albumArtists[i] = TextTranslator.Translate(albumArtists[i]);
                }

                // Check if function has been cancelled if called asynchronously
                if (ct != CancellationToken.None) {
                    ct.ThrowIfCancellationRequested();
                }

                // Set new tags
                tagFile.Tag.Title = title;
                tagFile.Tag.Album = album;
                tagFile.Tag.Performers = performers;
                tagFile.Tag.AlbumArtists = albumArtists;
                tagFile.Save();

                // Replace illegal filename characters from the new filename
                foreach (string s in IllegalFilenameMap.Keys) {
                    string sVal;
                    if (IllegalFilenameMap.TryGetValue(s, out sVal)) {
                        newFileName = newFileName.Replace(s, sVal);
                    }
                }

                // Move file to new path
                string newFilePath = directoryPath + Path.DirectorySeparatorChar + newFileName + extension;
                // TODO exception
                File.Move(filePath, newFilePath);

                // Check if function has been cancelled if called asynchronously
                if (ct != CancellationToken.None) {
                    ct.ThrowIfCancellationRequested();
                }

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
