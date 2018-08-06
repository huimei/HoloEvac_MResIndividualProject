namespace MicronTrackerCalibration
{

  partial class MainForm : System.Windows.Forms.Form
  {

    //Form overrides dispose to clean up the component list.
    [System.Diagnostics.DebuggerNonUserCode()]
    protected override void Dispose(bool disposing)
    {
      if (disposing && components != null)
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    //Required by the Windows Form Designer

    private System.ComponentModel.IContainer components;
    //NOTE: The following procedure is required by the Windows Form Designer
    //It can be modified using the Windows Form Designer.  
    //Do not modify it using the code editor.
    [System.Diagnostics.DebuggerStepThrough()]
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.vsGain = new System.Windows.Forms.VScrollBar();
            this.vsShutter = new System.Windows.Forms.VScrollBar();
            this.tbShutter = new System.Windows.Forms.TextBox();
            this.mnuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tbGainF = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.videoRecorderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRecordStop = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSaveLeftImage = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCalibrationInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMTCAPIDocumentation = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.labelCaptureDisabled = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ToolStrip2 = new System.Windows.Forms.ToolStrip();
            this.ToolStripCapture = new System.Windows.Forms.ToolStripButton();
            this.ToolStripTemplates = new System.Windows.Forms.ToolStripButton();
            this.ToolStripPoseRecorder = new System.Windows.Forms.ToolStripButton();
            this.ToolStripHDR = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSettings = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripHalfSize = new System.Windows.Forms.ToolStripButton();
            this.ToolStripCalibrationInfo = new System.Windows.Forms.ToolStripButton();
            this.ToolStripCameraInfo = new System.Windows.Forms.ToolStripButton();
            this.ToolStripVideoRecorder = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripAPIDocumentation = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.PicBox2 = new System.Windows.Forms.PictureBox();
            this.picBox1 = new System.Windows.Forms.PictureBox();
            this.recordPanel = new System.Windows.Forms.Panel();
            this.picBox0 = new System.Windows.Forms.PictureBox();
            this.panelExposure1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.timerAviSaver = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.mnuSnapShots = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPoseRecorder = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMarkerTemplates = new System.Windows.Forms.ToolStripMenuItem();
            this.displayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplayImageAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplayImageLeft = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplayImageRight = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplayImageMiddle = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHalfSize = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMirrorImage = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHistEqualize = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGammaCorrection = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuMarkersPositions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuXPointsPositions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCloudsPositions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPositionsAtRefSpace = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDistancesAngles = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMagnifiedToolTip = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuVectors = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFiducials = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDisplayConnections = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClouds = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuXPoints = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuCameraInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.mnu3DTracer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDataCapture = new System.Windows.Forms.ToolStripMenuItem();
            this.activateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.BackgroundProcessToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMarkersProcessing = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuXPointsProcessing = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCloudsProcessing = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHdrMode = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExternallyTrigger = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuStrobeSignal = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAutoExposure = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCamCamRegistration = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mmTimer = new Multimedia.Timer(this.components);
            this.panel1.SuspendLayout();
            this.ToolStrip2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBox0)).BeginInit();
            this.panelExposure1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // vsGain
            // 
            this.vsGain.Location = new System.Drawing.Point(8, 25);
            this.vsGain.Maximum = 200;
            this.vsGain.Minimum = 1;
            this.vsGain.Name = "vsGain";
            this.vsGain.Size = new System.Drawing.Size(17, 157);
            this.vsGain.SmallChange = 10;
            this.vsGain.TabIndex = 1;
            this.vsGain.Value = 10;
            this.vsGain.ValueChanged += new System.EventHandler(this.vsGain1_ValueChanged);
            // 
            // vsShutter
            // 
            this.vsShutter.Location = new System.Drawing.Point(48, 48);
            this.vsShutter.Maximum = 1000;
            this.vsShutter.Name = "vsShutter";
            this.vsShutter.Size = new System.Drawing.Size(17, 157);
            this.vsShutter.SmallChange = 10;
            this.vsShutter.TabIndex = 2;
            this.vsShutter.Value = 10;
            this.vsShutter.ValueChanged += new System.EventHandler(this.vsShutter1_ValueChanged);
            // 
            // tbShutter
            // 
            this.tbShutter.Location = new System.Drawing.Point(8, 185);
            this.tbShutter.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tbShutter.Name = "tbShutter";
            this.tbShutter.Size = new System.Drawing.Size(34, 20);
            this.tbShutter.TabIndex = 3;
            this.tbShutter.Text = "77.3";
            this.tbShutter.TextChanged += new System.EventHandler(this.tbShutter1_TextChanged);
            // 
            // mnuAbout
            // 
            this.mnuAbout.Name = "mnuAbout";
            this.mnuAbout.Size = new System.Drawing.Size(225, 22);
            this.mnuAbout.Text = "About MicronTracker...";
            this.mnuAbout.Click += new System.EventHandler(this.mnuAbout_Click);
            // 
            // tbGainF
            // 
            this.tbGainF.Location = new System.Drawing.Point(29, 25);
            this.tbGainF.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tbGainF.Name = "tbGainF";
            this.tbGainF.Size = new System.Drawing.Size(36, 20);
            this.tbGainF.TabIndex = 4;
            this.tbGainF.Text = "12.3";
            this.tbGainF.TextChanged += new System.EventHandler(this.tbGainF1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "GainF1";
            // 
            // videoRecorderToolStripMenuItem
            // 
            this.videoRecorderToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRecordStop});
            this.videoRecorderToolStripMenuItem.Name = "videoRecorderToolStripMenuItem";
            this.videoRecorderToolStripMenuItem.Size = new System.Drawing.Size(99, 22);
            this.videoRecorderToolStripMenuItem.Text = "Video Recorder";
            // 
            // mnuRecordStop
            // 
            this.mnuRecordStop.Name = "mnuRecordStop";
            this.mnuRecordStop.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
            this.mnuRecordStop.Size = new System.Drawing.Size(184, 22);
            this.mnuRecordStop.Text = "Record/ Stop";
            this.mnuRecordStop.Click += new System.EventHandler(this.mnuRecordStop_Click);
            // 
            // mnuSaveLeftImage
            // 
            this.mnuSaveLeftImage.Name = "mnuSaveLeftImage";
            this.mnuSaveLeftImage.Size = new System.Drawing.Size(220, 22);
            this.mnuSaveLeftImage.Text = "Save Left Image...";
            this.mnuSaveLeftImage.Click += new System.EventHandler(this.mnuSaveLeftImage_Click);
            // 
            // mnuCalibrationInfo
            // 
            this.mnuCalibrationInfo.Name = "mnuCalibrationInfo";
            this.mnuCalibrationInfo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.mnuCalibrationInfo.Size = new System.Drawing.Size(220, 22);
            this.mnuCalibrationInfo.Text = "Calibration Info...";
            this.mnuCalibrationInfo.Click += new System.EventHandler(this.mnuCalibrationInfo_Click);
            // 
            // mnuMTCAPIDocumentation
            // 
            this.mnuMTCAPIDocumentation.Name = "mnuMTCAPIDocumentation";
            this.mnuMTCAPIDocumentation.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.mnuMTCAPIDocumentation.Size = new System.Drawing.Size(225, 22);
            this.mnuMTCAPIDocumentation.Text = "MTC API Documentation";
            this.mnuMTCAPIDocumentation.Click += new System.EventHandler(this.mnuMTCAPIDocumentation_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMTCAPIDocumentation,
            this.ToolStripSeparator9,
            this.mnuAbout});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 22);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // ToolStripSeparator9
            // 
            this.ToolStripSeparator9.Name = "ToolStripSeparator9";
            this.ToolStripSeparator9.Size = new System.Drawing.Size(222, 6);
            // 
            // labelCaptureDisabled
            // 
            this.labelCaptureDisabled.ForeColor = System.Drawing.Color.Red;
            this.labelCaptureDisabled.Location = new System.Drawing.Point(372, 6);
            this.labelCaptureDisabled.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelCaptureDisabled.Name = "labelCaptureDisabled";
            this.labelCaptureDisabled.Size = new System.Drawing.Size(74, 33);
            this.labelCaptureDisabled.TabIndex = 11;
            this.labelCaptureDisabled.Text = "CAPTURE DISABLED";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.ToolStrip2);
            this.panel1.Controls.Add(this.PicBox2);
            this.panel1.Controls.Add(this.picBox1);
            this.panel1.Controls.Add(this.recordPanel);
            this.panel1.Controls.Add(this.picBox0);
            this.panel1.Controls.Add(this.panelExposure1);
            this.panel1.Controls.Add(this.labelCaptureDisabled);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(767, 568);
            this.panel1.TabIndex = 17;
            // 
            // ToolStrip2
            // 
            this.ToolStrip2.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.ToolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripCapture,
            this.ToolStripTemplates,
            this.ToolStripPoseRecorder,
            this.ToolStripHDR,
            this.ToolStripSettings,
            this.ToolStripSeparator1,
            this.ToolStripHalfSize,
            this.ToolStripCalibrationInfo,
            this.ToolStripCameraInfo,
            this.ToolStripVideoRecorder,
            this.ToolStripSeparator2,
            this.ToolStripAPIDocumentation,
            this.ToolStripSeparator3});
            this.ToolStrip2.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip2.Name = "ToolStrip2";
            this.ToolStrip2.Size = new System.Drawing.Size(750, 39);
            this.ToolStrip2.TabIndex = 19;
            this.ToolStrip2.Text = "ToolStrip2";
            // 
            // ToolStripCapture
            // 
            this.ToolStripCapture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripCapture.Image = global::MicronTrackerCalibration.Properties.Resources.start_icon;
            this.ToolStripCapture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripCapture.Name = "ToolStripCapture";
            this.ToolStripCapture.Size = new System.Drawing.Size(36, 36);
            this.ToolStripCapture.Text = "Capture/Freeze Frame";
            this.ToolStripCapture.Click += new System.EventHandler(this.ToolStripCapture_Click);
            // 
            // ToolStripTemplates
            // 
            this.ToolStripTemplates.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripTemplates.Image = global::MicronTrackerCalibration.Properties.Resources.New_Template;
            this.ToolStripTemplates.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripTemplates.Name = "ToolStripTemplates";
            this.ToolStripTemplates.Size = new System.Drawing.Size(36, 36);
            this.ToolStripTemplates.Text = "Templates";
            this.ToolStripTemplates.Click += new System.EventHandler(this.ToolStripTemplates_Click);
            // 
            // ToolStripPoseRecorder
            // 
            this.ToolStripPoseRecorder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripPoseRecorder.Image = global::MicronTrackerCalibration.Properties.Resources.Pose_Recorder;
            this.ToolStripPoseRecorder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripPoseRecorder.Name = "ToolStripPoseRecorder";
            this.ToolStripPoseRecorder.Size = new System.Drawing.Size(36, 36);
            this.ToolStripPoseRecorder.Text = "Pose Recorder";
            this.ToolStripPoseRecorder.Click += new System.EventHandler(this.ToolStripPoseRecorder_Click);
            // 
            // ToolStripHDR
            // 
            this.ToolStripHDR.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripHDR.Image = global::MicronTrackerCalibration.Properties.Resources.HDR;
            this.ToolStripHDR.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripHDR.Name = "ToolStripHDR";
            this.ToolStripHDR.Size = new System.Drawing.Size(36, 36);
            this.ToolStripHDR.Text = "HDR Mode";
            this.ToolStripHDR.Click += new System.EventHandler(this.ToolStripHDR_Click);
            // 
            // ToolStripSettings
            // 
            this.ToolStripSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripSettings.Image = global::MicronTrackerCalibration.Properties.Resources.settings;
            this.ToolStripSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripSettings.Name = "ToolStripSettings";
            this.ToolStripSettings.Size = new System.Drawing.Size(36, 36);
            this.ToolStripSettings.Text = "Settings";
            this.ToolStripSettings.Click += new System.EventHandler(this.ToolStripSettings_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // ToolStripHalfSize
            // 
            this.ToolStripHalfSize.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripHalfSize.Image = global::MicronTrackerCalibration.Properties.Resources.half_size_image;
            this.ToolStripHalfSize.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripHalfSize.Name = "ToolStripHalfSize";
            this.ToolStripHalfSize.Size = new System.Drawing.Size(36, 36);
            this.ToolStripHalfSize.Text = "Half/Full Size Display";
            this.ToolStripHalfSize.Click += new System.EventHandler(this.ToolStripHalfSize_Click);
            // 
            // ToolStripCalibrationInfo
            // 
            this.ToolStripCalibrationInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripCalibrationInfo.Image = global::MicronTrackerCalibration.Properties.Resources.Calib_Info_icon;
            this.ToolStripCalibrationInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripCalibrationInfo.Name = "ToolStripCalibrationInfo";
            this.ToolStripCalibrationInfo.Size = new System.Drawing.Size(36, 36);
            this.ToolStripCalibrationInfo.Text = "Camera Calibration Information";
            this.ToolStripCalibrationInfo.Click += new System.EventHandler(this.ToolStripCalibrationInfo_Click);
            // 
            // ToolStripCameraInfo
            // 
            this.ToolStripCameraInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripCameraInfo.Image = global::MicronTrackerCalibration.Properties.Resources.Camera_Info_icon;
            this.ToolStripCameraInfo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripCameraInfo.Name = "ToolStripCameraInfo";
            this.ToolStripCameraInfo.Size = new System.Drawing.Size(36, 36);
            this.ToolStripCameraInfo.Text = "Camera Info";
            this.ToolStripCameraInfo.Click += new System.EventHandler(this.ToolStripCameraInfo_Click);
            // 
            // ToolStripVideoRecorder
            // 
            this.ToolStripVideoRecorder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripVideoRecorder.Image = global::MicronTrackerCalibration.Properties.Resources.video_recorder;
            this.ToolStripVideoRecorder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripVideoRecorder.Name = "ToolStripVideoRecorder";
            this.ToolStripVideoRecorder.Size = new System.Drawing.Size(36, 36);
            this.ToolStripVideoRecorder.Text = "Video recorder";
            this.ToolStripVideoRecorder.Click += new System.EventHandler(this.ToolStripVideoRecorder_Click);
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // ToolStripAPIDocumentation
            // 
            this.ToolStripAPIDocumentation.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ToolStripAPIDocumentation.Image = global::MicronTrackerCalibration.Properties.Resources.Help_icon;
            this.ToolStripAPIDocumentation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ToolStripAPIDocumentation.Name = "ToolStripAPIDocumentation";
            this.ToolStripAPIDocumentation.Size = new System.Drawing.Size(36, 36);
            this.ToolStripAPIDocumentation.Text = "MTC API Documentation";
            this.ToolStripAPIDocumentation.Click += new System.EventHandler(this.ToolStripAPIDocumentation_Click);
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(6, 39);
            // 
            // PicBox2
            // 
            this.PicBox2.Location = new System.Drawing.Point(365, 426);
            this.PicBox2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.PicBox2.Name = "PicBox2";
            this.PicBox2.Size = new System.Drawing.Size(359, 206);
            this.PicBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PicBox2.TabIndex = 17;
            this.PicBox2.TabStop = false;
            // 
            // picBox1
            // 
            this.picBox1.Location = new System.Drawing.Point(2, 285);
            this.picBox1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.picBox1.Name = "picBox1";
            this.picBox1.Size = new System.Drawing.Size(359, 206);
            this.picBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picBox1.TabIndex = 9;
            this.picBox1.TabStop = false;
            // 
            // recordPanel
            // 
            this.recordPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.recordPanel.Location = new System.Drawing.Point(326, 16);
            this.recordPanel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.recordPanel.Name = "recordPanel";
            this.recordPanel.Size = new System.Drawing.Size(26, 23);
            this.recordPanel.TabIndex = 16;
            this.recordPanel.Visible = false;
            // 
            // picBox0
            // 
            this.picBox0.Location = new System.Drawing.Point(2, -2);
            this.picBox0.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.picBox0.Name = "picBox0";
            this.picBox0.Size = new System.Drawing.Size(359, 274);
            this.picBox0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.picBox0.TabIndex = 8;
            this.picBox0.TabStop = false;
            this.picBox0.Click += new System.EventHandler(this.picBox0_Click);
            // 
            // panelExposure1
            // 
            this.panelExposure1.Controls.Add(this.vsGain);
            this.panelExposure1.Controls.Add(this.vsShutter);
            this.panelExposure1.Controls.Add(this.tbShutter);
            this.panelExposure1.Controls.Add(this.tbGainF);
            this.panelExposure1.Controls.Add(this.label1);
            this.panelExposure1.Controls.Add(this.label2);
            this.panelExposure1.Location = new System.Drawing.Point(375, 51);
            this.panelExposure1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.panelExposure1.Name = "panelExposure1";
            this.panelExposure1.Size = new System.Drawing.Size(74, 227);
            this.panelExposure1.TabIndex = 12;
            this.panelExposure1.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 208);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Shutter1";
            // 
            // timerAviSaver
            // 
            this.timerAviSaver.Interval = 500;
            // 
            // mnuSnapShots
            // 
            this.mnuSnapShots.Name = "mnuSnapShots";
            this.mnuSnapShots.Size = new System.Drawing.Size(222, 22);
            this.mnuSnapShots.Text = "Snap Shots";
            this.mnuSnapShots.Click += new System.EventHandler(this.mnuSnapShots_Click);
            // 
            // mnuPoseRecorder
            // 
            this.mnuPoseRecorder.Name = "mnuPoseRecorder";
            this.mnuPoseRecorder.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.mnuPoseRecorder.Size = new System.Drawing.Size(222, 22);
            this.mnuPoseRecorder.Text = "Pose Recorder";
            this.mnuPoseRecorder.Click += new System.EventHandler(this.mnuPoseRecorder_Click);
            // 
            // mnuMarkerTemplates
            // 
            this.mnuMarkerTemplates.Name = "mnuMarkerTemplates";
            this.mnuMarkerTemplates.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.T)));
            this.mnuMarkerTemplates.Size = new System.Drawing.Size(222, 22);
            this.mnuMarkerTemplates.Text = "Templates";
            this.mnuMarkerTemplates.Click += new System.EventHandler(this.mnuMarkerTemplates_Click);
            // 
            // displayToolStripMenuItem
            // 
            this.displayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDisplayImageAll,
            this.mnuDisplayImageLeft,
            this.mnuDisplayImageRight,
            this.mnuDisplayImageMiddle,
            this.mnuHalfSize,
            this.mnuMirrorImage,
            this.mnuHistEqualize,
            this.mnuGammaCorrection,
            this.ToolStripSeparator4,
            this.mnuMarkersPositions,
            this.mnuXPointsPositions,
            this.mnuCloudsPositions,
            this.mnuPositionsAtRefSpace,
            this.mnuDistancesAngles,
            this.mnuMagnifiedToolTip,
            this.ToolStripSeparator6,
            this.mnuVectors,
            this.mnuFiducials,
            this.mnuDisplayConnections,
            this.mnuClouds,
            this.mnuXPoints,
            this.ToolStripSeparator5,
            this.mnuCameraInfo,
            this.mnuCalibrationInfo,
            this.mnuSaveLeftImage});
            this.displayToolStripMenuItem.Name = "displayToolStripMenuItem";
            this.displayToolStripMenuItem.Size = new System.Drawing.Size(57, 22);
            this.displayToolStripMenuItem.Text = "Display";
            // 
            // mnuDisplayImageAll
            // 
            this.mnuDisplayImageAll.Name = "mnuDisplayImageAll";
            this.mnuDisplayImageAll.Size = new System.Drawing.Size(220, 22);
            this.mnuDisplayImageAll.Text = "Show All Images";
            this.mnuDisplayImageAll.Click += new System.EventHandler(this.mnuDisplayImageAll_Click);
            // 
            // mnuDisplayImageLeft
            // 
            this.mnuDisplayImageLeft.Name = "mnuDisplayImageLeft";
            this.mnuDisplayImageLeft.Size = new System.Drawing.Size(220, 22);
            this.mnuDisplayImageLeft.Text = "Left Image";
            this.mnuDisplayImageLeft.Click += new System.EventHandler(this.mnuDisplayImageLeft_Click);
            // 
            // mnuDisplayImageRight
            // 
            this.mnuDisplayImageRight.Name = "mnuDisplayImageRight";
            this.mnuDisplayImageRight.Size = new System.Drawing.Size(220, 22);
            this.mnuDisplayImageRight.Text = "Right Image";
            this.mnuDisplayImageRight.Click += new System.EventHandler(this.mnuDisplayImageRight_Click);
            // 
            // mnuDisplayImageMiddle
            // 
            this.mnuDisplayImageMiddle.Name = "mnuDisplayImageMiddle";
            this.mnuDisplayImageMiddle.Size = new System.Drawing.Size(220, 22);
            this.mnuDisplayImageMiddle.Text = "Middle Image";
            this.mnuDisplayImageMiddle.Click += new System.EventHandler(this.mnuDisplayImageMiddle_Click);
            // 
            // mnuHalfSize
            // 
            this.mnuHalfSize.Name = "mnuHalfSize";
            this.mnuHalfSize.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.mnuHalfSize.Size = new System.Drawing.Size(220, 22);
            this.mnuHalfSize.Text = "1/2 Size Image";
            this.mnuHalfSize.Click += new System.EventHandler(this.mnuHalfSize_Click);
            // 
            // mnuMirrorImage
            // 
            this.mnuMirrorImage.Name = "mnuMirrorImage";
            this.mnuMirrorImage.Size = new System.Drawing.Size(220, 22);
            this.mnuMirrorImage.Text = "Mirror Image";
            this.mnuMirrorImage.Click += new System.EventHandler(this.mnuMirrorImage_Click);
            // 
            // mnuHistEqualize
            // 
            this.mnuHistEqualize.Name = "mnuHistEqualize";
            this.mnuHistEqualize.Size = new System.Drawing.Size(220, 22);
            this.mnuHistEqualize.Text = "Hist. Equalizer";
            this.mnuHistEqualize.Click += new System.EventHandler(this.mnuHistEqualize_Click);
            // 
            // mnuGammaCorrection
            // 
            this.mnuGammaCorrection.Name = "mnuGammaCorrection";
            this.mnuGammaCorrection.Size = new System.Drawing.Size(220, 22);
            this.mnuGammaCorrection.Text = "Gamma Correction";
            this.mnuGammaCorrection.Click += new System.EventHandler(this.mnuGammaCorrection_Click);
            // 
            // ToolStripSeparator4
            // 
            this.ToolStripSeparator4.Name = "ToolStripSeparator4";
            this.ToolStripSeparator4.Size = new System.Drawing.Size(217, 6);
            // 
            // mnuMarkersPositions
            // 
            this.mnuMarkersPositions.Checked = true;
            this.mnuMarkersPositions.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuMarkersPositions.Name = "mnuMarkersPositions";
            this.mnuMarkersPositions.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.mnuMarkersPositions.Size = new System.Drawing.Size(220, 22);
            this.mnuMarkersPositions.Text = "Markers Positions";
            this.mnuMarkersPositions.Click += new System.EventHandler(this.mnuMarkersPositions_Click);
            // 
            // mnuXPointsPositions
            // 
            this.mnuXPointsPositions.Name = "mnuXPointsPositions";
            this.mnuXPointsPositions.Size = new System.Drawing.Size(220, 22);
            this.mnuXPointsPositions.Text = "XPoints Positions ";
            this.mnuXPointsPositions.Click += new System.EventHandler(this.mnuXPointsPositions_Click);
            // 
            // mnuCloudsPositions
            // 
            this.mnuCloudsPositions.Name = "mnuCloudsPositions";
            this.mnuCloudsPositions.Size = new System.Drawing.Size(220, 22);
            this.mnuCloudsPositions.Text = "Clouds Positions";
            this.mnuCloudsPositions.Click += new System.EventHandler(this.mnuCloudsPositions_Click);
            // 
            // mnuPositionsAtRefSpace
            // 
            this.mnuPositionsAtRefSpace.Name = "mnuPositionsAtRefSpace";
            this.mnuPositionsAtRefSpace.Size = new System.Drawing.Size(220, 22);
            this.mnuPositionsAtRefSpace.Text = "Positions at reference space";
            this.mnuPositionsAtRefSpace.Click += new System.EventHandler(this.mnuMarkersPositionsAtRefSpace_Click);
            // 
            // mnuDistancesAngles
            // 
            this.mnuDistancesAngles.Name = "mnuDistancesAngles";
            this.mnuDistancesAngles.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mnuDistancesAngles.Size = new System.Drawing.Size(220, 22);
            this.mnuDistancesAngles.Text = "Distances, Angles";
            this.mnuDistancesAngles.Click += new System.EventHandler(this.mnuDistancesAngles_Click);
            // 
            // mnuMagnifiedToolTip
            // 
            this.mnuMagnifiedToolTip.Checked = true;
            this.mnuMagnifiedToolTip.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuMagnifiedToolTip.Name = "mnuMagnifiedToolTip";
            this.mnuMagnifiedToolTip.Size = new System.Drawing.Size(220, 22);
            this.mnuMagnifiedToolTip.Text = "Magnified Tool Tip";
            this.mnuMagnifiedToolTip.Click += new System.EventHandler(this.mnuMagnifiedToolTip_Click);
            // 
            // ToolStripSeparator6
            // 
            this.ToolStripSeparator6.Name = "ToolStripSeparator6";
            this.ToolStripSeparator6.Size = new System.Drawing.Size(217, 6);
            // 
            // mnuVectors
            // 
            this.mnuVectors.Name = "mnuVectors";
            this.mnuVectors.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.mnuVectors.Size = new System.Drawing.Size(220, 22);
            this.mnuVectors.Text = "Display Vectors";
            this.mnuVectors.Click += new System.EventHandler(this.mnuVectors_Click);
            // 
            // mnuFiducials
            // 
            this.mnuFiducials.Name = "mnuFiducials";
            this.mnuFiducials.Size = new System.Drawing.Size(220, 22);
            this.mnuFiducials.Text = "Display Fiducials";
            this.mnuFiducials.Click += new System.EventHandler(this.mnuFiducials_Click);
            // 
            // mnuDisplayConnections
            // 
            this.mnuDisplayConnections.Name = "mnuDisplayConnections";
            this.mnuDisplayConnections.Size = new System.Drawing.Size(220, 22);
            this.mnuDisplayConnections.Text = "Display Connections";
            this.mnuDisplayConnections.Visible = false;
            this.mnuDisplayConnections.Click += new System.EventHandler(this.mnuDisplayConnections_Click);
            // 
            // mnuClouds
            // 
            this.mnuClouds.Name = "mnuClouds";
            this.mnuClouds.Size = new System.Drawing.Size(220, 22);
            this.mnuClouds.Text = "Display Clouds";
            this.mnuClouds.Click += new System.EventHandler(this.mnuClouds_Click);
            // 
            // mnuXPoints
            // 
            this.mnuXPoints.Name = "mnuXPoints";
            this.mnuXPoints.Size = new System.Drawing.Size(220, 22);
            this.mnuXPoints.Text = "Display XPoints";
            this.mnuXPoints.Click += new System.EventHandler(this.mnuXPoints_Click);
            // 
            // ToolStripSeparator5
            // 
            this.ToolStripSeparator5.Name = "ToolStripSeparator5";
            this.ToolStripSeparator5.Size = new System.Drawing.Size(217, 6);
            // 
            // mnuCameraInfo
            // 
            this.mnuCameraInfo.Name = "mnuCameraInfo";
            this.mnuCameraInfo.Size = new System.Drawing.Size(220, 22);
            this.mnuCameraInfo.Text = "Camera Info...";
            this.mnuCameraInfo.Click += new System.EventHandler(this.mnuCameraInfo_Click);
            // 
            // mnuSettings
            // 
            this.mnuSettings.Name = "mnuSettings";
            this.mnuSettings.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnuSettings.Size = new System.Drawing.Size(222, 22);
            this.mnuSettings.Text = "Settings...";
            this.mnuSettings.Click += new System.EventHandler(this.mnuOptions_Click);
            // 
            // mnu3DTracer
            // 
            this.mnu3DTracer.Name = "mnu3DTracer";
            this.mnu3DTracer.Size = new System.Drawing.Size(222, 22);
            this.mnu3DTracer.Text = "3D Tracer...";
            this.mnu3DTracer.Click += new System.EventHandler(this.mnu3DTracer_Click);
            // 
            // mnuDataCapture
            // 
            this.mnuDataCapture.Checked = true;
            this.mnuDataCapture.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuDataCapture.Name = "mnuDataCapture";
            this.mnuDataCapture.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.mnuDataCapture.Size = new System.Drawing.Size(222, 22);
            this.mnuDataCapture.Text = "Data Capture";
            this.mnuDataCapture.Click += new System.EventHandler(this.mnuDataCapture_Click);
            // 
            // activateToolStripMenuItem
            // 
            this.activateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDataCapture,
            this.ToolStripSeparator8,
            this.BackgroundProcessToolStripMenuItem,
            this.mnuMarkersProcessing,
            this.mnuXPointsProcessing,
            this.mnuCloudsProcessing,
            this.ToolStripSeparator7,
            this.mnuHdrMode,
            this.mnuMarkerTemplates,
            this.mnuPoseRecorder,
            this.mnuExternallyTrigger,
            this.mnuStrobeSignal,
            this.mnuAutoExposure,
            this.mnuCamCamRegistration,
            this.mnuSnapShots,
            this.mnu3DTracer,
            this.mnuSettings,
            this.ToolStripMenuItem1,
            this.ExitToolStripMenuItem});
            this.activateToolStripMenuItem.Name = "activateToolStripMenuItem";
            this.activateToolStripMenuItem.Size = new System.Drawing.Size(62, 22);
            this.activateToolStripMenuItem.Text = "Activate";
            // 
            // ToolStripSeparator8
            // 
            this.ToolStripSeparator8.Name = "ToolStripSeparator8";
            this.ToolStripSeparator8.Size = new System.Drawing.Size(219, 6);
            // 
            // BackgroundProcessToolStripMenuItem
            // 
            this.BackgroundProcessToolStripMenuItem.Name = "BackgroundProcessToolStripMenuItem";
            this.BackgroundProcessToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B)));
            this.BackgroundProcessToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.BackgroundProcessToolStripMenuItem.Text = "Background Process";
            this.BackgroundProcessToolStripMenuItem.Click += new System.EventHandler(this.BackgroundProcessToolStripMenuItem_Click);
            // 
            // mnuMarkersProcessing
            // 
            this.mnuMarkersProcessing.Checked = true;
            this.mnuMarkersProcessing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuMarkersProcessing.Name = "mnuMarkersProcessing";
            this.mnuMarkersProcessing.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.mnuMarkersProcessing.Size = new System.Drawing.Size(222, 22);
            this.mnuMarkersProcessing.Text = "Markers Processing";
            this.mnuMarkersProcessing.Click += new System.EventHandler(this.mnuMarkersProcessing_Click);
            // 
            // mnuXPointsProcessing
            // 
            this.mnuXPointsProcessing.Name = "mnuXPointsProcessing";
            this.mnuXPointsProcessing.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.mnuXPointsProcessing.Size = new System.Drawing.Size(222, 22);
            this.mnuXPointsProcessing.Text = "XPoints Processing";
            this.mnuXPointsProcessing.Click += new System.EventHandler(this.mnuXPointsProcessing_Click);
            // 
            // mnuCloudsProcessing
            // 
            this.mnuCloudsProcessing.Name = "mnuCloudsProcessing";
            this.mnuCloudsProcessing.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.mnuCloudsProcessing.Size = new System.Drawing.Size(222, 22);
            this.mnuCloudsProcessing.Text = "Clouds Processing";
            this.mnuCloudsProcessing.Click += new System.EventHandler(this.mnuCloudsProcessing_Click);
            // 
            // ToolStripSeparator7
            // 
            this.ToolStripSeparator7.Name = "ToolStripSeparator7";
            this.ToolStripSeparator7.Size = new System.Drawing.Size(219, 6);
            // 
            // mnuHdrMode
            // 
            this.mnuHdrMode.Name = "mnuHdrMode";
            this.mnuHdrMode.Size = new System.Drawing.Size(222, 22);
            this.mnuHdrMode.Text = "High Dynamic Range Mode";
            this.mnuHdrMode.Visible = false;
            this.mnuHdrMode.Click += new System.EventHandler(this.mnuHdrMode_Click);
            // 
            // mnuExternallyTrigger
            // 
            this.mnuExternallyTrigger.Name = "mnuExternallyTrigger";
            this.mnuExternallyTrigger.Size = new System.Drawing.Size(222, 22);
            this.mnuExternallyTrigger.Text = "Externally Trigger";
            this.mnuExternallyTrigger.Click += new System.EventHandler(this.mnuExternallyTrigger_Click);
            // 
            // mnuStrobeSignal
            // 
            this.mnuStrobeSignal.Name = "mnuStrobeSignal";
            this.mnuStrobeSignal.Size = new System.Drawing.Size(222, 22);
            this.mnuStrobeSignal.Text = "Strobe Signal";
            this.mnuStrobeSignal.Click += new System.EventHandler(this.mnuStrobeSignal_Click);
            // 
            // mnuAutoExposure
            // 
            this.mnuAutoExposure.Checked = true;
            this.mnuAutoExposure.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuAutoExposure.Name = "mnuAutoExposure";
            this.mnuAutoExposure.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
            this.mnuAutoExposure.Size = new System.Drawing.Size(222, 22);
            this.mnuAutoExposure.Text = "Auto Exposure";
            this.mnuAutoExposure.Click += new System.EventHandler(this.mnuAutoExposure_Click);
            // 
            // mnuCamCamRegistration
            // 
            this.mnuCamCamRegistration.Checked = true;
            this.mnuCamCamRegistration.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuCamCamRegistration.Name = "mnuCamCamRegistration";
            this.mnuCamCamRegistration.Size = new System.Drawing.Size(222, 22);
            this.mnuCamCamRegistration.Text = "Cam Cam Registration";
            this.mnuCamCamRegistration.Click += new System.EventHandler(this.mnuCamCamRegistration_Click);
            // 
            // ToolStripMenuItem1
            // 
            this.ToolStripMenuItem1.Name = "ToolStripMenuItem1";
            this.ToolStripMenuItem1.Size = new System.Drawing.Size(219, 6);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(222, 22);
            this.ExitToolStripMenuItem.Text = "Exit";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.activateToolStripMenuItem,
            this.displayToolStripMenuItem,
            this.videoRecorderToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(6, 1, 0, 1);
            this.menuStrip1.Size = new System.Drawing.Size(767, 24);
            this.menuStrip1.TabIndex = 15;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mmTimer
            // 
            this.mmTimer.Mode = Multimedia.TimerMode.OneShot;
            this.mmTimer.Period = 100;
            this.mmTimer.Resolution = 1;
            this.mmTimer.SynchronizingObject = this;
            this.mmTimer.Tick += new System.EventHandler(this.mmTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(767, 592);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "MicronTracker Demo App C#";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ToolStrip2.ResumeLayout(false);
            this.ToolStrip2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBox0)).EndInit();
            this.panelExposure1.ResumeLayout(false);
            this.panelExposure1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }
    private System.Windows.Forms.VScrollBar vsGain;
    private System.Windows.Forms.VScrollBar vsShutter;
    private System.Windows.Forms.TextBox tbShutter;
    private System.Windows.Forms.ToolStripMenuItem mnuAbout;
    private System.Windows.Forms.TextBox tbGainF;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ToolStripMenuItem videoRecorderToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem mnuRecordStop;
    private System.Windows.Forms.ToolStripMenuItem mnuSaveLeftImage;
    private System.Windows.Forms.ToolStripMenuItem mnuCalibrationInfo;
    private System.Windows.Forms.ToolStripMenuItem mnuMTCAPIDocumentation;
    private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
    private System.Windows.Forms.Label labelCaptureDisabled;
    private System.Windows.Forms.Panel panel1;
    public System.Windows.Forms.PictureBox picBox1;
    public System.Windows.Forms.PictureBox picBox0;
    private System.Windows.Forms.Panel panelExposure1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Panel recordPanel;
    private System.Windows.Forms.Timer timerAviSaver;
    private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    private System.Windows.Forms.ToolStripMenuItem mnuSnapShots;
    private System.Windows.Forms.ToolStripMenuItem mnuPoseRecorder;
    private System.Windows.Forms.ToolStripMenuItem mnuMarkerTemplates;
    private System.Windows.Forms.ToolStripMenuItem displayToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem mnuMirrorImage;
    public System.Windows.Forms.ToolStripMenuItem mnuHistEqualize;
    private System.Windows.Forms.ToolStripMenuItem mnuHalfSize;
    private System.Windows.Forms.ToolStripMenuItem mnuMarkersPositions;
    private System.Windows.Forms.ToolStripMenuItem mnuDistancesAngles;
    private System.Windows.Forms.ToolStripMenuItem mnuMagnifiedToolTip;
    private System.Windows.Forms.ToolStripMenuItem mnuVectors;
    private System.Windows.Forms.ToolStripMenuItem mnuFiducials;
    private System.Windows.Forms.ToolStripMenuItem mnuDisplayConnections;
    private System.Windows.Forms.ToolStripMenuItem mnuXPoints;
    private System.Windows.Forms.ToolStripMenuItem mnuCameraInfo;
    private System.Windows.Forms.ToolStripMenuItem mnuSettings;
    private System.Windows.Forms.ToolStripMenuItem mnu3DTracer;
    private System.Windows.Forms.ToolStripMenuItem mnuDataCapture;
    private System.Windows.Forms.ToolStripMenuItem activateToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem mnuMarkersProcessing;
    private System.Windows.Forms.ToolStripMenuItem mnuAutoExposure;
    private System.Windows.Forms.ToolStripMenuItem mnuCamCamRegistration;
    private System.Windows.Forms.MenuStrip menuStrip1;
    internal System.Windows.Forms.ToolStripMenuItem mnuXPointsProcessing;
    internal System.Windows.Forms.ToolStripMenuItem mnuXPointsPositions;
    internal System.Windows.Forms.ToolStripMenuItem mnuExternallyTrigger;
    internal System.Windows.Forms.ToolStripMenuItem mnuStrobeSignal;
    internal System.Windows.Forms.ToolStripMenuItem mnuHdrMode;
    internal System.Windows.Forms.ToolStripMenuItem mnuDisplayImageLeft;
    internal System.Windows.Forms.ToolStripMenuItem mnuDisplayImageRight;
    internal System.Windows.Forms.ToolStripMenuItem mnuDisplayImageMiddle;
    internal System.Windows.Forms.ToolStripMenuItem mnuDisplayImageAll;
    public System.Windows.Forms.PictureBox PicBox2;
    internal System.Windows.Forms.ToolStripSeparator ToolStripMenuItem1;
    internal System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
    internal System.Windows.Forms.ToolStripMenuItem BackgroundProcessToolStripMenuItem;
    internal System.Windows.Forms.ToolStripMenuItem mnuPositionsAtRefSpace;
    private Multimedia.Timer mmTimer;
    internal System.Windows.Forms.ToolStripMenuItem mnuCloudsProcessing;
    internal System.Windows.Forms.ToolStripMenuItem mnuCloudsPositions;
    internal System.Windows.Forms.ToolStripMenuItem mnuClouds;
    internal System.Windows.Forms.ToolStrip ToolStrip2;
    internal System.Windows.Forms.ToolStripButton ToolStripHalfSize;
    internal System.Windows.Forms.ToolStripButton ToolStripPoseRecorder;
    internal System.Windows.Forms.ToolStripButton ToolStripSettings;
    internal System.Windows.Forms.ToolStripButton ToolStripCapture;
    internal System.Windows.Forms.ToolStripButton ToolStripAPIDocumentation;
    internal System.Windows.Forms.ToolStripButton ToolStripVideoRecorder;
    internal System.Windows.Forms.ToolStripButton ToolStripHDR;
    internal System.Windows.Forms.ToolStripButton ToolStripTemplates;
    internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator1;
    internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator2;
    internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator3;
    internal System.Windows.Forms.ToolStripButton ToolStripCalibrationInfo;
    internal System.Windows.Forms.ToolStripButton ToolStripCameraInfo;
    internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator4;
    internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator6;
    internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator5;
    internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator7;
    internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator8;
    internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator9;
    public System.Windows.Forms.ToolStripMenuItem mnuGammaCorrection;

  }

}

