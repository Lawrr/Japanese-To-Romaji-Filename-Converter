using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace JapaneseToRomajiFileConverter {
    public partial class MainForm : Form {

        private List<string> Files = new List<string>();

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
        }

        private void ConvertBTN_Click(object sender, EventArgs e) {
            ConverterForm convertForm = new ConverterForm(this);
            convertForm.ShowDialog();
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
            Files.Clear();
            FilesBox.Items.Clear();
            OnHasNoFiles();
        }

        private void RemoveFiles(int[] indices) {
            for (int i = indices.Count() - 1; i >= 0; i--) {
                Files.RemoveAt(indices[i]);
                FilesBox.Items.RemoveAt(indices[i]);
            }

            if (FilesBox.Items.Count == 0) {
                OnHasNoFiles();
            }
            RemoveBTN.Enabled = false;
        }

        private void AddFiles(string[] items) {
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
        }

        private void AddFile(string filePath) {
            if (File.Exists(filePath) && Files.IndexOf(filePath) == -1) {
                Files.Add(filePath);
                FilesBox.Items.Add(filePath.Split('\\').Last());
            }
        }

        public List<string> GetFiles() {
            return Files;
        }

    }
}
