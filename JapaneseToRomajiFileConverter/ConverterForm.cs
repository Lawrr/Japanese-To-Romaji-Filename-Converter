using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JapaneseToRomajiFileConverter {
    public partial class ConverterForm : Form {

        private Task FileConversionTask;
        private CancellationTokenSource FileConversionTaskCts;

        public ConverterForm(List<string> files) {
            InitializeComponent();

            ConvertFiles(files);
        }

        private void ConverterForm_Load(object sender, EventArgs e) {
            CenterToParent();
            BringToFront();
            Activate();

            ProgressBox.LanguageOption = RichTextBoxLanguageOptions.DualFont;
        }

        private void ConverterForm_FormClosed(object sender, FormClosedEventArgs e) {
            // Stop async file conversion task
            if (FileConversionTaskCts != null) {
                FileConversionTaskCts.Cancel();
            }
        }

        private async void ConvertFiles(List<string> files) {
            FileConverter fileConverter = new FileConverter(files);
            fileConverter.Progress += new EventHandler<ProgressEventArgs>(Converter_Progress);

            // Async file conversion task
            FileConversionTaskCts = new CancellationTokenSource();
            FileConversionTask = Task.Factory.StartNew(() => {
                try {
                    fileConverter.Convert(FileConversionTaskCts.Token);
                } catch (OperationCanceledException) {
                    // Task cancelled
                }
            });
            await FileConversionTask;
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
