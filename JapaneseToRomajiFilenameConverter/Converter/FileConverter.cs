using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace JapaneseToRomajiFilenameConverter.Converter {
    public class FileConverter {

        public event EventHandler<ProgressEventArgs> Progress;

        // Maps illegal filename characters to legal ones
        public static Dictionary<string, string> IllegalFilenameMap { get; } = new Dictionary<string, string>() {
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

        public FileConverter() {
        }

        public void Convert(IEnumerable<string> files) {
            Convert(files, CancellationToken.None);
        }

        public void Convert(IEnumerable<string> files, CancellationToken ct) {
            // Convert each file
            foreach (string filePath in files) {
                // Check if function has been cancelled if called asynchronously
                if (ct != CancellationToken.None) {
                    ct.ThrowIfCancellationRequested();
                }

                if (!File.Exists(filePath)) {
                    ConversionData nonExistentData = new ConversionData(filePath);
                    ConversionItem item = new ConversionItem(nonExistentData, null);
                    OnProgressEvent(ProgressEvent.FileDoesNotExist, item);
                    continue;
                }

                // Get file details
                string directoryPath = Path.GetDirectoryName(filePath);
                string extension = Path.GetExtension(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                // Get tags
                TagLib.File tagFile = null;
                try {
                    tagFile = TagLib.File.Create(filePath);
                } catch (Exception) {
                    // ignored
                }
                string title = tagFile?.Tag.Title ?? "";
                string album = tagFile?.Tag.Album ?? "";
                string[] performers = tagFile?.Tag.Performers ?? new string[] {};
                string[] albumArtists = tagFile?.Tag.AlbumArtists ?? new string[] {};

                // Store old conversion data
                ConversionData oldData = new ConversionData(filePath,
                                                            title,
                                                            album,
                                                            // Ensure values remain the same even if array is modified
                                                            (string[])performers.Clone(),
                                                            (string[])albumArtists.Clone());

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

                // Replace illegal filename characters from the new filename
                foreach (string s in IllegalFilenameMap.Keys) {
                    string sVal;
                    if (IllegalFilenameMap.TryGetValue(s, out sVal)) {
                        newFileName = newFileName.Replace(s, sVal);
                    }
                }

                string newFilePath = directoryPath + Path.DirectorySeparatorChar + newFileName + extension;
                if (File.Exists(newFilePath)) {
                    ConversionData existingData = new ConversionData(newFilePath);
                    ConversionItem item = new ConversionItem(oldData, existingData);
                    OnProgressEvent(ProgressEvent.FileAlreadyExists, item);
                    continue;
                }

                // Set new tags
                if (tagFile != null) {
                    tagFile.Tag.Title = title;
                    tagFile.Tag.Album = album;
                    tagFile.Tag.Performers = performers;
                    tagFile.Tag.AlbumArtists = albumArtists;
                    tagFile.Save();
                }

                File.Move(filePath, newFilePath);

                // Store new conversion data
                ConversionData newData = new ConversionData(newFilePath,
                                                            title,
                                                            album,
                                                            // Ensure values remain the same even if array is modified
                                                            (string[])performers.Clone(),
                                                            (string[])albumArtists.Clone());

                // Check if function has been cancelled if called asynchronously
                if (ct != CancellationToken.None) {
                    ct.ThrowIfCancellationRequested();
                }

                // Update progress
                ConversionItem conversionItem = new ConversionItem(oldData, newData);
                OnProgressEvent(ProgressEvent.Converted, conversionItem);
            }
            OnProgressEvent(ProgressEvent.Completed);
        }

        public void Revert(IEnumerable<ConversionItem> fileItems) {
            Revert(fileItems, CancellationToken.None);
        }

        public void Revert(IEnumerable<ConversionItem> fileItems, CancellationToken ct) {
            foreach (ConversionItem item in fileItems) {
                // Check if function has been cancelled if called asynchronously
                if (ct != CancellationToken.None) {
                    ct.ThrowIfCancellationRequested();
                }

                if (!File.Exists(item.NewData.FilePath)) {
                    OnProgressEvent(ProgressEvent.FileDoesNotExist, item);
                    continue;
                }
                if (File.Exists(item.OldData.FilePath)) {
                    ConversionItem reversedItem = new ConversionItem(item.NewData, item.OldData);
                    OnProgressEvent(ProgressEvent.FileAlreadyExists, reversedItem);
                    continue;
                }

                // Revert tags
                TagLib.File tagFile = null;
                try {
                    tagFile = TagLib.File.Create(item.NewData.FilePath);
                } catch (Exception) {
                    // ignored
                }

                if (tagFile != null) {
                    tagFile.Tag.Title = item.OldData.Title;
                    tagFile.Tag.Album = item.OldData.Album;
                    tagFile.Tag.Performers = item.OldData.Performers;
                    tagFile.Tag.AlbumArtists = item.OldData.AlbumArtists;
                    tagFile.Save();
                }

                File.Move(item.NewData.FilePath, item.OldData.FilePath);

                OnProgressEvent(ProgressEvent.Reverted, item);
            }
            OnProgressEvent(ProgressEvent.Completed);
        }

        private void OnProgressEvent(ProgressEvent type, ConversionItem item = null) {
            Progress?.Invoke(this, new ProgressEventArgs(type, item));
        }
    }

    public enum ProgressEvent {
        Converted,
        Reverted,
        FileDoesNotExist,
        FileAlreadyExists,
        Completed
    }

    public class ProgressEventArgs : EventArgs {

        public ProgressEvent Type;
        public ConversionItem Item;
        
        public ProgressEventArgs(ProgressEvent type, ConversionItem item) {
            Type = type;
            Item = item;
        }

    }

}
