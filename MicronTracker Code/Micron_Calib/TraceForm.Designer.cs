namespace MicronTrackerCalibration
{
    partial class TraceForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TraceForm));
			this.btnReset = new System.Windows.Forms.Button();
			this.chkEmphasizeLine = new System.Windows.Forms.CheckBox();
			this.chkShowMagnified = new System.Windows.Forms.CheckBox();
			this.tbTracerMarker = new System.Windows.Forms.TextBox();
			this.tbRefMarker = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.chkTrace = new System.Windows.Forms.CheckBox();
			this.tmrCheckFOM = new System.Windows.Forms.Timer(this.components);
			this.lblInfo = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnReset
			// 
			this.btnReset.Location = new System.Drawing.Point(16, 12);
			this.btnReset.Name = "btnReset";
			this.btnReset.Size = new System.Drawing.Size(75, 23);
			this.btnReset.TabIndex = 0;
			this.btnReset.Text = "Reset";
			this.btnReset.UseVisualStyleBackColor = true;
			this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
			// 
			// chkEmphasizeLine
			// 
			this.chkEmphasizeLine.AutoSize = true;
			this.chkEmphasizeLine.Checked = true;
			this.chkEmphasizeLine.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkEmphasizeLine.Location = new System.Drawing.Point(12, 243);
			this.chkEmphasizeLine.Name = "chkEmphasizeLine";
			this.chkEmphasizeLine.Size = new System.Drawing.Size(96, 17);
			this.chkEmphasizeLine.TabIndex = 2;
			this.chkEmphasizeLine.Text = "Emphasize line";
			this.chkEmphasizeLine.UseVisualStyleBackColor = true;
			// 
			// chkShowMagnified
			// 
			this.chkShowMagnified.AutoSize = true;
			this.chkShowMagnified.Location = new System.Drawing.Point(12, 266);
			this.chkShowMagnified.Name = "chkShowMagnified";
			this.chkShowMagnified.Size = new System.Drawing.Size(101, 17);
			this.chkShowMagnified.TabIndex = 3;
			this.chkShowMagnified.Text = "Show magnified";
			this.chkShowMagnified.UseVisualStyleBackColor = true;
			this.chkShowMagnified.CheckedChanged += new System.EventHandler(this.chkShowMagnified_CheckChanged);
			// 
			// tbTracerMarker
			// 
			this.tbTracerMarker.Location = new System.Drawing.Point(16, 64);
			this.tbTracerMarker.Name = "tbTracerMarker";
			this.tbTracerMarker.Size = new System.Drawing.Size(100, 20);
			this.tbTracerMarker.TabIndex = 4;
			this.tbTracerMarker.Tag = "0";
			this.tbTracerMarker.TextChanged += new System.EventHandler(this.tbMarkerName_TextChanged);
			// 
			// tbRefMarker
			// 
			this.tbRefMarker.Location = new System.Drawing.Point(12, 112);
			this.tbRefMarker.Name = "tbRefMarker";
			this.tbRefMarker.Size = new System.Drawing.Size(100, 20);
			this.tbRefMarker.TabIndex = 5;
			this.tbRefMarker.Tag = "1";
			this.tbRefMarker.TextChanged += new System.EventHandler(this.tbMarkerName_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(22, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(76, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Tracer marker:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 96);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(95, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Reference marker:";
			// 
			// chkTrace
			// 
			this.chkTrace.Appearance = System.Windows.Forms.Appearance.Button;
			this.chkTrace.AutoSize = true;
			this.chkTrace.Location = new System.Drawing.Point(12, 138);
			this.chkTrace.Name = "chkTrace";
			this.chkTrace.Size = new System.Drawing.Size(45, 23);
			this.chkTrace.TabIndex = 8;
			this.chkTrace.Text = "Trace";
			this.chkTrace.UseVisualStyleBackColor = true;
			this.chkTrace.CheckedChanged += new System.EventHandler(this.chkTrace_CheckChanged);
			// 
			// tmrCheckFOM
			// 
			this.tmrCheckFOM.Enabled = true;
			this.tmrCheckFOM.Interval = 20;
			this.tmrCheckFOM.Tick += new System.EventHandler(this.tmrCheckFOM_Tick);
			// 
			// lblInfo
			// 
			this.lblInfo.Location = new System.Drawing.Point(9, 168);
			this.lblInfo.Name = "lblInfo";
			this.lblInfo.Size = new System.Drawing.Size(127, 61);
			this.lblInfo.TabIndex = 9;
			this.lblInfo.Text = "lblInfo";
			// 
			// TraceForm
			// 
			this.ClientSize = new System.Drawing.Size(159, 290);
			this.Controls.Add(this.lblInfo);
			this.Controls.Add(this.chkTrace);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.tbRefMarker);
			this.Controls.Add(this.tbTracerMarker);
			this.Controls.Add(this.chkShowMagnified);
			this.Controls.Add(this.chkEmphasizeLine);
			this.Controls.Add(this.btnReset);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TraceForm";
			this.ShowInTaskbar = false;
			this.Text = "3D Tracer";
			this.Activated += new System.EventHandler(this.Form_Activate);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TraceForm_FormClosing);
			this.Load += new System.EventHandler(this.Form_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.CheckBox chkEmphasizeLine;
        private System.Windows.Forms.CheckBox chkShowMagnified;
        private System.Windows.Forms.TextBox tbTracerMarker;
        private System.Windows.Forms.TextBox tbRefMarker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkTrace;
		private System.Windows.Forms.Timer tmrCheckFOM;
		private System.Windows.Forms.Label lblInfo;
    }
}