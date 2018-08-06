namespace HoloEvac_Micron
{
    partial class CaliperForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelOffset = new System.Windows.Forms.Label();
            this.btnDone = new System.Windows.Forms.Button();
            this.tbOffset = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // labelOffset
            // 
            this.labelOffset.AutoSize = true;
            this.labelOffset.Location = new System.Drawing.Point(22, 37);
            this.labelOffset.Name = "labelOffset";
            this.labelOffset.Size = new System.Drawing.Size(35, 13);
            this.labelOffset.TabIndex = 0;
            this.labelOffset.Text = "Offset";
            // 
            // btnDone
            // 
            this.btnDone.Location = new System.Drawing.Point(101, 69);
            this.btnDone.Name = "btnDone";
            this.btnDone.Size = new System.Drawing.Size(75, 23);
            this.btnDone.TabIndex = 1;
            this.btnDone.Text = "Done";
            this.btnDone.UseVisualStyleBackColor = true;
            this.btnDone.Click += new System.EventHandler(this.btnDone_Click);
            // 
            // tbOffset
            // 
            this.tbOffset.Location = new System.Drawing.Point(76, 34);
            this.tbOffset.Name = "tbOffset";
            this.tbOffset.Size = new System.Drawing.Size(100, 20);
            this.tbOffset.TabIndex = 2;
            this.tbOffset.Text = "0.0";
            this.tbOffset.TextChanged += new System.EventHandler(this.tbOffset_TextChanged);
            // 
            // CaliperForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(209, 109);
            this.Controls.Add(this.tbOffset);
            this.Controls.Add(this.btnDone);
            this.Controls.Add(this.labelOffset);
            this.Name = "CaliperForm";
            this.Text = "CaliperForm";
            this.Load += new System.EventHandler(this.CaliperForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelOffset;
        private System.Windows.Forms.Button btnDone;
        private System.Windows.Forms.TextBox tbOffset;
    }
}