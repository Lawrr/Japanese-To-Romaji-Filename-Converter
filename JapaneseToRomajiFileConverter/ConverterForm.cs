using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JapaneseToRomajiFileConverter {
    public partial class ConverterForm : Form {

        public ConverterForm(MainForm parentForm) {
            InitializeComponent();

            ConvertFiles(parentForm.GetFiles());
        }

        private void ConverterForm_Load(object sender, EventArgs e) {
            CenterToParent();
            BringToFront();
            Activate();

            ProgressBox.LanguageOption = RichTextBoxLanguageOptions.DualFont;
        }

        private async void ConvertFiles(List<string> files) {
            FileConverter fileConverter = new FileConverter(files);
            fileConverter.Progress += new EventHandler<ProgressEventArgs>(Converter_Progress);

            // Async convert
            await Task.Factory.StartNew(() => {
                fileConverter.Convert();
            });
        }

        private void Converter_Progress(object sender, ProgressEventArgs e) {
            switch (e.Type) {
                case ProgressEvent.Converted:
                    this.InvokeSafe(() => {
                        ProgressBox.Text += String.Format("Converted: {0} to {1}{2}",
                                                          e.OldFilePath,
                                                          Path.GetFileName(e.NewFilePath),
                                                          Environment.NewLine);
                    });
                    break;

                case ProgressEvent.Completed:
                    this.InvokeSafe(() => {
                        ProgressBox.Text += String.Format("Conversion completed");
                        CloseBTN.Text = "Done";
                    });
                    break;

                default:
                    throw new Exception("Invalid progress update type");
            }
        }

        private void CloseBTN_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
