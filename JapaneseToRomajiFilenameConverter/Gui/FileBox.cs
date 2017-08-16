using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using TagLib;
using Timer = System.Timers.Timer;

namespace JapaneseToRomajiFilenameConverter.Gui {

    public partial class FileBox : ListBox {

        public Size ImageSize { get; } = new Size(100, 100);
        // Loads images which have been requested to be drawn
        // Loaded in the latest order so it is closest to where you last viewed... i.e. stack
        private readonly Stack<DrawItemEventArgs> OnDemandImageLoadStack = new Stack<DrawItemEventArgs>();
        private readonly Timer ImageLoadTimer;

        public FileBox() {
            InitializeComponent();
            DoubleBuffered = true;

            ImageLoadTimer = new Timer(10);
            ImageLoadTimer.Elapsed += new ElapsedEventHandler(LoadImage);
            ImageLoadTimer.Enabled = true;
        }

        private void LoadImage(object sender, ElapsedEventArgs e) {
            if (OnDemandImageLoadStack.Count == 0) {
                ImageLoadTimer.Enabled = false;
                return;
            }

            try {
                DrawItemEventArgs itemEventArgs = OnDemandImageLoadStack.Pop();
                int index = itemEventArgs.Index;
                FileBoxItem item = (FileBoxItem) Items[index];
                item.LoadImage(ImageSize.Width, ImageSize.Height);

                int numVisibleItems = ClientSize.Height/ItemHeight;
                int nextIndex = -1;
                if (OnDemandImageLoadStack.Count > 0) {
                    try {
                        nextIndex = OnDemandImageLoadStack.Peek().Index;
                    } catch (NullReferenceException) {
                        // Null event args
                    }
                }
                this.InvokeSafe(() => {
                    // Refresh only if we aren't going to have to refresh after the next item as well
                    if ((index >= TopIndex && index <= TopIndex + numVisibleItems) &&
                        !(nextIndex >= TopIndex && nextIndex <= TopIndex + numVisibleItems)) {
                        Refresh();
                    }
                });
            } catch (ArgumentOutOfRangeException) {
                // ignored
            }
        }

        protected override void OnDrawItem(DrawItemEventArgs e) {
            base.OnDrawItem(e);

            if (Items.Count > 0) {
                FileBoxItem item = (FileBoxItem)Items[e.Index];

                if (!item.IsImageLoaded) {
                    OnDemandImageLoadStack.Push(e);
                    ImageLoadTimer.Enabled = true;
                }

                item.DrawItem(e, this);
            }
        }

        public void ClearItems() {
            OnDemandImageLoadStack.Clear();
            Items.Clear();
        }
    }

    public class FileBoxItem {

        private StringFormat Alignment = new StringFormat();

        public string DirectoryName { get; set; }
        public string FileName { get; set; }
        public Image FileImage { get; set; }
        public bool IsImageLoaded { get; set; } = false;

        private FileBox Container;
        private Font ExtensionFont;
        private StringFormat ExtensionAlignment;
        private string ExtensionString;

        private Font FileNameFont;

        public FileBoxItem(FileBox fileBox, string path) {
            Alignment.Alignment = StringAlignment.Near;
            Alignment.LineAlignment = StringAlignment.Near;

            Container = fileBox;
            DirectoryName = Path.GetDirectoryName(path);
            FileName = Path.GetFileName(path);

            ExtensionFont = new Font(fileBox.Font.FontFamily, 20, FontStyle.Bold);
            ExtensionAlignment = new StringFormat();
            ExtensionAlignment.Alignment = StringAlignment.Center;
            ExtensionAlignment.LineAlignment = StringAlignment.Center;
            ExtensionString = Path.GetExtension(FileName).TrimStart('.');
            ExtensionString = ExtensionString.Substring(0, Math.Min(ExtensionString.Length, 4)).ToUpper();

            FileNameFont = new Font(fileBox.Font, FontStyle.Bold);
        }

        public void LoadImage(int width, int height) {
            if (IsImageLoaded) return;
            IsImageLoaded = true;

            Image thumbnail = null;
            string filePath = Path.Combine(DirectoryName, FileName);

            // Extract from audio file
            try {
                TagLib.File tagFile = TagLib.File.Create(filePath);
                if (tagFile.Tag.Pictures.Length > 0) {
                    IPicture pic = tagFile.Tag.Pictures[0];
                    using (MemoryStream ms = new MemoryStream(pic.Data.Data)) {
                        Image img = Image.FromStream(ms);
                        thumbnail = img.GetThumbnailImage(width, height, null, IntPtr.Zero);
                    }
                }
            } catch (Exception) {
                // ignored
            }

            // Extract from image file
            if (FileImage == null) {
                try {
                    using (Image img = Image.FromFile(filePath)) {
                        thumbnail = img.GetThumbnailImage(width, height, null, IntPtr.Zero);
                    }
                } catch (Exception) {
                    // ignored
                }
            }

            if (thumbnail != null) {
                FileImage = new Bitmap(thumbnail);
                thumbnail.Dispose();
            }
        }

        public void DrawItem(DrawItemEventArgs e, FileBox box) {
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected) {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(186, 229, 255)), e.Bounds);
            } else {
                e.Graphics.FillRectangle(Brushes.White, e.Bounds);
            }

            // draw some item separator
            e.Graphics.DrawLine(Pens.DarkGray, e.Bounds.X, e.Bounds.Y,
                e.Bounds.X + e.Bounds.Width, e.Bounds.Y);

            // draw item image
            if (FileImage != null) {
                e.Graphics.DrawImage(FileImage, e.Bounds.X, e.Bounds.Y + 1, box.ImageSize.Width,
                    box.ImageSize.Height - 1);
            } else {
                Rectangle extensionBounds = new Rectangle(e.Bounds.X,
                                                  e.Bounds.Y + 1,
                                                  box.ImageSize.Width,
                                                  box.ImageSize.Height - 1);

                e.Graphics.FillRectangle(Brushes.LightGray, extensionBounds);
                e.Graphics.DrawString(ExtensionString,
                    ExtensionFont, Brushes.Black, extensionBounds, ExtensionAlignment);
            }

            Rectangle fileNameBounds = new Rectangle(e.Bounds.X + box.Margin.Horizontal + box.ImageSize.Width,
                                                  e.Bounds.Y + box.Margin.Top,
                                                  e.Bounds.Width - box.Margin.Right - box.ImageSize.Width - box.Margin.Horizontal,
                                                  (int)box.Font.GetHeight() + 2);
            
            Rectangle detailBounds = new Rectangle(e.Bounds.X + box.Margin.Horizontal + box.ImageSize.Width,
                                                   e.Bounds.Y + (int)box.Font.GetHeight() + 2 + box.Margin.Vertical + box.Margin.Top,
                                                   e.Bounds.Width - box.Margin.Right - box.ImageSize.Width - box.Margin.Horizontal,
                                                   e.Bounds.Height - box.Margin.Bottom - (int)box.Font.GetHeight() - 2 - box.Margin.Vertical - box.Margin.Top);

            // draw the text within the bounds
            e.Graphics.DrawString(FileName, FileNameFont, Brushes.Black, fileNameBounds, Alignment);            
            e.Graphics.DrawString(DirectoryName, box.Font, Brushes.Black, detailBounds, Alignment);
            
            // put some focus rectangle
            e.DrawFocusRectangle();
        }

        protected bool Equals(string other) {
            return other.Equals(Path.Combine(DirectoryName, FileName));
        }

        protected bool Equals(FileBoxItem other) {
            return string.Equals(DirectoryName, other.DirectoryName, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(FileName, other.FileName, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() == typeof (string)) return Equals((string) obj);
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FileBoxItem) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((DirectoryName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(DirectoryName) : 0)*397) ^ (FileName != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(FileName) : 0);
            }
        }

        public override string ToString() {
            return Path.Combine(DirectoryName, FileName);
        }
    }
}