﻿using JapaneseToRomajiFilenameConverter.Gui;

namespace JapaneseToRomajiFilenameConverter {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.ConvertBTN = new System.Windows.Forms.Button();
            this.AddBTN = new System.Windows.Forms.Button();
            this.RemoveBTN = new System.Windows.Forms.Button();
            this.DragDropLabel = new System.Windows.Forms.Label();
            this.ClearBTN = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.totalFilesLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.selectedFilesLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.HistoryBTN = new System.Windows.Forms.Button();
            this.FilesBox = new JapaneseToRomajiFilenameConverter.Gui.FileBox();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConvertBTN
            // 
            this.ConvertBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ConvertBTN.Enabled = false;
            this.ConvertBTN.Location = new System.Drawing.Point(1102, 12);
            this.ConvertBTN.Name = "ConvertBTN";
            this.ConvertBTN.Size = new System.Drawing.Size(95, 45);
            this.ConvertBTN.TabIndex = 1;
            this.ConvertBTN.Text = "Convert";
            this.ConvertBTN.UseVisualStyleBackColor = true;
            this.ConvertBTN.Click += new System.EventHandler(this.ConvertBTN_Click);
            // 
            // AddBTN
            // 
            this.AddBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.AddBTN.Location = new System.Drawing.Point(1102, 401);
            this.AddBTN.Name = "AddBTN";
            this.AddBTN.Size = new System.Drawing.Size(95, 35);
            this.AddBTN.TabIndex = 2;
            this.AddBTN.Text = "Add Files";
            this.AddBTN.UseVisualStyleBackColor = true;
            this.AddBTN.Click += new System.EventHandler(this.AddBTN_Click);
            // 
            // RemoveBTN
            // 
            this.RemoveBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveBTN.Enabled = false;
            this.RemoveBTN.Location = new System.Drawing.Point(1102, 442);
            this.RemoveBTN.Name = "RemoveBTN";
            this.RemoveBTN.Size = new System.Drawing.Size(95, 35);
            this.RemoveBTN.TabIndex = 3;
            this.RemoveBTN.Text = "Remove Files";
            this.RemoveBTN.UseVisualStyleBackColor = true;
            this.RemoveBTN.Click += new System.EventHandler(this.RemoveBTN_Click);
            // 
            // DragDropLabel
            // 
            this.DragDropLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DragDropLabel.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.DragDropLabel.Font = new System.Drawing.Font("Segoe UI", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DragDropLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.DragDropLabel.Location = new System.Drawing.Point(36, 37);
            this.DragDropLabel.Name = "DragDropLabel";
            this.DragDropLabel.Size = new System.Drawing.Size(1038, 443);
            this.DragDropLabel.TabIndex = 0;
            this.DragDropLabel.Text = "Drag and Drop Files";
            this.DragDropLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ClearBTN
            // 
            this.ClearBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearBTN.Enabled = false;
            this.ClearBTN.Location = new System.Drawing.Point(1102, 483);
            this.ClearBTN.Name = "ClearBTN";
            this.ClearBTN.Size = new System.Drawing.Size(95, 35);
            this.ClearBTN.TabIndex = 3;
            this.ClearBTN.Text = "Clear Files";
            this.ClearBTN.UseVisualStyleBackColor = true;
            this.ClearBTN.Click += new System.EventHandler(this.ClearBTN_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.totalFilesLabel,
            this.selectedFilesLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 528);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1209, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "Status Strip";
            // 
            // totalFilesLabel
            // 
            this.totalFilesLabel.Name = "totalFilesLabel";
            this.totalFilesLabel.Size = new System.Drawing.Size(72, 17);
            this.totalFilesLabel.Text = "Total Files: 0";
            // 
            // selectedFilesLabel
            // 
            this.selectedFilesLabel.Name = "selectedFilesLabel";
            this.selectedFilesLabel.Size = new System.Drawing.Size(89, 17);
            this.selectedFilesLabel.Text = "Selected Files: 0";
            // 
            // HistoryBTN
            // 
            this.HistoryBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.HistoryBTN.Location = new System.Drawing.Point(1102, 63);
            this.HistoryBTN.Name = "HistoryBTN";
            this.HistoryBTN.Size = new System.Drawing.Size(95, 45);
            this.HistoryBTN.TabIndex = 1;
            this.HistoryBTN.Text = "View Conversion History";
            this.HistoryBTN.UseVisualStyleBackColor = true;
            this.HistoryBTN.Click += new System.EventHandler(this.HistoryBTN_Click);
            // 
            // FilesBox
            // 
            this.FilesBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilesBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.FilesBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FilesBox.FormattingEnabled = true;
            this.FilesBox.HorizontalScrollbar = true;
            this.FilesBox.ItemHeight = 100;
            this.FilesBox.Location = new System.Drawing.Point(13, 13);
            this.FilesBox.Name = "FilesBox";
            this.FilesBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.FilesBox.Size = new System.Drawing.Size(1083, 504);
            this.FilesBox.TabIndex = 0;
            this.FilesBox.SelectedIndexChanged += new System.EventHandler(this.FilesBox_SelectedIndexChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1209, 550);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.DragDropLabel);
            this.Controls.Add(this.ClearBTN);
            this.Controls.Add(this.RemoveBTN);
            this.Controls.Add(this.AddBTN);
            this.Controls.Add(this.HistoryBTN);
            this.Controls.Add(this.ConvertBTN);
            this.Controls.Add(this.FilesBox);
            this.MinimumSize = new System.Drawing.Size(433, 302);
            this.Name = "MainForm";
            this.Text = "Japanese to Romaji Filename Converter " + ProductVersion;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ConvertBTN;
        private System.Windows.Forms.Button AddBTN;
        private System.Windows.Forms.Button RemoveBTN;
        private System.Windows.Forms.Label DragDropLabel;
        private System.Windows.Forms.Button ClearBTN;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel totalFilesLabel;
        private System.Windows.Forms.ToolStripStatusLabel selectedFilesLabel;
        private System.Windows.Forms.Button HistoryBTN;
        private FileBox FilesBox;
    }
}

