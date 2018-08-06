namespace MicronTrackerCSDemo
{
    partial class InspectImageForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.chkStats = new System.Windows.Forms.CheckBox();
            this.chkXP = new System.Windows.Forms.CheckBox();
            this.btnBrk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbHistLen = new System.Windows.Forms.TextBox();
            this.labelXY = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelPixVal = new System.Windows.Forms.Label();
            this.labelHistCount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(22, 109);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(288, 212);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(38, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "<";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(71, 21);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(22, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = ">";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(99, 21);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(33, 23);
            this.button3.TabIndex = 3;
            this.button3.Text = "/\\";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(138, 21);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(35, 23);
            this.button4.TabIndex = 4;
            this.button4.Text = "\\/";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // chkStats
            // 
            this.chkStats.AutoSize = true;
            this.chkStats.Location = new System.Drawing.Point(22, 59);
            this.chkStats.Name = "chkStats";
            this.chkStats.Size = new System.Drawing.Size(74, 17);
            this.chkStats.TabIndex = 5;
            this.chkStats.Text = "Stats over";
            this.chkStats.UseVisualStyleBackColor = true;
            this.chkStats.CheckedChanged += new System.EventHandler(this.chkStats_CheckedChanged);
            // 
            // chkXP
            // 
            this.chkXP.AutoSize = true;
            this.chkXP.Location = new System.Drawing.Point(22, 86);
            this.chkXP.Name = "chkXP";
            this.chkXP.Size = new System.Drawing.Size(40, 17);
            this.chkXP.TabIndex = 6;
            this.chkXP.Text = "XP";
            this.chkXP.UseVisualStyleBackColor = true;
            this.chkXP.CheckedChanged += new System.EventHandler(this.chkXP_CheckedChanged);
            // 
            // btnBrk
            // 
            this.btnBrk.Location = new System.Drawing.Point(68, 82);
            this.btnBrk.Name = "btnBrk";
            this.btnBrk.Size = new System.Drawing.Size(36, 23);
            this.btnBrk.TabIndex = 7;
            this.btnBrk.Text = "brk";
            this.btnBrk.UseVisualStyleBackColor = true;
            this.btnBrk.Click += new System.EventHandler(this.btnBrk_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(154, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "samples. actual: ";
            // 
            // tbHistLen
            // 
            this.tbHistLen.Location = new System.Drawing.Point(102, 56);
            this.tbHistLen.Name = "tbHistLen";
            this.tbHistLen.Size = new System.Drawing.Size(46, 20);
            this.tbHistLen.TabIndex = 9;
            this.tbHistLen.Text = "0";
            // 
            // labelXY
            // 
            this.labelXY.AutoSize = true;
            this.labelXY.Location = new System.Drawing.Point(219, 21);
            this.labelXY.Name = "labelXY";
            this.labelXY.Size = new System.Drawing.Size(30, 13);
            this.labelXY.TabIndex = 10;
            this.labelXY.Text = "X: Y:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(294, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Stats at center:";
            // 
            // labelPixVal
            // 
            this.labelPixVal.AutoSize = true;
            this.labelPixVal.ForeColor = System.Drawing.Color.Red;
            this.labelPixVal.Location = new System.Drawing.Point(135, 173);
            this.labelPixVal.Name = "labelPixVal";
            this.labelPixVal.Size = new System.Drawing.Size(25, 13);
            this.labelPixVal.TabIndex = 12;
            this.labelPixVal.Text = "112";
            // 
            // labelHistCount
            // 
            this.labelHistCount.AutoSize = true;
            this.labelHistCount.Location = new System.Drawing.Point(247, 60);
            this.labelHistCount.Name = "labelHistCount";
            this.labelHistCount.Size = new System.Drawing.Size(13, 13);
            this.labelHistCount.TabIndex = 13;
            this.labelHistCount.Text = "1";
            // 
            // InspectImageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(438, 347);
            this.Controls.Add(this.labelHistCount);
            this.Controls.Add(this.labelPixVal);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelXY);
            this.Controls.Add(this.tbHistLen);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBrk);
            this.Controls.Add(this.chkXP);
            this.Controls.Add(this.chkStats);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "InspectImageForm";
            this.Text = "InspectImageForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InspectImageForm_FormClosing);
            this.ResizeEnd += new System.EventHandler(this.InspectImageForm_ResizeEnd);
            this.Load += new System.EventHandler(this.InspectImageForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.CheckBox chkStats;
        private System.Windows.Forms.CheckBox chkXP;
        private System.Windows.Forms.Button btnBrk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbHistLen;
        private System.Windows.Forms.Label labelXY;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelPixVal;
        private System.Windows.Forms.Label labelHistCount;
    }
}