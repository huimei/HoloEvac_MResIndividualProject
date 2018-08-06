namespace HoloEvac_Micron
{
    partial class SnapshotsForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SnapshotsForm));
            this.btnSnap = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.btnLoadAppend = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.scrImgNum = new System.Windows.Forms.VScrollBar();
            this.lblSnapPath = new System.Windows.Forms.Label();
            this.tbFrameNum = new System.Windows.Forms.TextBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnTimeIt = new System.Windows.Forms.Button();
            this.btnReprocess = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.chkDisableHdr = new System.Windows.Forms.CheckBox();
            this.Play_CheckBox = new System.Windows.Forms.CheckBox();
            this.Play_Timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnSnap
            // 
            this.btnSnap.Location = new System.Drawing.Point(64, 49);
            this.btnSnap.Name = "btnSnap";
            this.btnSnap.Size = new System.Drawing.Size(105, 23);
            this.btnSnap.TabIndex = 0;
            this.btnSnap.Text = "&Snap frame";
            this.btnSnap.UseVisualStyleBackColor = true;
            this.btnSnap.Click += new System.EventHandler(this.btnSnap_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(64, 114);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(105, 23);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear list";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(64, 140);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(105, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "Save...";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.Location = new System.Drawing.Point(64, 178);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(105, 23);
            this.btnLoadFile.TabIndex = 3;
            this.btnLoadFile.Tag = "0";
            this.btnLoadFile.Text = "&Load new...";
            this.btnLoadFile.UseVisualStyleBackColor = true;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // btnLoadAppend
            // 
            this.btnLoadAppend.Location = new System.Drawing.Point(64, 207);
            this.btnLoadAppend.Name = "btnLoadAppend";
            this.btnLoadAppend.Size = new System.Drawing.Size(105, 23);
            this.btnLoadAppend.TabIndex = 4;
            this.btnLoadAppend.Tag = "1";
            this.btnLoadAppend.Text = "Load append...";
            this.btnLoadAppend.UseVisualStyleBackColor = true;
            this.btnLoadAppend.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(77, 358);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // scrImgNum
            // 
            this.scrImgNum.LargeChange = 1;
            this.scrImgNum.Location = new System.Drawing.Point(20, 49);
            this.scrImgNum.Name = "scrImgNum";
            this.scrImgNum.Size = new System.Drawing.Size(20, 332);
            this.scrImgNum.TabIndex = 6;
            this.scrImgNum.ValueChanged += new System.EventHandler(this.scrImgNum_ValueChanged);
            // 
            // lblSnapPath
            // 
            this.lblSnapPath.Location = new System.Drawing.Point(61, 243);
            this.lblSnapPath.Name = "lblSnapPath";
            this.lblSnapPath.Size = new System.Drawing.Size(108, 63);
            this.lblSnapPath.TabIndex = 7;
            this.lblSnapPath.Text = "Snap path";
            // 
            // tbFrameNum
            // 
            this.tbFrameNum.Location = new System.Drawing.Point(12, 12);
            this.tbFrameNum.Name = "tbFrameNum";
            this.tbFrameNum.Size = new System.Drawing.Size(43, 20);
            this.tbFrameNum.TabIndex = 8;
            this.tbFrameNum.Text = "0";
            this.tbFrameNum.TextChanged += new System.EventHandler(this.tbFrameNum_TextChanged);
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(61, 15);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(31, 13);
            this.lblCount.TabIndex = 9;
            this.lblCount.Text = "of 12";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnTimeIt
            // 
            this.btnTimeIt.Location = new System.Drawing.Point(77, 303);
            this.btnTimeIt.Name = "btnTimeIt";
            this.btnTimeIt.Size = new System.Drawing.Size(75, 23);
            this.btnTimeIt.TabIndex = 10;
            this.btnTimeIt.Text = "time it";
            this.btnTimeIt.UseVisualStyleBackColor = true;
            this.btnTimeIt.Click += new System.EventHandler(this.btnTimeIt_Click);
            // 
            // btnReprocess
            // 
            this.btnReprocess.Location = new System.Drawing.Point(77, 332);
            this.btnReprocess.Name = "btnReprocess";
            this.btnReprocess.Size = new System.Drawing.Size(75, 23);
            this.btnReprocess.TabIndex = 11;
            this.btnReprocess.Text = "Reprocess";
            this.btnReprocess.UseVisualStyleBackColor = true;
            this.btnReprocess.Click += new System.EventHandler(this.btnReprocess_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(64, 76);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(105, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "Snap 16 frames";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // chkDisableHdr
            // 
            this.chkDisableHdr.AutoSize = true;
            this.chkDisableHdr.Location = new System.Drawing.Point(19, 395);
            this.chkDisableHdr.Name = "chkDisableHdr";
            this.chkDisableHdr.Size = new System.Drawing.Size(81, 17);
            this.chkDisableHdr.TabIndex = 13;
            this.chkDisableHdr.Text = "Disable Hdr";
            this.chkDisableHdr.UseVisualStyleBackColor = true;
            // 
            // Play_CheckBox
            // 
            this.Play_CheckBox.AutoSize = true;
            this.Play_CheckBox.Location = new System.Drawing.Point(106, 393);
            this.Play_CheckBox.Name = "Play_CheckBox";
            this.Play_CheckBox.Size = new System.Drawing.Size(46, 17);
            this.Play_CheckBox.TabIndex = 28;
            this.Play_CheckBox.Text = "Play";
            this.Play_CheckBox.UseVisualStyleBackColor = true;
            this.Play_CheckBox.CheckedChanged += new System.EventHandler(this.Play_CheckBox_CheckedChanged);
            // 
            // Play_Timer
            // 
            this.Play_Timer.Interval = 1;
            this.Play_Timer.Tick += new System.EventHandler(this.Play_Timer_Tick);
            // 
            // SnapshotsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(194, 422);
            this.Controls.Add(this.Play_CheckBox);
            this.Controls.Add(this.chkDisableHdr);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnReprocess);
            this.Controls.Add(this.btnTimeIt);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.tbFrameNum);
            this.Controls.Add(this.lblSnapPath);
            this.Controls.Add(this.scrImgNum);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnLoadAppend);
            this.Controls.Add(this.btnLoadFile);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSnap);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SnapshotsForm";
            this.ShowInTaskbar = false;
            this.Text = "Snapshots";
            this.Load += new System.EventHandler(this.Form_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SnapshotsForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSnap;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.Button btnLoadAppend;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.VScrollBar scrImgNum;
        private System.Windows.Forms.Label lblSnapPath;
        private System.Windows.Forms.TextBox tbFrameNum;
        private System.Windows.Forms.Label lblCount;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.Button btnTimeIt;
		private System.Windows.Forms.Button btnReprocess;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkDisableHdr;
        internal System.Windows.Forms.CheckBox Play_CheckBox;
        internal System.Windows.Forms.Timer Play_Timer;
    }
}