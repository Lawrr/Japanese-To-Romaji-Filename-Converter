namespace Japanese_To_Romaji_File_Converter {
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
            this.FilesBox = new System.Windows.Forms.ListBox();
            this.ConvertBTN = new System.Windows.Forms.Button();
            this.AddBTN = new System.Windows.Forms.Button();
            this.RemoveBTN = new System.Windows.Forms.Button();
            this.DragDropLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // FilesBox
            // 
            this.FilesBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FilesBox.FormattingEnabled = true;
            this.FilesBox.Location = new System.Drawing.Point(13, 13);
            this.FilesBox.Name = "FilesBox";
            this.FilesBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.FilesBox.Size = new System.Drawing.Size(534, 368);
            this.FilesBox.TabIndex = 0;
            // 
            // ConvertBTN
            // 
            this.ConvertBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ConvertBTN.Location = new System.Drawing.Point(553, 361);
            this.ConvertBTN.Name = "ConvertBTN";
            this.ConvertBTN.Size = new System.Drawing.Size(95, 25);
            this.ConvertBTN.TabIndex = 1;
            this.ConvertBTN.Text = "Convert";
            this.ConvertBTN.UseVisualStyleBackColor = true;
            this.ConvertBTN.Click += new System.EventHandler(this.ConvertBTN_Click);
            // 
            // AddBTN
            // 
            this.AddBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddBTN.Location = new System.Drawing.Point(553, 12);
            this.AddBTN.Name = "AddBTN";
            this.AddBTN.Size = new System.Drawing.Size(95, 25);
            this.AddBTN.TabIndex = 2;
            this.AddBTN.Text = "Add Files";
            this.AddBTN.UseVisualStyleBackColor = true;
            this.AddBTN.Click += new System.EventHandler(this.AddBTN_Click);
            // 
            // RemoveBTN
            // 
            this.RemoveBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveBTN.Location = new System.Drawing.Point(553, 43);
            this.RemoveBTN.Name = "RemoveBTN";
            this.RemoveBTN.Size = new System.Drawing.Size(95, 25);
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
            this.DragDropLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DragDropLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.DragDropLabel.Location = new System.Drawing.Point(27, 29);
            this.DragDropLabel.Name = "DragDropLabel";
            this.DragDropLabel.Size = new System.Drawing.Size(506, 337);
            this.DragDropLabel.TabIndex = 0;
            this.DragDropLabel.Text = "Drag and Drop Files";
            this.DragDropLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 396);
            this.Controls.Add(this.DragDropLabel);
            this.Controls.Add(this.RemoveBTN);
            this.Controls.Add(this.AddBTN);
            this.Controls.Add(this.ConvertBTN);
            this.Controls.Add(this.FilesBox);
            this.Name = "MainForm";
            this.Text = "Japanese to Romaji File Converter";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox FilesBox;
        private System.Windows.Forms.Button ConvertBTN;
        private System.Windows.Forms.Button AddBTN;
        private System.Windows.Forms.Button RemoveBTN;
        private System.Windows.Forms.Label DragDropLabel;
    }
}

