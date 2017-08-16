using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JapaneseToRomajiFilenameConverter.Gui;
using File = System.IO.File;

namespace JapaneseToRomajiFilenameConverter {
    public partial class MainForm : Form {

        private CancellationTokenSource AddFilesCts;

        public MainForm() {
            InitializeComponent();

            AllowDrop = true;
            DragEnter += new DragEventHandler(MainForm_DragEnter);
            DragDrop += new DragEventHandler(MainForm_DragDrop);
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
            AddFilesCts.Cancel();
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

        private async void AddFiles(IEnumerable<string> items) {
            AddFilesCts = new CancellationTokenSource();
            OnHasFiles();
            AddFilesAsync(items, AddFilesCts.Token);

            if (AddFilesCts.IsCancellationRequested) {
                ClearFiles();
            }
        }

        private async void AddFilesAsync(IEnumerable<string> items, CancellationToken ct) {
            await Task.Run(() => {
                try {
                    List<FileBoxItem> itemList = new List<FileBoxItem>();
                    foreach (string item in items) {
                        ct.ThrowIfCancellationRequested();

                        if (Directory.Exists(item)) {
                            // if the item is a directory get the files within the directory 
                            string[] directoryFiles = Directory.GetFiles(item, "*", SearchOption.AllDirectories);
                            foreach (string file in directoryFiles) {
                                ct.ThrowIfCancellationRequested();

                                if (!File.Exists(file) || FilesBox.Items.IndexOf(file) != -1) continue;
                                FileBoxItem fbi = new FileBoxItem(FilesBox, file);
                                itemList.Add(fbi);
                            }
                        } else {
                            if (!File.Exists(item) || FilesBox.Items.IndexOf(item) != -1) continue;
                            FileBoxItem fbi = new FileBoxItem(FilesBox, item);
                            itemList.Add(fbi);
                        }
                    }

                    ct.ThrowIfCancellationRequested();
                    this.InvokeSafe(() => {
                        FilesBox.Items.AddRange(itemList.Cast<object>().ToArray());
                        totalFilesLabel.Text = "Total Files: " + FilesBox.Items.Count;
                    });
                } catch (OperationCanceledException) {
                    // ignored
                }
            }, ct);
        }
    }
}
