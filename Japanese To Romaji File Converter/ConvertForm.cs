using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Japanese_To_Romaji_File_Converter {
    public partial class ConvertForm : Form {

        public ConvertForm(MainForm parentForm) {
            InitializeComponent();

            ConvertFiles(parentForm.GetFiles());
        }

        private void ConvertForm_Load(object sender, EventArgs e) {
            CenterToParent();
            BringToFront();
            Activate();
        }

        private async void ConvertFiles(List<string> files) {
            Converter converter = new Converter(files);
            converter.Progress += new EventHandler<ProgressEventArgs>(Converter_Progress);
            await Task.Factory.StartNew(() => {
                converter.Convert();
            });
        }

        private void Converter_Progress(object sender, ProgressEventArgs e) {
            switch (e.Type) {
                case ProgressUpdate.Converted:
                    this.InvokeSafe(() => {
                        ProgressBox.Text += String.Format("Converted: {0} to {1}{2}", e.OldFileName, e.NewFileName, Environment.NewLine);
                    });
                    break;

                case ProgressUpdate.Completed:
                    this.InvokeSafe(() => {
                        ProgressBox.Text += String.Format("Conversion completed{0}", Environment.NewLine);
                    });
                    break;

                default:
                    throw new Exception("Invalid progress update type");
            }
        }

        private void CancelBTN_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
