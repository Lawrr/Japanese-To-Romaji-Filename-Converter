using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JapaneseToRomajiFilenameConverter.Converter;

namespace JapaneseToRomajiFilenameConverter {
    public partial class ReverterForm : Form {

        public ReverterForm() {
            InitializeComponent();

            IEnumerable<ConversionItem> conversionItems = HistoryManager.GetConversions();
            AddConversions(conversionItems);

            if (ConversionsBox.Items.Count > 0) {
                ClearBTN.Enabled = true;
            }
        }

        private void ReverterForm_Load(object sender, EventArgs e) {
            CenterToParent();
            BringToFront();
            Activate();
        }

        private void AddConversions(IEnumerable<ConversionItem> items) {
            foreach (ConversionItem item in items) {
                ConversionsBox.Items.Add(item);
            }
        }

        private void RevertBTN_Click(object sender, System.EventArgs e) {
            var confirmRes = MessageBox.Show("Are you sure you want to revert the selected files?",
                                             "Revert Files", MessageBoxButtons.OKCancel);
            if (confirmRes != DialogResult.OK) return;

            ConverterForm convertForm = new ConverterForm();
            convertForm.Progress += new EventHandler<ProgressEventArgs>(Revert_Progress);
            convertForm.RevertFiles(ConversionsBox.SelectedItems.Cast<ConversionItem>().ToList());
            convertForm.ShowDialog();
        }

        private void ClearBTN_Click(object sender, EventArgs e) {
            var confirmRes = MessageBox.Show("Are you sure you want to clear your history?",
                                             "Clear History", MessageBoxButtons.OKCancel);
            if (confirmRes != DialogResult.OK) return;

            HistoryManager historyManager = new HistoryManager();
            foreach (ConversionItem item in HistoryManager.GetConversions()) {
                historyManager.RemoveConversion(item);
            }

            ConversionsBox.Items.Clear();
            ClearBTN.Enabled = false;
        }

        private void ConversionsBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (ConversionsBox.SelectedIndices.Count > 0) {
                RevertBTN.Enabled = true;
            } else {
                RevertBTN.Enabled = false;
            }
        }

        private void Revert_Progress(object sender, ProgressEventArgs e) {
            switch (e.Type) {
                case ProgressEvent.Reverted:
                case ProgressEvent.RevertFailed:
                    this.InvokeSafe(() => {
                        ConversionsBox.Items.Remove(e.Item);
                    });
                    break;

            }
        }
    }
}
