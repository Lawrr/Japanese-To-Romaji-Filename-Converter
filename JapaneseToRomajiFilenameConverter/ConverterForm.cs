using JapaneseToRomajiFileConverter.Converter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JapaneseToRomajiFileConverter {
    public partial class ConverterForm : Form {

        public event EventHandler<ProgressEventArgs> Progress;

        private HistoryManager HistoryManager;
        private Task FileConversionTask;
        private CancellationTokenSource FileConversionTaskCts;

        public ConverterForm() {
            InitializeComponent();

            HistoryManager = new HistoryManager();
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

        public async void ConvertFiles(List<string> files) {
            FileConverter fileConverter = new FileConverter();
            fileConverter.Progress += new EventHandler<ProgressEventArgs>(Converter_Progress);

            // Async file conversion task
            FileConversionTaskCts = new CancellationTokenSource();
            FileConversionTask = Task.Factory.StartNew(() => {
                try {
                    fileConverter.Convert(files, FileConversionTaskCts.Token);
                } catch (OperationCanceledException) {
                    // Task cancelled
                }
            });
            await FileConversionTask;
        }

        public async void RevertFiles(List<ConversionItem> fileItems) {
            FileConverter fileConverter = new FileConverter();
            fileConverter.Progress += new EventHandler<ProgressEventArgs>(Converter_Progress);

            // Async file conversion task
            FileConversionTaskCts = new CancellationTokenSource();
            FileConversionTask = Task.Factory.StartNew(() => {
                try {
                    fileConverter.Revert(fileItems);
                } catch (OperationCanceledException) {
                    // Task cancelled
                }
            });
            await FileConversionTask;
        }

        private void Converter_Progress(object sender, ProgressEventArgs e) {
            Progress?.Invoke(this, e);

            switch (e.Type) {
                case ProgressEvent.Converted:
                    // Save the conversion if data was changed in the conversion
                    if (!e.Item.OldData.Equals(e.Item.NewData)) {
                        HistoryManager.SaveConversion(e.Item);
                    }

                    this.InvokeSafe(() => {
                        ProgressBox.AppendText(string.Format("Converted: {0} to {1}{2}",
                                                             e.Item.OldData.FilePath,
                                                             Path.GetFileName(e.Item.NewData.FilePath),
                                                             Environment.NewLine));
                    });
                    break;

                case ProgressEvent.Reverted:
                    // Remove the conversion from history
                    HistoryManager.RemoveConversion(e.Item);

                    this.InvokeSafe(() => {
                        ProgressBox.AppendText(string.Format("Reverted: {0} to {1}{2}",
                                                             e.Item.NewData.FilePath,
                                                             Path.GetFileName(e.Item.OldData.FilePath),
                                                             Environment.NewLine));
                    });
                    break;

                case ProgressEvent.Completed:
                    this.InvokeSafe(() => {
                        ProgressBox.AppendText("Conversion completed");
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
