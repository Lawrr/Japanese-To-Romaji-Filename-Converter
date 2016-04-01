namespace Japanese_To_Romaji_File_Converter {
    partial class ConvertForm {
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
            this.CancelBTN = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ProgressBox
            // 
            this.ProgressBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ProgressBox.Cursor = System.Windows.Forms.Cursors.Default;
            this.ProgressBox.DetectUrls = false;
            this.ProgressBox.Location = new System.Drawing.Point(12, 12);
            this.ProgressBox.Name = "ProgressBox";
            this.ProgressBox.ReadOnly = true;
            this.ProgressBox.Size = new System.Drawing.Size(600, 342);
            this.ProgressBox.TabIndex = 0;
            this.ProgressBox.Text = "";
            // 
            // CancelBTN
            // 
            this.CancelBTN.Location = new System.Drawing.Point(537, 360);
            this.CancelBTN.Name = "CancelBTN";
            this.CancelBTN.Size = new System.Drawing.Size(75, 23);
            this.CancelBTN.TabIndex = 2;
            this.CancelBTN.Text = "Cancel";
            this.CancelBTN.UseVisualStyleBackColor = true;
            this.CancelBTN.Click += new System.EventHandler(this.CancelBTN_Click);
            // 
            // ConvertForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 392);
            this.ControlBox = false;
            this.Controls.Add(this.CancelBTN);
            this.Controls.Add(this.ProgressBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "ConvertForm";
            this.ShowInTaskbar = false;
            this.Text = "Conversion Progress";
            this.Load += new System.EventHandler(this.ConvertForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox ProgressBox;
        private System.Windows.Forms.Button CancelBTN;
    }
}