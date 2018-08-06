using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MTInterfaceDotNet;
using AviFile;
using System.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Multimedia;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace MicronTrackerVerification
{
    public partial class MainForm : Form
    {
        #region declaration
        private IList<XPoint> IXPoints;
        private bool ignore = false;
        private const int LeftI = 0;
        private const int RightI = 1;
        private const int MiddleI = 2;
        private int mFontSize;
        private int TextLine;
        private int TextLine_Increments = 20;
        private int DisplayPB_Index = 0;
        private string fps;
        private int CurrHDR_CycleIndex = 0;
        private bool RefreshDisplay;
        private int ErrorMessagenumber;
        private int ToolstripHeight = 40;
        private int mtTimerInterval;

        public Persistence PP = null;
        public string DocumentationPath = Environment.GetEnvironmentVariable("MTHome") + "\\Doc\\html\\index.html";
        public string calibrationFilesDir = Environment.GetEnvironmentVariable("MTHome") + "\\CalibrationFiles";

        public string TemplatesFolderPath = Application.StartupPath + "\\Markers";
        private Cameras mCameras = MT.Cameras;
        private Markers markers = MT.Markers;
        private XPoints xpoints = MT.XPoints;
        private Clouds clouds = MT.Clouds;
        public double TTCalib_Offset_X;
        public double TTCalib_Offset_Y;
        public double TTCalib_Offset_Z;
        public double TTCalib_Offset_RX;
        public double TTCalib_Offset_RY;
        public double TTCalib_Offset_RZ;
        private double Frames;
        private bool initialized = false;
        private Stopwatch SWFPS = new Stopwatch();
        private Stopwatch intervalsetting = new Stopwatch();
        private long Prev_Frame_No;

        private Stopwatch ProcessCurrFrame_SW = new Stopwatch();
        // Overlay colors
        private Color XPointsOverlayColor = Color.FromArgb(60, 60, 255);
        private Color LongVectorColor = Color.Blue;
        private Color ShortVectorColor = Color.Red;

        private Color FiducialColor = Color.Lime;
        // To record the video
        private AviManager aviManager;

        private VideoStream aviStream;
        private TraceForm frmTrace;
        private PositionRecorderForm frmpositionrecorder;
        private OptionsForm optionsForm;
        private CameraProblemsForm CameraProblemsFrm;

        //Added by Mei, logger to file 
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static UdpClient udpclient;
        private static bool connect = false;
        public static int port = 8080;
        //End added by Mei
        #endregion

        #region "properties"

        private double m_StrobSignalDuration;
        public double StrobSignalDuration
        {
            get { return m_StrobSignalDuration; }
            set
            {
                if ((value > 63.93))
                    value = 63.93;
                if ((value < 0.5))
                    value = 0.5;
                m_StrobSignalDuration = value;
            }
        }

        private double m_StrobSignalDelay;
        public double StrobSignalDelay
        {
            get { return m_StrobSignalDelay; }
            set
            {
                if ((value > 63.93))
                    value = 63.93;
                if ((value < 0.5))
                    value = 0.5;
                m_StrobSignalDelay = value;
            }
        }

        public Camera CurrCamera { get; set; }

        public int CurrCam_SensorNum
        {
            get { return CurrCamera.sensorsnum; }
        }
        private bool m_CaptureEnabled;
        public bool CaptureEnabled
        {
            get { return m_CaptureEnabled; }
            set
            {
                m_CaptureEnabled = value;
                if (value)
                {
                    if (mCameras.Count != mCameras.OntheBus())
                    {
                        SetupCameras();
                        // cameras got added or removed: try again after resetting
                        UpdateUI();
                    }
                    try
                    {
                        mmTimer.Start();
                        mmTimer.Mode = TimerMode.Periodic;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                    }
                    mnuDataCapture.Checked = value;

                }
                else
                {
                    if ((mmTimer != null))
                    {
                        mmTimer.Stop();
                    }

                }
                mnuDataCapture.Checked = value;
                labelCaptureDisabled.Visible = !value;
            }
        }


        private bool m_MarkersProcessingEnabled;
        public bool MarkersProcessingEnabled
        {
            get { return m_MarkersProcessingEnabled; }
            set
            {
                m_MarkersProcessingEnabled = value;
                mnuMarkersProcessing.Checked = value;
            }
        }

        public bool PoseRecorderEnabled { get; set; }

        private bool m_XPointsProcessingEnabled;
        public bool XPointsProcessingEnabled
        {
            get { return m_XPointsProcessingEnabled; }
            set
            {
                m_XPointsProcessingEnabled = value;
                mnuXPointsProcessing.Checked = value;
            }
        }

        private bool m_CloudsProcessingEnabled;
        public bool CloudsProcessingEnabled
        {
            get { return m_CloudsProcessingEnabled; }
            set
            {
                m_CloudsProcessingEnabled = value;
                mnuCloudsProcessing.Checked = value;
            }
        }

        private bool m_IsExternallyTriggered;
        public bool IsExternallyTriggered
        {
            get { return m_IsExternallyTriggered; }
            set
            {
                m_IsExternallyTriggered = value;
                mnuExternallyTrigger.Checked = value;
            }
        }


        private bool m_IsStrobeSignal;
        public bool IsStrobeSignal
        {
            get { return m_IsStrobeSignal; }
            set
            {
                m_IsStrobeSignal = value;
                mnuStrobeSignal.Checked = value;
            }
        }


        private bool m_HdrEnabled;
        public bool HdrEnabled
        {
            get { return m_HdrEnabled; }
            set
            {
                m_HdrEnabled = value;
                int i = 0;
                while (i < mCameras.Count)
                {
                    mCameras[i].HdrEnabled = value;
                    i += 1;
                }
                mnuHdrMode.Checked = value;
                if (CurrCamera != null && CurrCamera.sensorsnum == 1)
                {
                    DisplayMode = 0;
                }
                else
                {
                    DisplayMode = 3;
                }
                UpdateUI();
            }
        }


        private bool m_AutoExposure;
        public bool AutoExposure
        {
            get { return m_AutoExposure; }
            set
            {
                m_AutoExposure = value;
                mnuAutoExposure.Checked = AutoExposure;
                panelExposure1.Visible = !m_AutoExposure;

                if (panelExposure1.Visible)
                {
                    AdjustGain(CurrCamera.GainF);
                    AdjustShutter(CurrCamera.ShutterMsecs);
                }
            }
        }

        public double DisplayScale { get; set; }

        private bool m_HalfSizeImageDisplay;
        public bool HalfSizeImageDisplay
        {
            get { return m_HalfSizeImageDisplay; }
            set
            {
                m_HalfSizeImageDisplay = value;
                if ((!object.ReferenceEquals(this.ActiveControl, mnuHalfSize)))
                {
                    mnuHalfSize.Checked = value;
                }

                if (value)
                {
                    DisplayScale = (0.5);
                }
                else
                {
                    DisplayScale = (1);
                }
                picBox0.Image = null;
                picBox1.Image = null;
                PicBox2.Image = null;
                DisplayImages(true);
                AdjustFormToPictureSize();
                PP.Section = "General";
                PP.SaveBool("HalfSizeImageDisplay", m_HalfSizeImageDisplay);
            }
        }


        private bool m_DisplayMirrorImage;
        public bool DisplayMirrorImage
        {
            get { return m_DisplayMirrorImage; }
            set
            {
                m_DisplayMirrorImage = value;
                mnuMirrorImage.Checked = value;
                picBox0.Image = null;
                picBox1.Image = null;
                PicBox2.Image = null;
                DisplayImages(true);
                PP.Section = "General";
                PP.SaveBool("DisplayMirrorImage", m_DisplayMirrorImage);
            }
        }


        private bool m_IsShowingMarkersPositions;
        public bool IsShowingMarkersPositions
        {
            get { return m_IsShowingMarkersPositions; }
            set
            {
                m_IsShowingMarkersPositions = value;
                mnuMarkersPositions.Checked = value;
            }
        }

        private bool m_IsShowingMarkersPositionsAtRefSpace;
        public bool IsShowingMarkersPositionsAtRefSpace
        {
            get { return m_IsShowingMarkersPositionsAtRefSpace; }
            set
            {
                m_IsShowingMarkersPositionsAtRefSpace = value;
                mnuPositionsAtRefSpace.Checked = value;
            }
        }

        private bool m_IsShowingXPointsPositions;
        public bool IsShowingXPointsPositions
        {
            get { return m_IsShowingXPointsPositions; }
            set
            {
                m_IsShowingXPointsPositions = value;
                if ((!object.ReferenceEquals(ActiveControl, mnuXPointsPositions)))
                {
                    mnuXPointsPositions.Checked = value;
                }
            }
        }

        private bool m_IsShowingCloudsPositionsAtRefSpace;
        public bool IsShowingCloudsPositionsAtRefSpace
        {
            get { return m_IsShowingCloudsPositionsAtRefSpace; }
            set
            {
                m_IsShowingCloudsPositionsAtRefSpace = value;
                mnuPositionsAtRefSpace.Checked = value;
            }
        }

        private bool m_IsShowingCloudsPositions;
        public bool IsShowingCloudsPositions
        {
            get { return m_IsShowingCloudsPositions; }
            set
            {
                m_IsShowingCloudsPositions = value;
                if ((!object.ReferenceEquals(ActiveControl, mnuCloudsPositions)))
                {
                    mnuCloudsPositions.Checked = value;
                }
            }
        }

        private bool m_IsShowingConnections;
        public bool IsShowingConnections
        {
            get { return m_IsShowingConnections; }
            set
            {
                m_IsShowingConnections = value;
                mnuDisplayConnections.Checked = value;
            }
        }


        private bool m_IsShowingDistancesAndAngles;
        public bool IsShowingDistancesAndAngles
        {
            get { return m_IsShowingDistancesAndAngles; }
            set
            {
                m_IsShowingDistancesAndAngles = value;
                mnuDistancesAngles.Checked = value;
            }
        }


        private bool m_IsShowingVectors;
        public bool IsShowingVectors
        {
            get { return m_IsShowingVectors; }
            set
            {
                m_IsShowingVectors = value;
                mnuVectors.Checked = value;
            }
        }

        public bool IsShowingVectorsForTemplateRegistration { get; set; }

        private bool m_IsShowingMagnifiedToolTip;
        public bool IsShowingMagnifiedToolTip
        {
            get { return m_IsShowingMagnifiedToolTip; }
            set
            {
                m_IsShowingMagnifiedToolTip = value;
                mnuMagnifiedToolTip.Checked = value;
            }
        }


        private bool m_IsShowingFiducials;
        public bool IsShowingFiducials
        {
            get { return m_IsShowingFiducials; }
            set
            {
                m_IsShowingFiducials = value;
                mnuFiducials.Checked = value;
            }
        }


        private bool m_IsShowingXPoints;
        public bool IsShowingXPoints
        {
            get { return m_IsShowingXPoints; }
            set
            {
                m_IsShowingXPoints = value;
                mnuClouds.Checked = value;
            }
        }

        private bool m_IsShowingClouds;
        public bool IsShowingClouds
        {
            get { return m_IsShowingClouds; }
            set
            {
                m_IsShowingClouds = value;
                mnuXPoints.Checked = value;
            }
        }


        private bool m_IsShowingExposure;
        public bool IsShowingExposure
        {
            get { return m_IsShowingExposure; }
            set
            {
                m_IsShowingExposure = value;
                mnuCameraInfo.Checked = value;
                PP.Section = "General";
                PP.SaveBool("IsShowingExposure", IsShowingExposure);
            }
        }


        private bool m_AutoAdjustLightCoolness;
        public bool AutoAdjustLightCoolness
        {
            get { return m_AutoAdjustLightCoolness; }
            set { m_AutoAdjustLightCoolness = value; }
        }


        private double m_FiducialVectorMaxLength;
        public double FiducialVectorMaxLength
        {
            get { return m_FiducialVectorMaxLength; }
            set { m_FiducialVectorMaxLength = Math.Max(0, value); }
        }

        public double WarmupSensitivity
        {
            get { return CurrCamera.ThermalWarmupSensitivity; }
            set { CurrCamera.ThermalWarmupSensitivity = value; }
        }

        private double m_FrameRatePercentage;
        public double FrameRatePercentage
        {
            get { return m_FrameRatePercentage; }
            set
            {
                m_FrameRatePercentage = value;
                if (m_FrameRatePercentage > 100)
                    m_FrameRatePercentage = 100;
                if (m_FrameRatePercentage < 1)
                    m_FrameRatePercentage = 1;
            }
        }


        private double m_ShownVectorsMinLength;
        public double ShownVectorsMinLength
        {
            get { return m_ShownVectorsMinLength; }
            set { m_ShownVectorsMinLength = Math.Max(0, value); }
        }


        private int m_DisplayMode;
        public int DisplayMode
        {
            get { return m_DisplayMode; }
            set
            {
                m_DisplayMode = value;
                //show all of them
                if (DisplayMode == 3)
                {
                    mnuDisplayImageLeft.Checked = true;
                    mnuDisplayImageRight.Checked = true;
                    //mnuDisplayImageMiddle.Checked = True
                }
                else
                {
                    if (DisplayMode == 0)
                        mnuDisplayImageLeft.Checked = true;
                    //Else mnuDisplayImageLeft.Checked = False
                    if (DisplayMode == 1)
                        mnuDisplayImageRight.Checked = true;
                    //Else mnuDisplayImageRight.Checked = False
                    if (DisplayMode == 2)
                        mnuDisplayImageMiddle.Checked = true;
                    //Else mnuDisplayImageLeft.Checked = False
                    if (DisplayMode == 3)
                        mnuDisplayImageAll.Checked = true;
                    //Else mnuDisplayImageLeft.Checked = False

                }
                PP.SaveInt("DisplayMode", m_DisplayMode);
            }
        }


        public double CurrGainF { get; set; }
        public double CurrShutterMs { get; set; }

        private bool m_HistogramEqualizeImages;
        public bool HistogramEqualizeImages
        {
            get { return m_HistogramEqualizeImages; }
            set
            {
                m_HistogramEqualizeImages = value;
                mCameras.HistogramEqualizeImages = value;
                mnuHistEqualize.Checked = value;
                PP.Section = "General";
                PP.SaveBool("HistogramEqualizeImages", value);
            }
        }

        private bool m_GammaCorrectionEnabled;
        public bool GammaCorrectionEnabled
        {
            get { return m_GammaCorrectionEnabled; }
            set
            {
                m_GammaCorrectionEnabled = value;
                mCameras.ImageGammaCorrectionEnabled = value;
                mnuGammaCorrection.Checked = value;
                PP.Section = "General";
                PP.SaveBool("ImageGammaCorrectionEnabled", value);
            }
        }

        public double GammaValue
        {
            get { return mCameras.ImageGammaValue; }
            set { mCameras.ImageGammaValue = value; }
        }

        #endregion

        #region "Menu Button Events"

        private void mnuDataCapture_Click(System.Object sender, System.EventArgs e)
        {
            mnuDataCapture.Checked = !mnuDataCapture.Checked;
            CaptureEnabled = mnuDataCapture.Checked;
        }

        private void mnuMarkersProcessing_Click(System.Object sender, System.EventArgs e)
        {
            mnuMarkersProcessing.Checked = !mnuMarkersProcessing.Checked;
            MarkersProcessingEnabled = mnuMarkersProcessing.Checked;
            UpdateUI();
        }

        private void mnuXPointsProcessing_Click(System.Object sender, System.EventArgs e)
        {
            mnuXPointsProcessing.Checked = !mnuXPointsProcessing.Checked;
            XPointsProcessingEnabled = mnuXPointsProcessing.Checked;
            mnuXPoints.Checked = XPointsProcessingEnabled;
            IsShowingXPoints = mnuXPoints.Checked;
            if ((!XPointsProcessingEnabled))
            {
                // Disable Clouds processing
                CloudsProcessingEnabled = false;
                mnuCloudsProcessing.Checked = false;
            }
            UpdateUI();
        }

        private void mnuCloudsProcessing_Click(System.Object sender, System.EventArgs e)
        {
            mnuCloudsProcessing.Checked = !mnuCloudsProcessing.Checked;
            CloudsProcessingEnabled = mnuCloudsProcessing.Checked;
            mnuXPoints.Checked = CloudsProcessingEnabled;
            IsShowingClouds = CloudsProcessingEnabled;
            if ((CloudsProcessingEnabled))
            {
                // enable xpoints processing
                XPointsProcessingEnabled = true;
                mnuXPointsProcessing.Checked = true;
            }
            UpdateUI();
        }

        private void mnuExternallyTrigger_Click(System.Object sender, System.EventArgs e)
        {
            mnuExternallyTrigger.Checked = !mnuExternallyTrigger.Checked;
            IsExternallyTriggered = mnuExternallyTrigger.Checked;
            bool output = false;
            if (IsExternallyTriggered)
            {
                CaptureEnabled = true;
                CurrCamera.TriggerMode = true;
                output = CurrCamera.TriggerMode;
            }
            else
            {
                CurrCamera.TriggerMode = false;
                CaptureEnabled = true;
            }
        }

        private void mnuStrobeSignal_Click(System.Object sender, System.EventArgs e)
        {
            mnuStrobeSignal.Checked = !mnuStrobeSignal.Checked;
            IsStrobeSignal = mnuStrobeSignal.Checked;
            bool output = false;
            output = CurrCamera.TransmitSignal(IsStrobeSignal, StrobSignalDelay, StrobSignalDuration);
            //Delay and Duration are in milliseconds
        }

        private void mnuHdrMode_Click(System.Object sender, System.EventArgs e)
        {
            mnuHdrMode.Checked = !mnuHdrMode.Checked;
            HdrEnabled = mnuHdrMode.Checked;
            AutoExposure = AutoExposure;
            if (HdrEnabled)
            {
                mnuHistEqualize.Checked = true;
                HistogramEqualizeImages = mnuHistEqualize.Checked;
            }
        }

        private void mnuAutoExposure_Click(System.Object sender, System.EventArgs e)
        {
            mnuAutoExposure.Checked = !mnuAutoExposure.Checked;
            AutoExposure = mnuAutoExposure.Checked;
            AdjustFormToPictureSize();
        }

        private void mnuCamCamRegistration_Click(System.Object sender, System.EventArgs e)
        {
            mnuCamCamRegistration.Checked = !mnuCamCamRegistration.Checked;
            markers.AutoAdjustCam2CamRegistration = mnuCamCamRegistration.Checked;
        }

        private void mnuMarkerTemplates_Click(System.Object sender, System.EventArgs e)
        {
            //CaptureEnabled = true;
            MarkersProcessingEnabled = true;
            BackgroundProcessToolStripMenuItem.Checked = false;
            MT.Markers.BackGroundProcess = false;
            MT.XPoints.BackGroundProcess = false;
            MT.Markers.AutoAdjustShortCycleHdrExposureLockedMarkers = false;
            MarkersForm markersFrom = new MarkersForm(this);
            HdrEnabled = false;
            mnuHdrMode.Checked = HdrEnabled;
            ShowAuxiliaryForm(markersFrom, false);
        }

        private void mnuPoseRecorder_Click(System.Object sender, System.EventArgs e)
        {
            frmpositionrecorder = new PositionRecorderForm(this);
            ShowAuxiliaryForm(frmpositionrecorder, false);
        }

        private void mnuSnapShots_Click(System.Object sender, System.EventArgs e)
        {
            SnapshotsForm snapShotForm = new SnapshotsForm(this);
            ShowAuxiliaryForm(snapShotForm, false);
        }

        private void mnu3DTracer_Click(System.Object sender, System.EventArgs e)
        {
            frmTrace = new TraceForm(this);
            ShowAuxiliaryForm(frmTrace, false);
        }

        private void mnuOptions_Click(System.Object sender, System.EventArgs e)
        {
            optionsForm = new OptionsForm(this);
            ShowAuxiliaryForm(optionsForm, false);
        }

        private void mnuMirrorImage_Click(System.Object sender, System.EventArgs e)
        {
            mnuMirrorImage.Checked = !mnuMirrorImage.Checked;
            DisplayMirrorImage = mnuMirrorImage.Checked;
        }

        public void mnuHistEqualize_Click(System.Object sender, System.EventArgs e)
        {
            mnuHistEqualize.Checked = !mnuHistEqualize.Checked;
            HistogramEqualizeImages = mnuHistEqualize.Checked;
        }

        private void mnuHalfSize_Click(System.Object sender, System.EventArgs e)
        {
            mnuHalfSize.Checked = !mnuHalfSize.Checked;
            HalfSizeImageDisplay = mnuHalfSize.Checked;
        }

        private void mnuMarkersPositions_Click(System.Object sender, System.EventArgs e)
        {
            mnuMarkersPositions.Checked = !mnuMarkersPositions.Checked;
            IsShowingMarkersPositions = mnuMarkersPositions.Checked;
        }

        private void mnuMarkersPositionsAtRefSpace_Click(System.Object sender, System.EventArgs e)
        {
            mnuPositionsAtRefSpace.Checked = !mnuPositionsAtRefSpace.Checked;
            IsShowingMarkersPositionsAtRefSpace = mnuPositionsAtRefSpace.Checked;
        }

        private void mnuXPointsPositions_Click(System.Object sender, System.EventArgs e)
        {
            mnuXPointsPositions.Checked = !mnuXPointsPositions.Checked;
            IsShowingXPointsPositions = mnuXPointsPositions.Checked;
        }

        private void mnuCloudsPositions_Click(System.Object sender, System.EventArgs e)
        {
            mnuCloudsPositions.Checked = !mnuCloudsPositions.Checked;
            IsShowingCloudsPositions = mnuCloudsPositions.Checked;
        }

        private void mnuDistancesAngles_Click(System.Object sender, System.EventArgs e)
        {
            mnuDistancesAngles.Checked = !mnuDistancesAngles.Checked;
            IsShowingDistancesAndAngles = mnuDistancesAngles.Checked;
        }

        private void mnuMagnifiedToolTip_Click(System.Object sender, System.EventArgs e)
        {
            mnuMagnifiedToolTip.Checked = !mnuMagnifiedToolTip.Checked;
            IsShowingMagnifiedToolTip = mnuMagnifiedToolTip.Checked;
        }

        private void mnuVectors_Click(System.Object sender, System.EventArgs e)
        {
            mnuVectors.Checked = !mnuVectors.Checked;
            IsShowingVectors = mnuVectors.Checked;
        }

        private void mnuFiducials_Click(System.Object sender, System.EventArgs e)
        {
            mnuFiducials.Checked = !mnuFiducials.Checked;
            IsShowingFiducials = mnuFiducials.Checked;
        }


        private void mnuDisplayConnections_Click(System.Object sender, System.EventArgs e)
        {
        }

        private void mnuXPoints_Click(System.Object sender, System.EventArgs e)
        {
            mnuXPoints.Checked = !mnuXPoints.Checked;
            IsShowingXPoints = mnuXPoints.Checked;
        }

        private void mnuCameraInfo_Click(System.Object sender, System.EventArgs e)
        {
            mnuCameraInfo.Checked = !mnuCameraInfo.Checked;
            IsShowingExposure = mnuCameraInfo.Checked;
        }

        private void mnuCalibrationInfo_Click(System.Object sender, System.EventArgs e)
        {
            if (CurrCamera == null)
            {
                MessageBox.Show("Camera is not attached", "Error...");
                return;
            }
            string[] strArray = CurrCamera.CalibrationInfo();
            StringBuilder strBuilder = new StringBuilder(strArray[0]);
            int i = 1;
            while (i < strArray.Length)
            {
                strBuilder.Append(Environment.NewLine).Append(strArray[i]);
                i += 1;
            }
            MessageBox.Show(strBuilder.ToString(), "Calibration Info");
        }

        private void mnuSaveLeftImage_Click(System.Object sender, System.EventArgs e)
        {
            // Note that as we are picking images from pictureBox Valid image 
            // will be saved only when DisplayEnabled
            if (CurrCamera == null)
            {
                MessageBox.Show("Camera is not attached", "Error...");
                return;
            }

            Bitmap bitmap = (Bitmap)picBox0.Image;

            if (bitmap == null)
            {
                MessageBox.Show("Image is null : Check Camera is connected properly");
                return;
            }
            string InitDir = null;
            string filename = null;
            SaveImageWithUI(bitmap, InitDir, filename);
        }

        private void mnuRecordStop_Click(System.Object sender, System.EventArgs e)
        {
            // Note that as we are picking images from pictureBox Valid avi 
            // will be saved only when DisplayEnabled
            if (CurrCamera == null)
            {
                MessageBox.Show("Camera is not attached", "Error...");
                return;
            }

            if ((!mnuRecordStop.Checked) == true)
            {
                saveFileDialog1.AddExtension = true;
                saveFileDialog1.DefaultExt = ".avi";
                if (System.Windows.Forms.DialogResult.OK == saveFileDialog1.ShowDialog(this))
                {
                    StartRecording(saveFileDialog1.FileName);
                    mnuRecordStop.Checked = !mnuRecordStop.Checked;
                }

            }
            else
            {
                StopRecording();
                mnuRecordStop.Checked = !mnuRecordStop.Checked;
            }
        }

        //OK TODO path is hard coded get it from application environment variables
        private void mnuMTCAPIDocumentation_Click(System.Object sender, System.EventArgs e)
        {
            if ((!File.Exists(DocumentationPath)))
            {
                MessageBox.Show(DocumentationPath + " cannot be located.");
            }
            else
            {
                Process pr = new Process();
                pr.StartInfo = new ProcessStartInfo(DocumentationPath);
                pr.Start();
            }

        }

        private void mnuAbout_Click(System.Object sender, System.EventArgs e)
        {
            AboutForm aboutForm = new AboutForm(this);
            aboutForm.ShowDialog(this);
        }

        private void mnuDisplayImageAll_Click(System.Object sender, System.EventArgs e)
        {
            mnuDisplayImageAll.Checked = !mnuDisplayImageAll.Checked;

            if (mnuDisplayImageAll.Checked)
            {
                DisplayMode = 3;
                mnuDisplayImageMiddle.Checked = false;
                mnuDisplayImageLeft.Checked = false;
                mnuDisplayImageRight.Checked = false;
            }
            else
            {
                if (CurrCamera.sensorsnum == 3)
                {
                    DisplayMode = 2;
                    //show middle image
                    mnuDisplayImageMiddle.Checked = true;
                }
                else
                {
                    DisplayMode = 0;
                    //Show left Images
                }
            }
            UpdateUI();
        }

        private void mnuDisplayImageLeft_Click(System.Object sender, System.EventArgs e)
        {
            mnuDisplayImageLeft.Checked = !mnuDisplayImageLeft.Checked;
            if (mnuDisplayImageLeft.Checked)
            {
                DisplayMode = 0;
                //Shows the Left Image
                if (mnuDisplayImageRight.Checked)
                {
                    DisplayMode = 3;
                    //Shows Stereo Images
                }
            }
            else
            {
                if (mnuDisplayImageRight.Checked)
                {
                    DisplayMode = 1;
                    //Shows Right Image
                }
                else
                {
                    DisplayMode = -1;
                    //Doesn't Show the Images
                    picBox0.Image = null;
                    picBox1.Image = null;
                    PicBox2.Image = null;
                }
            }
            UpdateUI();
        }

        private void mnuDisplayImageRight_Click(System.Object sender, System.EventArgs e)
        {
            mnuDisplayImageRight.Checked = !mnuDisplayImageRight.Checked;
            if (mnuDisplayImageRight.Checked)
            {
                DisplayMode = 1;
                // Display only the Right Image
                if (mnuDisplayImageLeft.Checked)
                {
                    DisplayMode = 3;
                    //Display all images
                }
            }
            else
            {
                if (mnuDisplayImageLeft.Checked)
                {
                    DisplayMode = 0;
                    //Shows Left Image
                }
                else
                {
                    DisplayMode = -1;
                    //Doesn't Show the Images
                }
            }
            UpdateUI();
        }

        private void mnuDisplayImageMiddle_Click(System.Object sender, System.EventArgs e)
        {
            mnuDisplayImageMiddle.Checked = !mnuDisplayImageMiddle.Checked;
            if (mnuDisplayImageMiddle.Checked)
            {
                DisplayMode = 2;
                mnuDisplayImageAll.Checked = false;
            }
            else
            {
                //If (mnuDisplayImageLeft.Checked Or mnuDisplayImageRight.Checked) Then
                if (mnuDisplayImageAll.Checked)
                {
                    DisplayMode = 3;
                    //Shows All the Images
                }
                else
                {
                    DisplayMode = -1;
                    //Doesn't Show the Images
                }
            }
            UpdateUI();
        }
        // Methods of Exposure Panel #1

        private void vsGain1_ValueChanged(System.Object sender, System.EventArgs e)
        {
            if (!initialized)
                return;
            if (ignore)
                return;

            ignore = true;
            AdjustGain(vsGain.Value / 10.0);
            ignore = false;
        }


        private void vsShutter1_ValueChanged(System.Object sender, System.EventArgs e)
        {
            if (!initialized)
                return;
            if (ignore)
                return;

            ignore = true;
            AdjustShutter(vsShutter.Value / 10.0);
            ignore = false;
        }


        private void tbShutter1_TextChanged(System.Object sender, System.EventArgs e)
        {
            if (!initialized)
                return;
            if (ignore)
                return;

            ignore = true;
            double shutter = 0;
            if (double.TryParse(tbShutter.Text, out shutter))
            {
                AdjustShutter(double.Parse(tbShutter.Text));
            }
            ignore = false;
        }


        private void tbGainF1_TextChanged(System.Object sender, System.EventArgs e)
        {
            if (!initialized)
                return;
            if (ignore)
                return;

            ignore = true;
            double gainF = 0;
            if (double.TryParse(tbGainF.Text, out gainF))
            {
                AdjustGain(double.Parse(tbGainF.Text));
            }
            ignore = false;
        }


        private void AdjustShutter(double newval)
        {
            CurrCamera.ShutterMsecs = Utils.Constrain(CurrCamera.ShutterMsecsMin, newval, CurrCamera.ShutterMsecsMax);
            vsShutter.Value = (int)(10 * CurrCamera.ShutterMsecs);
            tbShutter.Text = Math.Round(CurrCamera.ShutterMsecs, 1).ToString();
        }


        private void AdjustGain(double newval)
        {
            CurrCamera.GainF = Utils.Constrain(CurrCamera.GainFMin, newval, CurrCamera.GainFMax);
            vsGain.Value = (int)(10 * CurrCamera.GainF);
            tbGainF.Text = Math.Round(CurrCamera.GainF, 1).ToString();
        }


        private void BackgroundProcessToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            BackgroundProcessToolStripMenuItem.Checked = !BackgroundProcessToolStripMenuItem.Checked;
            MT.Markers.BackGroundProcess = BackgroundProcessToolStripMenuItem.Checked;
            if ((XPointsProcessingEnabled))
            {
                MT.XPoints.BackGroundProcess = BackgroundProcessToolStripMenuItem.Checked;
            }
        }

        private void mnuClouds_Click(System.Object sender, System.EventArgs e)
        {
            mnuClouds.Checked = !mnuClouds.Checked;
            IsShowingClouds = mnuClouds.Checked;
        }


        private void ExitToolStripMenuItem_Click(System.Object sender, System.EventArgs e)
        {
            exitdemo(0);
        }


        #endregion

        private void MainForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            exitdemo(0);
        }

        private void MainForm_Load(System.Object sender, System.EventArgs e)
        {

            if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MTHome")))
            {
                MessageBox.Show("\"MTHome\" environment variable is not set!", "Error");
                return;
            }
            initialized = true;
            PP = new Persistence();

            GraphicsPath path = new GraphicsPath(FillMode.Alternate);
            Rectangle rect = new Rectangle(0, 0, 15, 15);
            path.AddEllipse(rect);
            recordPanel.Region = new Region(path);

            DisplayScale = 1;

            PP.Path = Application.StartupPath + "\\" + this.Text + ".ini";
            markers.PredictiveFramesInterleave = PP.RetrieveInt("PredictiveFramesInterleave", 0);
            markers.OverExposureControlInterleave = PP.RetrieveInt("OverExposureControlInterleave", 10);
            markers.TemplateMatchToleranceMM = PP.RetrieveDouble("MarkersTemplateMatchToleranceMM", 1);
            clouds.TemplateMatchToleranceMM = PP.RetrieveDouble("CloudsTemplateMatchToleranceMM", 1);
            markers.JitterFilterCoefficient = PP.RetrieveDouble("JitterFilterCoefficient", 0.5);
            markers.JitterFilterEnabled = PP.RetrieveBool("JitterFilter", true);
            markers.TemplateBasedWarmupCorrection = PP.RetrieveBool("TemplateBasedWarmUpCorrection", false);
            markers.SmallerXPFootprint = PP.RetrieveBool("SmallerFootprint", false);
            mCameras.GainFLimiter = PP.RetrieveDouble("GainFLimiter", 5);
            mCameras.ShutterMsecsLimiter = PP.RetrieveDouble("ShutterMsecsLimiter", mCameras.ShutterMsecsLimiterDefault);
            mCameras.LightCoolness = PP.RetrieveDouble("LightCoolness", mCameras.LightCoolness);
            mCameras.HDRLightCoolness = PP.RetrieveDouble("HDRLightCoolness", mCameras.HDRLightCoolness);
            IsShowingExposure = PP.RetrieveBool("IsShowingExposure", false);
            DisplayMirrorImage = PP.RetrieveBool("DisplayMirrorImage", false);
            DisplayMode = PP.RetrieveInt("DisplayMode", 0);
            MarkersProcessingEnabled = PP.RetrieveBool("MarkersProcessingEnabled", true);
            XPointsProcessingEnabled = PP.RetrieveBool("XPointsProcessingEnabled", true);
            CloudsProcessingEnabled = PP.RetrieveBool("CloudsProcessingEnabled", true);
            IsShowingXPointsPositions = PP.RetrieveBool("IsShowingXPointsPositions", true);
            IsShowingCloudsPositions = PP.RetrieveBool("IsShowingCloudsPositions", true);
            IsShowingXPoints = PP.RetrieveBool("IsShowingXPoints", true);
            IsShowingClouds = PP.RetrieveBool("IsShowingClouds", true);
            xpoints.Sensitivity = PP.RetrieveInt("Sensitivity", 50);
            xpoints.MisalignmentSensitivity = PP.RetrieveInt("MisalignmentSensitivity", 35);
            FrameRatePercentage = PP.RetrieveInt("FrameRatePercentage", 100);
            if (FrameRatePercentage == 0)
            {
                FrameRatePercentage = 100;
            }
            HistogramEqualizeImages = PP.RetrieveBool("HistogramEqualizeImages", mCameras.HistogramEqualizeImages);
            IsExternallyTriggered = false;
            AutoAdjustLightCoolness = true;
            //the default setting
            markers.AngularDotProductToleranceDeg = PP.RetrieveDouble("AngularDotProductToleranceDeg", 8);
            StrobSignalDuration = PP.RetrieveDouble("StrobSignalDuration", 1);
            StrobSignalDelay = PP.RetrieveDouble("StrobSignalDelay", 0.5);
            TTCalib_Offset_X = PP.RetrieveDouble("TTCalibratorOffset_X", 0);
            TTCalib_Offset_Y = PP.RetrieveDouble("TTCalibratorOffset_Y", 0);
            TTCalib_Offset_Z = PP.RetrieveDouble("TTCalibratorOffset_Z", 0);
            TTCalib_Offset_RX = PP.RetrieveDouble("TTCalibratorOffset_RX", 0);
            TTCalib_Offset_RY = PP.RetrieveDouble("TTCalibratorOffset_RY", 0);
            TTCalib_Offset_RZ = PP.RetrieveDouble("TTCalibratorOffset_RZ", 0);

            //Load the marker templates from files
            if ((!Directory.Exists(TemplatesFolderPath)))
            {
                MessageBox.Show("Templates folder " + TemplatesFolderPath + " does not exist. Creating empty folder.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Directory.CreateDirectory(TemplatesFolderPath);
            }
            RefreshMarkerTemplates();
            RefreshCloudTemplates();

            markers.ExtrapolatedFrames = PP.RetrieveInt("ExtrapolatedFrames", 0);

            //Set initial processing/display settings from the UI controls
            markers.AutoAdjustCam2CamRegistration = mnuCamCamRegistration.Checked;
            IsShowingDistancesAndAngles = mnuDistancesAngles.Checked;
            IsShowingMarkersPositions = mnuMarkersPositions.Checked;
            IsShowingMarkersPositionsAtRefSpace = mnuPositionsAtRefSpace.Checked;

            IsShowingXPointsPositions = mnuXPointsPositions.Checked;
            IsShowingCloudsPositions = mnuCloudsPositions.Checked;
            IsShowingConnections = mnuDisplayConnections.Checked;
            IsShowingMagnifiedToolTip = mnuMagnifiedToolTip.Checked;
            MarkersProcessingEnabled = mnuMarkersProcessing.Checked;
            XPointsProcessingEnabled = mnuXPointsProcessing.Checked;
            CloudsProcessingEnabled = mnuCloudsProcessing.Checked;

            mnuMarkerTemplates.Enabled = MarkersProcessingEnabled | CloudsProcessingEnabled;
            mnuPoseRecorder.Enabled = MarkersProcessingEnabled;

            AutoExposure = mnuAutoExposure.Checked;
            FiducialVectorMaxLength = 2 * (markers.SmallerXPFootprint ? 8 : 10);
            ShownVectorsMinLength = (markers.SmallerXPFootprint ? 8 : 10);

            //Locate and set up the camera
            if ((!SetupCameras()))
            {
                if (MessageBox.Show("Failed to activate camera." + System.Environment.NewLine + "Do you want troubleshooting advice?", "MT Demo", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    CameraProblemsFrm = new CameraProblemsForm(this);
                    ShowAuxiliaryForm(CameraProblemsFrm, false);
                }
                mnuDataCapture.Checked = false;
                return;
            }

            HalfSizeImageDisplay = PP.RetrieveBool("HalfSizeImageDisplay", CurrCamera.Xres > 700);

            CaptureEnabled = mnuDataCapture.Checked;

            CurrCamera.TriggerMode = false;
            IsShowingVectorsForTemplateRegistration = false;

            //Start added by Mei
            frmpositionrecorder = new PositionRecorderForm(this);
            ShowAuxiliaryForm(frmpositionrecorder, false);

            this.KeyPress += new KeyPressEventHandler(keyPressEvent);
            //End added by Mei

            UpdateUI();

            UdpNetworkClientManager(port);
        }

        private bool SetupCameras()
        {
            string ErrStr = "";
            try
            {
                mCameras.AttachAvailableCameras(calibrationFilesDir);
            }
            catch (Exception ex)
            {
                ErrStr = ex.Message;
            }

            if (ErrStr.Contains("Cannot open calibration file"))
            {
                MessageBox.Show(ErrStr, "Error", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
            }

            //failed to find any camera
            if (mCameras.Count == 0 || !string.IsNullOrEmpty(ErrStr))
            {

                return false;
            }

            CurrCamera = mCameras[0];
            string FirmwareVersion = null;
            FirmwareVersion = CurrCamera.FirmwareVersion;
            if ((FirmwareVersion == "0.0.3.33" | FirmwareVersion == "0.9.0.44" | FirmwareVersion == "0.9.0.12" | FirmwareVersion == "0.9.1.28" | FirmwareVersion == "0.9.1.38" | FirmwareVersion == "0.9.1.40" | FirmwareVersion == "0.9.1.44" | FirmwareVersion == "0.9.2.44" | FirmwareVersion == "0.9.3.44" | FirmwareVersion == "0.9.0.14"))
            {
                mnuHdrMode.Visible = true;
                HdrEnabled = false;
            }

            // Now set the CurrCamera as a reference space for all template markers
            IList<Marker> markerList = markers.GetTemplateMarkers();
            foreach (Marker m in markerList)
            {
                m.ReferenceCamera = CurrCamera;
            }


            //first, clean up any previous controls
            while (menuStrip1.Items.Count > 4)
            {
                menuStrip1.Items[menuStrip1.Items.Count - 1].Dispose();
            }

            //show the attached cameras in the UI
            int i = 0;
            while (i < mCameras.Count)
            {
                Camera cam = mCameras[i];
                string btnText = cam.SerialNumber.ToString() + " " + cam.Xres.ToString() + "x" + cam.Yres.ToString();
                ToolStripMenuItem mnuCam = new ToolStripMenuItem(btnText);
                mnuCam.Click += mnuCam_Click;
                menuStrip1.Items.Add(mnuCam);
                i += 1;
            }

            SelectCamera(0);
            mnuCamCamRegistration.Enabled = mCameras.Count > 1; //Meaningful when there is more than one camera
            return true;
        }

        private void SelectCamera(int index)
        {
            index = Utils.Constrain(0, index, mCameras.Count - 1);
            //make this camera the 'current' one and show its button checked
            CurrCamera = mCameras[index];
            AdjustFormToPictureSize();
            int i = 0;
            while (i < mCameras.Count)
            {
                string btnText = mCameras[i].SerialNumber.ToString() + " " + mCameras[i].Xres.ToString() + "x" + mCameras[i].Yres.ToString();
                menuStrip1.Items[i + 4].Text = btnText;
                if (i == index)
                {
                    btnText = mCameras[i].ModelName + " =>" + mCameras[i].SerialNumber.ToString() + " " + mCameras[i].Xres.ToString() + "x" + mCameras[i].Yres.ToString();
                    menuStrip1.Items[i + 4].Text = btnText;
                }
                i += 1;
            }

            vsGain.Maximum = Convert.ToInt32((10 * CurrCamera.GainFMax));
            vsGain.Minimum = Convert.ToInt32((10 * CurrCamera.GainFMin));
            vsShutter.Maximum = Convert.ToInt32((10 * CurrCamera.ShutterMsecsMax));
            vsShutter.Minimum = Convert.ToInt32((10 * CurrCamera.ShutterMsecsMin));

            // Update the Main Window based on the camera type
            if (CurrCamera.MainBoardInfo == "Bumblebee2")
            {
                mnuExternallyTrigger.Visible = true;
                mnuStrobeSignal.Visible = true;
            }
            else if (CurrCamera.Xres == 1024)
            {
                mnuExternallyTrigger.Visible = false;
                mnuStrobeSignal.Visible = false;
            }
            else
            {
                mnuExternallyTrigger.Visible = false;
                mnuStrobeSignal.Visible = false;
            }

            if (CurrCamera.sensorsnum == 1)
            {
                mnuDisplayImageRight.Visible = false;
                mnuDisplayImageLeft.Visible = true;
                mnuDisplayImageMiddle.Visible = false;
                mnuDisplayImageAll.Visible = false;
                DisplayMode = 0;
                // for the monocular systems we assume the stream coming from left sensor 
                DisplayPB_Index = 0;
                mnuXPointsProcessing.Visible = false;
            }
            else if (CurrCamera.sensorsnum == 2)
            {
                mnuDisplayImageRight.Visible = true;
                mnuDisplayImageLeft.Visible = true;
                mnuDisplayImageMiddle.Visible = false;
                mnuDisplayImageAll.Visible = false;
                if (DisplayMode == 2)
                    DisplayMode = 0;
                DisplayPB_Index = 0;
            }
            else if (CurrCamera.sensorsnum == 3)
            {
                mnuDisplayImageRight.Visible = false;
                mnuDisplayImageLeft.Visible = false;
                mnuDisplayImageMiddle.Visible = true;
                mnuDisplayImageAll.Visible = true;
                if (DisplayMode < 3)
                    DisplayMode = 2;
                DisplayPB_Index = 2;
            }
            TextLine_Increments = (int)picBox0.Font.Size + 5;

            UpdateUI();

        }

        public void RefreshMarkerTemplates()
        {
            markers.LoadTemplates(TemplatesFolderPath);
            string[] violationStr = markers.Validate();

            if (violationStr.Length != 0)
            {
                StringBuilder strBuilder = new StringBuilder(violationStr[0]);
                int i = 1;
                while (i < violationStr.Length)
                {
                    strBuilder.Append(Environment.NewLine).Append(violationStr[i]);
                    i += 1;
                }
                MessageBox.Show(strBuilder.ToString(), "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (CurrCamera != null)
            {
                // Now set the CurrCamera as a reference space for all template markers
                IList<Marker> markerList = markers.GetTemplateMarkers();
                foreach (Marker m in markerList)
                {
                    m.ReferenceCamera = CurrCamera;
                }
            }
        }

        public void RefreshCloudTemplates()
        {
            clouds.LoadTemplates(TemplatesFolderPath);
            string[] violationStr = clouds.Validate();

            if (violationStr.Length != 0)
            {
                StringBuilder strBuilder = new StringBuilder(violationStr[0]);
                int i = 1;
                while (i < violationStr.Length)
                {
                    strBuilder.Append(Environment.NewLine).Append(violationStr[i]);
                    i += 1;
                }
                MessageBox.Show(strBuilder.ToString(), "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (CurrCamera != null)
            {
                // Now set the CurrCamera as a reference space for all template markers
                IList<Cloud> cloudList = clouds.GetTemplateClouds();
                foreach (Cloud cl in cloudList)
                {
                    cl.ReferenceCamera = CurrCamera;
                }
            }


        }

        public void UpdatePP()
        {
            // PP not set because no MTHome
            if (PP == null)
            {
                return;
            }

            //updates all the persistent properties from the current state
            PP.Path = Application.StartupPath + "\\" + this.Text + ".ini";
            PP.Section = "General";
            PP.SaveInt("PredictiveFramesInterleave", markers.PredictiveFramesInterleave);
            PP.SaveInt("OverExposureControlInterleave", markers.OverExposureControlInterleave);
            PP.SaveDouble("MarkersTemplateMatchToleranceMM", markers.TemplateMatchToleranceMM);
            PP.SaveDouble("CloudsTemplateMatchToleranceMM", clouds.TemplateMatchToleranceMM);
            PP.SaveDouble("JitterFilterCoefficient", markers.JitterFilterCoefficient);
            PP.SaveDouble("GainFLimiter", mCameras.GainFLimiter);
            PP.SaveDouble("ShutterMsecsLimiter", mCameras.ShutterMsecsLimiter);
            PP.SaveDouble("LightCoolness", mCameras.LightCoolness);
            PP.SaveDouble("HDRLightCoolness", mCameras.HDRLightCoolness);
            PP.SaveBool("JitterFilter", (markers.JitterFilterEnabled ? true : false));
            PP.SaveBool("SmallerFootprint", (markers.SmallerXPFootprint ? true : false));
            PP.SaveInt("ExtrapolatedFrames", markers.ExtrapolatedFrames);
            PP.SaveBool("MarkersProcessingEnabled", MarkersProcessingEnabled);
            PP.SaveBool("XPointsProcessingEnabled", XPointsProcessingEnabled);
            PP.SaveBool("CloudsProcessingEnabled", CloudsProcessingEnabled);
            PP.SaveBool("IsShowingXPointsPositions", IsShowingXPointsPositions);
            PP.SaveBool("IsShowingCloudsPositions", IsShowingCloudsPositions);
            PP.SaveBool("IsShowingXPoints", IsShowingXPoints);
            PP.SaveBool("IsShowingClouds", IsShowingClouds);
            PP.SaveInt("Sensitivity", xpoints.Sensitivity);
            PP.SaveInt("MisalignmentSensitivity", xpoints.MisalignmentSensitivity);
            PP.SaveDouble("AngularDotProductToleranceDeg", markers.AngularDotProductToleranceDeg);
            PP.SaveInt("FrameRatePercentage", (int)this.FrameRatePercentage);
            PP.SaveBool("TemplateBasedWarmUpCorrection", markers.TemplateBasedWarmupCorrection);
            PP.SaveDouble("LightCoolness", mCameras.LightCoolness);
            PP.SaveDouble("StrobSignalDuration", StrobSignalDuration);
            PP.SaveDouble("StrobSignalDelay", StrobSignalDelay);
            PP.SaveDouble("TTCalibratorOffset_X", TTCalib_Offset_X);
            PP.SaveDouble("TTCalibratorOffset_Y", TTCalib_Offset_Y);
            PP.SaveDouble("TTCalibratorOffset_Z", TTCalib_Offset_Z);
            PP.SaveDouble("TTCalibratorOffset_RX", TTCalib_Offset_RX);
            PP.SaveDouble("TTCalibratorOffset_RY", TTCalib_Offset_RY);
            PP.SaveDouble("TTCalibratorOffset_RZ", TTCalib_Offset_RZ);

        }

        public void AdjustLightCoolnessByCoolCard()
        {
            //Uses the "RGB vector" in a special marker whose name starts with "CoolCard",
            //if present, to adjust the light coolness setting of all cameras
            Vector ColorLongVector = null;
            Vector ColorShortVector = null;
            //Set the markerList to contain all the identified markers

            IList<Marker> markerList = markers.GetIdentifiedMarkers(CurrCamera);
            int markerNum = 0;
            while (markerNum < markerList.Count)
            {
                Marker M = markerList[markerNum];
                if (M.WasIdentified(CurrCamera))
                {
                    if (M.Name.Length > 3)
                    {
                        if (M.Name.Substring(0, 4).ToUpper() == "COOL")
                        {
                            //Get the first vector of the first facet identified.
                            Facet Ft = M.GetIdentifiedFacets(CurrCamera, true)[0];
                            Ft.GetIdentifiedVectors(out ColorLongVector, out ColorShortVector);
                        }
                    }
                }
                markerNum += 1;
            }

            //found it - use it
            if ((ColorLongVector != null))
            {
                try
                {
                    CurrCamera.AdjustLightCoolnessFromColorVector(ColorLongVector, 0);
                    //only supported profile 0 = duramark CoolCard

                }
                catch
                {
                }
                //Note that if it does not contain the color patches, the light coolness is not adjusted.
                mCameras.LightCoolness = CurrCamera.LightCoolness;
                //copy to all cameras
            }
        }

        private void StartRecording(string filePath)
        {
            //create a new AVI file
            Bitmap bitmap = new Bitmap(picBox0.Width, picBox0.Height);
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            picBox0.DrawToBitmap(bitmap, rect);
            aviManager = new AviManager(filePath, false);
            //Application.StartupPath + "\test.avi"
            aviStream = aviManager.AddVideoStream(true, 2, bitmap);
            //add a new video stream and one frame to the new file

            recordPanel.Visible = true;
            bitmap.Dispose();
            timerAviSaver.Enabled = true;
        }

        private void StopRecording()
        {
            aviManager.Close();
            timerAviSaver.Enabled = false;
            recordPanel.Visible = false;
        }

        private void AddFrame()
        {
            Bitmap bitmap = new Bitmap(picBox0.Width, picBox0.Height);
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            picBox0.DrawToBitmap(bitmap, rect);
            aviStream.AddFrame(bitmap);
            bitmap.Dispose();
        }

        //Multimedia timer
        public void mmTimer_Tick(System.Object sender, System.EventArgs e)
        {
            if (!mmTimer.IsRunning || !CaptureEnabled)
            {
                return;
            }

            if (mtTimerInterval < 1 || mtTimerInterval > 1000)
            {
                mmTimer.Period = 100;
            }
            else
            {
                mmTimer.Period = mtTimerInterval;
            }

            // to obtain the process time
            intervalsetting.Reset();
            intervalsetting.Start();

            if (!SWFPS.IsRunning)
            {
                SWFPS.Start();
            }

            int FramesGrabbed = 0;
            try
            {
                FramesGrabbed = CurrCamera.FramesGrabbed;
            }
            catch
            {
                exitdemo(1);
            }

            string ErrorStr = "";
            // When background processing is enabled, the "grab frame" method has already been invoked in the background.
            if (MT.Markers.BackGroundProcess == false)
            {
                try
                {
                    mCameras.GrabFrame();
                }
                catch (Exception e1)
                {
                    ErrorStr = e1.Message;
                }
            }

            if ((MT.Markers.BackGroundProcess == true) && (Prev_Frame_No == FramesGrabbed))
            {
                //the results of this frame has already been displayed
                return;
            }
            else
            {
                try
                {
                    Prev_Frame_No = CurrCamera.FramesGrabbed;
                }
                catch
                {
                    exitdemo(1);
                }
                if (MT.Markers.BackGroundProcess)
                {
                    MT.Markers.GetIdentifiedMarkersFromBackgroundThread(null);
                    MT.XPoints.GetIdentifiedXPointsFromBackgroundThread(null);
                }
            }

            //So the grab process encountered an error
            if (!string.IsNullOrEmpty(ErrorStr))
            {
                if (mCameras.Count == 0)
                {
                    ErrorMessagenumber = 1;
                    //No camera connected!
                    exitdemo(ErrorMessagenumber);
                    //Exits the program and shows the appropriate messages
                }
                //So there is at least one camera plugged in

                Boolean IsExternalTriggeredMode = false;
                try
                {
                    IsExternalTriggeredMode = CurrCamera.TriggerMode;
                }
                catch
                {
                    ErrorMessagenumber = 1;
                    //No camera connected!
                    exitdemo(ErrorMessagenumber);
                    //Exits the program and shows the appropriate messages
                }

                if (IsExternalTriggeredMode == false && !ErrorStr.Equals("Grab frame time out"))
                {
                    CaptureEnabled = false;
                    //Turn off further capturing
                    SetupCameras();
                    //perhaps cameras got added or removed: try again after resetting
                    ErrorStr = "";
                    try
                    {
                        mCameras.GrabFrame();
                    }
                    catch (Exception e1)
                    {
                        ErrorStr = e1.Message;
                    }
                    //Something is wrong with camera
                    if (!string.IsNullOrEmpty(ErrorStr) && !ErrorStr.Equals("Grab frame time out"))
                    {
                        ErrorMessagenumber = 2;
                        //Camera Failed
                        exitdemo(ErrorMessagenumber);
                        //Exits the program and shows the appropriate messages
                    }
                    else
                    {
                        CaptureEnabled = true;
                    }
                }
                if (ErrorStr.Equals("Grab frame time out"))
                    mmTimer.Period = 10;
            }
            else
            {
                RefreshDisplay = IsStrobeSignal || IsExternallyTriggered || (MarkersProcessingEnabled && (IsShowingMarkersPositions || IsShowingDistancesAndAngles)) || XPointsProcessingEnabled || CloudsProcessingEnabled;
                if (MarkersProcessingEnabled || XPointsProcessingEnabled || CloudsProcessingEnabled)
                {
                    try
                    {
                        ProcessCurrFrame(DisplayMode);
                    }
                    catch
                    {
                        exitdemo(2);
                        //Exits the program and shows the appropriate messages
                    }
                }
                else
                {
                    string str = null;
                    TextLine = 1;
                    try
                    {
                        CurrCamera.AutoExposure = AutoExposure;
                        //simple automatic exposure without frame processing
                        DisplayImages(true, DisplayMode);
                    }
                    catch
                    {
                        ErrorMessagenumber = 1;
                        //No camera connected!
                        exitdemo(ErrorMessagenumber);
                        //Exits the program and shows the appropriate messages
                    }
                    PictureBox pictBox = (DisplayPB_Index == 2 ? PicBox2 : (DisplayPB_Index == 0 ? picBox0 : picBox1));
                    if (IsStrobeSignal)
                    {
                        str = "Camera Transmits Strobe Signal";
                        ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Yellow);
                        TextLine += TextLine_Increments;
                    }
                    if (IsExternallyTriggered)
                    {
                        str = "Camera is in Externally Trigger Mode";
                        ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Red);
                        TextLine += TextLine_Increments;
                    }
                }

                //'Update the FPS indicator
                //True: number of frames grabbed/processed by MTC. False: Demo Application refresh rate
                bool IsShowingProcessedfps = false;
                if (!IsShowingProcessedfps)
                {
                    Frames += 1;
                }

                if (SWFPS.ElapsedMilliseconds > 1000)
                {
                    if (Frames > 0)
                    {
                        if (IsShowingProcessedfps)
                        {
                            fps = ((CurrCamera.FramesGrabbed - Frames) * 1000 / Convert.ToDouble(SWFPS.ElapsedMilliseconds)).ToString("0.0") + "Hz";
                        }
                        else
                        {
                            fps = (Frames * 1000 / Convert.ToDouble(SWFPS.ElapsedMilliseconds)).ToString("0.0") + "Hz";
                        }
                    }
                    SWFPS.Reset();
                    SWFPS.Start();
                    if (IsShowingProcessedfps)
                    {
                        Frames = CurrCamera.FramesGrabbed;
                    }
                    else
                    {
                        Frames = 0;
                    }
                    if (!RefreshDisplay & DisplayMode == -1)
                    {
                        picBox0.Refresh();
                    }
                }

                //Update the Timer Interval
                if (IsExternallyTriggered)
                {
                    mtTimerInterval = 10;
                }
                else
                {
                    string CameraModel = "";
                    try
                    {
                        CameraModel = CurrCamera.ModelName;
                    }
                    catch
                    {
                        ErrorMessagenumber = 1;
                        //No camera connected!
                        exitdemo(ErrorMessagenumber);
                        //Exits the program and shows the appropriate messages
                    }

                    int CameraMaxFps = 10;
                    if (CameraModel.Equals("GS60"))
                        CameraMaxFps = 200;
                    else if (CameraModel.Equals("H40") || CameraModel.Equals("H60"))
                        CameraMaxFps = 15;
                    else if (CameraModel.Equals("Hx40") || CameraModel.Equals("Hx60"))
                        CameraMaxFps = 20;
                    else if (CameraModel.Equals("S60"))
                        CameraMaxFps = 30;
                    else if (CameraModel.Equals("Sx60"))
                        CameraMaxFps = 48;
                    else if (CameraModel.Equals("H3-60"))
                        CameraMaxFps = 16;

                    mtTimerInterval = (int)Math.Max(intervalsetting.ElapsedMilliseconds * (1.2 + 1 / (1 + FramesGrabbed)), 1000 / (CameraMaxFps * FrameRatePercentage / 100));
                    if ((PoseRecorderEnabled))
                    {
                        mtTimerInterval = Math.Max(mtTimerInterval, frmpositionrecorder.PoseRecorderIntervalsGet);
                    }
                }
            }

            //Added by huimei
            //this.PosRecorder();
            //End added by huimei
        }

        private void AdjustFormToPictureSize()
        {
            if (CurrCamera == null)
                return;
            //Shows the Left Image
            if (DisplayMode == 0)
            {
                picBox0.Visible = true;
                picBox0.Top = ToolstripHeight;
                picBox0.Left = 1;
                picBox1.Visible = false;
                PicBox2.Visible = false;
            }
            //Shows the Right Image
            if (DisplayMode == 1)
            {
                picBox1.Visible = true;
                picBox1.Top = ToolstripHeight;
                picBox1.Left = 1;
                picBox0.Visible = false;
                PicBox2.Visible = false;
            }
            //Shows the Middle Image
            if (DisplayMode == 2)
            {
                PicBox2.Visible = true;
                PicBox2.Top = ToolstripHeight;
                PicBox2.Left = 1;
                picBox0.Visible = false;
                picBox1.Visible = false;
            }
            //Shows the Stereo Images
            if (DisplayMode == 3)
            {
                picBox0.Visible = true;
                picBox1.Visible = true;
                PicBox2.Visible = false;
                picBox0.Top = ToolstripHeight;
                picBox0.Left = 1;
                picBox1.Top = ToolstripHeight;
                picBox1.Left = picBox0.Left + (int)(CurrCamera.Xres * DisplayScale + 5);
                if (CurrCamera.sensorsnum == 3)
                {
                    PicBox2.Visible = true;
                    picBox1.Top = ToolstripHeight;
                    picBox1.Left = (int)(CurrCamera.Xres * DisplayScale + 5);
                    PicBox2.Top = picBox0.Top + (int)(CurrCamera.Yres * DisplayScale + 5);
                    PicBox2.Left = (int)(CurrCamera.Xres * DisplayScale / 2);
                }
            }

            //Doesn't show Images but shows the results
            if (DisplayMode == -1)
            {
                picBox0.Visible = true;
                picBox1.Visible = false;
                PicBox2.Visible = false;
                picBox0.Top = ToolstripHeight;
                picBox0.Left = 1;
                if (CurrCamera.sensorsnum == 3)
                {
                    picBox0.Visible = false;
                    PicBox2.Visible = true;
                    PicBox2.Top = ToolstripHeight;
                    PicBox2.Left = 1;
                }
            }
            if (this.WindowState == FormWindowState.Normal)
            {
                picBox0.Width = (int)(CurrCamera.Xres * DisplayScale);
                picBox1.Width = (int)(CurrCamera.Xres * DisplayScale);
                PicBox2.Width = (int)(CurrCamera.Xres * DisplayScale);
                picBox0.Height = (int)(CurrCamera.Yres * DisplayScale);
                picBox1.Height = (int)(CurrCamera.Yres * DisplayScale);
                PicBox2.Height = (int)(CurrCamera.Yres * DisplayScale);


                //Single image, either left or right or middle or even no image
                if ((DisplayMode < 3))
                {
                    this.Width = picBox0.Left + (int)(CurrCamera.Xres * DisplayScale + 20);
                    this.Height = picBox0.Top + (int)(CurrCamera.Yres * DisplayScale + 65);
                    //show all of the images
                }
                else if ((DisplayMode == 3))
                {
                    if (CurrCamera.sensorsnum == 2)
                    {
                        this.Width = picBox1.Left + (int)(CurrCamera.Xres * DisplayScale + 20);
                        this.Height = picBox0.Top + (int)(CurrCamera.Yres * DisplayScale + 65);
                    }
                    else if (CurrCamera.sensorsnum == 3)
                    {
                        this.Width = picBox1.Left + (int)(CurrCamera.Xres * DisplayScale + 20);
                        this.Height = PicBox2.Top + (int)(CurrCamera.Yres * DisplayScale + 65);
                    }
                }
            }

            if (!mnuAutoExposure.Checked)
            {
                panelExposure1.Left = this.Width - 20;
                panelExposure1.Top = ToolstripHeight;
                this.Width = this.Width + panelExposure1.Width;
            }
            mFontSize = 9 + (int)(CurrCamera.Xres * DisplayScale / 150);
            TextLine_Increments = 10 + mFontSize;

            labelCaptureDisabled.Left = Convert.ToInt32((picBox0.Left + CurrCamera.Xres * DisplayScale + 10));
            recordPanel.Left = Convert.ToInt32((picBox0.Left + CurrCamera.Xres * DisplayScale - recordPanel.Width));
            this.Refresh();
        }

        #region ProcessCurrFrame
        public double ProcessCurrFrame()
        {
            return ProcessCurrFrame(0);
        }

        public double ProcessCurrFrame(int Displaymode)
        {
            double ProcessCurrFrameMsec = 0;
            TextLine = 1;
            ProcessCurrFrame_SW.Reset();
            ProcessCurrFrame_SW.Start();

            if ((MT.Markers.BackGroundProcess == false))
            {
                if (MarkersProcessingEnabled)
                    markers.AutoAdjustCameraExposure = AutoExposure;
                if (XPointsProcessingEnabled)
                    xpoints.AutoAdjustCameraExposure = AutoExposure;
                if (MarkersProcessingEnabled)
                    markers.ProcessFrame(null);
                if (XPointsProcessingEnabled)
                    xpoints.ProcessFrame(null);
                if (CloudsProcessingEnabled)
                {
                    try
                    {
                        clouds.ProcessFrame(null);
                    }
                    catch { }

                }
                ProcessCurrFrameMsec = ProcessCurrFrame_SW.ElapsedMilliseconds;
            }

            if (HdrEnabled && CurrCamera.ModelName.Equals("Sx60"))
            {
                CurrShutterMs = CurrCamera.FrameEmbeddedShutterMsecs;
                //frame embedded shutter time updates its values more faster than shutter_registry_value
                CurrGainF = CurrCamera.FrameEmbeddedGainF;
                //Save for overlay display, since it may be modified later by auto-exposure
            }
            else
            {
                CurrShutterMs = CurrCamera.ShutterMsecs;
                CurrGainF = CurrCamera.GainF;
            }

            if (HdrEnabled)
                CurrHDR_CycleIndex = CurrCamera.HdrFrameIndexInCycle;
            DisplayPB_Index = (CurrCamera.sensorsnum == 3 ? 2 : 0);

            if (AutoAdjustLightCoolness)
            {
                AdjustLightCoolnessByCoolCard();
            }

            DisplayImages(false, Displaymode);
            //To the back buffer - don't refresh until all overlays were drawn

            if ((frmTrace != null))
                frmTrace.ShowTraceOverlay();

            if (MarkersProcessingEnabled && (IsShowingMarkersPositions || IsShowingDistancesAndAngles))
            {
                try
                {
                    ShowIdentifiedMarkers();
                    ShowUnidentifiedVectors();
                    //as well (if toggled on)
                }
                catch
                {
                    exitdemo(1);
                }
            }

            if (XPointsProcessingEnabled && IsShowingXPoints)
            {
                ShowDetectedXPoints();
            }

            if (CloudsProcessingEnabled)
            {
                ShowIdentifiedClouds();
            }

            //If the position recorder form was enabled then record it.
            if (frmpositionrecorder != null)
            {
                frmpositionrecorder.PoseRecorder();
            }
            if (PoseRecorderEnabled)
            {
                frmpositionrecorder.RecordPoseData();
            }


            //Now Refresh the PictureBoxes
            if (Displaymode == 0)
            {
                picBox0.Refresh();
            }
            else if (Displaymode == 1)
            {
                picBox1.Refresh();
            }
            else if (Displaymode == 2)
            {
                PicBox2.Refresh();
            }
            else if ((Displaymode == 3))
            {
                picBox0.Refresh();
                picBox1.Refresh();
                if (CurrCamera.sensorsnum == 3)
                {
                    PicBox2.Refresh();
                }
            }

            return ProcessCurrFrameMsec;
        }
        #endregion

        #region Display related part
        public void DisplayImages(bool RefreshPBs)
        {
            DisplayImages(RefreshPBs, -1);
        }

        public void DisplayImages()
        {
            DisplayImages(false, -1);
        }

        private Bitmap ImageL = null;
        private Bitmap ImageR = null;
        private Bitmap ImageM = null;

        public void DisplayImages(bool RefreshPBs, int SideI)
        {
            if (CurrCamera == null)
                return;
            try
            {
                if (CurrCamera.FramesGrabbed == 0)
                    return;
            }
            catch
            {
                exitdemo(1);
            }

            PictureBox pictBox = null;
            //DisplayMode = -1 = No images
            if (DisplayMode == -1)
            {
                //Is it necessary to clean the picture boxes - It is time consuming process!
                if (RefreshDisplay)
                {
                    pictBox = (DisplayPB_Index == 2 ? PicBox2 : (DisplayPB_Index == 0 ? picBox0 : picBox1));
                    pictBox.Image = null;
                    pictBox.Refresh();
                    goto showtexts;
                }
                else
                {
                    return;
                }
            }


            //access the image buffer
            if (HalfSizeImageDisplay)
            {
                if (CurrCamera.sensorsnum == 3)
                {
                    CurrCamera.Get24BitHalfSizeImages3(ref ImageL, ref ImageR, ref ImageM);
                }
                else
                {
                    CurrCamera.Get24BitHalfSizeImages(ref ImageL, ref ImageR);
                }
            }
            else
            {
                if (CurrCamera.sensorsnum == 3)
                {
                    CurrCamera.Get24BitImages3(ref ImageL, ref ImageR, ref ImageM);
                }
                else
                {
                    CurrCamera.Get24BitImages(ref ImageL, ref ImageR);
                }
            }

            if (DisplayMirrorImage)
            {
                ImageL.RotateFlip(RotateFlipType.RotateNoneFlipX);
                ImageR.RotateFlip(RotateFlipType.RotateNoneFlipX);
                if (CurrCamera.sensorsnum == 3)
                {
                    ImageM.RotateFlip(RotateFlipType.RotateNoneFlipX);
                }
            }

            if (ImageL == null)
                return;
            //nothing to display
            //Show the Left image
            if (DisplayMode == 0)
            {
                picBox0.Image = ImageL;
            }
            //Show the Right image
            if (DisplayMode == 1)
            {
                picBox1.Image = ImageR;
            }
            //Show the Middle image
            if (DisplayMode == 2)
            {
                PicBox2.Image = ImageM;
            }
            //Show all of the images
            if (DisplayMode == 3)
            {
                picBox0.Image = ImageL;
                picBox1.Image = ImageR;
                if (CurrCamera.sensorsnum == 3)
                {
                    PicBox2.Image = ImageM;
                }
            }
            showtexts:

            // Update Information on the picture boxes
            string str = null;
            pictBox = (DisplayPB_Index == 2 ? PicBox2 : (DisplayPB_Index == 0 ? picBox0 : picBox1));

            if (HdrEnabled)
            {
                str = "HDR (" + Math.Round(CurrCamera.HdrMinLux, 0) + "..Lux) #" + CurrHDR_CycleIndex + "/" + CurrCamera.HdrCycleLength;
                ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Red);
                TextLine += TextLine_Increments;
            }

            if (IsStrobeSignal)
            {
                str = "Camera Transmits Strobe Signal";
                ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Yellow);
                TextLine += TextLine_Increments;
            }
            if (IsExternallyTriggered)
            {
                str = "Camera is in Externally Trigger Mode";
                ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Red);
                TextLine += TextLine_Increments;
            }

            if (CurrCamera.ThermalHazardCode != 0)
            {
                ShowTextWithShadow(CurrCamera.ThermalHazardText, pictBox, 3, TextLine, Color.Yellow);
                TextLine += TextLine_Increments;
            }

            if (IsShowingExposure)
            {
                if (CurrCamera.HdrEnabled)
                {
                    double[] ShutterMsec = new double[5];
                    double[] GainF = new double[5];
                    int hdrFrameIndex = 0;
                    for (hdrFrameIndex = 0; hdrFrameIndex <= CurrCamera.HdrCycleLength - 1; hdrFrameIndex++)
                    {
                        CurrCamera.HdrCycledShutterGet(out ShutterMsec[hdrFrameIndex], hdrFrameIndex);
                        CurrCamera.HdrCycledGainGet(out GainF[hdrFrameIndex], hdrFrameIndex);
                    }
                    str = "Shutter = [" + Math.Round(ShutterMsec[0], 2) + ", " + Math.Round(ShutterMsec[1], 2) + ", " + Math.Round(ShutterMsec[2], 2) + ", " + Math.Round(ShutterMsec[3], 3) + "]ms";
                    str = str + ", GainF = [" + Math.Round(GainF[0], 2) + ", " + Math.Round(GainF[1], 2) + ", " + Math.Round(GainF[2], 2) + ", " + Math.Round(GainF[3], 2) + "]";
                }
                else
                {
                    str = "GainF " + Math.Round(CurrGainF, 2) + ", Shutter " + Math.Round(CurrShutterMs, 3) + "ms";
                }
                //add Lux
                if (markers.GetIdentifiedMarkers(null).Count >= 1)
                {
                    double LuxF = 0;
                    //Light intensity at an exposure value of 1
                    if (CurrCamera.ModelName.Equals("H40"))
                    {
                        LuxF = 10000;
                        //H60 is 2 times more sensitive to light
                    }
                    else if (CurrCamera.ModelName.Equals("H60"))
                    {
                        LuxF = 5000;
                        //S60 is 2.5 times more sensitive to light
                    }
                    else if (CurrCamera.ModelName.Equals("S60"))
                    {
                        LuxF = 4000;
                    }
                    else
                    {
                        LuxF = 4000;
                    }
                    double Exposure = CurrShutterMs * CurrGainF;
                    if ((Exposure > 0 & !CurrCamera.HdrEnabled))
                        str = str + " (" + Convert.ToInt64(LuxF / (Exposure)) + " Lux)";

                }
                ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Lime);
                TextLine = TextLine + TextLine_Increments;
                str = "Light Coolness: " + Math.Round(CurrCamera.LightCoolness, 2);
                if (HdrEnabled)
                {
                    str = "Light Coolness: (Ambient: " + Math.Round(CurrCamera.LightCoolness, 2) + " - HDR: " + Math.Round(CurrCamera.HDRLightCoolnessOR, 2) + ")";
                }
                ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Lime);
                TextLine = TextLine + TextLine_Increments;
                str = Math.Round((double)CurrCamera.SecondsFromPowerup / 60, 1).ToString() + " min. since powerup";
                ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Lime);
                TextLine = TextLine + TextLine_Increments;
            }

            if (CurrCamera.sensorsnum < 3)
            {
                for (SideI = 0; SideI <= CurrCamera.sensorsnum - 1; SideI++)
                {
                    pictBox = (SideI == 2 ? PicBox2 : (SideI == 0 ? picBox0 : picBox1));
                    ShowTextWithShadow(fps, pictBox, (int)(0.88 * pictBox.Width), (int)(0.02 * pictBox.Height), Color.Red);
                }
            }
            else if (CurrCamera.sensorsnum == 3)
            {
                ShowTextWithShadow("Left", picBox0, (int)(0.94 * picBox0.Width), (int)(ToolstripHeight + 0.02 * picBox0.Height), Color.Red);
                ShowTextWithShadow("Right", picBox1, (int)(0.92 * picBox1.Width), (int)(ToolstripHeight + 0.02 * picBox1.Height), Color.Red);
                //ShowTextWithShadow("Middle", PictureBox2, (int)(0.9 * PictureBox2.Width), (int)(ToolstripHeight + 0.02 * PictureBox2.Height), Color.Red);
                ShowTextWithShadow(fps, PicBox2, (int)(0.9 * PicBox2.Width), (int)(ToolstripHeight + 0.02 * PicBox2.Height + mFontSize + 2), Color.Red);
                if (CurrCamera.MiddleSensorEnabled)
                {
                    ShowTextWithShadow("Middle", PicBox2, (int)(0.9 * PicBox2.Width), (int)(ToolstripHeight + 0.02 * PicBox2.Height), Color.Red);
                }
                else
                {
                    ShowTextWithShadow("Middle Sensor Disable", PicBox2, (int)(0.4 * PicBox2.Width), (int)(ToolstripHeight + 0.02 * PicBox2.Height), Color.Red);
                }
            }

        }


        private void ShowDetectedXPoints()
        {
            int i = 0;
            int SideI = 0;
            XPoint XP = default(XPoint);
            int result = 0;
            string str = null;
            double[] X = new double[3];
            double[] Y = new double[3];

            PictureBox pictBox = null;
            for (SideI = 0; SideI <= CurrCamera.sensorsnum - 1; SideI++)
            {
                pictBox = (SideI == 2 ? PicBox2 : (SideI == 0 ? picBox0 : picBox1));
                pictBox.ForeColor = Color.Green;
            }

            IXPoints = xpoints.GetdetectedXPoints(null);
            pictBox = (DisplayPB_Index == 2 ? PicBox2 : (DisplayPB_Index == 0 ? picBox0 : picBox1));
            str = "Xpoints Processing: " + IXPoints.Count.ToString() + "  XPs are detected by Current Camera";
            ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Yellow);
            TextLine = TextLine + TextLine_Increments;

            for (i = 0; i <= IXPoints.Count - 1; i++)
            {
                XP = IXPoints[i];
                double[] xp3d_out = null;
                xp3d_out = XP.Position3D;
                result = XP.Position2D(out X[0], out Y[0], out X[1], out Y[1], out X[2], out Y[2]);

                //Shows Index
                bool MirrorImage = DisplayMirrorImage;
                if (IsShowingXPoints)
                {
                    for (SideI = 0; SideI <= CurrCamera.sensorsnum - 1; SideI++)
                    {
                        //Just show the XPs in the image field (H3-60)
                        if (X[SideI] > 1)
                        {
                            CirclePixel(X[SideI], Y[SideI], Color.Red, 2, SideI, MirrorImage);
                            CirclePixel(X[SideI], Y[SideI], Color.Red, 3, SideI, MirrorImage);
                            CirclePixel(X[SideI], Y[SideI], Color.Red, 4, SideI, MirrorImage);
                            if (IsShowingXPointsPositions)
                            {
                                pictBox = (SideI == 2 ? PicBox2 : (SideI == 0 ? picBox0 : picBox1));
                                ShowTextWithShadow(Convert.ToString(i), pictBox, (int)(X[SideI] * DisplayScale), (int)(Y[SideI] * DisplayScale), Color.Red);
                            }
                        }
                    }
                }

                if (IsShowingXPointsPositions)
                {
                    pictBox = (DisplayPB_Index == 2 ? PicBox2 : (DisplayPB_Index == 0 ? picBox0 : picBox1));
                    //ShowTextWithShadow(CStr(i), pictBox, X(DisplayPB_Index) * DisplayScale, Y(DisplayPB_Index) * DisplayScale, Color.Red)
                    str = "XP #" + i.ToString() + ". (";
                    str = str + xp3d_out[0].ToString("000.0") + ", ";
                    str = str + xp3d_out[1].ToString("000.0") + ", ";
                    str = str + xp3d_out[2].ToString("000.0");
                    str = str + ")";
                    TextLine = TextLine + TextLine_Increments;
                    ShowTextWithShadow(str, pictBox, 10, TextLine, Color.Yellow);
                }
            }

        }


        private void ShowDetectedSliderControlledXpoints(Marker m)
        {
            int i = 0;
            int Id = 0;
            int j = 0;
            int SideI = 0;
            XPoint XP = default(XPoint);
            int result = 0;
            string str = null;
            double[] X = new double[3];
            double[] Y = new double[3];

            PictureBox pictBox = null;
            for (SideI = 0; SideI <= CurrCamera.sensorsnum - 1; SideI++)
            {
                pictBox = (SideI == 2 ? PicBox2 : (SideI == 0 ? picBox0 : picBox1));
                pictBox.ForeColor = Color.Green;
            }

            IList<XPoint> DetectedSliderControlledXpointsGet = null;
            DetectedSliderControlledXpointsGet = m.DetectedSliderControlledXpointsGet();

            // Code snippet for retrieving template Slider Controlled Xpoints
            IList<XPoint> TemplateSliderXPoints = null;
            for (Id = 1; Id <= m.SliderControlledXpointsCount; Id++)
            {
                TemplateSliderXPoints = m.TemplateSliderControlledXpointsGet(Id);
                for (j = 0; j <= TemplateSliderXPoints.Count - 1; j++)
                {
                    XP = TemplateSliderXPoints[j];
                    double[] xp3d_out = null;
                    xp3d_out = XP.Position3D;
                }
            }

            pictBox = (DisplayPB_Index == 2 ? PicBox2 : (DisplayPB_Index == 0 ? picBox0 : picBox1));
            if ((DetectedSliderControlledXpointsGet.Count < 1))
            {
                str = "Warning: Slider XPoint is not detected";
            }
            else
            {
                str = "Slider XPoints: " + DetectedSliderControlledXpointsGet.Count.ToString() + "  detected by Current Camera";
            }
            ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Yellow);
            TextLine = TextLine + TextLine_Increments;

            Boolean MirrorImage = DisplayMirrorImage;
            for (i = 0; i <= DetectedSliderControlledXpointsGet.Count - 1; i++)
            {
                XP = DetectedSliderControlledXpointsGet[i];
                int XpointIndex = XP.Index;
                double[] xp3d_out = null;
                xp3d_out = XP.Position3D;
                result = XP.Position2D(out X[0], out Y[0], out X[1], out Y[1], out X[2], out Y[2]);

                //Shows Index
                for (SideI = 0; SideI <= CurrCamera.sensorsnum - 1; SideI++)
                {
                    //Just show the XPs in the image field (H3-60)
                    if (X[SideI] > 1)
                    {
                        CirclePixel(X[SideI], Y[SideI], Color.Lime, 2, SideI, MirrorImage);
                        CirclePixel(X[SideI], Y[SideI], Color.Lime, 3, SideI, MirrorImage);
                        CirclePixel(X[SideI], Y[SideI], Color.Lime, 4, SideI, MirrorImage);
                        if (IsShowingXPointsPositions)
                        {
                            pictBox = (SideI == 2 ? PicBox2 : (SideI == 0 ? picBox0 : picBox1));
                            ShowTextWithShadow(Convert.ToString(XpointIndex), pictBox, (int)(X[SideI] * DisplayScale), (int)(Y[SideI] * DisplayScale), Color.Lime);
                        }
                    }
                }

                if (IsShowingXPointsPositions)
                {
                    pictBox = (DisplayPB_Index == 2 ? PicBox2 : (DisplayPB_Index == 0 ? picBox0 : picBox1));
                    //ShowTextWithShadow(CStr(i), pictBox, X(DisplayPB_Index) * DisplayScale, Y(DisplayPB_Index) * DisplayScale, Color.Red)
                    str = "XP #" + XpointIndex.ToString() + ". (";
                    str = str + xp3d_out[0].ToString("000.0") + ", ";
                    str = str + xp3d_out[1].ToString("000.0") + ", ";
                    str = str + xp3d_out[2].ToString("000.0");
                    str = str + ")";
                    ShowTextWithShadow(str, pictBox, 10, TextLine, Color.Yellow);
                    TextLine = TextLine + TextLine_Increments;
                }
            }

        }


        private void ShowIdentifiedMarkers()
        {
            Marker M = null;

            double[,,,] XPs = null; //XPs are indexed (LV/SV, L/R/M, base/head, X/Y)
            int MarkerNum = 0;
            int i = 0;
            int j = 0;

            double[,,] Pos = new double[3, 2, 101];
            string str = null;

            Facet Ft = null;
            int SideI = 0;
            int ImgX = 0;

            IList<Marker> IMarkers = null;
            try
            {
                if ((markers.BackGroundProcess))
                {
                    if (!markers.IsBackgroundFrameProcessed)
                    {
                        return;
                    }
                }
                IMarkers = markers.GetIdentifiedMarkers(null);
            }
            catch
            {
                exitdemo(1); // an error occurred!
            }
            //Show all overlays on the current displayed image
            //Presentation near the Facet projection in the image
            //----------------------------------------------------
            PictureBox pictBox = (DisplayPB_Index == 2 ? PicBox2 : (DisplayPB_Index == 0 ? picBox0 : picBox1));
            while (MarkerNum < IMarkers.Count)
            {
                M = IMarkers[MarkerNum];
                if (M.WasIdentified(CurrCamera))
                {
                    //The marker was identified by the current camera - overlay it on the images
                    int ifCount = M.GetIdentifiedFacets(CurrCamera, false).Count;

                    int kk = 0;
                    while (kk < ifCount)
                    {
                        try
                        {
                            Ft = M.GetIdentifiedFacets(CurrCamera, false)[kk];
                            XPs = Ft.GetIdentifiedXPoints(CurrCamera);
                        }
                        catch
                        {
                            exitdemo(1);
                        }

                        SideI = LeftI;
                        while (SideI < CurrCamera.sensorsnum)
                        {
                            //draw the long vector, base->head and then the short one
                            if (IsShowingVectors)
                            {
                                DrawLine(XPs[0, SideI, 0, 0], XPs[0, SideI, 0, 1], XPs[0, SideI, 1, 0], XPs[0, SideI, 1, 1], LongVectorColor, SideI, DashStyle.Solid, true, false, DisplayMirrorImage);
                                double arrow_length = 0.16;
                                double arrow_angle = 0.4;
                                double l = Math.Sqrt(Math.Pow((XPs[0, SideI, 0, 0] - XPs[0, SideI, 1, 0]), 2) + Math.Pow((XPs[0, SideI, 0, 1] - XPs[0, SideI, 1, 1]), 2));
                                double RX = (XPs[0, SideI, 0, 0] - XPs[0, SideI, 1, 0]) / l * (arrow_length * l);
                                double RY = (XPs[0, SideI, 0, 1] - XPs[0, SideI, 1, 1]) / l * (arrow_length * l);
                                double RX1 = Math.Cos(-arrow_angle) * RX + Math.Sin(-arrow_angle) * RY;
                                double RY1 = -Math.Sin(-arrow_angle) * RX + Math.Cos(-arrow_angle) * RY;
                                double RX2 = Math.Cos(arrow_angle) * RX + Math.Sin(arrow_angle) * RY;
                                double RY2 = -Math.Sin(arrow_angle) * RX + Math.Cos(arrow_angle) * RY;
                                DrawLine(XPs[0, SideI, 1, 0], XPs[0, SideI, 1, 1], XPs[0, SideI, 1, 0] + RX1, XPs[0, SideI, 1, 1] + RY1, LongVectorColor, SideI, DashStyle.Solid, true, false, DisplayMirrorImage);
                                DrawLine(XPs[0, SideI, 1, 0], XPs[0, SideI, 1, 1], XPs[0, SideI, 1, 0] + RX2, XPs[0, SideI, 1, 1] + RY2, LongVectorColor, SideI, DashStyle.Solid, true, false, DisplayMirrorImage);

                                DrawLine(XPs[1, SideI, 0, 0], XPs[1, SideI, 0, 1], XPs[1, SideI, 1, 0], XPs[1, SideI, 1, 1], ShortVectorColor, SideI, DashStyle.Solid, true, false, DisplayMirrorImage);
                                l = Math.Sqrt(Math.Pow((XPs[1, SideI, 0, 0] - XPs[1, SideI, 1, 0]), 2) + Math.Pow((XPs[1, SideI, 0, 1] - XPs[1, SideI, 1, 1]), 2));
                                RX = (XPs[1, SideI, 0, 0] - XPs[1, SideI, 1, 0]) / l * (arrow_length * l);
                                RY = (XPs[1, SideI, 0, 1] - XPs[1, SideI, 1, 1]) / l * (arrow_length * l);
                                RX1 = Math.Cos(-arrow_angle) * RX + Math.Sin(-arrow_angle) * RY;
                                RY1 = -Math.Sin(-arrow_angle) * RX + Math.Cos(-arrow_angle) * RY;
                                RX2 = Math.Cos(arrow_angle) * RX + Math.Sin(arrow_angle) * RY;
                                RY2 = -Math.Sin(arrow_angle) * RX + Math.Cos(arrow_angle) * RY;
                                DrawLine(XPs[1, SideI, 1, 0], XPs[1, SideI, 1, 1], XPs[1, SideI, 1, 0] + RX1, XPs[1, SideI, 1, 1] + RY1, ShortVectorColor, SideI, DashStyle.Solid, true, false, DisplayMirrorImage);
                                DrawLine(XPs[1, SideI, 1, 0], XPs[1, SideI, 1, 1], XPs[1, SideI, 1, 0] + RX2, XPs[1, SideI, 1, 1] + RY2, ShortVectorColor, SideI, DashStyle.Solid, true, false, DisplayMirrorImage);

                            }

                            //draw circle around XPs
                            if (IsShowingXPoints)
                            {
                                for (int VI = 0; VI <= 1; VI++)
                                {
                                    for (int EndI = 0; EndI <= 1; EndI++)
                                    {
                                        CirclePixel(XPs[VI, SideI, EndI, 0], XPs[VI, SideI, EndI, 1], XPointsOverlayColor, 3, SideI, DisplayMirrorImage);
                                    }
                                }
                            }
                            SideI += 1;
                        }

                        //Show the facet's name near the center of the long vector
                        if (IsShowingMarkersPositions | IsShowingDistancesAndAngles)
                        {
                            Pos[DisplayPB_Index, 0, MarkerNum] = (XPs[0, DisplayPB_Index, 0, 0] + XPs[0, DisplayPB_Index, 1, 0]) / 2 + 2 * M.GetFacetIndex(Ft);
                            Pos[DisplayPB_Index, 1, MarkerNum] = (XPs[0, DisplayPB_Index, 0, 1] + XPs[0, DisplayPB_Index, 1, 1]) / 2 + 2 * M.GetFacetIndex(Ft);
                            str = M.Name;
                            if (M.TemplateFacets.Count > 1)
                            {
                                str = str + "/" + (M.GetFacetIndex(Ft) + 1).ToString();
                            }
                            str = (MarkerNum + 1).ToString() + ". " + str;
                            ImgX = Convert.ToInt32((Math.Round((Pos[DisplayPB_Index, 0, MarkerNum] - 30) * DisplayScale)));
                            if (DisplayMirrorImage)
                            {
                                ImgX = pictBox.Width - ImgX;
                            }
                            ShowTextWithShadow(str, pictBox, ImgX, Convert.ToInt32(((Pos[DisplayPB_Index, 1, MarkerNum] - 10) * DisplayScale)), Color.Yellow);
                        }
                        kk += 1;
                    }
                }
                MarkerNum += 1;
            }

            //presentation at the upper left corner
            //(to cover all markers identified by any camera)
            //---------------------------------------------------
            Xform3D Marker2CurrCameraXf = null;

            long polycount = 0;
            double[,] polyline = null;
            if (IsShowingMarkersPositions)
            {
                if (IMarkers.Count > 0)
                {
                    double[] NewPos = null;
                    if (IsShowingConnections)
                    {
                        polyline = new double[3, 200];
                        polycount = 0;
                    }

                    MarkerNum = 0;
                    while (MarkerNum < IMarkers.Count)
                    {
                        M = IMarkers[MarkerNum];
                        if (M.WasIdentified(null))
                        {
                            try
                            {
                                Marker2CurrCameraXf = M.GetMarker2CameraXf(CurrCamera);
                            }
                            catch
                            {
                                //exitdemo(1);
                            }
                            //There may be a situation where marker was identified by another camera (not CurrCamera)
                            //and the identifying camera is not registered with CurrCamera. In this case, the pose is
                            //not known in CurrCamera coordinates and Marker2CurrCameraXf is Nothing.
                            if ((Marker2CurrCameraXf != null))
                            {
                                if (IsShowingConnections)
                                {
                                    // Store marker origin
                                    NewPos = M.GetMarker2CameraXf(CurrCamera).ShiftVector;
                                    polyline[0, polycount] = NewPos[0];
                                    polyline[1, polycount] = NewPos[1];
                                    polyline[2, polycount] = NewPos[2];
                                    polycount = polycount + 1;
                                }
                                double[] xyz = null;
                                str = "";
                                if ((IsShowingMarkersPositionsAtRefSpace == true))
                                {
                                    MTInterfaceDotNet.Camera Identifiedcamera = default(MTInterfaceDotNet.Camera);
                                    xyz = M.GetMarker2ReferenceXf(CurrCamera, out Identifiedcamera).ShiftVector;
                                    if ((M.WasIdentifiedInReferenceSpace(null) == true))
                                    {
                                        string ref_name = null;
                                        try
                                        {
                                            ref_name = M.ReferenceMarker.Name + " Space";
                                        }
                                        catch
                                        {
                                            ref_name = "Camera Space";
                                            if ((Identifiedcamera != CurrCamera))
                                            {
                                                ref_name = " Cam Space  - Detected by S/N" + Identifiedcamera.SerialNumber.ToString();
                                            }
                                        }
                                        str = (MarkerNum + 1).ToString() + ". (" + xyz[0].ToString("000.0") + ", " + xyz[1].ToString("000.0") + ", " + xyz[2].ToString("000.0") + ") @ " + ref_name;
                                    }
                                    else
                                    {
                                        str = (MarkerNum + 1).ToString() + ". (Reference Coordinate System)";
                                    }
                                }
                                else
                                {
                                    xyz = M.GetMarker2CameraXf(CurrCamera).ShiftVector;
                                    str = (MarkerNum + 1).ToString() + ". (" + xyz[0].ToString("000.0") + ", " + xyz[1].ToString("000.0") + ", " + xyz[2].ToString("000.0") + ")";
                                }
                                //Show the XYZ position of the Marker's origin.
                                //If there's a tooltip, add it
                                Xform3D TT = null;
                                bool ToolTipShown = false;
                                double[] Svec = null;
                                var IsMarkerSliderControlled = M.MarkerSliderControlled;
                                if (IsMarkerSliderControlled)
                                {
                                    ShowDetectedSliderControlledXpoints(M);
                                    Svec = M.SliderControlledTooltip2MarkerXfGet(1).ShiftVector;
                                }
                                else
                                {
                                    Svec = M.Tooltip2MarkerXf.ShiftVector;
                                }
                                //non-null transform
                                if (Svec[0] != 0 || Svec[1] != 0 || Svec[2] != 0)
                                {
                                    if (IsShowingMarkersPositionsAtRefSpace)
                                    {
                                        if ((M.WasIdentifiedInReferenceSpace(CurrCamera) == true))
                                        {
                                            if (IsMarkerSliderControlled)
                                            {
                                                TT = M.SliderControlledTooltip2ReferenceXfGet(CurrCamera);
                                            }
                                            else
                                            {
                                                TT = M.Tooltip2ReferenceXf(CurrCamera);
                                            }
                                        }
                                        else
                                        {
                                            TT = new Xform3D();
                                        }
                                    }
                                    else
                                    {
                                        if (IsMarkerSliderControlled)
                                        {
                                            TT = M.SliderControlledTooltip2MarkerXfGet(1).Concatenate(M.GetMarker2CameraXf(CurrCamera));
                                        }
                                        else
                                        {
                                            TT = M.Tooltip2MarkerXf.Concatenate(M.GetMarker2CameraXf(CurrCamera));
                                        }
                                    }

                                    //Write the numerical coordinates of the tip
                                    xyz = TT.ShiftVector;
                                    str = str + " tip(" + xyz[0].ToString("000.0") + ", " + xyz[1].ToString("000.0") + ", " + xyz[2].ToString("000.0") + ")";
                                    if (IsMarkerSliderControlled)
                                    {
                                        str = str + " Fraction:" + M.SliderControlledXpointFractionGet(1).ToString("0.00");
                                    }

                                    //TT is now the tooltip->camera transform.
                                    if (IsMarkerSliderControlled)
                                    {
                                        TT = M.SliderControlledTooltip2MarkerXfGet(1).Concatenate(M.GetMarker2CameraXf(CurrCamera));
                                    }
                                    else
                                    {
                                        TT = M.Tooltip2MarkerXf.Concatenate(M.GetMarker2CameraXf(CurrCamera));
                                    }
                                    xyz = TT.ShiftVector;
                                    //If possible, show the tip and tool axis as an "enhanced reality" overlay.
                                    //TT's origin is the tool tip location in camera coordinates.
                                    //Draw an arrow pointing at the tooltip Z direction, with its two wings at
                                    //the XZ and YZ planes
                                    double[] TipOnImage = new double[2];
                                    double[] VectEndOnImage = new double[2];
                                    double[] AxisVec = new double[3];
                                    double[] ArrowVecL = new double[3];
                                    double[] arrowVecR = new double[3];
                                    double[] VecInCam = null;

                                    AxisVec[2] = -30;
                                    //along the Z axis
                                    ArrowVecL[0] = -4;
                                    //on XZ
                                    ArrowVecL[2] = -6;
                                    arrowVecR[0] = 4;
                                    //on YZ
                                    arrowVecR[2] = -6;
                                    //left/right/middle images
                                    for (SideI = 0; SideI <= CurrCamera.sensorsnum - 1; SideI++)
                                    {
                                        if (CurrCamera.ProjectionOnImage(SideI, TT.ShiftVector, out TipOnImage[0], out TipOnImage[1]))
                                        {
                                            //tip can be seen on the image. Try to show the three vectors
                                            VecInCam = TT.XformLocation(AxisVec);
                                            if (CurrCamera.ProjectionOnImage(SideI, VecInCam, out VectEndOnImage[0], out VectEndOnImage[1]))
                                            {
                                                DrawLine(TipOnImage[0], TipOnImage[1], VectEndOnImage[0], VectEndOnImage[1], Color.Lime, SideI, DashStyle.Solid, true, false, DisplayMirrorImage);
                                            }
                                            VecInCam = TT.XformLocation(ArrowVecL);
                                            if (CurrCamera.ProjectionOnImage(SideI, VecInCam, out VectEndOnImage[0], out VectEndOnImage[1]))
                                            {
                                                DrawLine(TipOnImage[0], TipOnImage[1], VectEndOnImage[0], VectEndOnImage[1], Color.Yellow, SideI, DashStyle.Solid, true, false, DisplayMirrorImage);
                                            }
                                            VecInCam = TT.XformLocation(arrowVecR);
                                            if (CurrCamera.ProjectionOnImage(SideI, VecInCam, out VectEndOnImage[0], out VectEndOnImage[1]))
                                            {
                                                DrawLine(TipOnImage[0], TipOnImage[1], VectEndOnImage[0], VectEndOnImage[1], Color.Yellow, SideI, DashStyle.Solid, true, false, DisplayMirrorImage);
                                            }
                                            if (IsShowingMagnifiedToolTip & (!ToolTipShown))
                                            {
                                                //show the image region around the tooltip, magnified
                                                int RegionHalfSide = 0;
                                                //pictBox = IIf(SideI = 2, PictureBox2, IIf(SideI = 0, pictureBox0, pictureBox1))
                                                RegionHalfSide = Math.Min(40, pictBox.Height / 8);
                                                ShowMagnifiedRegion(SideI, TipOnImage[0], TipOnImage[1], RegionHalfSide, 2, 4);
                                            }
                                        }
                                    }
                                    ToolTipShown = true;
                                }

                                if (M.Name.Length > 3)
                                {
                                    if (M.Name.Substring(0, 4).ToUpper().Equals("COOL"))
                                    {
                                        str += " Please use duramark CoolCard to set light coolness";
                                    }
                                }
                                str = str + " " + M.GetMarker2CameraXf(CurrCamera).HazardText;
                                //not known
                            }
                            else
                            {
                                str = (MarkerNum + 1).ToString() + ". another (unregistered) camera";
                            }
                            //Add average illumination on the XPs of the first detected facet in Lux
                            var ShowXPsLux = false;
                            if (ShowXPsLux)
                            {
                                if (!HdrEnabled && M.GetIdentifiedFacets(CurrCamera).Count >= 1)
                                {
                                    int AvgWhitePixValue = 0;
                                    var longVector = default(Vector);
                                    Vector shortVector = new Vector();
                                    short LV_minlevel = 0;
                                    short LV_maxlevel = 0;
                                    short SV_minlevel = 0;
                                    short SV_maxlevel = 0;
                                    try
                                    {
                                        Ft = M.GetIdentifiedFacets(CurrCamera)[0];
                                        Ft.GetIdentifiedVectors(out longVector, out shortVector);
                                        longVector.GetPixelValueRange(out LV_minlevel, out LV_maxlevel);
                                        shortVector.GetPixelValueRange(out SV_minlevel, out SV_maxlevel);
                                    }
                                    catch
                                    {
                                        exitdemo(1);

                                    }
                                    AvgWhitePixValue = (LV_maxlevel + SV_maxlevel) / 2;
                                    if (AvgWhitePixValue < 255)
                                    {
                                        str = str + " " + Convert.ToInt64(AvgWhitePixValue * CurrCamera.LuxPerPixelUnit) + "Lux";
                                    }
                                }
                            }
                            if (IsShowingMarkersPositionsAtRefSpace)
                            {
                                ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Orange);
                            }
                            else
                            {
                                ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Yellow);
                            }

                            TextLine += TextLine_Increments;
                        }
                        MarkerNum += 1;
                    }

                }
                TextLine += TextLine_Increments;
            }

            if (IsShowingConnections)
            {
                if (polycount > 0)
                {
                    ShowConnectedMarkers(polycount, polyline);
                }
            }
            if (IsShowingDistancesAndAngles && IMarkers.Count >= 2)
            {
                //show distances and angles
                Xform3D Mi2Cam = null;
                Xform3D Mj2Cam = null;
                i = 0;
                while (i < IMarkers.Count)
                {
                    Mi2Cam = IMarkers[i].GetMarker2CameraXf(CurrCamera);
                    j = i + 1;
                    while (j < IMarkers.Count)
                    {
                        try
                        {
                            Mj2Cam = IMarkers[j].GetMarker2CameraXf(CurrCamera);
                        }
                        catch
                        {
                            exitdemo(1);
                        }
                        if (!(Mi2Cam == null || Mj2Cam == null))
                        {
                            Xform3D[] XVect = new Xform3D[2];

                            double[] XUnitV = new double[3];
                            double Dist = 0;

                            double AngleRads = 0;
                            Dist = Utils.Distance(IMarkers[i].GetMarker2CameraXf(CurrCamera).ShiftVector, IMarkers[j].GetMarker2CameraXf(CurrCamera).ShiftVector);

                            double[] tempAxis = null;

                            IMarkers[i].GetMarker2CameraXf(CurrCamera).AngularDifference(IMarkers[j].GetMarker2CameraXf(CurrCamera), out AngleRads, out tempAxis);
                            str = (i + 1).ToString() + "-" + (j + 1).ToString() + ": " + Dist.ToString("0.0") + "mm / " + (AngleRads * 180 / Math.PI).ToString("0.0") + "°";
                            ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Yellow);
                            TextLine = TextLine + TextLine_Increments;
                        }
                        j += 1;
                    }
                    i += 1;
                }
            }

        }


        private void ShowIdentifiedClouds()
        {
            Cloud Cl = null;
            //cloud's identified xpoints
            int CloudNum = 0;
            int i = 0;
            double[] X = new double[3];
            double[] Y = new double[3];

            double[,,] Pos = new double[3, 2, 101];
            string str = null;

            XPoint XP = null;
            int SideI = 0;

            IList<Cloud> IClouds = clouds.GetIdentifiedClouds(null);
            IList<XPoint> IXPs = null;

            // code snippet for retrieving template xpoints
            //Dim IcloudTemps As List(Of Cloud) = clouds.GetTemplateClouds()
            //Cl = IcloudTemps(0)
            //Dim xyz3d As Double()
            //For i = 0 To Cl.TemplateXPoints.Count - 1
            //    XP = Cl.TemplateXPoints(i)
            //    xyz3d = XP.Position3D()
            //    Dim tt = 0
            //Next

            //Show all overlays on the current displayed image
            //Presentation near the Facet projection in the image
            //----------------------------------------------------
            Boolean MirrorImage = DisplayMirrorImage;
            PictureBox pictBox = (DisplayPB_Index == 2 ? PicBox2 : (DisplayPB_Index == 0 ? picBox0 : picBox1));
            while (CloudNum < IClouds.Count)
            {
                Cl = IClouds[CloudNum];
                if (Cl.WasIdentified(CurrCamera))
                {
                    //The Cloud was identified by the current camera - overlay it on the images
                    IXPs = Cl.GetIdentifiedXPoints(CurrCamera, false);
                    int iXPCount = Cl.GetIdentifiedXPoints(CurrCamera, false).Count;

                    for (i = 0; i <= IXPs.Count - 1; i++)
                    {
                        XP = IXPs[i];
                        //Shows Index
                        if (IsShowingClouds)
                        {
                            XP.Position2D(out X[0], out Y[0], out X[1], out Y[1], out X[2], out Y[2]);
                            for (SideI = 0; SideI <= CurrCamera.sensorsnum - 1; SideI++)
                            {
                                //Just show the XPs in the image field (H3-60)
                                if (X[SideI] > 1)
                                {
                                    CirclePixel(X[SideI], Y[SideI], Color.Lime, 2, SideI, MirrorImage);
                                    CirclePixel(X[SideI], Y[SideI], Color.Lime, 3, SideI, MirrorImage);
                                    CirclePixel(X[SideI], Y[SideI], Color.Lime, 4, SideI, MirrorImage);
                                    if (IsShowingCloudsPositions)
                                    {
                                        pictBox = (SideI == 2 ? PicBox2 : (SideI == 0 ? picBox0 : picBox1));
                                        ShowTextWithShadow(Convert.ToString(XP.Index), pictBox, (int)(X[SideI] * DisplayScale), (int)(Y[SideI] * DisplayScale), Color.Lime);
                                    }
                                }
                            }
                        }
                    }

                    // show the coordinate system

                    //If IsShowingCloudsPositions Or IsShowingDistancesAndAngles Then 'Show the facet's name near the center of the long vector
                    //    Pos(DisplayPB_Index, 0, CloudNum) = (XPs(0, DisplayPB_Index, 0, 0) + XPs(0, DisplayPB_Index, 1, 0)) / 2 + 2 * M.GetFacetIndex(Ft)
                    //    Pos(DisplayPB_Index, 1, CloudNum) = (XPs(0, DisplayPB_Index, 0, 1) + XPs(0, DisplayPB_Index, 1, 1)) / 2 + 2 * M.GetFacetIndex(Ft)
                    //    str = Cl.Name
                    //    If Cl.TemplateXPoints.Count > 1 Then
                    //        str = str & "/" & (Cl.GetFacetIndex(Ft) + 1).ToString()
                    //    End If
                    //    str = (CloudNum + 1).ToString() & ". " & str
                    //    ImgX = CInt(Fix(Math.Round((Pos(DisplayPB_Index, 0, CloudNum) - 30) * DisplayScale)))
                    //    If DisplayMirrorImage Then
                    //        ImgX = pictBox.Width - ImgX
                    //    End If
                    //    ShowTextWithShadow(str, pictBox, ImgX, CInt(Fix((Pos(DisplayPB_Index, 1, CloudNum) - 10) * DisplayScale)), Color.Yellow)
                    //End If
                }
                CloudNum += 1;
            }

            //presentation at the upper left corner
            //(to cover all Clouds identified by any camera)
            //---------------------------------------------------
            Xform3D Cloud2CurrCameraXf = null;

            long polycount = 0;
            double[,] polyline = null;
            if (IsShowingCloudsPositions)
            {
                if (IClouds.Count > 0)
                {
                    double[] NewPos = null;
                    if (IsShowingConnections)
                    {
                        polyline = new double[3, 200];
                        polycount = 0;
                    }

                    CloudNum = 0;
                    while (CloudNum < IClouds.Count)
                    {
                        Cl = IClouds[CloudNum];
                        if (Cl.WasIdentified(null))
                        {
                            try
                            {
                                Cloud2CurrCameraXf = Cl.GetCloud2CameraXf(CurrCamera);
                            }
                            catch
                            {
                                break; // TODO: might not be correct. Was : Exit Do
                            }
                            //There may be a situation where Cloud was identified by another camera (not CurrCamera)
                            //and the identifying camera is not registered with CurrCamera. In this case, the pose is
                            //not known in CurrCamera coordinates and Cloud2CurrCameraXf is Nothing.
                            if (Cloud2CurrCameraXf != null)
                            {
                                if (IsShowingConnections)
                                {
                                    // Store Cloud origin
                                    NewPos = Cl.GetCloud2CameraXf(CurrCamera).ShiftVector;
                                    polyline[0, polycount] = NewPos[0];
                                    polyline[1, polycount] = NewPos[1];
                                    polyline[2, polycount] = NewPos[2];
                                    polycount = polycount + 1;
                                }
                                double[] xyz = null;
                                str = "";
                                if (IsShowingCloudsPositionsAtRefSpace)
                                {
                                    MTInterfaceDotNet.Camera Identifiedcamera = default(MTInterfaceDotNet.Camera);
                                    xyz = Cl.GetCloud2ReferenceXf(CurrCamera, out Identifiedcamera).ShiftVector;
                                    if ((Cl.WasIdentifiedInReferenceSpace(null) == true))
                                    {
                                        string ref_name = null;
                                        try
                                        {
                                            ref_name = Cl.ReferenceCloud.Name + " Space";
                                        }
                                        catch
                                        {
                                            ref_name = "Camera Space";
                                            if ((Identifiedcamera != CurrCamera))
                                            {
                                                ref_name = " Cam Space  - Detected by S/N" + Identifiedcamera.SerialNumber.ToString();
                                            }
                                        }
                                        str = (CloudNum + 1).ToString() + ". (" + xyz[0].ToString("000.0") + ", " + xyz[1].ToString("000.0") + ", " + xyz[2].ToString("000.0") + ") @ " + ref_name;
                                    }
                                    else
                                    {
                                        str = (CloudNum + 1).ToString() + ". (Reference Coordinate System)";
                                    }
                                }
                                else
                                {
                                    xyz = Cl.GetCloud2CameraXf(CurrCamera).ShiftVector;
                                    str = (CloudNum + 1).ToString() + "." + Cl.Name + "(Cloud): (" + xyz[0].ToString("000.0") + ", " + xyz[1].ToString("000.0") + ", " + xyz[2].ToString("000.0") + ")";
                                }
                                //Show the XYZ position of the Cloud's origin.
                                //If there's a tooltip, add it
                                Xform3D TT = null;
                                bool ToolTipShown = false;
                                double[] Svec = null;
                                Svec = Cl.Tooltip2CloudXf.ShiftVector;
                                //non-null transform
                                if (Svec[0] != 0 || Svec[1] != 0 || Svec[2] != 0)
                                {
                                    if (IsShowingCloudsPositionsAtRefSpace)
                                    {
                                        if ((Cl.WasIdentifiedInReferenceSpace(CurrCamera) == true))
                                        {
                                            TT = Cl.Tooltip2ReferenceXf(CurrCamera);
                                        }
                                        else
                                        {
                                            TT = new Xform3D();
                                        }
                                    }
                                    else
                                    {
                                        TT = Cl.Tooltip2CloudXf.Concatenate(Cl.GetCloud2CameraXf(CurrCamera));
                                    }

                                    //Write the numerical coordinates of the tip
                                    xyz = TT.ShiftVector;
                                    str = str + " tip(" + xyz[0].ToString("000.0") + ", " + xyz[1].ToString("000.0") + ", " + xyz[2].ToString("000.0") + ")";

                                    //TT is now the tooltip->camera transform.
                                    TT = Cl.Tooltip2CloudXf.Concatenate(Cl.GetCloud2CameraXf(CurrCamera));
                                    xyz = TT.ShiftVector;
                                    //If possible, show the tip and tool axis as an "enhanced reality" overlay.
                                    //TT's origin is the tool tip location in camera coordinates.
                                    //Draw an arrow pointing at the tooltip Z direction, with its two wings at
                                    //the XZ and YZ planes
                                    double[] TipOnImage = new double[2];
                                    double[] VectEndOnImage = new double[2];
                                    double[] AxisVec = new double[3];
                                    double[] ArrowVecL = new double[3];
                                    double[] arrowVecR = new double[3];
                                    double[] VecInCam = null;

                                    AxisVec[2] = -30;
                                    //along the Z axis
                                    ArrowVecL[0] = -4;
                                    //on XZ
                                    ArrowVecL[2] = -6;
                                    arrowVecR[0] = 4;
                                    //on YZ
                                    arrowVecR[2] = -6;
                                    //left/right/middle images
                                    for (SideI = 0; SideI <= CurrCamera.sensorsnum - 1; SideI++)
                                    {
                                        if (CurrCamera.ProjectionOnImage(SideI, TT.ShiftVector, out TipOnImage[0], out TipOnImage[1]))
                                        {
                                            //tip can be seen on the image. Try to show the three vectors
                                            VecInCam = TT.XformLocation(AxisVec);
                                            if (CurrCamera.ProjectionOnImage(SideI, VecInCam, out VectEndOnImage[0], out VectEndOnImage[1]))
                                            {
                                                DrawLine(TipOnImage[0], TipOnImage[1], VectEndOnImage[0], VectEndOnImage[1], Color.Lime, SideI, DashStyle.Solid, true, false, DisplayMirrorImage);
                                            }
                                            VecInCam = TT.XformLocation(ArrowVecL);
                                            if (CurrCamera.ProjectionOnImage(SideI, VecInCam, out VectEndOnImage[0], out VectEndOnImage[1]))
                                            {
                                                DrawLine(TipOnImage[0], TipOnImage[1], VectEndOnImage[0], VectEndOnImage[1], Color.Yellow, SideI, DashStyle.Solid, true, false, DisplayMirrorImage);
                                            }
                                            VecInCam = TT.XformLocation(arrowVecR);
                                            if (CurrCamera.ProjectionOnImage(SideI, VecInCam, out VectEndOnImage[0], out VectEndOnImage[1]))
                                            {
                                                DrawLine(TipOnImage[0], TipOnImage[1], VectEndOnImage[0], VectEndOnImage[1], Color.Yellow, SideI, DashStyle.Solid, true, false, DisplayMirrorImage);
                                            }
                                            if (IsShowingMagnifiedToolTip & (!ToolTipShown))
                                            {
                                                //show the image region around the tooltip, magnified
                                                int RegionHalfSide = 0;
                                                //pictBox = IIf(SideI = 2, PictureBox2, IIf(SideI = 0, pictureBox0, pictureBox1))
                                                RegionHalfSide = Math.Min(40, pictBox.Height / 8);
                                                ShowMagnifiedRegion(SideI, TipOnImage[0], TipOnImage[1], RegionHalfSide, 2, 4);
                                            }
                                        }
                                    }
                                    ToolTipShown = true;
                                }

                                str = str + " " + Cl.GetCloud2CameraXf(CurrCamera).HazardText;
                                //not known
                            }
                            else
                            {
                                str = (CloudNum + 1).ToString() + ". another (unregistered) camera";
                            }
                            TextLine = TextLine + TextLine_Increments;
                            if (IsShowingCloudsPositionsAtRefSpace)
                            {
                                ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Orange);
                            }
                            else
                            {
                                pictBox = (DisplayPB_Index == 2 ? PicBox2 : (DisplayPB_Index == 0 ? picBox0 : picBox1));
                                ShowTextWithShadow(str, pictBox, 3, TextLine, Color.Lime);
                            }

                            TextLine += TextLine_Increments;
                        }
                        CloudNum += 1;
                    }

                }
                TextLine += TextLine_Increments;
            }

        }


        public void ShowConnectedMarkers(long PointsCount, double[,] Trace)
        {
            int i = 0;

            double[] PosInCam = null;

            Camera CurrCam = null;
            double[] CurrPosInImg = new double[2];
            double[] PrevPosInImg = new double[2];
            CurrCam = CurrCamera;
            //convenience
            const int UnknownX = int.MaxValue;

            //Show in main image areas
            double[] XYBounds = new double[4];
            //xmin,xmax,ymin,ymax
            Color LineColor = Color.FromArgb(255, 150, 150);
            //pink

            DashStyle LineStyle = DashStyle.Solid;
            PosInCam = new double[3];
            PrevPosInImg[0] = UnknownX;
            XYBounds[0] = 100000;
            //init
            XYBounds[1] = 0;
            XYBounds[2] = 100000;
            XYBounds[3] = 0;

            i = 0;
            while (i < PointsCount)
            {
                if (Trace[0, i] != UnknownX)
                {
                    PosInCam[0] = Trace[0, i];
                    PosInCam[1] = Trace[1, i];
                    PosInCam[2] = Trace[2, i];
                    if (CurrCam.ProjectionOnImage(0, PosInCam, out CurrPosInImg[0], out CurrPosInImg[1]))
                    {
                        //We have a point we can draw.
                        //Update bounding box (for magnified area later)
                        if (XYBounds[0] > CurrPosInImg[0])
                        {
                            XYBounds[0] = CurrPosInImg[0];
                        }
                        if (XYBounds[1] < CurrPosInImg[0])
                        {
                            XYBounds[1] = CurrPosInImg[0];
                        }
                        if (XYBounds[2] > CurrPosInImg[1])
                        {
                            XYBounds[2] = CurrPosInImg[1];
                        }
                        if (XYBounds[3] < CurrPosInImg[1])
                        {
                            XYBounds[3] = CurrPosInImg[1];
                        }
                        //draw a line between prev and curr
                        if (PrevPosInImg[0] != UnknownX)
                        {
                            DrawLine(PrevPosInImg[0], PrevPosInImg[1], CurrPosInImg[0], CurrPosInImg[1], LineColor, 0, LineStyle, true, false, false);
                        }
                        //current position unknown
                    }
                    else
                    {
                        CurrPosInImg[0] = UnknownX;
                    }
                    PrevPosInImg[0] = CurrPosInImg[0];
                    PrevPosInImg[1] = CurrPosInImg[1];
                    //unknown
                }
                else
                {
                    PrevPosInImg[0] = UnknownX;
                }
                i += 1;
            }
        }

        public void ShowUnidentifiedVectors()
        {
            IList<Vector> IVs = null;
            Vector V = null;
            double[,,] XPs = null;
            int i = 0;
            int c = 0;
            double[,] EndPos3x2 = null;
            string s = null;
            int ImgX = 0;

            if (!(IsShowingVectors || IsShowingFiducials))
            {
                return;
            }
            IVs = markers.GetUnidentifiedVectors(CurrCamera);
            //nothing to show
            if (IVs.Count == 0)
            {
                return;
            }

            int SideI = 0;
            SideI = (CurrCamera.sensorsnum == 3 ? 2 : 0);

            i = 0;
            while (i < IVs.Count)
            {
                V = IVs[i];
                XPs = V.EndXPoints;
                //short vectors are fiducials
                if (IsShowingFiducials & V.Length <= FiducialVectorMaxLength)
                {
                    //draw a circle around the vector's center in the left image
                    double[] VCenter2D = new double[2];
                    double[] VCenter3D = new double[3];

                    VCenter2D[0] = (XPs[SideI, 0, 0] + XPs[SideI, 1, 0]) / 2;
                    VCenter2D[1] = (XPs[SideI, 0, 1] + XPs[SideI, 1, 1]) / 2;
                    EndPos3x2 = V.EndPos;
                    for (c = 0; c <= 2; c++)
                    {
                        VCenter3D[c] = (EndPos3x2[0, c] + EndPos3x2[1, c]) / 2;
                    }

                    CirclePixel(VCenter2D[0], VCenter2D[1], FiducialColor, 4, SideI, DisplayMirrorImage);
                    s = (Convert.ToInt64((VCenter3D[0]))).ToString() + "," + (Convert.ToInt64((VCenter3D[1]))).ToString() + "," + (Convert.ToInt64((VCenter3D[2]))).ToString();
                    ImgX = Convert.ToInt32((DisplayScale * VCenter2D[0] + 3));
                    if (DisplayMirrorImage)
                    {
                        ImgX = picBox0.Width - ImgX;
                    }
                    PictureBox pictBox = (SideI == 2 ? PicBox2 : (SideI == 0 ? picBox0 : picBox1));
                    ShowTextWithShadow("f", pictBox, ImgX, Convert.ToInt32((DisplayScale * VCenter2D[1] + 3)), Color.Lime);
                }
                else if ((IsShowingVectorsForTemplateRegistration & IsShowingVectors & V.Length >= ShownVectorsMinLength))
                {
                    DrawLine(XPs[0, 0, 0], XPs[0, 0, 1], XPs[0, 1, 0], XPs[0, 1, 1], Color.Yellow, 0, DashStyle.Dot, true, false, DisplayMirrorImage);
                    //left image
                    DrawLine(XPs[1, 0, 0], XPs[1, 0, 1], XPs[1, 1, 0], XPs[1, 1, 1], Color.Yellow, 1, DashStyle.Dot, true, false, DisplayMirrorImage);
                    //right image
                    if (CurrCamera.sensorsnum == 3)
                    {
                        DrawLine(XPs[2, 0, 0], XPs[2, 0, 1], XPs[2, 1, 0], XPs[2, 1, 1], Color.Yellow, 2, DashStyle.Dot, true, false, DisplayMirrorImage);
                        //middle image
                    }
                }
                if (IsShowingXPoints)
                {
                    int Side = 0;
                    while (Side <= (CurrCamera.sensorsnum - 1))
                    {
                        CirclePixel(XPs[Side, 0, 0], XPs[Side, 0, 1], XPointsOverlayColor, 3, Side, DisplayMirrorImage);
                        //base
                        CirclePixel(XPs[Side, 1, 0], XPs[Side, 1, 1], XPointsOverlayColor, 3, Side, DisplayMirrorImage);
                        //head
                        Side += 1;
                    }
                }
                i += 1;
            }
        }


        public void ShowMagnifiedRegion(int SideI, double CenterX, double CenterY, int RegionHalfSide, long Corner, double MagnificationFactor, bool ShowCrossAtCenter, IList<double[]> LinesToDraw, Color DrawColor)
        {
            ShowMagnifiedRegion(SideI, CenterX, CenterY, RegionHalfSide, Corner, MagnificationFactor, ShowCrossAtCenter, LinesToDraw, DrawColor, DashStyle.Dot);
        }

        public void ShowMagnifiedRegion(int SideI, double CenterX, double CenterY, int RegionHalfSide, long Corner, double MagnificationFactor, bool ShowCrossAtCenter, IList<double[]> LinesToDraw)
        {
            ShowMagnifiedRegion(SideI, CenterX, CenterY, RegionHalfSide, Corner, MagnificationFactor, ShowCrossAtCenter, LinesToDraw, Color.Yellow, DashStyle.Dot);
        }

        public void ShowMagnifiedRegion(int SideI, double CenterX, double CenterY, int RegionHalfSide, long Corner, double MagnificationFactor, bool ShowCrossAtCenter)
        {
            ShowMagnifiedRegion(SideI, CenterX, CenterY, RegionHalfSide, Corner, MagnificationFactor, ShowCrossAtCenter, null, Color.Yellow, DashStyle.Dot);
        }

        public void ShowMagnifiedRegion(int SideI, double CenterX, double CenterY, int RegionHalfSide, long Corner, double MagnificationFactor)
        {
            ShowMagnifiedRegion(SideI, CenterX, CenterY, RegionHalfSide, Corner, MagnificationFactor, true, null, Color.Yellow, DashStyle.Dot);
        }

        public void ShowMagnifiedRegion(int SideI, double CenterX, double CenterY, int RegionHalfSide, long Corner, double MagnificationFactor, bool ShowCrossAtCenter, IList<double[]> LinesToDraw, Color DrawColor, DashStyle DrawStyle)
        {
            //Inserts an inset image within the picturebox, where a magnified region around the given
            //coordinates is shown.
            //Corner index is 0-based, indexed clockwise from the top left
            //LinesToDraw contains arrays (x0,y0,x1,y1) of line segments to draw in image coordiantes.
            //Only lines that fall within the region are drawn

            if (CurrCamera == null)
            {
                return;
            }

            try
            {
                if (CurrCamera.FramesGrabbed == 0)
                    return;
            }
            catch
            {
                exitdemo(1);
            }

            int[] OutXY = new int[2];
            double[] InXY = new double[2];
            double InDxy = 0;
            InDxy = 1 / MagnificationFactor;
            //delta input coordinates from one pixel to next

            //Get the source image region
            Bitmap Image = null;
            int InputLeft = 0;
            int InputTop = 0;
            int InputSide = 0;

            InputLeft = Convert.ToInt32(CenterX - RegionHalfSide * InDxy);
            //offset of first pixel in image
            InputTop = Convert.ToInt32(CenterY - RegionHalfSide * InDxy);
            InputSide = Convert.ToInt32(2 * RegionHalfSide * InDxy);

            Image = CurrCamera.Get24BitPixels(SideI, InputLeft, InputTop, InputSide, InputSide);

            MT.ContrastStretch24BitBitmap(Image);

            //Now loop on output coordinates, with bilinear interpolation at the input
            InXY[1] = CenterY - RegionHalfSide * InDxy - InputTop;
            //start input Y

            float[] Pix = new float[2];
            //intermediate results during interpolation

            PictureBox pictBox = (SideI == 2 ? PicBox2 : (SideI == 0 ? picBox0 : picBox1));
            //Compute display coordinates
            if (Corner == 1 | Corner == 2)
            {
                OutXY[0] = pictBox.Width - 2 * RegionHalfSide + 1;
            }
            if (Corner == 2 | Corner == 3)
            {
                OutXY[1] = pictBox.Height - 2 * RegionHalfSide + 1;
            }

            if ((pictBox.Image != null) && pictBox.Image.PixelFormat == PixelFormat.Format24bppRgb)
            {
                using (Graphics gr = Graphics.FromImage(pictBox.Image))
                {
                    int side = 2 * RegionHalfSide;
                    if (DisplayMirrorImage)
                    {
                        Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        gr.DrawImage(Image, new Rectangle(0, OutXY[1], side, side), new Rectangle(0, 0, Image.Width, Image.Height), GraphicsUnit.Pixel);
                    }
                    else
                    {
                        gr.DrawImage(Image, new Rectangle(OutXY[0], OutXY[1], side, side), new Rectangle(0, 0, Image.Width, Image.Height), GraphicsUnit.Pixel);
                    }
                }
            }
            else
            {
                //pictBox.Refresh()
                using (Graphics gr = pictBox.CreateGraphics())
                {
                    int side = 2 * RegionHalfSide;
                    if (DisplayMirrorImage)
                    {
                        Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        gr.DrawImage(Image, new Rectangle(0, OutXY[1], side, side), new Rectangle(0, 0, Image.Width, Image.Height), GraphicsUnit.Pixel);
                    }
                    else
                    {
                        gr.DrawImage(Image, new Rectangle(OutXY[0], OutXY[1], side, side), new Rectangle(0, 0, Image.Width, Image.Height), GraphicsUnit.Pixel);
                    }
                }
            }

            long[] RegionCenter = new long[2];
            RegionCenter[0] = OutXY[0] + RegionHalfSide;
            //move output coordinates to the region's center
            RegionCenter[1] = OutXY[1] + RegionHalfSide;
            if (ShowCrossAtCenter)
            {
                long HalfLineLen = 0;
                HalfLineLen = Math.Max(20, RegionHalfSide / 2);
                DrawLine(RegionCenter[0] - HalfLineLen, RegionCenter[1], RegionCenter[0] + HalfLineLen, RegionCenter[1], DrawColor, SideI, DrawStyle, true, true, DisplayMirrorImage);
                DrawLine(RegionCenter[0], RegionCenter[1] - HalfLineLen, RegionCenter[0], RegionCenter[1] + HalfLineLen, DrawColor, SideI, DrawStyle, true, true, DisplayMirrorImage);
            }

            if (LinesToDraw != null)
            {
                //	double LineSegment = 0; //(x/y,end)
                int EndI = 0;
                double[,] RegionXY = new double[2, 2];
                bool InRegion = false;
                foreach (double[] LineSegment in LinesToDraw)
                {
                    //transform the segment to region coordinates
                    InRegion = true;
                    for (EndI = 0; EndI <= 1; EndI++)
                    {
                        RegionXY[0, EndI] = (LineSegment[2 * EndI] - CenterX) * MagnificationFactor;
                        //outside of region
                        if (RegionXY[0, EndI] < -RegionHalfSide | RegionXY[0, EndI] > RegionHalfSide)
                        {
                            InRegion = false;
                        }
                        else
                        {
                            RegionXY[0, EndI] = RegionCenter[0] + RegionXY[0, EndI];
                        }
                        RegionXY[1, EndI] = (LineSegment[2 * EndI + 1] - CenterY) * MagnificationFactor;
                        //outside of region
                        if (RegionXY[1, EndI] < -RegionHalfSide | RegionXY[1, EndI] > RegionHalfSide)
                        {
                            InRegion = false;
                        }
                        else
                        {
                            RegionXY[1, EndI] = RegionCenter[1] + RegionXY[1, EndI];
                        }
                    }
                    //draw
                    if (InRegion)
                    {
                        DrawLine(RegionXY[0, 0], RegionXY[1, 0], RegionXY[0, 1], RegionXY[1, 1], DrawColor, SideI, DrawStyle, true, true, DisplayMirrorImage);
                    }
                }
            }
        }

        //TODO we have put contrastStretchBitmap and 24Bit in MT. 
        public void ShowAuxiliaryForm(Form f)
        {
            ShowAuxiliaryForm(f, true);
        }

        public void ShowAuxiliaryForm(Form f, bool Modal)
        {
            //Show the given form on the right of the main form (but within the screen)
            f.Left = Math.Min(this.Left + this.Width - 20, Screen.PrimaryScreen.Bounds.Width - f.Width);
            f.Top = this.Top;

            f.StartPosition = FormStartPosition.Manual;
            if (Modal)
            {
                f.ShowDialog(this);
            }
            else
            {
                f.Show(this);
            }
        }

        public void ShowTextWithShadow(string s, PictureBox PB, int X, int Y)
        {
            ShowTextWithShadow(s, PB, X, Y, Color.FromArgb(0xc0ffff));
        }

        public void ShowTextWithShadow(string s, PictureBox PB, int X, int Y, Color color)
        {
            if ((PB.Image != null) && PB.Image.PixelFormat == PixelFormat.Format24bppRgb)
            {
                Font myfont = new Font(this.Font.Name, mFontSize, this.Font.Style, this.Font.Unit);


                using (Graphics gr = Graphics.FromImage(PB.Image))
                {
                    using (Brush brush = new SolidBrush(color))
                    {
                        //for (int j = -1; j <= 1; j++) {
                        //	for (int i = -1; i <= 1; i++) {
                        //		gr.DrawString(s, myfont, Brushes.Black, X + i, Y + j);
                        //	}
                        //}
                        gr.DrawString(s, myfont, Brushes.Black, X + 1, Y + 1);
                        gr.DrawString(s, myfont, brush, X, Y);
                    }
                }
            }
            else
            {
                //PB.Refresh()
                Font myfont = new Font(this.Font.Name, mFontSize, this.Font.Style, this.Font.Unit);
                using (Graphics gr = PB.CreateGraphics())
                {
                    using (Brush brush = new SolidBrush(color))
                    {
                        //for (int j = -1; j <= 1; j++) {
                        //	for (int i = -1; i <= 1; i++) {
                        //		gr.DrawString(s, myfont, Brushes.Black, X + i, Y + j);
                        //		//PB.Font
                        //	}
                        //}
                        gr.DrawString(s, myfont, Brushes.Black, X + 1, Y + 1);
                        gr.DrawString(s, myfont, brush, X, Y);
                    }
                }
            }
        }

        public void CirclePixel(double ImgX, double ImgY, Color color, int Radius, int SideI)
        {
            CirclePixel(ImgX, ImgY, color, Radius, SideI, false);
        }

        public void CirclePixel(double ImgX, double ImgY, Color color, int Radius)
        {
            CirclePixel(ImgX, ImgY, color, Radius, LeftI, false);
        }

        public void CirclePixel(double ImgX, double ImgY, Color color)
        {
            CirclePixel(ImgX, ImgY, color, 2, LeftI, false);
        }


        public void CirclePixel(double ImgX, double ImgY, Color color, int Radius, int SideI, bool MirrorImage)
        {
            PictureBox pictBox = (SideI == 2 ? PicBox2 : (SideI == 0 ? picBox0 : picBox1));

            ImgX = ImgX * DisplayScale;
            ImgY = ImgY * DisplayScale;
            if (MirrorImage)
            {
                int width = pictBox.Width;
                ImgX = width - ImgX;
            }
            if ((pictBox.Image != null) && pictBox.Image.PixelFormat == PixelFormat.Format24bppRgb)
            {
                //Using gr As Graphics = pictBox.CreateGraphics()
                using (Graphics gr = Graphics.FromImage(pictBox.Image))
                {
                    using (Pen pen = new Pen(color))
                    {
                        //TODO check whether it is correct or not VB line 1240
                        Rectangle rect = new Rectangle(Convert.ToInt32((Math.Round(ImgX - Radius))), Convert.ToInt32((Math.Round(ImgY - Radius))), 2 * Radius, 2 * Radius);
                        gr.DrawEllipse(pen, rect);

                    }
                }
            }
            else
            {
                //pictBox.Refresh()
                using (Graphics gr = pictBox.CreateGraphics())
                {
                    using (Pen pen = new Pen(color))
                    {
                        //TODO check whether it is correct or not VB line 1240
                        Rectangle rect = new Rectangle(Convert.ToInt32((Math.Round(ImgX - Radius))), Convert.ToInt32((Math.Round(ImgY - Radius))), 2 * Radius, 2 * Radius);
                        gr.DrawEllipse(pen, rect);

                    }
                }
            }
        }
        #endregion

        public bool SaveImageWithUI(Bitmap img, string DefaultDir, string DefaultFileName)
        {
            //returns True IFF successfully saved the image
            string path = null;
            string Extension = null;

            saveFileDialog1.Filter = "Bitmap files (*.bmp)|*.bmp";
            saveFileDialog1.Title = "Save the 3D image";
            saveFileDialog1.InitialDirectory = DefaultDir;
            saveFileDialog1.FileName = "MTimage.bmp";
            if (saveFileDialog1.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return false;
            }
            path = saveFileDialog1.FileName;
            Extension = System.IO.Path.GetExtension(path);
            if (string.IsNullOrEmpty(Extension))
            {
                path = path + ".jpg";
                Extension = "jpg";
            }
            if (Extension == "jpeg" || Extension == "jpg")
            {
                img.Save("C:\\tmp.bmp");
                //Save to JPEG
                img.Save(path, ImageFormat.Jpeg);
            }
            else
            {
                img.Save(path);
            }
            return true;
            //saved successfully
        }

        public void DrawLine(double ImgX0, double ImgY0, double ImgX1, double ImgY1, Color color, int SideI, DashStyle DrawStyle, bool EnsureContrast, bool IgnoreDisplayScale, bool MirrorImage)
        {
            if ((!IgnoreDisplayScale))
            {
                ImgX0 = ImgX0 * DisplayScale;
                ImgY0 = ImgY0 * DisplayScale;
                ImgX1 = ImgX1 * DisplayScale;
                ImgY1 = ImgY1 * DisplayScale;
            }

            PictureBox pictBox = (SideI == 2 ? PicBox2 : (SideI == 0 ? picBox0 : picBox1));
            if (MirrorImage)
            {
                int width = pictBox.Width;
                ImgX0 = width - ImgX0;
                ImgX1 = width - ImgX1;
            }

            if ((pictBox.Image != null) && pictBox.Image.PixelFormat == PixelFormat.Format24bppRgb)
            {
                using (Graphics gr = Graphics.FromImage(pictBox.Image))
                {
                    int width = 0;
                    if ((EnsureContrast && (DrawStyle == DashStyle.Solid)))
                    {
                        width = 3;
                    }
                    else
                    {
                        width = 1;
                    }
                    if (EnsureContrast)
                    {
                        using (Pen pen = new Pen(Color.Black, width))
                        {
                            pen.DashStyle = DashStyle.Solid;
                            //If requested line is solid, draw a wide black line around it, otherwise, narrow line
                            gr.DrawLine(pen, new PointF(Convert.ToSingle(ImgX0), Convert.ToSingle(ImgY0)), new PointF(Convert.ToSingle(ImgX1), Convert.ToSingle(ImgY1)));
                        }
                    }
                    width = 1;
                    using (Pen pen = new Pen(color, width))
                    {
                        pen.DashStyle = DrawStyle;
                        gr.DrawLine(pen, new PointF(Convert.ToSingle(ImgX0), Convert.ToSingle(ImgY0)), new PointF(Convert.ToSingle(ImgX1), Convert.ToSingle(ImgY1)));
                    }
                }
            }
            else
            {
                //pictBox.Refresh()
                using (Graphics gr = pictBox.CreateGraphics())
                {
                    int width = 0;
                    if ((EnsureContrast && (DrawStyle == DashStyle.Solid)))
                    {
                        width = 3;
                    }
                    else
                    {
                        width = 1;
                    }
                    if (EnsureContrast)
                    {
                        using (Pen pen = new Pen(System.Drawing.Color.Black, width))
                        {
                            pen.DashStyle = DashStyle.Solid;
                            //If requested line is solid, draw a wide black line around it, otherwise, narrow line
                            gr.DrawLine(pen, new PointF(Convert.ToSingle(ImgX0), Convert.ToSingle(ImgY0)), new PointF(Convert.ToSingle(ImgX1), Convert.ToSingle(ImgY1)));
                        }
                    }
                    width = 1;
                    using (Pen pen = new Pen(color, width))
                    {
                        pen.DashStyle = DrawStyle;
                        gr.DrawLine(pen, new PointF(Convert.ToSingle(ImgX0), Convert.ToSingle(ImgY0)), new PointF(Convert.ToSingle(ImgX1), Convert.ToSingle(ImgY1)));
                    }
                }
            }
        }

        private void UpdateUI()
        {
            mnuMarkerTemplates.Enabled = MarkersProcessingEnabled | CloudsProcessingEnabled;
            mnuPoseRecorder.Enabled = MarkersProcessingEnabled | CloudsProcessingEnabled;
            mnu3DTracer.Enabled = MarkersProcessingEnabled;

            //Update the Display Menu
            mnuDistancesAngles.Enabled = MarkersProcessingEnabled;
            mnuMagnifiedToolTip.Enabled = MarkersProcessingEnabled;
            mnuVectors.Enabled = MarkersProcessingEnabled;
            mnuFiducials.Enabled = MarkersProcessingEnabled;
            mnuMarkersPositions.Enabled = MarkersProcessingEnabled;
            mnuXPointsPositions.Enabled = XPointsProcessingEnabled;
            mnuCloudsPositions.Enabled = CloudsProcessingEnabled;
            IsShowingXPoints = (mnuXPoints.Checked | IsShowingXPoints);

            switch (DisplayMode)
            {
                case -1:
                    //No Image
                    mnuDisplayImageLeft.Checked = false;
                    mnuDisplayImageRight.Checked = false;
                    mnuDisplayImageMiddle.Checked = false;
                    mnuDisplayImageAll.Checked = false;

                    break;
                case 0:
                    //Only Left image
                    mnuDisplayImageLeft.Checked = true;
                    mnuDisplayImageRight.Checked = false;
                    mnuDisplayImageMiddle.Checked = false;
                    mnuDisplayImageAll.Checked = false;

                    break;
                case 1:
                    //Only right Image
                    mnuDisplayImageLeft.Checked = false;
                    mnuDisplayImageRight.Checked = true;
                    mnuDisplayImageMiddle.Checked = false;
                    mnuDisplayImageAll.Checked = false;

                    break;
                case 2:
                    //Only Middle Image
                    mnuDisplayImageLeft.Checked = false;
                    mnuDisplayImageRight.Checked = false;
                    mnuDisplayImageMiddle.Checked = true;
                    mnuDisplayImageAll.Checked = false;

                    break;
                case 3:
                    //Show all images
                    mnuDisplayImageLeft.Checked = true;
                    mnuDisplayImageRight.Checked = true;
                    mnuDisplayImageMiddle.Checked = false;
                    mnuDisplayImageAll.Checked = true;

                    break;
            }

            AdjustFormToPictureSize();

        }

        public void exitdemo(int showmessagenumber)
        {
            CaptureEnabled = false;
            this.UpdatePP();
            MT.Markers.BackGroundProcess = false;

            BackgroundProcessToolStripMenuItem.Checked = false;

            //mCameras.Detach();  this is called automatically by the Cameras destructor.

            //AVIFileExit()
            DialogResult msg_res = new DialogResult();
            string ErrorStr = "";

            switch (showmessagenumber)
            {
                case 0:
                    System.Environment.Exit(0);
                    //force termination
                    break;
                case 1:
                    msg_res = MessageBox.Show("No camera connected! Reconnect the cable and try again.", "No camera connected!", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
                    break;
                case 2:
                    //identify it
                    if ((mCameras.FailedCamera() != null))
                    {
                        ErrorStr = "camera " + mCameras.FailedCamera().SerialNumber + ": " + ErrorStr;
                    }
                    msg_res = MessageBox.Show("Capture failed: Reconnect the cable and try again" + ErrorStr + ".", "Capture Failed", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
                    break;
            }
            if (msg_res == DialogResult.Retry)
            {
                int i = 0;
                while (((SetupCameras() == false) & i < 5))
                {
                    System.Threading.Thread.Sleep(2000);
                    i = i + 1;
                }
                if ((i < 5))
                {
                    CaptureEnabled = true;
                }
                else
                {
                    System.Environment.Exit(0);
                }
            }
            else if ((msg_res == DialogResult.Ignore | msg_res == DialogResult.Abort))
            {
                System.Environment.Exit(0);
                //force termination
            }
        }

        //Function added by Mei
        private void PosRecorder()
        {
            if (frmpositionrecorder.AllowedToRecord())
            {
                frmpositionrecorder.AddRecord();
                frmpositionrecorder.ShowStats();
            }
        }

        void keyPressEvent(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                this.PosRecorder();
            }
        }

        public static void UdpNetworkClientManager(int port)
        {
            log.Debug("In UdpNetworkClientManager");

            udpclient = new UdpClient();
            udpclient.EnableBroadcast = true;
            udpclient.Connect(new IPEndPoint(IPAddress.Broadcast, port));

            log.Debug("Connected");
            connect = true;
        }

        public static void SendMessage(string data)
        {
            log.Debug("In SendMessage, data = " + data);
            Thread thread = new Thread(() =>
            {
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                udpclient.Send(bytes, bytes.Length);
            });
            thread.Start();
        }

        #region properties
        public MainForm()
        {
            InitializeComponent();
            Load += MainForm_Load;
            FormClosing += MainForm_FormClosing;
        }

        private void ToolStripCapture_Click(object sender, EventArgs e)
        {
            mnuDataCapture_Click(sender, e);
            if (CaptureEnabled)
            {
                ToolStripCapture.Image = MicronTrackerVerification.Properties.Resources.start_icon;
            }
            else
            {
                ToolStripCapture.Image = MicronTrackerVerification.Properties.Resources.Standby_icon;
            }
        }
        private void ToolStripTemplates_Click(object sender, EventArgs e)
        {
            mnuMarkerTemplates_Click(sender, e);
        }

        private void ToolStripPoseRecorder_Click(object sender, EventArgs e)
        {
            mnuPoseRecorder_Click(sender, e);
        }

        private void ToolStripHDR_Click(object sender, EventArgs e)
        {
            mnuHdrMode_Click(sender, e);
        }

        private void ToolStripHalfSize_Click(object sender, EventArgs e)
        {
            mnuHalfSize_Click(sender, e);
        }

        private void ToolStripCalibrationInfo_Click(object sender, EventArgs e)
        {
            mnuCalibrationInfo_Click(sender, e);
        }

        private void ToolStripCameraInfo_Click(object sender, EventArgs e)
        {
            mnuCameraInfo_Click(sender, e);
        }

        private void ToolStripVideoRecorder_Click(object sender, EventArgs e)
        {
            mnuRecordStop_Click(sender, e);
        }

        private void ToolStripAPIDocumentation_Click(object sender, EventArgs e)
        {
            mnuAbout_Click(sender, e);
        }

        private void ToolStripSettings_Click(object sender, EventArgs e)
        {
            mnuOptions_Click(sender, e);
        }

        private void mnuGammaCorrection_Click(object sender, EventArgs e)
        {
            mnuGammaCorrection.Checked = !mnuGammaCorrection.Checked;
            GammaCorrectionEnabled = mnuGammaCorrection.Checked;
        }

        private void mnuCam_Click(object sender, EventArgs e)
        {
            SelectCamera(menuStrip1.Items.IndexOf((ToolStripMenuItem)sender) - 4);
        }

        private void picBox0_Click(object sender, EventArgs e)
        {

        }
        #endregion
    }
}





