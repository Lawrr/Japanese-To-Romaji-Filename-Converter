namespace JapaneseToRomajiFilenameConverter {
    partial class ConverterForm {
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
            this.ProgressBox = new System.Windows.Forms.RichTextBox();
            this.CloseBTN = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ProgressBox
            // 
            this.ProgressBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ProgressBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.ProgressBox.DetectUrls = false;
            this.ProgressBox.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProgressBox.Location = new System.Drawing.Point(12, 12);
            this.ProgressBox.Name = "ProgressBox";
            this.ProgressBox.ReadOnly = true;
            this.ProgressBox.Size = new System.Drawing.Size(720, 327);
            this.ProgressBox.TabIndex = 0;
            this.ProgressBox.Text = "";
            this.ProgressBox.WordWrap = false;
            // 
            // CloseBTN
            // 
            this.CloseBTN.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseBTN.Location = new System.Drawing.Point(12, 345);
            this.CloseBTN.Name = "CloseBTN";
            this.CloseBTN.Size = new System.Drawing.Size(720, 37);
            this.CloseBTN.TabIndex = 2;
            this.CloseBTN.Text = "Abort";
            this.CloseBTN.UseVisualStyleBackColor = true;
            this.CloseBTN.Click += new System.EventHandler(this.CloseBTN_Click);
            // 
            // ConverterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 391);
            this.ControlBox = false;
            this.Controls.Add(this.CloseBTN);
            this.Controls.Add(this.ProgressBox);
            this.MinimumSize = new System.Drawing.Size(400, 200);
            this.Name = "ConverterForm";
            this.ShowInTaskbar = false;
            this.Text = "Conversion Progress";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConverterForm_FormClosed);
            this.Load += new System.EventHandler(this.ConverterForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox ProgressBox;
        private System.Windows.Forms.Button CloseBTN;
    }
}