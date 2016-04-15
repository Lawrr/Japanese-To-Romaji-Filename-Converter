using System.Collections.Generic;
using System.Windows.Forms;
using JapaneseToRomajiFileConverter.Converter;

namespace JapaneseToRomajiFileConverter {
    public partial class ReverterForm : Form {

        private Reverter Reverter;
        private List<ConversionItem> ConversionItems;

        public ReverterForm() {
            InitializeComponent();

            Reverter = new Reverter();
            ConversionItems = Reverter.GetConversions();
            AddConversions(ConversionItems);
        }

        private void AddConversions(List<ConversionItem> items) {
            foreach (ConversionItem item in items) {
                ConversionsBox.Items.Add(item);
            }
        }

        private void RevertBTN_Click(object sender, System.EventArgs e) {
            int[] indices = new int[ConversionsBox.SelectedIndices.Count];
            for (int i = ConversionsBox.SelectedIndices.Count - 1; i >= 0; i--) {
                indices[i] = ConversionsBox.SelectedIndices[i];
            }
        }

    }
}
