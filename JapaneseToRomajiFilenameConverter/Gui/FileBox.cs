using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TagLib;

namespace JapaneseToRomajiFilenameConverter.Gui {

    public partial class FileBox : ListBox {

        public Size ImageSize { get; } = new Size(100, 100);

        public FileBox() {
            InitializeComponent();
            DoubleBuffered = true;
        }

        protected override void OnDrawItem(DrawItemEventArgs e) {
            if (Items.Count > 0) {
                FileBoxItem item = (FileBoxItem)Items[e.Index];

                if (!item.LoadedImage) {
                    item.LoadedImage = true;

                    Image thumbnail = null;
                    string filePath = Path.Combine(item.DirectoryName, item.FileName);

                    // Extract from audio file
                    try {
                        TagLib.File tagFile = TagLib.File.Create(filePath);
                        if (tagFile.Tag.Pictures.Length > 0) {
                            IPicture pic = tagFile.Tag.Pictures[0];
                            using (MemoryStream ms = new MemoryStream(pic.Data.Data)) {
                                Image img = Image.FromStream(ms);
                                thumbnail = img.GetThumbnailImage(ImageSize.Width, ImageSize.Height, null,
                                    IntPtr.Zero);
                            }
                        }
                    } catch (Exception) {
                        // ignored
                    }

                    // Extract from image file
                    if (item.FileImage == null) {
                        try {
                            using (Image img = Image.FromFile(filePath)) {
                                thumbnail = img.GetThumbnailImage(ImageSize.Width, ImageSize.Height, null,
                                    IntPtr.Zero);
                            }
                        } catch (Exception) {
                            // ignored
                        }
                    }

                    if (thumbnail != null) {
                        item.FileImage = new Bitmap(thumbnail);
                        thumbnail.Dispose();
                    }
                }

                item.drawItem(e, this);
            }
        }
    }

    public class FileBoxItem {

        private StringFormat Alignment = new StringFormat();

        public string DirectoryName { get; set; }
        public string FileName { get; set; }
        public Image FileImage { get; set; }
        public bool LoadedImage { get; set; } = false;

        private Font ExtensionFont;
        private StringFormat ExtensionAlignment;
        private string ExtensionString;

        private Font FileNameFont;

        public FileBoxItem(ListBox listBox, string path) {
            Alignment.Alignment = StringAlignment.Near;
            Alignment.LineAlignment = StringAlignment.Near;

            DirectoryName = Path.GetDirectoryName(path);
            FileName = Path.GetFileName(path);

            ExtensionFont = new Font(listBox.Font.FontFamily, 20, FontStyle.Bold);
            ExtensionAlignment = new StringFormat();
            ExtensionAlignment.Alignment = StringAlignment.Center;
            ExtensionAlignment.LineAlignment = StringAlignment.Center;
            ExtensionString = Path.GetExtension(FileName).TrimStart('.');
            ExtensionString = ExtensionString.Substring(0, Math.Min(ExtensionString.Length, 4)).ToUpper();

            FileNameFont = new Font(listBox.Font, FontStyle.Bold);
        }

        public void drawItem(DrawItemEventArgs e, FileBox box) {            
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