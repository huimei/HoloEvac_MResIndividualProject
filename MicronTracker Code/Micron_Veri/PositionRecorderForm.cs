using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MTInterfaceDotNet;
using System.IO;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

//==============================================================================
// Position recorder form of MicronTracker API usage demonstration program.
//==============================================================================
// Records positions and orientations of all the markers visible in the FOM either
// on manual or in a streaming mode. Selected data derived from the recorded poses
// can be saved to a text file in a tabular format, which can then be loaded directly
// into MS-Excel or into another program for further processing.
//
// This form uses the capture and processing code in the main form to obtain measurements.
//------------------------------------------------------------------------------

namespace MicronTrackerVerification
{
    public partial class PositionRecorderForm : Form
    {
        //Added by huimei, logger to file 
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        int holoFlag = 0;
        int refFlag = 0;

        String holoMarkerName = "Marker2";
        String refMarkerName = "Reference1";

        double[,] holoMarker = new double[4, 4];
        double[,] refMarker = new double[4, 4];

        // plug in calibration matrix 
        double[,] leftEyeCalib = new double[4, 4] { { -0.006602, -0.00004307, 0.00013566, 0.18651107 }, { -0.00001732, 0.00649731, 0.00044547, -0.81390249 }, { -0.00034853, 0.0000621, -0.00653838, 0.4093864 }, { 0, -0, -0, 1 } };
        double[,] rightEyeCalib = new double[4, 4] { { -0.006602, -0.00004307, 0.00013566, 0.18651107 }, { -0.00001732, 0.00649731, 0.00044547, -0.81390249 }, { -0.00034853, 0.0000621, -0.00653838, 0.4093864 }, { 0, -0, -0, 1 } };
        //End added by huimei

        private MainForm fMain;
        public PositionRecorderForm(MainForm mainForm)
        {
            InitializeComponent();
            fMain = mainForm;
        }

        private Cameras mCameras = MT.Cameras;
        private Markers markers = MT.Markers;
        private Clouds clouds = MT.Clouds;
        private class PoseRecordType
        {
            public double ElapsedSecs;
            public List<string> TemplateNames;
            //For each marker in "TemplateNames"
            public List<Xform3D> MarkerXforms;
            //For each cloud in "TemplateNames"
            public List<Xform3D> CloudXforms;

            //For each marker in "TemplateNames"
            public List<HazardCodes> Hazards;

            //For each marker in "TemplateNames"
            public List<Xform3D> TooltipXforms;
            //For each marker in "TemplateNames"
            public List<Xform3D> Tooltip2RefXforms;
            //For each marker in "TemplateNames"
            public List<Xform3D> Marker2RefXforms;

            //MaxFacets
            public List<double[,]>[] FacetXPoints = new List<double[,]>[11];
            public int TemplateFacetsCount;
            //The XPoints in the first facet detected for that marker by the
            //current camera (only!), added as two 3x2 arrays (x/y/z, base/head)
            //for each marker (long vector followed by short vector).

            public float Shutter;
            public float Gain;
            public double Temperature;
            public double TemperatureRate;
        }

        //Usually enough
        const int MaxRecordsCount = 500000;
        //Usually enough
        const int MaxCameras = 3;
        public int MaxFacets = 5;

        public bool UseFrameEmbeddedTimeStamp = true;

        const int TheCamera = 0;
        PoseRecordType[,] PoseRecords = new PoseRecordType[MaxCameras, MaxRecordsCount];

        long PoseRecordsCount = 0;
        Stopwatch SW = new Stopwatch();

        double[,] EndPos2x3 = new double[2, 3];

        bool SomeRecordsUnsaved = false;
        //Properties /state
        //------------------------

        private bool mRecordStream;

        private bool RecordingByFile;
        public bool RecordStream
        {
            get { return mRecordStream; }
            set
            {
                if (((!value) && (PoseRecordsCount >= PoseRecords.GetLength(1))))
                {
                    MessageBox.Show("Maximum records count exceeded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    mRecordStream = value;
                    if ((!object.ReferenceEquals(chkRecordStream, this.ActiveControl)))
                    {
                        chkRecordStream.Checked = value;
                    }
                    int Msecs = 0;
                    if (int.TryParse(tbMsecs.Text, out Msecs))
                    {
                        //Msecs = 50
                        tbMsecs.Text = Msecs.ToString();
                    }
                    else
                    {
                        Msecs = Math.Max(5, Msecs);
                        Msecs = Math.Min(Msecs, 30000);
                        tbMsecs.Text = Msecs.ToString();
                    }

                    tbMsecs.Enabled = !RecordStream;

                    //turn on
                    if (!value)
                    {
                        if (PoseRecordsCount > 1)
                        {
                            ShowStats();
                        }
                    }
                }
                //remember
                if (value)
                {
                    SaveToINI();
                }
            }
        }

        public int PoseRecorderIntervalsGet
        {
            get
            {
                int Msecs = 0;
                int.TryParse(tbMsecs.Text, out Msecs);
                return Msecs;
            }
        }

        private void chkMultiFiles_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            //Reset
            if (!initialized)
            {
                return;
            }
            PoseRecordsCount = 0;
            lblRecordsCount.Text = PoseRecordsCount.ToString();
        }

        private void radiob2markers_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (!initialized)
            {
                return;
            }
            ShowStats();
        }

        private void radiobTooltip_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (!initialized)
            {
                return;
            }
            ShowStats();
        }

        private long PrevFrameCounter = 0;
        public void RecordPoseData()
        {
            //ignore: same frame as before
            if (PrevFrameCounter == fMain.CurrCamera.FramesGrabbed)
            {
                return;
            }
            PrevFrameCounter = fMain.CurrCamera.FramesGrabbed;
            //update for next time

            btnSave.Enabled = (PoseRecordsCount > 0);
            if (RecordStream)
            {
                if (AllowedToRecord())
                {
                    AddRecord();
                }
            }
        }

        //Private Sub timerCheckFile_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles timerCheckFile.Tick
        //    If File.Exists("C:\MTmeasure") Then 'take a measure
        //        AddRecord()
        //        ShowStats()
        //        Do While File.Exists("C:\MTmeasure")
        //            File.Delete("C:\MTmeasure")
        //        Loop
        //    End If
        //End Sub
        public void PoseRecorder()
        {
            //take a measure
            if (File.Exists("C:\\MTmeasure"))
            {
                AddRecord();
                ShowStats();
                while (File.Exists("C:\\MTmeasure"))
                {
                    File.Delete("C:\\MTmeasure");
                }
            }
        }

        private void chkRecordStream_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (!initialized)
            {
                return;
            }
            RecordStream = chkRecordStream.Checked;
        }

        private void btnClose_Click(System.Object sender, System.EventArgs e)
        {
            this.Close();
        }

        private void chkRecordByFile_CheckedChanged(System.Object sender, System.EventArgs e)
        {
            if (!initialized)
            {
                return;
            }
            //When the file "C:\MTmeasure" exists, record a sample, then erase the file and
            //wait for it to appear again.
            RecordingByFile = chkRecordByFile.Checked;
            btnRecordPose.Enabled = !RecordingByFile;
            chkRecordStream.Enabled = !RecordingByFile;
            timerCheckFile.Enabled = RecordingByFile;
        }

        private void btnReset_Click(System.Object sender, System.EventArgs e)
        {
            PoseRecordsCount = 0;
            lblRecordsCount.Text = PoseRecordsCount.ToString();
        }

        private void btnSave_Click(System.Object sender, System.EventArgs e)
        {
            string FilePath = null;
            string currentFilePath = null;
            long camIndex = 0;

            MaxFacets = 0;
            for (int i = 0; i <= PoseRecordsCount - 1; i++)
            {
                if (PoseRecords[0, i].TemplateFacetsCount > MaxFacets)
                {
                    MaxFacets = PoseRecords[0, i].TemplateFacetsCount;
                }
            }

            saveFileDialog1.Filter = "Text File|*.txt";
            saveFileDialog1.Title = "Save Recorder Positions and Angle as text";
            saveFileDialog1.InitialDirectory = fMain.PP.RetrieveString("LastFilesDir", "C:\\MicroTracker");
            saveFileDialog1.FileName = "MT poses " + System.DateTime.Now.ToString("hh-mm-ss") + ".txt";

            saveFileDialog1.AddExtension = true;
            saveFileDialog1.DefaultExt = ".txt";
            if (saveFileDialog1.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            FilePath = saveFileDialog1.FileName;

            if (chkMultiFiles.Checked)
            {
                camIndex = mCameras.Count - 1;
                // last camera
            }
            else
            {
                camIndex = 0;
            }

            //turn off capturing until the saving process is complete
            if ((PoseRecordsCount > 5000))
            {
                fMain.CaptureEnabled = false;
            }

            int c = 0;
            while (c <= camIndex)
            {
                if (chkMultiFiles.Checked)
                {
                    currentFilePath = FilePath.Substring(0, FilePath.Length - 4) + "-" + mCameras[c].SerialNumber + ".txt";
                }
                else
                {
                    currentFilePath = FilePath;
                }
                //Find the superset of all the markers that appeared in the records
                List<string> AllNames = new List<string>();
                string Name = null;
                bool NameFound = false;

                int i = 0;
                while (i < PoseRecordsCount)
                {
                    int j = 0;
                    while (j < PoseRecords[c, i].TemplateNames.Count)
                    {
                        NameFound = false;
                        Name = PoseRecords[c, i].TemplateNames[j];

                        int k = 0;
                        while (k < AllNames.Count)
                        {
                            if (Name == AllNames[k])
                            {
                                NameFound = true;
                                break; // TODO: might not be correct. Was : Exit Do
                            }
                            k += 1;
                        }
                        if ((!NameFound))
                        {
                            AllNames.Add(Name);
                        }
                        j += 1;
                    }
                    i += 1;
                }

                fMain.PP.SaveString("LastFilesDir", Path.GetDirectoryName(currentFilePath));

                if (Utils.ReadyForWriting(currentFilePath))
                {
                    FileStream fileStream = File.Open(currentFilePath, FileMode.Create, FileAccess.Write);
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        //print headline
                        string s = null;
                        string T = null;
                        s = "";

                        if (chkTimeStamp.Checked)
                        {
                            s = s + "\"t\"" + Constants.vbTab;
                        }
                        i = 0;
                        //each marker
                        while (i < AllNames.Count)
                        {
                            if (chkMarkerPosition.Checked)
                            {
                                s = s + "\"" + AllNames[i] + " X\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " Y\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " Z\"" + Constants.vbTab;
                            }
                            if (chkCloudPosition.Checked)
                            {
                                s = s + "\"" + AllNames[i] + " X(Cloud)\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " Y(Cloud)\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " Z(Cloud)\"" + Constants.vbTab;
                            }
                            if (chkTooltipPosition.Checked)
                            {
                                s = s + "\"" + AllNames[i] + " tip X\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " tip Y\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " tip Z\"" + Constants.vbTab;
                            }
                            if (chkMarkerPositionRefSpace.Checked)
                            {
                                s = s + "\"" + AllNames[i] + " X (Ref)\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " Y (Ref)\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " Z (Ref)\"" + Constants.vbTab;
                            }
                            if (chkTooltipPositionRefSpace.Checked)
                            {
                                s = s + "\"" + AllNames[i] + " tip X (Ref)\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " tip Y (Ref)\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " tip Z (Ref)\"" + Constants.vbTab;
                            }
                            if (chkMarkerAngularPosition.Checked)
                            {
                                s = s + "\"" + AllNames[i] + " Rx (rad)\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " Ry (rad)\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " Rz (rad)\"" + Constants.vbTab;
                            }
                            if (chkMarkerAngularPositionInRefSpace.Checked)
                            {
                                s = s + "\"" + AllNames[i] + " Rx_Ref (rad)\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " Ry_Ref (rad)\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " Rz_Ref (rad)\"" + Constants.vbTab;
                            }
                            if (chkTooltipAngularPositionInRefSpace.Checked)
                            {
                                s = s + "\"" + AllNames[i] + " tip Rx_Ref (rad)\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " tip Ry_Ref (rad)\"" + Constants.vbTab;
                                s = s + "\"" + AllNames[i] + " tip Rz_Ref (rad)\"" + Constants.vbTab;
                            }

                            if (chkMarkerRotation.Checked)
                            {
                                //each row
                                for (int j = 0; j <= 2; j++)
                                {
                                    s = s + "\"" + AllNames[i] + " R" + (j + 1).ToString() + "1\"" + Constants.vbTab;
                                    s = s + "\"" + AllNames[i] + " R" + (j + 1).ToString() + "2\"" + Constants.vbTab;
                                    s = s + "\"" + AllNames[i] + " R" + (j + 1).ToString() + "3\"" + Constants.vbTab;
                                }
                            }
                            if (chkHazards.Checked)
                            {
                                s = s + "Hazard?" + Constants.vbTab;
                            }
                            if (chkXPoints.Checked)
                            {
                                for (int fi = 0; fi <= MaxFacets - 1; fi++)
                                {
                                    //Longer vector / shorter vector
                                    for (int VI = 0; VI <= 1; VI++)
                                    {
                                        //base/head
                                        for (int j = 0; j <= 1; j++)
                                        {
                                            if ((j == 0))
                                            {
                                                T = " F" + fi + " V" + VI + "1" + ("B");
                                            }
                                            else
                                            {
                                                T = " F" + fi + " V" + VI + "1" + ("H");
                                            }
                                            s = s + "\"" + AllNames[i] + T + " X\"" + Constants.vbTab;
                                            s = s + "\"" + AllNames[i] + T + " Y\"" + Constants.vbTab;
                                            s = s + "\"" + AllNames[i] + T + " Z\"" + Constants.vbTab;
                                        }
                                    }
                                }
                            }
                            i += 1;
                        }
                        if (chkExposure.Checked)
                        {
                            s = s + "Shutter" + Constants.vbTab + "Gain" + Constants.vbTab;
                        }
                        if (chkTemperature.Checked)
                        {
                            s = s + "Deg C" + Constants.vbTab;
                        }
                        if (chkTemperatureRate.Checked)
                        {
                            s = s + "Deg C/h" + Constants.vbTab;
                        }
                        //some contents
                        if (!string.IsNullOrEmpty(s))
                        {
                            s = "\"#\"" + Constants.vbTab + s + Environment.NewLine;
                            //New line added
                            streamWriter.Write(s);
                            i = 0;
                            while (i < PoseRecordsCount)
                            {
                                WritePoseRecord(c, i, streamWriter, AllNames);
                                i += 1;
                            }
                        }

                        //add distance statistics between all XPs
                        if (chkDistanceStats.Checked)
                        {

                            WriteDistanceStats(c, streamWriter, AllNames);
                        }
                    }

                    SomeRecordsUnsaved = false;
                    SaveToINI();
                    //remember
                }

                c += 1;
            }

            if ((fMain.CaptureEnabled == false))
            {
                fMain.CaptureEnabled = true;
            }

        }

        private void PositionRecorderForm_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (SomeRecordsUnsaved && PoseRecordsCount > 0)
            {
                e.Cancel = MessageBox.Show("Records not saved. Exit without saving?", "Warning...", MessageBoxButtons.OKCancel) == DialogResult.Cancel;
            }
            fMain.PoseRecorderEnabled = false;

        }

        public void ShowStats()
        {
            double AvgDist = 0;
            double CI95 = 0;
            double RMSError = 0;
            double avgAngle = 0;

            double RMSAngleError = 0;
            string s = null;

            List<double> Dists = new List<double>();
            List<double> Angles = new List<double>();
            List<double[]> Pose2ndIn1st = new List<double[]>();

            double[] AvgPos = new double[3];

            long camIndex = 0;

            //no records
            if (PoseRecordsCount == 0)
            {
                return;
            }

            if (radiob2markers.Checked)
            {
                //Go through the records and look for cases where there were exactly two markers.
                //For each one, compute the angle and distance between them, as well as the
                //position of the 2nd marker in the 1st marker's coordinate space and enter it into the stats.

                camIndex = mCameras.CameraIndex(fMain.CurrCamera);

                int i = 0;
                while (i < PoseRecordsCount)
                {
                    PoseRecordType pr = PoseRecords[camIndex, i];
                    if ((pr != null))
                    {
                        if (pr.TemplateNames.Count == 2)
                        {
                            Xform3D[] XVect = new Xform3D[2];
                            Xform3D Xf = null;

                            Dists.Add(Utils.Distance(pr.MarkerXforms[0].ShiftVector, pr.MarkerXforms[1].ShiftVector));

                            double angleRad = 0;
                            double[] axis = null;
                            pr.MarkerXforms[0].AngularDifference(pr.MarkerXforms[1], out angleRad, out axis);
                            Angles.Add(angleRad * 180 / Math.PI);

                            Xf = pr.MarkerXforms[1].Concatenate(pr.MarkerXforms[0].Inverse());
                            //m2->camera->m1
                            Pose2ndIn1st.Add(Xf.ShiftVector);
                        }
                    }
                    i += 1;
                }
                if (Dists.Count < 3)
                {
                    rtxtStats.Text = "";
                    //clear
                    return;
                }

                AvgDist = Average(Dists);
                CI95 = ConfidenceInterval(Dists, AvgDist, 0.95);

                RMSError = RMSE(Dists, AvgDist);

                s = "In " + Dists.Count.ToString() + " Measurements:" + Environment.NewLine;
                s = s + "DIST: Avg = " + Math.Round(AvgDist, 2).ToString() + "mm" + Environment.NewLine;
                s = s + "  STD=" + Math.Round(RMSError, 3).ToString() + "mm, 95%CI = " + Math.Round(CI95, 3).ToString() + System.Environment.NewLine;

                avgAngle = Average(Angles);
                CI95 = ConfidenceInterval(Angles, avgAngle, 0.95);
                RMSAngleError = RMSE(Angles, avgAngle);
                s = s + "ANGLE: Avg = " + Math.Round(avgAngle, 2).ToString() + "°" + Environment.NewLine;
                s = s + "  STD=" + Math.Round(RMSAngleError, 3).ToString() + "°, 95%CI = " + Math.Round(CI95, 3).ToString() + Environment.NewLine;

                //compute the average position

                double PosCount = 0;

                PosCount = Pose2ndIn1st.Count;
                foreach (double[] Pos in Pose2ndIn1st)
                {
                    AvgPos[0] = AvgPos[0] + Pos[0] / PosCount;
                    AvgPos[1] = AvgPos[1] + Pos[1] / PosCount;
                    AvgPos[2] = AvgPos[2] + Pos[2] / PosCount;
                }
                //and the distance stats
                Dists = new List<double>();
                foreach (double[] Pos in Pose2ndIn1st)
                {
                    Dists.Add(Utils.Distance(Pos, AvgPos));
                }
                CI95 = ConfidenceInterval(Dists, 0, 0.95);
                RMSError = RMSE(Dists, 0);
                s = s + "Marker 2 in 1: (" + Math.Round(AvgPos[0], 1).ToString() + "," + Math.Round(AvgPos[1], 1).ToString() + "," + Math.Round(AvgPos[2], 1).ToString() + ")" + Environment.NewLine;
                s = s + "  STD=" + Math.Round(RMSError, 3).ToString() + "mm, 95%CI = " + Math.Round(CI95, 3).ToString() + Environment.NewLine;

                rtxtStats.Text = s;

            }
            else if (radiobTooltip.Checked)
            {
                //Use the first marker's name
                string Mname = null;
                List<double[]> Poses = new List<double[]>();


                Mname = PoseRecords[TheCamera, 0].TemplateNames[0];

                int i = 0;
                while (i < PoseRecordsCount)
                {
                    PoseRecordType pr = PoseRecords[camIndex, i];

                    int j = 0;
                    while (j < pr.TemplateNames.Count)
                    {
                        if (pr.TemplateNames[j] == markers[cbMarkerSelector.SelectedIndex].Name)
                        {
                            Poses.Add(pr.Tooltip2RefXforms[j].ShiftVector);
                        }
                        j += 1;
                    }
                    i += 1;
                }
                //compute average position

                i = 0;
                while (i < Poses.Count)
                {
                    AvgPos[0] = AvgPos[0] + Poses[i][0] / Poses.Count;
                    AvgPos[1] = AvgPos[1] + Poses[i][1] / Poses.Count;
                    AvgPos[2] = AvgPos[2] + Poses[i][2] / Poses.Count;
                    i += 1;
                }
                //compute distances
                i = 0;
                while (i < Poses.Count)
                {
                    Dists.Add(Utils.Distance(Poses[i], AvgPos));
                    i += 1;
                }

                if (Dists.Count < 3)
                {
                    rtxtStats.Text = "";
                    //clear
                    return;
                }
                CI95 = ConfidenceInterval(Dists, AvgDist, 0.95);
                RMSError = RMSE(Dists, AvgDist);

                s = Mname + " tooltip, " + Dists.Count.ToString() + " Measurements:" + Environment.NewLine;
                s = s + "STD=" + Math.Round(RMSError, 2).ToString() + "mm, 95%CI = " + Math.Round(CI95, 2).ToString() + Environment.NewLine;
                rtxtStats.Text = s;
            }


        }

        private double Average(List<double> inputList)
        {
            double tempAverage = 0;
            int i = 0;
            while (i < inputList.Count)
            {
                tempAverage = tempAverage + inputList[i];
                i += 1;
            }
            return tempAverage / (inputList.Count);
        }

        //OK
        private double ConfidenceInterval(IList<double> inputList, double TrueValue, double IntervalFraction)
        {
            //Reasonable performance if the number of items is small (in the hundreds)
            int i = 0;
            int SEi = 0;
            double ErrorSize = 0;
            List<double> SortedErrors = new List<double>();

            if (inputList.Count == 0)
            {
                return 0;
            }

            IntervalFraction = Math.Max(0, IntervalFraction);
            IntervalFraction = Math.Min(IntervalFraction, 1);

            //Calculate error size and sort into SortedErrors
            SortedErrors.Add(Math.Abs(inputList[0] - TrueValue));

            i = 1;
            while (i < inputList.Count)
            {
                ErrorSize = Math.Abs(inputList[i] - TrueValue);
                SEi = 0;
                foreach (double e in SortedErrors)
                {
                    //next one
                    if (e < ErrorSize)
                    {
                        SEi = SEi + 1;
                        //found it
                    }
                    else
                    {
                        break; // TODO: might not be correct. Was : Exit For
                    }
                }
                //add before SEi
                if (SEi < SortedErrors.Count)
                {
                    SortedErrors.Insert(SEi, ErrorSize);
                    //append at the end
                }
                else
                {
                    SortedErrors.Add(ErrorSize);
                }
                i += 1;
            }
            SEi = Convert.ToInt32(Conversion.Fix((SortedErrors.Count - 1) * IntervalFraction));
            return SortedErrors[SEi];
        }

        private double MaxE(IList<double> inputList, double value)
        {
            double tempMaxE = 0;
            double D = 0;

            int i = 0;
            while (i < inputList.Count)
            {
                D = inputList[i];
                if (Math.Abs(value - D) > tempMaxE)
                {
                    tempMaxE = Math.Abs(value - D);
                }
                i += 1;
            }
            return tempMaxE;
        }

        //OK
        private double RMSE(IList<double> inputList, double TrueValue)
        {
            double SquaredError = 0;
            double TotalSquaredError = 0;
            if (inputList.Count == 0)
            {
                return 0;
            }

            int i = 0;
            while (i < inputList.Count)
            {
                SquaredError = Math.Pow((inputList[i] - TrueValue), 2);
                TotalSquaredError = TotalSquaredError + SquaredError;
                i += 1;
            }
            return Math.Sqrt(TotalSquaredError / inputList.Count);
        }

        public bool AllowedToRecord()
        {
            bool Allowed = false;
            Marker M = default(Marker);
            //always record
            if (RecordingByFile)
            {
                return true;
            }

            switch (cboRecordingCondition.SelectedIndex)
            {
                case 0:
                    //at least 1 marker
                    Allowed = (markers.GetIdentifiedMarkers(null).Count > 0 | clouds.GetIdentifiedClouds(null).Count > 0);
                    break;
                case 1:
                    //at least 2 markers
                    Allowed = (markers.GetIdentifiedMarkers(null).Count > 1);
                    break;
                case 2:
                    //exactly 1 facet
                    if (markers.GetIdentifiedMarkers(null).Count == 1)
                    {
                        M = markers.GetIdentifiedMarkers(null)[0];
                        Allowed = (M.GetIdentifiedFacets(null, false).Count == 1);
                    }
                    break;
                case 3:
                    //exactly 1 marker
                    Allowed = (markers.GetIdentifiedMarkers(null).Count == 1);
                    break;
                case 4:
                    //exactly 2 markers
                    Allowed = (markers.GetIdentifiedMarkers(null).Count == 2);
                    break;
                default:
                    Allowed = true;
                    //no conditions
                    break;
            }
            return Allowed;
        }

        //OK
        public void AddRecord()
        {
            if (chkMultiFiles.Checked)
            {
                AddRecordMultipleFiles();
            }
            else
            {
                AddRecordSingleFile();
            }
        }

        private void AddRecordMultipleFiles()
        {
            if (PoseRecordsCount >= PoseRecords.GetLength(1))
            {
                RecordStream = false;
                //stop
                return;
            }
            Marker M = default(Marker);
            Cloud Cl = default(Cloud);
            Facet f = default(Facet);
            MTInterfaceDotNet.Vector V = default(MTInterfaceDotNet.Vector);
            IList<Facet> c = null;

            Camera cCam = default(Camera);

            int j = 0;
            while (j < mCameras.Count)
            {
                cCam = mCameras[j];

                PoseRecords[j, PoseRecordsCount] = new PoseRecordType();
                if ((UseFrameEmbeddedTimeStamp))
                {
                    PoseRecords[j, PoseRecordsCount].ElapsedSecs = cCam.FrameMTTimeSecs;
                }
                else
                {
                    PoseRecords[j, PoseRecordsCount].ElapsedSecs = SW.ElapsedMilliseconds / 1000.0;
                    //cCam.FrameMTTimeSecs
                }

                PoseRecords[j, PoseRecordsCount].Shutter = Convert.ToSingle(cCam.ShutterMsecs);
                PoseRecords[j, PoseRecordsCount].Gain = Convert.ToSingle(cCam.GainF);
                PoseRecords[j, PoseRecordsCount].Temperature = cCam.LastFrameTemperature;
                PoseRecords[j, PoseRecordsCount].TemperatureRate = cCam.LastFrameTemperatureRate;
                PoseRecords[j, PoseRecordsCount].TemplateNames = new List<string>();
                PoseRecords[j, PoseRecordsCount].MarkerXforms = new List<Xform3D>();
                PoseRecords[j, PoseRecordsCount].CloudXforms = new List<Xform3D>();
                PoseRecords[j, PoseRecordsCount].TooltipXforms = new List<Xform3D>();
                PoseRecords[j, PoseRecordsCount].Marker2RefXforms = new List<Xform3D>();
                PoseRecords[j, PoseRecordsCount].Tooltip2RefXforms = new List<Xform3D>();
                PoseRecords[j, PoseRecordsCount].TemplateFacetsCount = 1;
                int fi = 0;
                for (fi = 0; fi <= MaxFacets - 1; fi++)
                {
                    PoseRecords[j, PoseRecordsCount].FacetXPoints[fi] = new List<double[,]>();
                }
                PoseRecords[j, PoseRecordsCount].Hazards = new List<HazardCodes>();

                int i = 0;
                while (i < markers.GetIdentifiedMarkers(cCam).Count)
                {
                    M = markers.GetIdentifiedMarkers(cCam)[i];
                    if ((M.GetMarker2CameraXf(cCam) != null))
                    {
                        PoseRecords[j, PoseRecordsCount].TemplateNames.Add(M.Name);
                        PoseRecords[j, PoseRecordsCount].MarkerXforms.Add(M.GetMarker2CameraXf(cCam).Clone());
                        PoseRecords[j, PoseRecordsCount].Marker2RefXforms.Add(M.GetMarker2ReferenceXf(cCam).Clone());
                        PoseRecords[j, PoseRecordsCount].Hazards.Add(M.GetMarker2CameraXf(cCam).HazardCode);
                        var IsMarkerSliderControlled = M.MarkerSliderControlled;
                        if (IsMarkerSliderControlled)
                        {
                            PoseRecords[j, PoseRecordsCount].TooltipXforms.Add(M.SliderControlledTooltip2MarkerXfGet(1).Concatenate(M.GetMarker2CameraXf(cCam)));
                            PoseRecords[j, PoseRecordsCount].Tooltip2RefXforms.Add(M.SliderControlledTooltip2ReferenceXfGet(cCam).Clone());
                        }
                        else
                        {
                            PoseRecords[j, PoseRecordsCount].TooltipXforms.Add(M.Tooltip2MarkerXf.Concatenate(M.GetMarker2CameraXf(cCam)));
                            PoseRecords[j, PoseRecordsCount].Tooltip2RefXforms.Add(M.GetTooltip2ReferenceXf(cCam).Clone());
                        }
                        PoseRecords[j, PoseRecordsCount].TemplateFacetsCount = M.TemplateFacets.Count;
                        c = M.GetIdentifiedFacets(cCam);
                        for (int fci = 0; fci <= c.Count - 1; fci++)
                        {
                            f = c[fci];
                            var longVector = default(MTInterfaceDotNet.Vector);
                            MTInterfaceDotNet.Vector shortVector = new MTInterfaceDotNet.Vector();
                            f.GetIdentifiedVectors(out longVector, out shortVector);
                            V = longVector;
                            PoseRecords[j, PoseRecordsCount].FacetXPoints[M.GetIdentifiedFacetIndex(f, fMain.CurrCamera)].Add(V.EndPos);
                            V = shortVector;
                            PoseRecords[j, PoseRecordsCount].FacetXPoints[M.GetIdentifiedFacetIndex(f, fMain.CurrCamera)].Add(V.EndPos);
                        }
                    }
                    i += 1;
                }

                i = 0;
                while (i < clouds.GetIdentifiedClouds(cCam).Count)
                {
                    Cl = clouds.GetIdentifiedClouds(cCam)[i];
                    if ((Cl.GetCloud2CameraXf(cCam) != null))
                    {
                        PoseRecords[j, PoseRecordsCount].TemplateNames.Add(Cl.Name);
                        PoseRecords[j, PoseRecordsCount].CloudXforms.Add(Cl.GetCloud2CameraXf(cCam).Clone());
                    }
                    i += 1;
                }
                j += 1;
            }

            PoseRecordsCount = PoseRecordsCount + 1;
            lblRecordsCount.Text = PoseRecordsCount.ToString();
            SomeRecordsUnsaved = true;
        }

        private void AddRecordSingleFile()
        {
            if (PoseRecordsCount >= PoseRecords.GetLength(1))
            {
                RecordStream = false;
                //stop
                return;
            }
            Marker M = default(Marker);
            Cloud Cl = default(Cloud);
            Facet f = default(Facet);
            MTInterfaceDotNet.Vector V = default(MTInterfaceDotNet.Vector);
            IList<Facet> c = null;
            Xform3D Cam2CurrCamXf = default(Xform3D);
            double[] BasePos = null;
            double[] HeadPos = null;
            PoseRecords[0, PoseRecordsCount] = new PoseRecordType();
            if ((UseFrameEmbeddedTimeStamp))
            {
                PoseRecords[0, PoseRecordsCount].ElapsedSecs = fMain.CurrCamera.FrameMTTimeSecs;
            }
            else
            {
                PoseRecords[0, PoseRecordsCount].ElapsedSecs = SW.ElapsedMilliseconds / 1000.0;
                //fMain.CurrCamera.FrameMTTimeSecs
            }

            PoseRecords[0, PoseRecordsCount].Shutter = Convert.ToSingle(fMain.CurrCamera.ShutterMsecs);
            PoseRecords[0, PoseRecordsCount].Gain = Convert.ToSingle(fMain.CurrCamera.GainF);
            PoseRecords[0, PoseRecordsCount].Temperature = fMain.CurrCamera.LastFrameTemperature;
            PoseRecords[0, PoseRecordsCount].TemperatureRate = fMain.CurrCamera.LastFrameTemperatureRate;

            PoseRecords[0, PoseRecordsCount].TemplateNames = new List<string>();
            PoseRecords[0, PoseRecordsCount].MarkerXforms = new List<Xform3D>();
            PoseRecords[0, PoseRecordsCount].CloudXforms = new List<Xform3D>();
            PoseRecords[0, PoseRecordsCount].TooltipXforms = new List<Xform3D>();
            PoseRecords[0, PoseRecordsCount].Marker2RefXforms = new List<Xform3D>();
            PoseRecords[0, PoseRecordsCount].Tooltip2RefXforms = new List<Xform3D>();
            PoseRecords[0, PoseRecordsCount].TemplateFacetsCount = 1;
            int fi = 0;
            for (fi = 0; fi <= MaxFacets - 1; fi++)
            {
                PoseRecords[0, PoseRecordsCount].FacetXPoints[fi] = new List<double[,]>();
            }
            PoseRecords[0, PoseRecordsCount].Hazards = new List<HazardCodes>();

            int i = 0;
            while (i < markers.GetIdentifiedMarkers(null).Count)
            {
                M = markers.GetIdentifiedMarkers(null)[i];
                if ((M.GetMarker2CameraXf(fMain.CurrCamera) != null))
                {
                    PoseRecords[0, PoseRecordsCount].TemplateNames.Add(M.Name);
                    PoseRecords[0, PoseRecordsCount].MarkerXforms.Add(M.GetMarker2CameraXf(fMain.CurrCamera).Clone());
                    PoseRecords[0, PoseRecordsCount].Marker2RefXforms.Add(M.GetMarker2ReferenceXf(fMain.CurrCamera).Clone());
                    PoseRecords[0, PoseRecordsCount].Hazards.Add(M.GetMarker2CameraXf(fMain.CurrCamera).HazardCode);
                    var IsMarkerSliderControlled = M.MarkerSliderControlled;
                    if (IsMarkerSliderControlled)
                    {
                        PoseRecords[0, PoseRecordsCount].TooltipXforms.Add(M.SliderControlledTooltip2MarkerXfGet(1).Concatenate(M.GetMarker2CameraXf(fMain.CurrCamera)));
                        PoseRecords[0, PoseRecordsCount].Tooltip2RefXforms.Add(M.SliderControlledTooltip2ReferenceXfGet(fMain.CurrCamera).Clone());
                    }
                    else
                    {
                        PoseRecords[0, PoseRecordsCount].TooltipXforms.Add(M.Tooltip2MarkerXf.Concatenate(M.GetMarker2CameraXf(fMain.CurrCamera)));
                        PoseRecords[0, PoseRecordsCount].Tooltip2RefXforms.Add(M.GetTooltip2ReferenceXf(fMain.CurrCamera).Clone());
                    }
                    PoseRecords[0, PoseRecordsCount].TemplateFacetsCount = M.TemplateFacets.Count;

                    if (M.WasIdentified(fMain.CurrCamera))
                    {
                        //end positions in current camera space recorded directly
                        c = M.GetIdentifiedFacets(fMain.CurrCamera);
                        for (int fci = 0; fci <= c.Count - 1; fci++)
                        {
                            //If c.Count > 0 Then
                            f = c[fci];
                            var longVector = default(MTInterfaceDotNet.Vector);
                            MTInterfaceDotNet.Vector shortVector = new MTInterfaceDotNet.Vector();
                            f.GetIdentifiedVectors(out longVector, out shortVector);
                            V = longVector;
                            PoseRecords[0, PoseRecordsCount].FacetXPoints[M.GetIdentifiedFacetIndex(f, fMain.CurrCamera)].Add(V.EndPos);
                            V = shortVector;
                            PoseRecords[0, PoseRecordsCount].FacetXPoints[M.GetIdentifiedFacetIndex(f, fMain.CurrCamera)].Add(V.EndPos);
                            //End If
                        }
                    }
                    else
                    {
                        //find the camera which detected the marker
                        int j = 0;
                        while (j < mCameras.Count)
                        {
                            if (M.WasIdentified(mCameras[j]))
                            {
                                c = M.GetIdentifiedFacets(mCameras[j]);
                                Cam2CurrCamXf = mCameras.GetCamera2CameraXf(mCameras[j], fMain.CurrCamera);
                                if (c.Count > 0 & (Cam2CurrCamXf != null))
                                {
                                    for (int fci = 0; fci <= c.Count - 1; fci++)
                                    {
                                        //If c.Count > 0 Then
                                        f = c[fci];
                                        //f = c(0)
                                        //for each vector
                                        for (int k = 0; k <= 1; k++)
                                        {
                                            var longVector = default(MTInterfaceDotNet.Vector);
                                            MTInterfaceDotNet.Vector shortVector = new MTInterfaceDotNet.Vector();
                                            f.GetIdentifiedVectors(out longVector, out shortVector);

                                            V = longVector;
                                            if (k == 1)
                                            {
                                                V = shortVector;
                                            }

                                            //end positions in current camera space recorded via xform
                                            BasePos = Cam2CurrCamXf.XformLocation(V.BasePos);
                                            HeadPos = Cam2CurrCamXf.XformLocation(V.HeadPos);

                                            for (int coord = 0; coord <= 2; coord++)
                                            {
                                                EndPos2x3[0, coord] = BasePos[coord];
                                                EndPos2x3[1, coord] = HeadPos[coord];
                                            }
                                            PoseRecords[0, PoseRecordsCount].FacetXPoints[M.GetIdentifiedFacetIndex(f, fMain.CurrCamera)].Add(EndPos2x3);
                                        }
                                    }
                                }
                                goto FoundCamera;
                            }
                            j += 1;
                        }
                        //camera
                    }
                    FoundCamera:;
                }
                i += 1;

                //Added by huimei
                //Extract out holoMarker and refMarker data
                for (int b = 0; b < PoseRecords[0, PoseRecordsCount].TemplateNames.Count; b++)
                {
                    string templateName = PoseRecords[0, PoseRecordsCount].TemplateNames[b];

                    if (templateName.Equals(holoMarkerName)){
                        holoMarker = this.combineMatrix(PoseRecords[0, PoseRecordsCount].MarkerXforms[b].ShiftVector, PoseRecords[0, PoseRecordsCount].MarkerXforms[b].RotMat);

                        holoFlag = 1;
                    }else if (templateName.Equals(refMarkerName))
                    {
                        refMarker = this.combineMatrix(PoseRecords[0, PoseRecordsCount].MarkerXforms[b].ShiftVector, PoseRecords[0, PoseRecordsCount].MarkerXforms[b].RotMat);

                        refFlag = 1;
                    }

                    var line = new StringBuilder();
                    if (holoFlag == 1 && refFlag == 1)
                    {
                        for (int l = 0; l < holoMarker.GetLength(0); l++)
                        {
                            for (int m = 0; m < holoMarker.GetLength(1); m++)
                            {
                                line.Append(holoMarker[l, m]);
                                line.Append(",");
                            }
                        }
                        for (int l = 0; l < refMarker.GetLength(0); l++)
                        {
                            for (int m = 0; m < refMarker.GetLength(1); m++)
                            {
                                line.Append(refMarker[l, m]);
                                line.Append(",");
                            }
                        }
                        log.Debug(line.ToString());

                        this.processData(holoMarker, refMarker);

                        holoFlag = 0;
                        refFlag = 0;

                        break;
                    }
                }
                //End added by huimei
            }
            //marker

            i = 0;
            while (i < clouds.GetIdentifiedClouds(null).Count)
            {
                Cl = clouds.GetIdentifiedClouds(null)[i];
                if ((Cl.GetCloud2CameraXf(fMain.CurrCamera) != null))
                {
                    PoseRecords[0, PoseRecordsCount].TemplateNames.Add(Cl.Name);
                    PoseRecords[0, PoseRecordsCount].CloudXforms.Add(Cl.GetCloud2CameraXf(fMain.CurrCamera).Clone());
                    PoseRecords[0, PoseRecordsCount].Hazards.Add(Cl.GetCloud2CameraXf(fMain.CurrCamera).HazardCode);
                }
                i += 1;

            }
            //cloud

            PoseRecordsCount = PoseRecordsCount + 1;
            lblRecordsCount.Text = PoseRecordsCount.ToString();
            SomeRecordsUnsaved = true;
        }

        //Added by huimei
        private void processData(double[,] holoMarker, double[,] refMarker)
        {
            if (holoMarker != null && refMarker != null && leftEyeCalib != null && rightEyeCalib != null)
            {
                if (holoMarker.GetLength(0) == 4 && holoMarker.GetLength(1) == 4 && refMarker.GetLength(0) == 4 && refMarker.GetLength(1) == 4)
                {
                    if (leftEyeCalib.GetLength(0) == 4 && leftEyeCalib.GetLength(1) == 4 && rightEyeCalib.GetLength(0) == 4 && rightEyeCalib.GetLength(1) == 4)
                    {
                        StringBuilder leftEyeString = new StringBuilder();
                        StringBuilder rightEyeString = new StringBuilder();

                        StringBuilder final = new StringBuilder();

                        Matrix<Double> inverseHololens = DenseMatrix.OfArray(holoMarker).Inverse();
                        Matrix<Double> tmp = inverseHololens.Multiply(DenseMatrix.OfArray(refMarker));

                        //double[,] tmp = matrixMultiplication(matrixInverse(holoMarker), refMarker);

                        double[,] translatedPosition = new double[4, 1];

                        for (int i = 0; i < 4; i++)
                        {
                            translatedPosition[i, 0] = tmp[i, 3];
                        }

                        if (tmp != null)
                        {
                            //Process left eye
                            Matrix<Double> calibratedPosition = DenseMatrix.OfArray(leftEyeCalib).Multiply(DenseMatrix.OfArray(translatedPosition));
                            //double[,] calibratedPosition = matrixMultiplication(leftEyeCalib, translatedPosition);
                            double[,] leftFinal = new double[4, 4];
                            for (int i = 0; i < 4; i++)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    if (i < 3)
                                    {
                                        if (j == 3)
                                        {
                                            leftFinal [i, j] = calibratedPosition [i, 0] * (1/ calibratedPosition[3,0]);
                                        }
                                        else
                                        {
                                            leftFinal [i, j] = tmp[i, j];
                                        }
                                    }
                                    else
                                    {
                                        if (j == 3)
                                        {
                                            leftFinal[i, j] = 1.0;
                                        }
                                        else
                                        {
                                            leftFinal[i, j] = 0.0;
                                        }
                                    }
                                     
                                }
                            }

                            //double[,] leftFinal = matrixMultiplication(leftEyeCalib, tmp);
                            leftEyeString = matrixToString(leftFinal);

                            //Process right eye
                            calibratedPosition = DenseMatrix.OfArray(rightEyeCalib).Multiply(DenseMatrix.OfArray(translatedPosition));
                            double[,] rightFinal = new double[4, 4];
                            for (int i = 0; i < 4; i++)
                            {
                                for (int j = 0; j < 4; j++)
                                {
                                    if (i < 3)
                                    {
                                        if (j == 3)
                                        {
                                            rightFinal [i, j] = calibratedPosition [i, 0] * (1 / calibratedPosition[3, 0]);
                                        }
                                        else
                                        {
                                            rightFinal [i, j] = tmp[i, j];
                                        }
                                    }
                                    else
                                    {
                                        if (j == 3)
                                        {
                                            rightFinal[i, j] = 1.0;
                                        }
                                        else
                                        {
                                            rightFinal[i, j] = 0.0;
                                        }
                                    }
                                     
                                }
                            }

                            //double[,] rightFinal = matrixMultiplication(rightEyeCalib, tmp);
                            rightEyeString = matrixToString(leftFinal);

                            if(leftEyeString.Trim() != null && rightEyeString.Trim() != null)
                            {
                                final = leftEyeString + ";" + rightEyeString;

                                MainForm.SendMessage(final);
                            }
                        }
                    }//End of check leftEye and rightEye calibration matrix size
                }//End of check holoMarker and refMarker matrix size
            }//End of check if any of the matrix is null
        }

        #region matrixOperation
        private String matrixToString (double[,] matrix)
        {
            String result = "";

            if (matrix != null)
            {
                for (int i = 0; i < matrix.GetLength(0) - 1; i++)
                {
                    for (int j = 0; j < matrix.GetLength(1); j++)
                    {
                        if (i == 2 && j == 3)
                        {
                            result = result + matrix[i,j].ToString();
                        }
                        else
                        {
                            result = result + matrix[i, j].ToString() + ",";
                        }
                    }
                }

                return result;
            }
            return null;
        }

        private double[,] combineMatrix(double[] translation, double[,] rotation)
        {
            if (rotation.GetLength(0) != 3 && rotation.GetLength(1) != 3)
            {
                return null;
            }
            else
            {
                if (translation.Length != 3)
                {
                    return null;
                }
            }

            double[,] combinedMatrix = new double[4, 4];

            for (int i = 0; i < combinedMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < combinedMatrix.GetLength(1) - 1; j++)
                {
                    if (i != 3)
                    {
                        combinedMatrix[i, j] = rotation[j, i];
                    }
                    else
                    {
                        combinedMatrix[i, j] = 0.00;
                    }
                }
            }

            for (int k = 0; k < combinedMatrix.GetLength(1); k++)
            {
                if (k != 3)
                {
                    combinedMatrix[k, 3] = translation[k];
                }
                else
                {
                    combinedMatrix[k, 3] = 1.00;
                }

            }
            return combinedMatrix;
        }

        private double[,] matrixMultiplication(double[,] matrixA, double[,] matrixB)
        {
            int aRows = matrixA.GetLength(0);
            int aCols = matrixA.GetLength(1);
            int bRows = matrixB.GetLength(0);
            int bCols = matrixB.GetLength(1);

            if (aCols != bRows)
            {
                //throw new Exception("Non-conformable matrices");
                return null;
            }

            double[,] result = new double[aRows, bCols];

            for (int i = 0; i < aRows; ++i)
            {
                for (int j = 0; j < bCols; ++j)
                {
                    for (int k = 0; k < aCols; ++k)
                        result[i, j] += matrixA[i, k] * matrixB[k, j];
                }
            }

            return result;
        }

        //Added by huimei, code gotten from https://jamesmccaffrey.wordpress.com/2011/08/03/lower-upper-matrix-decomposition-implementation-in-c/
        private double[,] matrixInverse(double[,] matrix)
        {
            int r = matrix.GetLength(0);
            int c = matrix.GetLength(1);

            if (r != c)
            {
                return null;
            }

            int n = r;
            double[,] result = new double[n, n];

            double[] col = new double[n];
            double[] x = new double[n];

            int[] indx = new int[n];
            double[,] luMatrix = MatrixDecomposition(matrix, indx);

            if (luMatrix == null) return null;

            for (int j = 0; j < n; ++j)
            {
                for (int i = 0; i < n; ++i) { col[i] = 0.0; }
                col[j] = 1.0;
                x = MatrixBackSub(luMatrix, indx, col);
                for (int i = 0; i < n; ++i) { result[i, j] = x[i]; }
            }
            return result;
        }

        static double[,] MatrixDecomposition(double[,] matrix, int[] indx)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            if (rows != cols)
            {
                return null;
            }

            int n = rows; // to sync with book notation
            int imax = 0; 
            double big = 0.0;
            double temp = 0.0;
            double sum = 0.0;
            double[] vv = new double[n];

            double[,] result = MatrixDuplicate(matrix);

            for (int i = 0; i < n; ++i)
            {
                big = 0.0;
                for (int j = 0; j < n; ++j)
                {
                    temp = Math.Abs(result[i,j]); 
                    if (temp > big)
                    {
                        big = temp;
                    } 
                }
                if (big == 0.0)
                {
                    return null;
                }

                vv[i] = 1.0 / big;
            }

            for (int j = 0; j < n; ++j)
            {
                for (int i = 0; i < j; ++i)
                {
                    sum = result[i,j];

                    for (int k = 0; k < i; ++k)
                    {
                        sum -= result[i,k] * result[k,j];
                    }

                    result[i,j] = sum;
                }

                big = 0.0;

                for (int i = j; i < n; ++i)
                {
                    sum = result[i, j];
                    for (int k = 0; k < j; ++k)
                    {
                        sum -= result[i, k] * result[k, j];
                    }

                    result[i,j] = sum;
                    temp = vv[i] * Math.Abs(sum);

                    if (temp >= big)
                    {
                        big = temp;
                        imax = i;
                    }
                }

                if (j != imax)
                {
                    for (int k = 0; k < n; ++k)
                    {
                        temp = result[imax,k];
                        result[imax,k] = result[j,k];
                        result[j,k] = temp;
                    }

                    vv[imax] = vv[j];
                }

                indx[j] = imax;

                if (result[j,j] == 0.0)
                {
                    result[j,j] = 1.0e-20;
                }

                if (j != n - 1){
                    temp = 1.0 / result[j,j];

                    for (int i = j + 1; i < n; ++i)
                    {
                        result[i,j] *= temp;
                    }
                }
            }

            return result ;
        }

        static double[,] MatrixDuplicate (double[,] matrix)
        {
            double[][] result = new double[matrix.GetLength(1)][];
            double[,] finalResult = new double[matrix.GetLength(0), matrix.GetLength(1)];

            int cols = matrix.GetLength(1); // assume all columns have equal size

            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = new double[cols];
            }
                
            for (int i = 0; i < matrix.GetLength(0); ++i)
            {
                for (int j = 0; j < matrix.GetLength(1); ++j)
                    result[i][j] = matrix[i,j];
            }
                
            for (int i = 0; i < finalResult.GetLength(0); i++)
            {
                for (int j = 0; j < finalResult.GetLength(1); j++)
                {
                    finalResult[i, j] = result[i][j];
;                }
            }
            return finalResult;
        }

        static double[] MatrixBackSub(double[,] luMatrix, int[] indx, double[] b)
        {
            int rows = luMatrix.GetLength(0);
            int cols = luMatrix.GetLength(1);
            if (rows != cols)
            {
                return null;
            }

            int ii = 0; int ip = 0;
            int n = b.Length;
            double sum = 0.0;

            double[] x = new double[b.Length];
            b.CopyTo(x, 0);

            for (int i = 0; i < n; ++i)
            {
                ip = indx[i];
                sum = x[ip];
                x[ip] = x[i];

                if (ii == 0)
                {
                    for (int j = ii; j <= i - 1; ++j)
                    {
                        sum -= luMatrix[i,j] * x[j];
                    }
                }
                else if(sum == 0.0)
                {
                    ii = 1;
                }
                x[i] = sum;
            }

            for (int i = n - 1; i >= 0; --i)
            {
                sum = x[i];

                for (int j = i + 1; j < n; ++j)
                {
                    sum -= luMatrix[i,j] * x[j];
                }
                x[i] = sum / luMatrix[i,i];
            }

            return x;
        }
        //End added by huimei, code gotten from https://jamesmccaffrey.wordpress.com/2011/08/03/lower-upper-matrix-decomposition-implementation-in-c/
        #endregion

        //End added by huimei

        private void btnRecordPose_Click(System.Object sender, System.EventArgs e)
        {
            if (AllowedToRecord())
            {
                AddRecord();
                ShowStats();
            }

        }

        private void WritePoseRecord(long camIndex, int RecordI, StreamWriter streamWriter, IList<string> TemplateNames)
        {
            int Ti = 0;
            string s = null;
            double[] P = null;
            s = (RecordI + 1).ToString() + Constants.vbTab;

            if (chkTimeStamp.Checked)
            {
                s = s + " " + Math.Round(PoseRecords[camIndex, RecordI].ElapsedSecs, 3).ToString() + Constants.vbTab;
            }
            int TNameI = 0;

            TNameI = 0;
            while (TNameI < TemplateNames.Count)
            {
                //find the index of this marker in this record
                Ti = -1;
                int i = 0;

                while (i < PoseRecords[camIndex, RecordI].TemplateNames.Count)
                {
                    if (PoseRecords[camIndex, RecordI].TemplateNames[i] == TemplateNames[TNameI])
                    {
                        Ti = i;
                        break; // TODO: might not be correct. Was : Exit Do
                    }
                    i += 1;
                }
                Xform3D Xf = default(Xform3D);
                if (chkMarkerPosition.Checked)
                {
                    //marker not in frame: insert 3 0s
                    if ((PoseRecords[camIndex, RecordI].MarkerXforms.Count < 1 | Ti < 0))
                    {
                        s = s + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab;
                        //s & "0 0 0 "
                        //marker in frame
                    }
                    else
                    {
                        //insert the origin (ie, the shift vector)
                        Xf = PoseRecords[camIndex, RecordI].MarkerXforms[Ti];
                        P = Xf.ShiftVector;
                        for (i = 0; i <= 2; i++)
                        {
                            s = s + P[i].ToString("000.0000") + Constants.vbTab;
                        }
                    }
                }
                if (chkCloudPosition.Checked)
                {
                    //cloud not in frame: insert 3 0s
                    if ((PoseRecords[camIndex, RecordI].CloudXforms.Count < 1 | Ti < 0))
                    {
                        s = s + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab;
                        //s & "0 0 0 "
                        //marker in frame
                    }
                    else
                    {
                        //insert the origin (ie, the shift vector)
                        Xf = PoseRecords[camIndex, RecordI].CloudXforms[Ti];
                        P = Xf.ShiftVector;
                        for (i = 0; i <= 2; i++)
                        {
                            s = s + P[i].ToString("000.0000") + Constants.vbTab;
                        }
                    }
                }
                if (chkTooltipPosition.Checked)
                {
                    //marker not in frame: insert 3 0s
                    if ((PoseRecords[camIndex, RecordI].MarkerXforms.Count < 1 | Ti < 0))
                    {
                        s = s + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab;
                        //s & "0 0 0 "
                        //marker in frame
                    }
                    else
                    {
                        //insert the origin (ie, the shift vector)
                        Xf = PoseRecords[camIndex, RecordI].TooltipXforms[Ti];
                        P = Xf.ShiftVector;
                        for (i = 0; i <= 2; i++)
                        {
                            s = s + P[i].ToString("000.0000") + Constants.vbTab;
                        }
                    }
                }
                if (chkMarkerPositionRefSpace.Checked)
                {
                    //marker not in frame: insert 3 0s
                    if ((PoseRecords[camIndex, RecordI].MarkerXforms.Count < 1 | Ti < 0))
                    {
                        s = s + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab;
                        //s & "0 0 0 "
                        //marker in frame
                    }
                    else
                    {
                        //insert the origin (ie, the shift vector)
                        Xf = PoseRecords[camIndex, RecordI].Marker2RefXforms[Ti];
                        P = Xf.ShiftVector;
                        for (i = 0; i <= 2; i++)
                        {
                            s = s + P[i].ToString("000.0000") + Constants.vbTab;
                        }
                    }
                }
                if (chkTooltipPositionRefSpace.Checked)
                {
                    //marker not in frame: insert 3 0s
                    if ((PoseRecords[camIndex, RecordI].MarkerXforms.Count < 1 | Ti < 0))
                    {
                        s = s + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab;
                        //s & "0 0 0 "
                        //marker in frame
                    }
                    else
                    {
                        //insert the origin (ie, the shift vector)
                        Xf = PoseRecords[camIndex, RecordI].Tooltip2RefXforms[Ti];
                        P = Xf.ShiftVector;
                        for (i = 0; i <= 2; i++)
                        {
                            s = s + P[i].ToString("000.0000") + Constants.vbTab;
                        }
                    }
                }

                if (chkMarkerAngularPosition.Checked)
                {
                    //marker not in frame: insert 3 0s
                    if ((PoseRecords[camIndex, RecordI].MarkerXforms.Count < 1 | Ti < 0))
                    {
                        s = s + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab;
                        //s & "0 0 0 "
                        //marker in frame
                    }
                    else
                    {
                        //insert the origin (ie, the shift vector)
                        Xf = PoseRecords[camIndex, RecordI].MarkerXforms[Ti];
                        P = Xf.RotAnglesRads;
                        for (i = 0; i <= 2; i++)
                        {
                            s = s + P[i].ToString("000.0000") + Constants.vbTab;
                        }
                    }
                }
                if (chkMarkerAngularPositionInRefSpace.Checked)
                {
                    //marker not in frame: insert 3 0s
                    if ((PoseRecords[camIndex, RecordI].MarkerXforms.Count < 1 | Ti < 0))
                    {
                        s = s + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab;
                        //s & "0 0 0 "
                        //marker in frame
                    }
                    else
                    {
                        //insert the origin (ie, the shift vector)
                        Xf = PoseRecords[camIndex, RecordI].Marker2RefXforms[Ti];
                        P = Xf.RotAnglesRads;
                        for (i = 0; i <= 2; i++)
                        {
                            s = s + P[i].ToString("000.0000") + Constants.vbTab;
                        }
                    }
                }
                if (chkTooltipAngularPositionInRefSpace.Checked)
                {
                    //marker not in frame: insert 3 0s
                    if ((PoseRecords[camIndex, RecordI].MarkerXforms.Count < 1 | Ti < 0))
                    {
                        s = s + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab;
                        //s & "0 0 0 "
                        //marker in frame
                    }
                    else
                    {
                        //insert the origin (ie, the shift vector)
                        Xf = PoseRecords[camIndex, RecordI].Tooltip2RefXforms[Ti];
                        P = Xf.RotAnglesRads;
                        for (i = 0; i <= 2; i++)
                        {
                            s = s + P[i].ToString("000.0000") + Constants.vbTab;
                        }
                    }
                }
                if (chkMarkerRotation.Checked)
                {
                    //repeat for the Xforms
                    //none found: insert 9 0s
                    if ((PoseRecords[camIndex, RecordI].MarkerXforms.Count < 1 | Ti < 0))
                    {
                        s = s + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab;
                        //s & "0 0 0 0 0 0 0 0 0 "
                        //insert rotation matrix
                    }
                    else
                    {
                        for (i = 0; i <= 2; i++)
                        {
                            for (int j = 0; j <= 2; j++)
                            {
                                s = s + PoseRecords[camIndex, RecordI].MarkerXforms[Ti].RotMat[j, i].ToString("0.00000") + Constants.vbTab;
                            }
                        }
                    }
                }
                if ((PoseRecords[camIndex, RecordI].Hazards.Count > 0 & chkHazards.Checked))
                {
                    if (Ti < 0)
                    {
                        s = s + "*" + Constants.vbTab;
                    }
                    else
                    {
                        s = s + PoseRecords[camIndex, RecordI].Hazards[Ti].ToString() + Constants.vbTab;
                    }
                }
                //insert the 4 XPoints' coordinates (4x3 = 12 numbers)
                if (chkXPoints.Checked)
                {
                    if (Ti < 0)
                    {
                        for (int fi = 0; fi <= MaxFacets - 1; fi++)
                        {
                            s = s + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab;
                            //s & "0 0 0 0 0 0 0 0 0 0 0 0 "
                        }
                        //insert the XPoints in order
                    }
                    else
                    {
                        int VI = 0;
                        double[,] EndPos = null;

                        for (int fi = 0; fi <= MaxFacets - 1; fi++)
                        {
                            if ((PoseRecords[camIndex, RecordI].FacetXPoints[fi].Count > 0))
                            {
                                if (fi < PoseRecords[camIndex, RecordI].TemplateFacetsCount)
                                {
                                    //LV, SV
                                    for (VI = 0; VI <= 1; VI++)
                                    {
                                        EndPos = PoseRecords[camIndex, RecordI].FacetXPoints[fi][2 * Ti + VI];
                                        //base/head
                                        for (i = 0; i <= 1; i++)
                                        {
                                            for (int j = 0; j <= 2; j++)
                                            {
                                                s = s + EndPos[i, j].ToString("0.00000") + Constants.vbTab;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                s = s + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab + "0 " + Constants.vbTab + " 0" + Constants.vbTab + "0" + Constants.vbTab;
                                //s & "0 0 0 0 0 0 0 0 0 0 0 0 "
                            }

                        }
                    }
                }
                TNameI += 1;
            }
            if (chkExposure.Checked)
            {
                s = s + PoseRecords[camIndex, RecordI].Shutter.ToString("0.0000") + Constants.vbTab + PoseRecords[camIndex, RecordI].Gain.ToString("0.0000") + Constants.vbTab;
            }
            if (chkTemperature.Checked)
            {
                s = s + PoseRecords[camIndex, RecordI].Temperature.ToString("0.00") + Constants.vbTab;
            }
            if (chkTemperatureRate.Checked)
            {
                s = s + PoseRecords[camIndex, RecordI].TemperatureRate.ToString("0.00") + Constants.vbTab;
            }
            streamWriter.Write(s + Environment.NewLine);
            //new line added
        }

        private void WriteDistanceStats(int camIndex, StreamWriter streamWriter, IList<string> TemplateNames)
        {
            streamWriter.Write(Environment.NewLine + "XP - XP" + Constants.vbTab + "Distance(mm)" + Constants.vbTab + "STD" + Constants.vbTab + "95%CI" + Environment.NewLine);
            //for each XP, calculate the average and STD of distance to each other XP

            long Count = 0;
            double Average = 0;

            int PRCI = 0;
            List<double> Dists = null;
            int BaseNameI = 0;
            int BaseVI = 0;
            int BaseEndI = 0;
            int NameI = 0;
            int VI = 0;
            int EndI = 0;
            string s = null;

            BaseNameI = 0;
            while (BaseNameI < TemplateNames.Count)
            {
                //vector LV, SV
                for (BaseVI = 0; BaseVI <= 1; BaseVI++)
                {
                    //base head
                    for (BaseEndI = 0; BaseEndI <= 1; BaseEndI++)
                    {
                        NameI = BaseNameI;
                        while (NameI < TemplateNames.Count)
                        {
                            //vector LV, SV
                            for (VI = 0; VI <= 1; VI++)
                            {
                                //base head
                                for (EndI = 0; EndI <= 1; EndI++)
                                {

                                    if (!(NameI == BaseNameI && (VI < BaseVI || (VI == BaseVI && EndI <= BaseEndI))))
                                    {
                                        Count = 0;
                                        Average = 0;

                                        Dists = new List<double>();
                                        //process all records that contain the pair

                                        PRCI = 0;
                                        while (PRCI < PoseRecordsCount)
                                        {
                                            //find the index of the base marker in this record
                                            int BaseMI = 0;
                                            int Ti = 0;
                                            BaseMI = -1;
                                            Ti = -1;

                                            int loopCount = PoseRecords[camIndex, PRCI].TemplateNames.Count;
                                            int i = 0;
                                            while (i < loopCount)
                                            {
                                                if (PoseRecords[camIndex, PRCI].TemplateNames[i] == TemplateNames[BaseNameI])
                                                {
                                                    BaseMI = i;
                                                }
                                                if (PoseRecords[camIndex, PRCI].TemplateNames[i] == TemplateNames[NameI])
                                                {
                                                    Ti = i;
                                                }
                                                i += 1;
                                            }
                                            if (Ti > -1 & BaseMI > -1 & (PoseRecords[camIndex, PRCI].FacetXPoints[0].Count > 0))
                                            {
                                                double[] BasePos = new double[3];
                                                double[] Pos = new double[3];
                                                double[,] BaseXPs = null;
                                                double[,] XPs = null;
                                                double D = 0;
                                                BaseXPs = PoseRecords[camIndex, PRCI].FacetXPoints[0][2 * (BaseMI + 1) - 1 + BaseVI - 1];
                                                XPs = PoseRecords[camIndex, PRCI].FacetXPoints[0][2 * (Ti + 1) - 1 + VI - 1];
                                                for (int j = 0; j <= 2; j++)
                                                {
                                                    BasePos[j] = BaseXPs[BaseEndI, j];
                                                    Pos[j] = XPs[EndI, j];
                                                }
                                                D = Utils.Distance(BasePos, Pos);
                                                Dists.Add(D);
                                                Count = Count + 1;
                                                Average = (Average * (Count - 1) + D) / Count;
                                            }
                                            PRCI += 1;
                                        }
                                        //record
                                        //Done with this pair, write it out
                                        s = TemplateNames[BaseNameI] + "." + BaseVI.ToString() + "." + BaseEndI.ToString() + " - " + TemplateNames[NameI] + "." + VI.ToString() + "." + EndI.ToString();
                                        s = s + Constants.vbTab + Math.Round(Average, 4).ToString() + Constants.vbTab + Math.Round(RMSE(Dists, Average), 4).ToString() + Constants.vbTab + Math.Round(ConfidenceInterval(Dists, Average, 0.95), 4).ToString();
                                        streamWriter.Write(s + Environment.NewLine);
                                        //new line added

                                    }
                                }
                            }
                            NameI += 1;
                        }
                    }
                }
                BaseNameI += 1;
            }

        }

        private void SaveToINI()
        {
            fMain.PP.Section = "General";
            fMain.PP.SaveString("RecordingIntervalMsecs", tbMsecs.Text);

            fMain.PP.SaveBool("chkTimeStamp", chkTimeStamp.Checked);
            fMain.PP.SaveBool("chkExposure", chkExposure.Checked);
            fMain.PP.SaveBool("chkTemperature", chkTemperature.Checked);
            fMain.PP.SaveBool("chkTemperatureRate", chkTemperatureRate.Checked);
            fMain.PP.SaveBool("chkDistanceStats", chkDistanceStats.Checked);
            fMain.PP.SaveBool("chkMarkerPosition", chkMarkerPosition.Checked);
            fMain.PP.SaveBool("chkTooltipPosition", chkTooltipPosition.Checked);
            fMain.PP.SaveBool("chkMarkerPositionInReferenceSpace", chkMarkerPositionRefSpace.Checked);
            fMain.PP.SaveBool("chkTooltipPositionInReferenceSpace", chkTooltipPositionRefSpace.Checked);
            fMain.PP.SaveBool("chkMarkerAngularPosition", chkMarkerAngularPosition.Checked);
            fMain.PP.SaveBool("chkMarkerAngularPositionInRefSpace", chkMarkerAngularPositionInRefSpace.Checked);
            fMain.PP.SaveBool("chkTooltipAngularPositionInRefSpace", chkTooltipAngularPositionInRefSpace.Checked);
            fMain.PP.SaveBool("chkMarkerRotation", chkMarkerRotation.Checked);
            fMain.PP.SaveBool("chkXPoints", chkXPoints.Checked);
            fMain.PP.SaveBool("chkHazards", chkHazards.Checked);
        }

        private void RestoreFromINI()
        {
            tbMsecs.Text = fMain.PP.RetrieveString("RecordingIntervalMsecs", "50");

            chkTimeStamp.Checked = fMain.PP.RetrieveBool("chkTimeStamp", false);
            chkExposure.Checked = fMain.PP.RetrieveBool("chkExposure", false);
            chkTemperature.Checked = fMain.PP.RetrieveBool("chkTemperature", false);
            chkTemperatureRate.Checked = fMain.PP.RetrieveBool("chkTemperatureRate", false);
            chkDistanceStats.Checked = fMain.PP.RetrieveBool("chkDistanceStats", false);
            chkMarkerPosition.Checked = fMain.PP.RetrieveBool("chkMarkerPosition", false);
            chkTooltipPosition.Checked = fMain.PP.RetrieveBool("chkTooltipPosition", false);
            chkMarkerPositionRefSpace.Checked = fMain.PP.RetrieveBool("chkMarkerPositionInReferenceSpace", false);
            chkTooltipPositionRefSpace.Checked = fMain.PP.RetrieveBool("chkTooltipPositionInReferenceSpace", false);
            chkMarkerAngularPosition.Checked = fMain.PP.RetrieveBool("chkMarkerAngularPosition", false);
            chkMarkerAngularPositionInRefSpace.Checked = fMain.PP.RetrieveBool("chkMarkerAngularPositionInRefSpace", false);
            chkTooltipAngularPositionInRefSpace.Checked = fMain.PP.RetrieveBool("chkTooltipAngularPositionInRefSpace", false);
            chkMarkerRotation.Checked = fMain.PP.RetrieveBool("chkMarkerRotation", false);
            chkXPoints.Checked = fMain.PP.RetrieveBool("chkXPoints", false);
            chkHazards.Checked = fMain.PP.RetrieveBool("chkHazards", false);
        }

        bool initialized = false;

        private void PositionRecorderForm_Load(System.Object sender, System.EventArgs e)
        {
            initialized = true;

            SW.Reset();
            SW.Start();

            fMain.CaptureEnabled = true;
            fMain.MarkersProcessingEnabled = true;
            int temp = cboRecordingCondition.Items.Count;
            cboRecordingCondition.SelectedIndex = 0;
            PoseRecordsCount = 0;
            lblRecordsCount.Text = PoseRecordsCount.ToString();
            RestoreFromINI();
            if (mCameras.Count > 1)
            {
                chkMultiFiles.Visible = true;
            }
            else
            {
                chkMultiFiles.Checked = false;
                chkMultiFiles.Visible = false;
            }
            fMain.PoseRecorderEnabled = true;
            cbMarkerSelector.Items.Clear();
            int i = 0;
            while (i < markers.Count)
            {
                cbMarkerSelector.Items.Insert(i, markers[i].Name + "(" + markers[i].TemplateFacets.Count.ToString() + ")");
                i += 1;
            }
            while (i < clouds.Count)
            {
                cbMarkerSelector.Items.Insert(i + markers.Count, clouds[i].Name + "(Cloud)");
                i += 1;
            }
        }

    }

}