namespace MicronTrackerVerification
{
    partial class PositionRecorderForm
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
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PositionRecorderForm));
          this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
          this.tbMsecs = new System.Windows.Forms.TextBox();
          this.timerCheckFile = new System.Windows.Forms.Timer(this.components);
          this.btnClose = new System.Windows.Forms.Button();
          this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
          this.chkRecordByFile = new System.Windows.Forms.CheckBox();
          this.groupBox1 = new System.Windows.Forms.GroupBox();
          this.chkCloudPosition = new System.Windows.Forms.CheckBox();
          this.chkTemperatureRate = new System.Windows.Forms.CheckBox();
          this.chkMarkerAngularPositionInRefSpace = new System.Windows.Forms.CheckBox();
          this.chkTooltipAngularPositionInRefSpace = new System.Windows.Forms.CheckBox();
          this.chkTooltipPositionRefSpace = new System.Windows.Forms.CheckBox();
          this.chkMarkerPositionRefSpace = new System.Windows.Forms.CheckBox();
          this.chkMarkerAngularPosition = new System.Windows.Forms.CheckBox();
          this.chkTemperature = new System.Windows.Forms.CheckBox();
          this.chkHazards = new System.Windows.Forms.CheckBox();
          this.chkDistanceStats = new System.Windows.Forms.CheckBox();
          this.chkExposure = new System.Windows.Forms.CheckBox();
          this.chkXPoints = new System.Windows.Forms.CheckBox();
          this.chkMarkerRotation = new System.Windows.Forms.CheckBox();
          this.chkTooltipPosition = new System.Windows.Forms.CheckBox();
          this.chkMarkerPosition = new System.Windows.Forms.CheckBox();
          this.chkTimeStamp = new System.Windows.Forms.CheckBox();
          this.btnSave = new System.Windows.Forms.Button();
          this.btnRecordPose = new System.Windows.Forms.Button();
          this.cboRecordingCondition = new System.Windows.Forms.ComboBox();
          this.chkRecordStream = new System.Windows.Forms.CheckBox();
          this.chkMultiFiles = new System.Windows.Forms.CheckBox();
          this.label4 = new System.Windows.Forms.Label();
          this.lblRecordsCount = new System.Windows.Forms.Label();
          this.label3 = new System.Windows.Forms.Label();
          this.radiob2markers = new System.Windows.Forms.RadioButton();
          this.rtxtStats = new System.Windows.Forms.RichTextBox();
          this.radiobTooltip = new System.Windows.Forms.RadioButton();
          this.label2 = new System.Windows.Forms.Label();
          this.label1 = new System.Windows.Forms.Label();
          this.groupBox2 = new System.Windows.Forms.GroupBox();
          this.btnReset = new System.Windows.Forms.Button();
          this.cbMarkerSelector = new System.Windows.Forms.ComboBox();
          this.groupBox1.SuspendLayout();
          this.groupBox2.SuspendLayout();
          this.SuspendLayout();
          // 
          // tbMsecs
          // 
          this.tbMsecs.Location = new System.Drawing.Point(50, 124);
          this.tbMsecs.Name = "tbMsecs";
          this.tbMsecs.Size = new System.Drawing.Size(31, 20);
          this.tbMsecs.TabIndex = 51;
          this.tbMsecs.Text = "50";
          // 
          // btnClose
          // 
          this.btnClose.Location = new System.Drawing.Point(173, 649);
          this.btnClose.Name = "btnClose";
          this.btnClose.Size = new System.Drawing.Size(44, 23);
          this.btnClose.TabIndex = 41;
          this.btnClose.Text = "Done";
          this.btnClose.UseVisualStyleBackColor = true;
          this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
          // 
          // openFileDialog1
          // 
          this.openFileDialog1.FileName = "openFileDialog1";
          // 
          // chkRecordByFile
          // 
          this.chkRecordByFile.Appearance = System.Windows.Forms.Appearance.Button;
          this.chkRecordByFile.AutoSize = true;
          this.chkRecordByFile.Location = new System.Drawing.Point(175, 123);
          this.chkRecordByFile.Name = "chkRecordByFile";
          this.chkRecordByFile.Size = new System.Drawing.Size(44, 23);
          this.chkRecordByFile.TabIndex = 53;
          this.chkRecordByFile.Text = "by file";
          this.chkRecordByFile.UseVisualStyleBackColor = true;
          this.chkRecordByFile.CheckedChanged += new System.EventHandler(this.chkRecordByFile_CheckedChanged);
          // 
          // groupBox1
          // 
          this.groupBox1.Controls.Add(this.chkCloudPosition);
          this.groupBox1.Controls.Add(this.chkTemperatureRate);
          this.groupBox1.Controls.Add(this.chkMarkerAngularPositionInRefSpace);
          this.groupBox1.Controls.Add(this.chkTooltipAngularPositionInRefSpace);
          this.groupBox1.Controls.Add(this.chkTooltipPositionRefSpace);
          this.groupBox1.Controls.Add(this.chkMarkerPositionRefSpace);
          this.groupBox1.Controls.Add(this.chkMarkerAngularPosition);
          this.groupBox1.Controls.Add(this.chkTemperature);
          this.groupBox1.Controls.Add(this.chkHazards);
          this.groupBox1.Controls.Add(this.chkDistanceStats);
          this.groupBox1.Controls.Add(this.chkExposure);
          this.groupBox1.Controls.Add(this.chkXPoints);
          this.groupBox1.Controls.Add(this.chkMarkerRotation);
          this.groupBox1.Controls.Add(this.chkTooltipPosition);
          this.groupBox1.Controls.Add(this.chkMarkerPosition);
          this.groupBox1.Controls.Add(this.chkTimeStamp);
          this.groupBox1.Controls.Add(this.btnSave);
          this.groupBox1.Location = new System.Drawing.Point(17, 314);
          this.groupBox1.Name = "groupBox1";
          this.groupBox1.Size = new System.Drawing.Size(200, 329);
          this.groupBox1.TabIndex = 43;
          this.groupBox1.TabStop = false;
          this.groupBox1.Text = "Save records";
          // 
          // chkCloudPosition
          // 
          this.chkCloudPosition.AutoSize = true;
          this.chkCloudPosition.Location = new System.Drawing.Point(18, 60);
          this.chkCloudPosition.Name = "chkCloudPosition";
          this.chkCloudPosition.Size = new System.Drawing.Size(120, 17);
          this.chkCloudPosition.TabIndex = 20;
          this.chkCloudPosition.Text = "Cloud origin position";
          this.chkCloudPosition.UseVisualStyleBackColor = true;
          // 
          // chkTemperatureRate
          // 
          this.chkTemperatureRate.AutoSize = true;
          this.chkTemperatureRate.Location = new System.Drawing.Point(18, 281);
          this.chkTemperatureRate.Name = "chkTemperatureRate";
          this.chkTemperatureRate.Size = new System.Drawing.Size(142, 17);
          this.chkTemperatureRate.TabIndex = 19;
          this.chkTemperatureRate.Text = "Camera temperature rate";
          this.chkTemperatureRate.UseVisualStyleBackColor = true;
          // 
          // chkMarkerAngularPositionInRefSpace
          // 
          this.chkMarkerAngularPositionInRefSpace.AutoSize = true;
          this.chkMarkerAngularPositionInRefSpace.Location = new System.Drawing.Point(18, 145);
          this.chkMarkerAngularPositionInRefSpace.Name = "chkMarkerAngularPositionInRefSpace";
          this.chkMarkerAngularPositionInRefSpace.Size = new System.Drawing.Size(144, 17);
          this.chkMarkerAngularPositionInRefSpace.TabIndex = 18;
          this.chkMarkerAngularPositionInRefSpace.Text = "Marker Angle in Ref (rad)";
          this.chkMarkerAngularPositionInRefSpace.UseVisualStyleBackColor = true;
          // 
          // chkTooltipAngularPositionInRefSpace
          // 
          this.chkTooltipAngularPositionInRefSpace.AutoSize = true;
          this.chkTooltipAngularPositionInRefSpace.Location = new System.Drawing.Point(18, 162);
          this.chkTooltipAngularPositionInRefSpace.Name = "chkTooltipAngularPositionInRefSpace";
          this.chkTooltipAngularPositionInRefSpace.Size = new System.Drawing.Size(143, 17);
          this.chkTooltipAngularPositionInRefSpace.TabIndex = 17;
          this.chkTooltipAngularPositionInRefSpace.Text = "Tooltip Angle in Ref (rad)";
          this.chkTooltipAngularPositionInRefSpace.UseVisualStyleBackColor = true;
          // 
          // chkTooltipPositionRefSpace
          // 
          this.chkTooltipPositionRefSpace.AutoSize = true;
          this.chkTooltipPositionRefSpace.Location = new System.Drawing.Point(18, 111);
          this.chkTooltipPositionRefSpace.Name = "chkTooltipPositionRefSpace";
          this.chkTooltipPositionRefSpace.Size = new System.Drawing.Size(163, 17);
          this.chkTooltipPositionRefSpace.TabIndex = 16;
          this.chkTooltipPositionRefSpace.Text = "Tooltip position in Ref. space";
          this.chkTooltipPositionRefSpace.UseVisualStyleBackColor = true;
          // 
          // chkMarkerPositionRefSpace
          // 
          this.chkMarkerPositionRefSpace.AutoSize = true;
          this.chkMarkerPositionRefSpace.Location = new System.Drawing.Point(18, 94);
          this.chkMarkerPositionRefSpace.Name = "chkMarkerPositionRefSpace";
          this.chkMarkerPositionRefSpace.Size = new System.Drawing.Size(153, 17);
          this.chkMarkerPositionRefSpace.TabIndex = 15;
          this.chkMarkerPositionRefSpace.Text = "Marker origin in Ref. space";
          this.chkMarkerPositionRefSpace.UseVisualStyleBackColor = true;
          // 
          // chkMarkerAngularPosition
          // 
          this.chkMarkerAngularPosition.AutoSize = true;
          this.chkMarkerAngularPosition.Checked = true;
          this.chkMarkerAngularPosition.CheckState = System.Windows.Forms.CheckState.Checked;
          this.chkMarkerAngularPosition.Location = new System.Drawing.Point(18, 128);
          this.chkMarkerAngularPosition.Name = "chkMarkerAngularPosition";
          this.chkMarkerAngularPosition.Size = new System.Drawing.Size(161, 17);
          this.chkMarkerAngularPosition.TabIndex = 14;
          this.chkMarkerAngularPosition.Text = "Marker Angular position (rad)";
          this.chkMarkerAngularPosition.UseVisualStyleBackColor = true;
          // 
          // chkTemperature
          // 
          this.chkTemperature.AutoSize = true;
          this.chkTemperature.Location = new System.Drawing.Point(18, 264);
          this.chkTemperature.Name = "chkTemperature";
          this.chkTemperature.Size = new System.Drawing.Size(121, 17);
          this.chkTemperature.TabIndex = 13;
          this.chkTemperature.Text = "Camera temperature";
          this.chkTemperature.UseVisualStyleBackColor = true;
          // 
          // chkHazards
          // 
          this.chkHazards.AutoSize = true;
          this.chkHazards.Location = new System.Drawing.Point(18, 247);
          this.chkHazards.Name = "chkHazards";
          this.chkHazards.Size = new System.Drawing.Size(65, 17);
          this.chkHazards.TabIndex = 12;
          this.chkHazards.Text = "Hazards";
          this.chkHazards.UseVisualStyleBackColor = true;
          // 
          // chkDistanceStats
          // 
          this.chkDistanceStats.AutoSize = true;
          this.chkDistanceStats.Location = new System.Drawing.Point(18, 230);
          this.chkDistanceStats.Name = "chkDistanceStats";
          this.chkDistanceStats.Size = new System.Drawing.Size(108, 17);
          this.chkDistanceStats.TabIndex = 11;
          this.chkDistanceStats.Text = "XP distance stats";
          this.chkDistanceStats.UseVisualStyleBackColor = true;
          // 
          // chkExposure
          // 
          this.chkExposure.AutoSize = true;
          this.chkExposure.Location = new System.Drawing.Point(18, 213);
          this.chkExposure.Name = "chkExposure";
          this.chkExposure.Size = new System.Drawing.Size(70, 17);
          this.chkExposure.TabIndex = 10;
          this.chkExposure.Text = "Exposure";
          this.chkExposure.UseVisualStyleBackColor = true;
          // 
          // chkXPoints
          // 
          this.chkXPoints.AutoSize = true;
          this.chkXPoints.Location = new System.Drawing.Point(18, 196);
          this.chkXPoints.Name = "chkXPoints";
          this.chkXPoints.Size = new System.Drawing.Size(100, 17);
          this.chkXPoints.TabIndex = 9;
          this.chkXPoints.Text = "Facets: XPoints";
          this.chkXPoints.UseVisualStyleBackColor = true;
          // 
          // chkMarkerRotation
          // 
          this.chkMarkerRotation.AutoSize = true;
          this.chkMarkerRotation.Location = new System.Drawing.Point(18, 179);
          this.chkMarkerRotation.Name = "chkMarkerRotation";
          this.chkMarkerRotation.Size = new System.Drawing.Size(144, 17);
          this.chkMarkerRotation.TabIndex = 8;
          this.chkMarkerRotation.Text = "Marker 3D rotation matrix";
          this.chkMarkerRotation.UseVisualStyleBackColor = true;
          // 
          // chkTooltipPosition
          // 
          this.chkTooltipPosition.AutoSize = true;
          this.chkTooltipPosition.Location = new System.Drawing.Point(18, 77);
          this.chkTooltipPosition.Name = "chkTooltipPosition";
          this.chkTooltipPosition.Size = new System.Drawing.Size(97, 17);
          this.chkTooltipPosition.TabIndex = 7;
          this.chkTooltipPosition.Text = "Tooltip position";
          this.chkTooltipPosition.UseVisualStyleBackColor = true;
          // 
          // chkMarkerPosition
          // 
          this.chkMarkerPosition.AutoSize = true;
          this.chkMarkerPosition.Checked = true;
          this.chkMarkerPosition.CheckState = System.Windows.Forms.CheckState.Checked;
          this.chkMarkerPosition.Location = new System.Drawing.Point(18, 43);
          this.chkMarkerPosition.Name = "chkMarkerPosition";
          this.chkMarkerPosition.Size = new System.Drawing.Size(126, 17);
          this.chkMarkerPosition.TabIndex = 6;
          this.chkMarkerPosition.Text = "Marker origin position";
          this.chkMarkerPosition.UseVisualStyleBackColor = true;
          // 
          // chkTimeStamp
          // 
          this.chkTimeStamp.AutoSize = true;
          this.chkTimeStamp.Checked = true;
          this.chkTimeStamp.CheckState = System.Windows.Forms.CheckState.Checked;
          this.chkTimeStamp.Location = new System.Drawing.Point(18, 26);
          this.chkTimeStamp.Name = "chkTimeStamp";
          this.chkTimeStamp.Size = new System.Drawing.Size(160, 17);
          this.chkTimeStamp.TabIndex = 5;
          this.chkTimeStamp.Text = "Time stamp (secs from reset)";
          this.chkTimeStamp.UseVisualStyleBackColor = true;
          // 
          // btnSave
          // 
          this.btnSave.Location = new System.Drawing.Point(121, 297);
          this.btnSave.Name = "btnSave";
          this.btnSave.Size = new System.Drawing.Size(75, 23);
          this.btnSave.TabIndex = 4;
          this.btnSave.Text = "Save";
          this.btnSave.UseVisualStyleBackColor = true;
          this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
          // 
          // btnRecordPose
          // 
          this.btnRecordPose.Location = new System.Drawing.Point(17, 64);
          this.btnRecordPose.Name = "btnRecordPose";
          this.btnRecordPose.Size = new System.Drawing.Size(112, 23);
          this.btnRecordPose.TabIndex = 40;
          this.btnRecordPose.Text = "Record 1 Pose";
          this.btnRecordPose.UseVisualStyleBackColor = true;
          this.btnRecordPose.Click += new System.EventHandler(this.btnRecordPose_Click);
          // 
          // cboRecordingCondition
          // 
          this.cboRecordingCondition.FormattingEnabled = true;
          this.cboRecordingCondition.Items.AddRange(new object[] {
            "at least 1 Marker",
            "at least 2 Markers",
            "exactly 1 Facet",
            "exactly 1 Marker",
            "exactly 2 Markers",
            "don\'t care"});
          this.cboRecordingCondition.Location = new System.Drawing.Point(17, 37);
          this.cboRecordingCondition.Name = "cboRecordingCondition";
          this.cboRecordingCondition.Size = new System.Drawing.Size(200, 21);
          this.cboRecordingCondition.TabIndex = 42;
          // 
          // chkRecordStream
          // 
          this.chkRecordStream.Appearance = System.Windows.Forms.Appearance.Button;
          this.chkRecordStream.AutoSize = true;
          this.chkRecordStream.Location = new System.Drawing.Point(17, 93);
          this.chkRecordStream.Name = "chkRecordStream";
          this.chkRecordStream.Size = new System.Drawing.Size(112, 23);
          this.chkRecordStream.TabIndex = 52;
          this.chkRecordStream.Text = "Record pose stream";
          this.chkRecordStream.UseVisualStyleBackColor = true;
          this.chkRecordStream.CheckedChanged += new System.EventHandler(this.chkRecordStream_CheckedChanged);
          // 
          // chkMultiFiles
          // 
          this.chkMultiFiles.AutoSize = true;
          this.chkMultiFiles.Location = new System.Drawing.Point(17, 649);
          this.chkMultiFiles.Name = "chkMultiFiles";
          this.chkMultiFiles.Size = new System.Drawing.Size(150, 17);
          this.chkMultiFiles.TabIndex = 44;
          this.chkMultiFiles.Text = "Separate multiple cameras";
          this.chkMultiFiles.UseVisualStyleBackColor = true;
          // 
          // label4
          // 
          this.label4.AutoSize = true;
          this.label4.Location = new System.Drawing.Point(88, 157);
          this.label4.Name = "label4";
          this.label4.Size = new System.Drawing.Size(81, 13);
          this.label4.TabIndex = 49;
          this.label4.Text = "Poses recorded";
          // 
          // lblRecordsCount
          // 
          this.lblRecordsCount.AutoSize = true;
          this.lblRecordsCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.lblRecordsCount.Location = new System.Drawing.Point(52, 157);
          this.lblRecordsCount.Name = "lblRecordsCount";
          this.lblRecordsCount.Size = new System.Drawing.Size(14, 13);
          this.lblRecordsCount.TabIndex = 50;
          this.lblRecordsCount.Text = "0";
          // 
          // label3
          // 
          this.label3.AutoSize = true;
          this.label3.Location = new System.Drawing.Point(81, 128);
          this.label3.Name = "label3";
          this.label3.Size = new System.Drawing.Size(84, 13);
          this.label3.TabIndex = 48;
          this.label3.Text = "msecs (no dups)";
          // 
          // radiob2markers
          // 
          this.radiob2markers.AutoSize = true;
          this.radiob2markers.Checked = true;
          this.radiob2markers.Location = new System.Drawing.Point(17, 19);
          this.radiob2markers.Name = "radiob2markers";
          this.radiob2markers.Size = new System.Drawing.Size(71, 17);
          this.radiob2markers.TabIndex = 1;
          this.radiob2markers.TabStop = true;
          this.radiob2markers.Text = "2 markers";
          this.radiob2markers.UseVisualStyleBackColor = true;
          // 
          // rtxtStats
          // 
          this.rtxtStats.Location = new System.Drawing.Point(8, 65);
          this.rtxtStats.Name = "rtxtStats";
          this.rtxtStats.Size = new System.Drawing.Size(188, 73);
          this.rtxtStats.TabIndex = 0;
          this.rtxtStats.Text = "";
          // 
          // radiobTooltip
          // 
          this.radiobTooltip.AutoSize = true;
          this.radiobTooltip.Location = new System.Drawing.Point(18, 42);
          this.radiobTooltip.Name = "radiobTooltip";
          this.radiobTooltip.Size = new System.Drawing.Size(57, 17);
          this.radiobTooltip.TabIndex = 2;
          this.radiobTooltip.Text = "Tooltip";
          this.radiobTooltip.UseVisualStyleBackColor = true;
          // 
          // label2
          // 
          this.label2.AutoSize = true;
          this.label2.Location = new System.Drawing.Point(17, 128);
          this.label2.Name = "label2";
          this.label2.Size = new System.Drawing.Size(33, 13);
          this.label2.TabIndex = 47;
          this.label2.Text = "every";
          // 
          // label1
          // 
          this.label1.AutoSize = true;
          this.label1.Location = new System.Drawing.Point(17, 21);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(148, 13);
          this.label1.TabIndex = 46;
          this.label1.Text = "Record only when recognized";
          // 
          // groupBox2
          // 
          this.groupBox2.Controls.Add(this.cbMarkerSelector);
          this.groupBox2.Controls.Add(this.radiobTooltip);
          this.groupBox2.Controls.Add(this.radiob2markers);
          this.groupBox2.Controls.Add(this.rtxtStats);
          this.groupBox2.Location = new System.Drawing.Point(17, 173);
          this.groupBox2.Name = "groupBox2";
          this.groupBox2.Size = new System.Drawing.Size(200, 137);
          this.groupBox2.TabIndex = 45;
          this.groupBox2.TabStop = false;
          this.groupBox2.Text = "Statistics";
          // 
          // btnReset
          // 
          this.btnReset.Location = new System.Drawing.Point(132, 93);
          this.btnReset.Name = "btnReset";
          this.btnReset.Size = new System.Drawing.Size(87, 23);
          this.btnReset.TabIndex = 39;
          this.btnReset.Text = "Reset";
          this.btnReset.UseVisualStyleBackColor = true;
          this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
          // 
          // cbMarkerSelector
          // 
          this.cbMarkerSelector.FormattingEnabled = true;
          this.cbMarkerSelector.Location = new System.Drawing.Point(104, 42);
          this.cbMarkerSelector.Name = "cbMarkerSelector";
          this.cbMarkerSelector.Size = new System.Drawing.Size(90, 21);
          this.cbMarkerSelector.TabIndex = 35;
          // 
          // PositionRecorderForm
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(232, 682);
          this.Controls.Add(this.tbMsecs);
          this.Controls.Add(this.btnClose);
          this.Controls.Add(this.chkRecordByFile);
          this.Controls.Add(this.groupBox1);
          this.Controls.Add(this.btnRecordPose);
          this.Controls.Add(this.cboRecordingCondition);
          this.Controls.Add(this.chkRecordStream);
          this.Controls.Add(this.chkMultiFiles);
          this.Controls.Add(this.label4);
          this.Controls.Add(this.lblRecordsCount);
          this.Controls.Add(this.label3);
          this.Controls.Add(this.label2);
          this.Controls.Add(this.label1);
          this.Controls.Add(this.groupBox2);
          this.Controls.Add(this.btnReset);
          this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
          this.MaximizeBox = false;
          this.MinimizeBox = false;
          this.Name = "PositionRecorderForm";
          this.ShowInTaskbar = false;
          this.Text = "Pose Recorder";
          this.Load += new System.EventHandler(this.PositionRecorderForm_Load);
          this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PositionRecorderForm_FormClosing);
          this.groupBox1.ResumeLayout(false);
          this.groupBox1.PerformLayout();
          this.groupBox2.ResumeLayout(false);
          this.groupBox2.PerformLayout();
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TextBox tbMsecs;
        private System.Windows.Forms.Timer timerCheckFile;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.CheckBox chkRecordByFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkCloudPosition;
        private System.Windows.Forms.CheckBox chkTemperatureRate;
        private System.Windows.Forms.CheckBox chkMarkerAngularPositionInRefSpace;
        private System.Windows.Forms.CheckBox chkTooltipAngularPositionInRefSpace;
        private System.Windows.Forms.CheckBox chkTooltipPositionRefSpace;
        private System.Windows.Forms.CheckBox chkMarkerPositionRefSpace;
        private System.Windows.Forms.CheckBox chkMarkerAngularPosition;
        private System.Windows.Forms.CheckBox chkTemperature;
        private System.Windows.Forms.CheckBox chkHazards;
        private System.Windows.Forms.CheckBox chkDistanceStats;
        private System.Windows.Forms.CheckBox chkExposure;
        private System.Windows.Forms.CheckBox chkXPoints;
        private System.Windows.Forms.CheckBox chkMarkerRotation;
        private System.Windows.Forms.CheckBox chkTooltipPosition;
        private System.Windows.Forms.CheckBox chkMarkerPosition;
        private System.Windows.Forms.CheckBox chkTimeStamp;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnRecordPose;
        private System.Windows.Forms.ComboBox cboRecordingCondition;
        private System.Windows.Forms.CheckBox chkRecordStream;
        private System.Windows.Forms.CheckBox chkMultiFiles;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblRecordsCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radiob2markers;
        private System.Windows.Forms.RichTextBox rtxtStats;
        private System.Windows.Forms.RadioButton radiobTooltip;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnReset;
        internal System.Windows.Forms.ComboBox cbMarkerSelector;


    }
}