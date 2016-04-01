using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Japanese_To_Romaji_File_Converter {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();

            FilesBox.AllowDrop = true;
            FilesBox.DragEnter += new DragEventHandler(FilesBox_DragEnter);
            FilesBox.DragDrop += new DragEventHandler(FilesBox_DragDrop);
        }

        private void FilesBox_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void FilesBox_DragDrop(object sender, DragEventArgs e) {
            string[] items = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string item in items) {
                if (Directory.Exists(item)) {
                    string[] directoryFiles = Directory.GetFiles(item, "*", SearchOption.AllDirectories);
                    foreach (string file in directoryFiles) {
                        FilesBox.Items.Add(file);
                    }
                } else {
                    FilesBox.Items.Add(item);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e) {
            CenterToScreen();
            BringToFront();
            Activate();
        }

    }
}
