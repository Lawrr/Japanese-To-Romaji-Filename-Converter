using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Japanese_To_Romaji_File_Converter {
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
            ConvertForm convertForm = new ConvertForm(this);
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

        private void RemoveFiles(int[] indices) {
            for (int i = indices.Count() - 1; i >= 0; i--) {
                Files.RemoveAt(indices[i]);
                FilesBox.Items.RemoveAt(indices[i]);
            }

            // Enable drag drop tip label
            if (FilesBox.Items.Count == 0) {
                DragDropLabel.Visible = true;
            }
        }

        private void AddFiles(string[] items) {
            foreach (string item in items) {
                if (Directory.Exists(item)) {
                    // if the item is a directory get the files within the directory 
                    string[] directoryFiles = Directory.GetFiles(item, "*", SearchOption.AllDirectories);
                    foreach (string file in directoryFiles) {
                        Files.Add(file);
                        FilesBox.Items.Add(file.Split('\\').Last());
                    }
                } else {
                    // else add file
                    Files.Add(item);
                    FilesBox.Items.Add(item.Split('\\').Last());
                }
            }

            // Disable drag drop tip label
            DragDropLabel.Visible = false;
        }

        public List<string> GetFiles() {
            return Files;
        }

    }
}
