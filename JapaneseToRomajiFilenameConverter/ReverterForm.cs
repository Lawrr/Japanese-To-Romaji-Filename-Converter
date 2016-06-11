using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using JapaneseToRomajiFilenameConverter.Converter;

namespace JapaneseToRomajiFilenameConverter {
    public partial class ReverterForm : Form {

        private List<ConversionItem> ConversionItems;

        public ReverterForm() {
            InitializeComponent();

            ConversionItems = HistoryManager.GetConversions();
            AddConversions(ConversionItems);
        }

        private void ReverterForm_Load(object sender, EventArgs e) {
            CenterToParent();
            BringToFront();
            Activate();
        }

        private void AddConversions(List<ConversionItem> items) {
            foreach (ConversionItem item in items) {
                ConversionsBox.Items.Add(item);
            }
        }

        private void RevertBTN_Click(object sender, System.EventArgs e) {
            ConverterForm convertForm = new ConverterForm();
            convertForm.Progress += new EventHandler<ProgressEventArgs>(Revert_Progress);
            convertForm.RevertFiles(ConversionsBox.SelectedItems.Cast<ConversionItem>().ToList());
            convertForm.ShowDialog();
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
                    this.InvokeSafe(() => {
                        ConversionsBox.Items.Remove(e.Item);
                    });
                    break;
            }
        }

    }
}
