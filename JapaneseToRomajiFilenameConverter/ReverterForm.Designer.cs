﻿namespace JapaneseToRomajiFileConverter {
    partial class ReverterForm {
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
            this.ConversionsBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // ConversionsBox
            // 
            this.ConversionsBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConversionsBox.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConversionsBox.FormattingEnabled = true;
            this.ConversionsBox.HorizontalScrollbar = true;
            this.ConversionsBox.IntegralHeight = false;
            this.ConversionsBox.ItemHeight = 21;
            this.ConversionsBox.Location = new System.Drawing.Point(12, 12);
            this.ConversionsBox.Name = "ConversionsBox";
            this.ConversionsBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.ConversionsBox.Size = new System.Drawing.Size(744, 459);
            this.ConversionsBox.TabIndex = 0;
            // 
            // ReverterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 484);
            this.Controls.Add(this.ConversionsBox);
            this.Name = "ReverterForm";
            this.Text = "Conversion Reverter";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox ConversionsBox;
    }
}