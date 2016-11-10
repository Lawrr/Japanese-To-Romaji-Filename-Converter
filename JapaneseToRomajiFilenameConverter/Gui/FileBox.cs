using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace JapaneseToRomajiFilenameConverter.Gui {

    public partial class FileBox : ListBox {

        public Size ImageSize { get; } = new Size(100, 100);

        public FileBox() {
            InitializeComponent();
        }

        protected override void OnDrawItem(DrawItemEventArgs e) {
            if (Items.Count > 0) {
                FileBoxItem item = (FileBoxItem)Items[e.Index];
                item.drawItem(e, this);
            }
        }
    }

    public class FileBoxItem {

        private StringFormat Alignment = new StringFormat();

        private string DirectoryName;
        private string FileName;
        private Image FileImage;

        private Font ExtensionFont;
        private StringFormat ExtensionAlignment;
        private string ExtensionString;

        private Font FileNameFont;

        public FileBoxItem(ListBox listBox, string path, Image image) {
            Alignment.Alignment = StringAlignment.Near;
            Alignment.LineAlignment = StringAlignment.Near;

            DirectoryName = Path.GetDirectoryName(path);
            FileName = Path.GetFileName(path);

            FileImage = image;

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