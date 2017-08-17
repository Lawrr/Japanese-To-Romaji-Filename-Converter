using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using JapaneseToRomajiFilenameConverter.Converter;

namespace JapaneseToRomajiFilenameConverter {
    public partial class ConverterForm : Form {

        public event EventHandler<ProgressEventArgs> Progress;

        private HistoryManager HistoryManager;
        private Task FileConversionTask;
        private CancellationTokenSource FileConversionTaskCts;

        private int ConvertedFiles = 0;
        private int TotalFiles = 0;

        public ConverterForm() {
            InitializeComponent();

            HistoryManager = new HistoryManager();

            if (!IsHandleCreated) {
                CreateHandle();
            }
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
            TotalFiles = files.Count;

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
            TotalFiles = fileItems.Count;

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

                    ConvertedFiles++;

                    this.InvokeSafe(() => {
                        ProgressBox.AppendText(string.Format("Converted:{3}\t{0}{3}\tFrom:\t{1}{3}\tTo:\t{2}{3}",
                                                             Path.GetDirectoryName(e.Item.OldData.FilePath),
                                                             Path.GetFileName(e.Item.OldData.FilePath),
                                                             Path.GetFileName(e.Item.NewData.FilePath),
                                                             Environment.NewLine));
                        Text = $"Conversion Progress {ConvertedFiles}/{TotalFiles}";
                    });
                    break;

                case ProgressEvent.FileDoesNotExist:
                    ConvertedFiles++;

                    this.InvokeSafe(() => {
                        ProgressBox.AppendText(string.Format("FAIL - File does not exist{2}\t{0}{2}\tFile: {1}{2}",
                                                             Path.GetDirectoryName(e.Item.OldData.FilePath),
                                                             Path.GetFileName(e.Item.OldData.FilePath),
                                                             Environment.NewLine));
                        Text = $"Conversion Progress {ConvertedFiles}/{TotalFiles}";
                    });
                    break;

                case ProgressEvent.FileAlreadyExists:
                    ConvertedFiles++;

                    this.InvokeSafe(() => {
                        ProgressBox.AppendText(string.Format("FAIL - Cannot override existing file with the same name{3}\t{0}{3}\tOriginal:\t{1}{3}\tExisting:\t{2}{3}",
                                                             Path.GetDirectoryName(e.Item.OldData.FilePath),
                                                             Path.GetFileName(e.Item.OldData.FilePath),
                                                             Path.GetFileName(e.Item.NewData.FilePath),
                                                             Environment.NewLine));
                        Text = $"Conversion Progress {ConvertedFiles}/{TotalFiles}";
                    });
                    break;

                case ProgressEvent.Reverted:
                    // Remove the conversion from history
                    HistoryManager.RemoveConversion(e.Item);

                    ConvertedFiles++;

                    this.InvokeSafe(() => {
                        ProgressBox.AppendText(string.Format("Reverted:{3}\t{0}{3}\tFrom:\t{1}{3}\tTo:\t{2}{3}",
                                                             Path.GetDirectoryName(e.Item.OldData.FilePath),
                                                             Path.GetFileName(e.Item.NewData.FilePath),
                                                             Path.GetFileName(e.Item.OldData.FilePath),
                                                             Environment.NewLine));
                        Text = $"Conversion Progress {ConvertedFiles}/{TotalFiles}";
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
