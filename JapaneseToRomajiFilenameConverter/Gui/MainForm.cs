using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JapaneseToRomajiFilenameConverter.Converter;
using JapaneseToRomajiFilenameConverter.Gui;
using TagLib;
using File = System.IO.File;

namespace JapaneseToRomajiFilenameConverter {
    public partial class MainForm : Form {

        public event EventHandler<ProgressEventArgs> Progress;

        private HistoryManager HistoryManager;
        private Task FileConversionTask;
        private CancellationTokenSource FileConversionTaskCts;

        public MainForm() {
            InitializeComponent();

            AllowDrop = true;
            DragEnter += new DragEventHandler(MainForm_DragEnter);
            DragDrop += new DragEventHandler(MainForm_DragDrop);

            HistoryManager = new HistoryManager();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            CenterToScreen();
            BringToFront();
            Activate();

            totalFilesLabel.Text = "Total Files: " + FilesBox.Items.Count;
            selectedFilesLabel.Text = "Selected Files: " + FilesBox.SelectedIndices.Count;
        }

        private void ConvertBTN_Click(object sender, EventArgs e) {
            ConverterForm convertForm = new ConverterForm();

            List<string> files = new List<string>();
            foreach (FileBoxItem item in FilesBox.Items) {
                files.Add(item.ToString());
            }

            convertForm.ConvertFiles(files);
            convertForm.ShowDialog();
        }

        private void HistoryBTN_Click(object sender, EventArgs e) {
            ReverterForm reverterForm = new ReverterForm();
            reverterForm.ShowDialog();
        }

        private void MainForm_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e) {
            // Get dropped items
            string[] items = (string[])e.Data.GetData(DataFormats.FileDrop);
            AddFiles(items);
        }

        private void AddBTN_Click(object sender, EventArgs e) {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;

            DialogResult result = fileDialog.ShowDialog();

            if (result == DialogResult.OK) {
                string[] items = fileDialog.FileNames;
                AddFiles(items);
            }
        }

        private void RemoveBTN_Click(object sender, EventArgs e) {
            int[] indices = new int[FilesBox.SelectedIndices.Count];
            for (int i = FilesBox.SelectedIndices.Count - 1; i >= 0; i--) {
                indices[i] = FilesBox.SelectedIndices[i];
            }
            RemoveFiles(indices);
        }

        private void ClearBTN_Click(object sender, EventArgs e) {
            ClearFiles();
        }

        private void FilesBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (FilesBox.SelectedIndices.Count > 0) {
                RemoveBTN.Enabled = true;
            }
            selectedFilesLabel.Text = "Selected Files: " + FilesBox.SelectedIndices.Count;
        }

        private void OnHasNoFiles() {
            // Toggle when no files
            ConvertBTN.Enabled = false;
            RemoveBTN.Enabled = false;
            ClearBTN.Enabled = false;
            DragDropLabel.Visible = true;
        }

        private void OnHasFiles() {
            // Toggle when files are added
            ConvertBTN.Enabled = true;
            ClearBTN.Enabled = true;
            DragDropLabel.Visible = false;
        }

        private void ClearFiles() {
            FilesBox.Items.Clear();
            OnHasNoFiles();

            totalFilesLabel.Text = "Total Files: " + FilesBox.Items.Count;
            selectedFilesLabel.Text = "Selected Files: " + FilesBox.SelectedIndices.Count;
        }

        private void RemoveFiles(IReadOnlyList<int> indices) {
            for (int i = indices.Count() - 1; i >= 0; i--) {
                FilesBox.Items.RemoveAt(indices[i]);
            }

            if (FilesBox.Items.Count == 0) {
                OnHasNoFiles();
            }
            // Disable because index is not selected anymore
            RemoveBTN.Enabled = false;

            totalFilesLabel.Text = "Total Files: " + FilesBox.Items.Count;
        }

        private void AddFiles(IEnumerable<string> items) {
            foreach (string item in items) {
                if (Directory.Exists(item)) {
                    // if the item is a directory get the files within the directory 
                    string[] directoryFiles = Directory.GetFiles(item, "*", SearchOption.AllDirectories);
                    foreach (string file in directoryFiles) {
                        AddFile(file);
                    }
                } else {
                    AddFile(item);
                }
            }

            OnHasFiles();

            totalFilesLabel.Text = "Total Files: " + FilesBox.Items.Count;
        }

        private void AddFile(string filePath) {
            if (!File.Exists(filePath) || FilesBox.Items.IndexOf(filePath) != -1) return;

            // Extract image from file (if it has one)
            Image fileImage = null;

            // Extract from audio file
            try {
                TagLib.File tagFile = TagLib.File.Create(filePath);
                if (tagFile.Tag.Pictures.Length > 0) {
                    IPicture pic = tagFile.Tag.Pictures[0];
                    MemoryStream ms = new MemoryStream(pic.Data.Data);
                    fileImage = Image.FromStream(ms);
                }
            } catch (Exception) {
                // ignored
            }

            // Extract from image file
            if (fileImage == null) {
                try {
                    fileImage = Image.FromFile(filePath);
                } catch (Exception) {
                    // ignored
                }
            }

            FileBoxItem item = new FileBoxItem(FilesBox, filePath,
                fileImage?.GetThumbnailImage(FilesBox.ImageSize.Width, FilesBox.ImageSize.Height, null, IntPtr.Zero));
            FilesBox.Items.Add(item);
        }

    }
}
