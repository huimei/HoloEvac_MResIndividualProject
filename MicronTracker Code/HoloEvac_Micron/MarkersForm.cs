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

namespace HoloEvac_Micron
{
    public partial class MarkersForm : Form
    {

        private MainForm fMain;
        public MarkersForm(MainForm mForm)
        {
            InitializeComponent();
            fMain = mForm;
        }

        private Marker currMarker_Renamed;
        public Marker CurrMarker
        {
            get { return currMarker_Renamed; }
            set
            {
                if ((currMarker_Renamed != value))
                {
                    currMarker_Renamed = value;
                    ResetSampling();
                }
            }
        }

        private Cloud currCloud_Renamed;
        public Cloud CurrCloud
        {
            get { return currCloud_Renamed; }
            set
            {
                if ((currCloud_Renamed != value))
                {
                    currCloud_Renamed = value;
                    ResetSampling();
                }
            }
        }

        private bool ignore = false;
        private Facet NewFacet;
        private Markers markers = MT.Markers;
        private bool DisplayUTinfobox = false;
        private XPoints xpoints = MT.XPoints;
        private Clouds clouds = MT.Clouds;
        private bool WasXPointsProcessingEnabled;
        private bool WasJitterFilterEnabled;
        private bool WaskalmanFilterEnabled;
        private bool WasSmallMarkerDetectionEnabled;

        private Cloud NewCloud;

        //vectors taken at each sampling point
        private List<Vector> VectorSamples = new List<Vector>();
        //xform3D object corresponding the to the vectors sample
        private List<Xform3D> Facet1ToCameraXfs = new List<Xform3D>();
        //vectors taken at each sampling point
        private List<Xform3D> Tip2ToolSamples = new List<Xform3D>();
        //vectors taken at each sampling point
        private List<XPoint> SliderXPointSamples = new List<XPoint>();
        // = New List(Of XPoint)() 'Dictionary(Of Integer, XPoint) = New Dictionary(Of Integer, XPoint)() ' COP samples
        private List<XPoint>[] COPsamples = new List<XPoint>[101];


        private int NumberofXPs;
        private string ToolName;

        private Marker TTCal;
        private bool SavedIsShowingVectors;
        private double SavedShownVectorsMinLength;

        private long SavedPredictiveFramesInterleave;

        private long PrevFrameCounter;

        //Form state
        private enum StateEnum : int
        {
            Idling,
            SamplingNewMarker,
            SamplingAdditionalFacet,
            SamplingToolTip,
            SamplingSlider,
            SamplingCOP2Marker,
            //cloud of Points
            SamplingCOP
            //cloud of Points
        }
        private bool SamplingSuspended;
        private StateEnum FormState;

        private bool initialized = false;

        // For sampling markers with known vector lengths
        private string[] KnownTemplateVectorsMarkerNames;
        private double[][] KnownTemplateVectorsLengths;
        private double[] KnownTemplateVectorsTolerances;

        //Added by huimei, logger to file 
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //End added by huimei

        private void MarkersForm_Load(System.Object sender, System.EventArgs e){
            //Added by huimei
            log.Info("In MarkersForm_Load");
            //End added by huimei

            WasXPointsProcessingEnabled = fMain.XPointsProcessingEnabled;
            WasJitterFilterEnabled = markers.JitterFilterEnabled;
            WaskalmanFilterEnabled = markers.KalmanFilterEnabled;
            WasSmallMarkerDetectionEnabled = markers.SmallerXPFootprint;

            // Optimize registration accuracy
            markers.JitterFilterEnabled = false;
            markers.KalmanFilterEnabled = false;
            markers.SmallerXPFootprint = false;

            initialized = true;
            fMain.IsShowingVectorsForTemplateRegistration = true;

            for (int i = 0; i <= 100; i++)
            {
                COPsamples[i] = new List<XPoint>();
            }
            NumberofXPs = 0;

            VectorSamples = new List<Vector>();

            Facet1ToCameraXfs = new List<Xform3D>();
            RefreshList(false);

            //Save current fmain settings for restoring later
            SavedPredictiveFramesInterleave = markers.PredictiveFramesInterleave;
            SavedIsShowingVectors = fMain.IsShowingVectors;
            SavedShownVectorsMinLength = fMain.ShownVectorsMinLength;

            //Modify the settings for recording new templates
            fMain.IsShowingVectors = true;
            fMain.ShownVectorsMinLength = Convert.ToDouble(nudMinVectorLength.Value);

            //Disable predictive tracking to allow unidentified vectors to be properly
            //identified in each frame
            markers.PredictiveFramesInterleave = 0;
            double TtCalibOffsetX = fMain.TTCalib_Offset_X;
            double TtCalibOffsetY = fMain.TTCalib_Offset_Y;
            double TtCalibOffsetZ = fMain.TTCalib_Offset_Z;
            double TtCalibOffsetRX = fMain.TTCalib_Offset_RX;
            double TtCalibOffsetRY = fMain.TTCalib_Offset_RY;
            double TtCalibOffsetRZ = fMain.TTCalib_Offset_RZ;
            tbOffsetX.Text = TtCalibOffsetX.ToString();
            tbOffsetY.Text = TtCalibOffsetY.ToString();
            tbOffsetZ.Text = TtCalibOffsetZ.ToString();
            tbOffsetRX.Text = TtCalibOffsetRX.ToString();
            tbOffsetRY.Text = TtCalibOffsetRY.ToString();
            tbOffsetRZ.Text = TtCalibOffsetRZ.ToString();

            LoadVectorPresets();

            timerSample.Enabled = true;
            FormState = StateEnum.Idling;
            UpdateUI();

        }

        private void LoadVectorPresets(bool createNew = true){

            //Added by huimei
            log.Info("In LoadVectorPresets");
            //End added by huimei

            while (knownVectorPresetsComboBox.Items.Count != 1){
                knownVectorPresetsComboBox.Items.RemoveAt(1);
            }
            knownVectorPresetsComboBox.SelectedIndex = 0;

            Persistence P = new Persistence();
            P.Path = Application.StartupPath + "\\KnownTemplateVectors.ini";
            if (File.Exists(P.Path)){
                P.Section = "MarkerNames";
                int count = P.RetrieveInt("Count", 0), i, j, vectors;
                string lookup;
                KnownTemplateVectorsMarkerNames = new string[count];
                KnownTemplateVectorsLengths = new double[count][];
                KnownTemplateVectorsTolerances = new double[count];
                for (i = 0; i < count; i++)
                {
                    P.Section = "MarkerNames";
                    lookup = String.Format("Marker{0}", i);
                    KnownTemplateVectorsMarkerNames[i] = P.RetrieveString(lookup, "");
                    P.Section = KnownTemplateVectorsMarkerNames[i];
                    KnownTemplateVectorsTolerances[i] = P.RetrieveDouble("Tolerance", 0.75);
                    vectors = P.RetrieveInt("VectorCount", 0);
                    KnownTemplateVectorsLengths[i] = new double[vectors];
                    for (j = 0; j < vectors; j++)
                    {
                        lookup = String.Format("Vector{0}", j);
                        KnownTemplateVectorsLengths[i][j] = P.RetrieveDouble(lookup, 0.0);
                    }
                    knownVectorPresetsComboBox.Items.Add(KnownTemplateVectorsMarkerNames[i]);
                }
            }else if (createNew){
                try{
                    P.Section = "MarkerNames";
                    P.SaveInt("Count", 1);
                    P.SaveString("Marker0", "TTBlock");
                    P.Section = "TTBlock";
                    P.SaveDouble("Tolerance", 0.75);
                    P.SaveInt("VectorCount", 2);
                    P.SaveDouble("Vector0", 90.0);
                    P.SaveDouble("Vector1", 86.5);
                    LoadVectorPresets(false);
                }catch{
                    MessageBox.Show("Unable to create KnowTemplateVectors.ini");
                }
            }
        }

        private void TemplateTab_Changed(System.Object sender, System.EventArgs e){
            if ((TemplateTab.SelectedIndex == 0)){
                fMain.IsShowingVectorsForTemplateRegistration = true;
                fMain.XPointsProcessingEnabled = false;
                fMain.IsShowingXPoints = false;
            }else if ((TemplateTab.SelectedIndex == 2)){
                fMain.IsShowingVectorsForTemplateRegistration = false;
                fMain.XPointsProcessingEnabled = true;
                fMain.IsShowingXPoints = true;
            }
        }

        private void UpdateUI(){

            //Added by huimei
            log.Info("In UpdateUI");
            //End added by huimei

            //Central routine for updating the UI controls to reflect the state of the form
            bool Exists = false;
            bool FileNameIsValid = false;
            if ((TemplateTab.SelectedIndex == 0)){
                FileNameIsValid = IsValidTemplateName(tbName.Text, ref Exists);
            }else if ((TemplateTab.SelectedIndex == 1)){
                FileNameIsValid = IsValidTemplateName(tbNameProp.Text, ref Exists);
            }else if ((TemplateTab.SelectedIndex == 2)){
                FileNameIsValid = IsValidTemplateName(tbNameCOP.Text, ref Exists);
            }

            //highlight name in markers list if found

            //See if we can save or delete
            btnDelete.Enabled = Exists && FormState == StateEnum.Idling;
            btnRename.Enabled = Exists && FormState == StateEnum.Idling;
            btnSetAsReference.Enabled = Exists && FormState == StateEnum.Idling;
            btnUpdateProperties.Enabled = Exists && FormState == StateEnum.Idling;
            if ((!Exists))
            {
                chkAddFacet.Checked = false;
            }
            chkAddFacet.Enabled = Exists && Tip2ToolSamples.Count == 0;
            //cannot add facet and tooltip at the same time
            chkSampleFacet.Enabled = FileNameIsValid && (((!Exists)) || chkAddFacet.Checked);
            chkSampleToolTip.Enabled = IsToolAndTooltipCalibratorDetected() || chkSampleToolTip.Checked;
            chkSampleSlider.Enabled = IsToolAndTooltipCalibratorDetected() || chkSampleSlider.Checked;
            //turn off if not enabled
            if (!(chkSampleFacet.Enabled)){
                chkSampleFacet.Checked = false;
            }

            bool COPsamplingDone = true;
            int NumberofCOPSamplingDone = 0;
            for (int i = 1; i <= NumberofXPs; i++){
                if ((COPsamples[i].Count >= nudCOPSamplesNumber.Value)){
                    NumberofCOPSamplingDone = NumberofCOPSamplingDone + 1;
                }
            }
            COPsamplingDone = (NumberofCOPSamplingDone >= nudCOPXpointsNumber.Value) | (FormState == StateEnum.SamplingCOP & SamplingSuspended);

            btnAcceptSave.Enabled = (FormState == StateEnum.Idling || SamplingSuspended) && (VectorSamples.Count > 2 || Tip2ToolSamples.Count > 2);
            btnCOPAcceptSave.Enabled = (FormState == StateEnum.Idling || SamplingSuspended) && (SliderXPointSamples.Count > 2 || nudSliderControlPoint.Value > 1 || COPsamplingDone);
            if ((COPsamplingDone)){
                btnCOPAcceptSave.Enabled = true;
                chkSampleCOP2Marker.Checked = false;
            }
            if ((TemplateTab.SelectedIndex == 0)){
                tbName.Enabled = !btnAcceptSave.Enabled;
                //cannot change the name after sampling
            }else if ((TemplateTab.SelectedIndex == 1)){
                tbNameProp.Enabled = !btnAcceptSave.Enabled;
                //cannot change the name after sampling
            }else if ((TemplateTab.SelectedIndex == 2)){
                tbNameCOP.Enabled = !btnAcceptSave.Enabled;
                //cannot change the name after sampling
            }

            if ((VectorSamples.Count > 0) || (Tip2ToolSamples.Count > 0) || (SliderXPointSamples.Count > 0) || (FormState == StateEnum.SamplingCOP2Marker) || (FormState == StateEnum.SamplingCOP)){
                btnReset.Enabled = true;
            }

            if ((DisplayUTinfobox)){
                Interaction.MsgBox("Reposition the camera and collect another set of samples to complete the registration", MsgBoxStyle.Information, "UniTracker Marker Registration");
                DisplayUTinfobox = false;
            }

            if ((Tip2ToolSamples.Count > 0)){

                log.Info("Tip2ToolSamples.Count > 0");

                if ((FormState == StateEnum.SamplingSlider)){
                    COPlabelInfo.Text = "Collected: " + Math.Min((Tip2ToolSamples.Count), SliderXPointSamples.Count).ToString();
                }else{
                    labelSampleCount.Text = (Tip2ToolSamples.Count.ToString());
                }
            }else if ((FormState == StateEnum.SamplingCOP2Marker)){

                log.Info("FormState == StateEnum.SamplingCOP2Marker");

                string counterlabe = "";
                for (int i = 1; i <= NumberofXPs; i++){
                    counterlabe = counterlabe + " XP#" + i.ToString() + ":" + COPsamples[i].Count.ToString();
                }
                if ((COPsamplingDone)){
                    COPlabelInfo.ForeColor = Color.Green;
                }
                COPlabelInfo.Text = counterlabe;
            }else{

                log.Info("Else");

                if ((FormState == StateEnum.SamplingSlider)){
                    log.Info("FormState == StateEnum.SamplingSlider)");

                    COPlabelInfo.Text = "Collected: " + ((VectorSamples.Count / 2).ToString());
                }else{
                    log.Info("Else: " + (VectorSamples.Count / 2).ToString());

                    labelSampleCount.Text = ((VectorSamples.Count / 2).ToString());
                    COPlabelSampleCount.Text = ((VectorSamples.Count / 2).ToString());
                }
            }
            btnOK.Enabled = FormState == StateEnum.Idling || SamplingSuspended;
            //cannot exit while sampling
            string Name = "";
            if ((TemplateTab.SelectedIndex == 0)){
                Name = tbName.Text;
            }else if ((TemplateTab.SelectedIndex == 1)){
                Name = tbNameProp.Text;
            }else if ((TemplateTab.SelectedIndex == 2)){
                Name = tbNameCOP.Text;
            }


            if (!string.IsNullOrEmpty(Name) && (!object.ReferenceEquals(ListBoxTemplates, this.ActiveControl))){
                //update "current" marker in markers list, if the name matches
                ListBoxTemplates.SelectedIndices.Clear();
                //default

                int i = 0;
                while (i < markers.Count)
                {
                    if (markers[i].Name.ToUpper() == Name.ToUpper())
                    {
                        ListBoxTemplates.Items[i].Selected = true;
                        break; // TODO: might not be correct. Was : Exit Do
                    }
                    i += 1;
                }
                i = 0;
                while (i < clouds.Count)
                {
                    if (clouds[i].Name.ToUpper() == Name.ToUpper())
                    {
                        ListBoxTemplates.Items[i].Selected = true;
                        break; // TODO: might not be correct. Was : Exit Do
                    }
                    i += 1;
                }
            }
        }

        private bool IsValidTemplateName(string Name, ref bool Exists){

            //Added by huimei
            log.Info("In IsValidTemplateName");
            //End added by huimei

            bool tempIsValidTemplateName = false;
            try
            {
                string FilePath = null;
                if (!string.IsNullOrEmpty(Name))
                {
                    //Is txtName a valid file name?
                    FilePath = fMain.TemplatesFolderPath + "\\" + Name;
                    if (File.Exists(FilePath))
                    {
                        Exists = true;
                        tempIsValidTemplateName = true;
                        //see if we can actually create it
                    }
                    else
                    {
                        try
                        {
                            FileStream fileStream = File.Open(FilePath, FileMode.OpenOrCreate, FileAccess.Write);
                            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
                            tempIsValidTemplateName = true;
                            //remove the temp file
                            binaryWriter.Close();
                            File.Delete(FilePath);
                        }
                        catch
                        {
                            MessageBox.Show("Not a valid name: " + Name + "." + Environment.NewLine + "Please use only letters and digits.");
                        }

                    }
                }
            }catch{

            }

            //Added by Mei
            log.Info("End IsValidTemplateName");
            //End added by Mei

            return tempIsValidTemplateName;
        }

        private void RefreshList(bool SetIndex){

            //Added by huimei
            log.Info("In RefreshList");
            //End added by huimei

            //If SetIndex is true, the index is set to the index of the entry matching the current
            //name in the text box (or the last one if no such entry is found).
            int currTemplateIndex = 0;
            fMain.RefreshMarkerTemplates();
            //update the main templates collection from the files
            fMain.RefreshCloudTemplates();
            ListBoxTemplates.Items.Clear();
            cbTTCalibrator.Items.Clear();
            currTemplateIndex = ListBoxTemplates.Items.Count - 1;
            //default in case none is found
            int i = 0;
            string name = "";
            if ((TemplateTab.SelectedIndex == 0)){
                name = tbName.Text;
            }else if ((TemplateTab.SelectedIndex == 1)){
                name = tbNameProp.Text;
            }else if ((TemplateTab.SelectedIndex == 2)){
                name = tbNameCOP.Text;
            }

            while (i < markers.Count){
                ListBoxTemplates.Items.Insert(i, markers[i].Name + "(" + markers[i].TemplateFacets.Count.ToString() + ")");
                cbTTCalibrator.Items.Insert(i, markers[i].Name + "(" + markers[i].TemplateFacets.Count.ToString() + ")");
                if (markers[i].Name == name){
                    currTemplateIndex = i;
                }
                i += 1;
            }

            i = 0;
            while (i < clouds.Count){
                ListBoxTemplates.Items.Insert(i + markers.Count, clouds[i].Name + "(Cloud)");
                cbTTCalibrator.Items.Insert(i + markers.Count, clouds[i].Name + "(Cloud)");
                if (clouds[i].Name == name){
                    currTemplateIndex = i + markers.Count;
                }
                i += 1;
            }

            if (SetIndex && ListBoxTemplates.Items.Count > 0){
                if (currTemplateIndex > -1){
                    ListBoxTemplates.Items[currTemplateIndex].Selected = true;
                }else{
                    ListBoxTemplates.SelectedIndices.Clear();
                }
            }else{
                ListBoxTemplates.SelectedIndices.Clear();
            }

            btnDelete.Enabled = ListBoxTemplates.SelectedIndices.Count > 0;
            btnRename.Enabled = ListBoxTemplates.SelectedIndices.Count > 0;
            btnSetAsReference.Enabled = ListBoxTemplates.SelectedIndices.Count > 0;
            btnUpdateProperties.Enabled = ListBoxTemplates.SelectedIndices.Count > 0;

            //Added by huimei
            log.Info("End RefreshList");
            //End added by huimei
        }

        private void timerSample_Tick(System.Object sender, System.EventArgs e){

            //Added by huimei
            log.Info("In timerSample_Tick");
            //End added by huimei

            //This part seems to be sampling the vectors, by huimei

            //Initialisation, by huimei
            Marker M = new Marker();
            Marker ToolM = new Marker();
            Marker TTCalibratorM = new Marker();
            Xform3D Xf = default(Xform3D);
            IList<Marker> ColMarkers = null;
            IList<XPoint> ColXPoints = null;
            IList<Vector> ColVectors = null;
            Xform3D Calibrator2Camera = default(Xform3D);
            Xform3D Tip2Tool = default(Xform3D);
            Xform3D CalibratorXf = default(Xform3D);
            XPoint SliderXP = default(XPoint);
            Vector V = default(Vector);
            Camera CurrCamera = default(Camera);

            chkSampleToolTip.Enabled = IsToolAndTooltipCalibratorDetected() || chkSampleToolTip.Checked;
            chkSampleSlider.Enabled = IsToolAndTooltipCalibratorDetected() || chkSampleSlider.Checked;

            if (FormState == StateEnum.Idling || SamplingSuspended){
                return;
            }

            //Max number of vector samples, tip2toolSamples and sliderXPointSamples shouldn't exist 2*2000, by huimei (?)
            //too many
            int MaxNumberOfSamples = 2 * 2000;
            if (VectorSamples.Count >= MaxNumberOfSamples){
                chkSampleFacet.Checked = false;
                //turn it off
                return;
            }

            //too many
            if ((Tip2ToolSamples.Count >= MaxNumberOfSamples | SliderXPointSamples.Count > MaxNumberOfSamples)){
                chkSampleToolTip.Checked = false;
                //turn it off
                chkSampleSlider.Checked = false;
                return;
            }

            CurrCamera = fMain.CurrCamera;
            //for convenience

            int GrabbedFrame = 0;

            try
            {
                GrabbedFrame = CurrCamera.FramesGrabbed;
            }
            catch
            {
                fMain.exitdemo(0);
            }

            //ignore: same frame as before
            if (PrevFrameCounter == GrabbedFrame)
            {
                return;
            }

            PrevFrameCounter = GrabbedFrame;
            //update for next time

            if (FormState == StateEnum.SamplingToolTip)
            {
                ColMarkers = markers.GetIdentifiedMarkers(null);
                //null means identified by all cameras
                if ((IsToolAndTooltipCalibratorDetected()))
                {
                    foreach (Marker mm in ColMarkers)
                    {
                        if (IsMarkerTooltipCalibrator(mm))
                            TTCalibratorM = mm;
                        if (mm.Name.ToUpper() == tbName.Text.ToUpper())
                            ToolM = mm;
                    }
                    //Rotate the tooltip calibrator coordinates such that Z  is along the tooltip axis
                    Xf = new Xform3D();
                    int CurrTTcalibratorIndex = cbTTCalibrator.SelectedIndex;
                    if ((CurrTTcalibratorIndex >= 0 & CurrTTcalibratorIndex < markers.Count))
                    {
                        double offset_x = double.Parse(tbOffsetX.Text);
                        double offset_y = double.Parse(tbOffsetY.Text);
                        double offset_z = double.Parse(tbOffsetZ.Text);
                        double offset_rx = double.Parse(tbOffsetRX.Text);
                        double offset_ry = double.Parse(tbOffsetRY.Text);
                        double offset_rz = double.Parse(tbOffsetRZ.Text);
                        Xf.SetRotAnglesDegs(offset_rx, offset_ry, offset_rz);
                        //tip Z axis is aligned with Y of calibrator
                        double[] TTCALIB_shift = new double[3] {
                        offset_x,
                        offset_y,
                        offset_z
                    };
                        Xf.ShiftVector = TTCALIB_shift;
                    }
                    else if (TTCalibratorM.Name.Length > 6)
                    {
                        if (TTCalibratorM.Name.Substring(0, 7).ToUpper() == "TOOLCAL")
                        {
                            Xf.SetRotAnglesDegs(-90, 0, 0);
                            //tip Z axis is aligned with Y of calibrator
                        }
                        if (TTCalibratorM.Name.Substring(0, 7).ToUpper() == "CALIBER")
                        {
                            Xf.SetRotAnglesDegs(0.0233 * 180 / Math.PI, -0.0159 * 180 / Math.PI, -0.0524 * 180 / Math.PI);
                            //tip Z axis is aligned with Y of calibrator
                            double[] CALIBER_shift = new double[3] {
                            -42.86,
                            -43.94,
                            -15.11
                        };
                            Xf.ShiftVector = CALIBER_shift;
                        }
                        if (TTCalibratorM.Name.Length > 7)
                        {
                            if (TTCalibratorM.Name.Substring(0, 8).ToUpper() == "UNICALIB")
                            {
                                Xf.SetRotAnglesDegs(0, 0, 0);
                                //tip Z axis is aligned with Y of calibrator
                                double[] TTCALIB_shift = new double[3] {
                                -33.7,
                                -35.2,
                                -15.5
                            };
                                Xf.ShiftVector = TTCALIB_shift;
                            }
                        }
                    }
                    else
                    {
                        //ttblock - tooltip Z is aligned
                        Xf.SetRotAnglesDegs(0, 0, 0);
                    }

                    Calibrator2Camera = Xf.Concatenate(TTCalibratorM.GetMarker2CameraXf(CurrCamera));
                    Xf = ToolM.GetMarker2CameraXf(CurrCamera).Inverse();
                    //camera->tool

                    Tip2Tool = Calibrator2Camera.Concatenate(Xf);
                    //tip->Tool = calibrator->camera->tool
                    Tip2ToolSamples.Add(Tip2Tool);
                }
            }
            else if (FormState == StateEnum.SamplingSlider)
            {
                ColMarkers = markers.GetIdentifiedMarkers(null);
                //null means identified by all cameras
                ColXPoints = xpoints.GetdetectedXPoints(null);
                if ((IsToolAndTooltipCalibratorDetected() & ColXPoints.Count == 1))
                {
                    SliderXP = ColXPoints[0].Clone();
                    foreach (Marker mm in ColMarkers)
                    {
                        if (IsMarkerTooltipCalibrator(mm))
                            TTCalibratorM = mm;
                        if (mm.Name.ToUpper() == tbName.Text.ToUpper())
                            ToolM = mm;
                    }
                    //Rotate the tooltip calibrator coordinates such that Z  is along the tooltip axis
                    Xf = new Xform3D();

                    int CurrTTcalibratorIndex = cbTTCalibrator.SelectedIndex;
                    if ((CurrTTcalibratorIndex >= 0 & CurrTTcalibratorIndex < markers.Count))
                    {
                        double offset_x = double.Parse(tbOffsetX.Text);
                        double offset_y = double.Parse(tbOffsetY.Text);
                        double offset_z = double.Parse(tbOffsetZ.Text);
                        double offset_rx = double.Parse(tbOffsetRX.Text);
                        double offset_ry = double.Parse(tbOffsetRY.Text);
                        double offset_rz = double.Parse(tbOffsetRZ.Text);
                        Xf.SetRotAnglesDegs(offset_rx, offset_ry, offset_rz);
                        //tip Z axis is aligned with Y of calibrator
                        double[] TTCALIB_shift = new double[3] {
                        offset_x,
                        offset_y,
                        offset_z
                    };
                        Xf.ShiftVector = TTCALIB_shift;
                    }
                    else if (TTCalibratorM.Name.Length > 6)
                    {
                        if (TTCalibratorM.Name.Substring(0, 7).ToUpper() == "TOOLCAL")
                        {
                            Xf.SetRotAnglesDegs(-90, 0, 0);
                            //tip Z axis is aligned with Y of calibrator
                        }
                        if (TTCalibratorM.Name.Substring(0, 7).ToUpper() == "CALIBER")
                        {
                            Xf.SetRotAnglesDegs(0.0233 * 180 / Math.PI, -0.0159 * 180 / Math.PI, -0.0524 * 180 / Math.PI);
                            //tip Z axis is aligned with Y of calibrator
                        }
                    }
                    else
                    {
                        //ttblock - tooltip Z is aligned
                        Xf.SetRotAnglesDegs(0, 0, 0);
                    }

                    Calibrator2Camera = Xf.Concatenate(TTCalibratorM.GetMarker2CameraXf(CurrCamera));
                    Xf = ToolM.GetMarker2CameraXf(CurrCamera).Inverse();
                    //camera->tool

                    Xform3D SliderXP2CameraXf = new Xform3D();
                    SliderXP2CameraXf.ShiftVector = SliderXP.Position3D;
                    Xform3D SliderXP2MarkerXf = default(Xform3D);
                    SliderXP2MarkerXf = SliderXP2CameraXf.Concatenate(ToolM.GetMarker2CameraXf(CurrCamera).Inverse());

                    SliderXP.Position3D = SliderXP2MarkerXf.ShiftVector;
                    Tip2Tool = Calibrator2Camera.Concatenate(Xf);
                    //tip->Tool = calibrator->camera->tool
                    Tip2ToolSamples.Add(Tip2Tool);
                    SliderXPointSamples.Add(SliderXP);
                }
            }
            else if (FormState == StateEnum.SamplingCOP2Marker)
            {
                ColMarkers = markers.GetIdentifiedMarkers(null);
                //null means identified by all cameras
                ColXPoints = xpoints.GetdetectedXPoints(null);
                if ((ColMarkers.Count == 1 & ColXPoints.Count >= 1))
                {
                    M = ColMarkers[0];
                    ToolM = M;


                    for (int i = 0; i <= ColXPoints.Count - 1; i++)
                    {
                        // first transform the xpoint to the marker space
                        SliderXP = ColXPoints[i];
                        Xform3D SliderXP2CameraXf = new Xform3D();
                        SliderXP2CameraXf.ShiftVector = SliderXP.Position3D;
                        Xform3D SliderXP2MarkerXf = default(Xform3D);
                        SliderXP2MarkerXf = SliderXP2CameraXf.Concatenate(ToolM.GetMarker2CameraXf(CurrCamera).Inverse());
                        SliderXP.Position3D = SliderXP2MarkerXf.ShiftVector;

                        bool XPointInCOP = false;
                        double COP_RegistrationTolerance = 3;
                        //3 mm

                        for (int j = 1; j <= NumberofXPs; j++)
                        {
                            double[] COPxp = new double[4];
                            COPxp = SliderXP.Position3D;

                            // get an average over all collected XPs
                            double[] xp3d = new double[4];
                            double[] xp3d_avg = new double[4];
                            xp3d_avg = COPsamples[j][0].Position3D;
                            int n = 1;
                            while (n < COPsamples[j].Count)
                            {
                                xp3d = COPsamples[j][n].Position3D;
                                xp3d_avg[0] = ((n) * xp3d_avg[0] + xp3d[0]) / (n + 1);
                                xp3d_avg[1] = ((n) * xp3d_avg[1] + xp3d[1]) / (n + 1);
                                xp3d_avg[2] = ((n) * xp3d_avg[2] + xp3d[2]) / (n + 1);
                                n += 1;
                            }

                            double dist = (Math.Abs(COPxp[0] - xp3d_avg[0]) + Math.Abs(COPxp[1] - xp3d_avg[1]) + Math.Abs(COPxp[2] - xp3d_avg[2])) / 3;
                            if ((dist < COP_RegistrationTolerance))
                            {
                                if ((COPsamples[j].Count < nudCOPSamplesNumber.Value))
                                {
                                    COPsamples[j].Add(SliderXP.Clone());
                                }
                                XPointInCOP = true;
                            }
                        }
                        // this is a new xpoint, add it to the end of the list
                        if (XPointInCOP == false)
                        {
                            NumberofXPs = NumberofXPs + 1;
                            MTInterfaceDotNet.XPoint newsliderxp = default(MTInterfaceDotNet.XPoint);
                            newsliderxp = new XPoint();
                            newsliderxp.Position3D = SliderXP.Position3D;
                            //newsliderxp.Position2D
                            COPsamples[NumberofXPs].Add(SliderXP.Clone());
                        }

                    }

                }
                //sampling vectors
            }
            else
            {
                //Here Markers.UnidentifiedVectors collection contains all the Vectors that have not
                //yet been associated with a known facet, and CurrMarker.IdentifiedFacets has
                //all the template facets that have been identified.
                //Since we are adding only a single facet at a time, we will record the UnidentifiedVectors
                //only if it contains exactly 2 vectors (presumably the ones for the new facet).
                //Note that for correctly establishing the facet's coordinate space, the vectors and the reference
                //facet need to be both at the identifying camera's coordinate space.

                ColVectors = markers.GetUnidentifiedVectors(null);
                //remove all the vectors that exceed the length threshold.
                bool Removed = false;
                do
                {
                    Removed = false;
                    int i = 0;
                    while (i < ColVectors.Count)
                    {
                        V = ColVectors[i];
                        if (knownVectorPresetsComboBox.SelectedIndex != 0)
                        {
                            int selectedPresetIndex = knownVectorPresetsComboBox.SelectedIndex - 1;
                            bool doRemove = true;
                            for (int j = 0; j < KnownTemplateVectorsLengths[selectedPresetIndex].Length; j++)
                            {
                                if (Math.Abs(KnownTemplateVectorsLengths[selectedPresetIndex][j] - V.Length) < KnownTemplateVectorsTolerances[selectedPresetIndex])
                                {
                                    doRemove = false;
                                    break;
                                }
                            }
                            if (doRemove)
                            {
                                ColVectors.RemoveAt(i);
                                Removed = true;
                            }
                        }
                        else
                        {
                            if (V.Length < Convert.ToDouble(nudMinVectorLength.Value))
                            {
                                ColVectors.RemoveAt(i);
                                Removed = true;
                                // break; // TODO: might not be correct. Was : Exit Do
                            }
                        }
                        i += 1;
                    }
                } while (Removed);
                //We need two new vectors/facet identified by the same camera
                //If ColVectors.Count = (2 + 4 * (MainForm.CurrCamera.sensorsnum - 2)) Then
                if (ColVectors.Count >= 2)
                {
                    //If (ColVectors(0).IdentifyingCamera = ColVectors(1).IdentifyingCamera) And ((ColVectors(MainForm.CurrCamera.sensorsnum - 2).IdentifyingCamera = ColVectors(MainForm.CurrCamera.sensorsnum - 1).IdentifyingCamera)) Then
                    if ((ColVectors[0].IdentifyingCamera == ColVectors[1].IdentifyingCamera))
                    {
                        int i = 0;
                        //add the sampled set
                        while (i < ColVectors.Count)
                        {
                            VectorSamples.Add(ColVectors[i]);
                            i += 1;
                        }

                        labelInfo.Text = "Collecting samples";
                        if (FormState == StateEnum.SamplingAdditionalFacet)
                        {
                            //We need to see at least one known facet of the selected maker
                            // null for all camera
                            if (CurrMarker.GetIdentifiedFacets(null).Count >= 1)
                            {
                                //Compute and save the xform of facet1 to the identifying camera
                                Camera IdentifyingCam = ColVectors[0].IdentifyingCamera;
                                if ((CurrMarker.GetMarker2CameraXf(IdentifyingCam) != null))
                                {
                                    Facet1ToCameraXfs.Add(CurrMarker.GetMarker2CameraXf(IdentifyingCam));
                                    //facet1->sensor for this sample
                                    //seen by a camera not registered with this one
                                }
                                else
                                {
                                    VectorSamples.RemoveAt(VectorSamples.Count - 1);
                                    //remove the first vector sample
                                    VectorSamples.RemoveAt(VectorSamples.Count - 1);
                                    //remove the second vector sample
                                    labelInfo.Text = "Cannot sample: unregistered camera";
                                }
                                //not seeing a known facet
                            }
                            else
                            {
                                VectorSamples.RemoveAt(VectorSamples.Count - 1);
                                //remove the first vector sample
                                VectorSamples.RemoveAt(VectorSamples.Count - 1);
                                //remove the second vector sample
                                labelInfo.Text = "Cannot sample: no known facet";
                            }
                        }
                    }
                    else
                    {
                        labelInfo.Text = "Each vector seen by a different camera";
                    }
                    //too few/too many vectors
                }
                else
                {
                    if (ColVectors.Count < 2)
                    {
                        labelInfo.Text = "No new facet detected";
                    }
                    else
                    {
                        labelInfo.Text = "More than 2 vectors (" + ColVectors.Count + ") detected";

                    }
                }

            }

            //Added by Mei
            log.Info("End timerSample_Tick");
            //End added by Mei

            UpdateUI();
        }

        private void ListBoxTemplates_SelectedIndexChanged(System.Object sender, System.EventArgs e){

            //Added by huimei
            log.Info("In ListBoxTemplates_SelectedIndexChanged");
            //End added by huimei

            if (!initialized)
            {
                return;
            }
            if (ignore)
            {
                return;
            }
            ignore = true;
            int CurrTemplateIndex = 0;
            if (ListBoxTemplates.SelectedIndices.Count > 0)
            {
                CurrTemplateIndex = ListBoxTemplates.SelectedIndices[0];
                Marker m = default(Marker);
                Cloud cl = default(Cloud);
                if ((CurrTemplateIndex < markers.Count))
                {
                    m = markers[ListBoxTemplates.SelectedIndices[0]];
                    CurrMarker = m;
                    string Mname = markers[ListBoxTemplates.SelectedIndices[0]].Name;
                    Debug.Assert((m != null), "There are no markers in the list");
                    Marker @ref = CurrMarker.ReferenceMarker;
                    tbJitterCoefficient.Text = CurrMarker.JitterFilterCoefficient.ToString();
                    tbExtrapolatedFrames.Text = CurrMarker.ExtrapolatedFrames.ToString();
                    try
                    {
                        tbReferenceMarker.Text = @ref.Name;
                    }
                    catch
                    {
                        tbReferenceMarker.Text = "Camera-Space";
                    }
                    if ((TemplateTab.SelectedIndex == 0))
                    {
                        tbName.Text = CurrMarker.Name;
                    }
                    else if ((TemplateTab.SelectedIndex == 1))
                    {
                        tbNameProp.Text = CurrMarker.Name;
                    }
                    else if ((TemplateTab.SelectedIndex == 2))
                    {
                        tbNameCOP.Text = CurrMarker.Name;
                    }
                }
                else
                {
                    cl = clouds[ListBoxTemplates.SelectedIndices[0] - markers.Count];
                    CurrCloud = cl;
                    if ((TemplateTab.SelectedIndex == 0))
                    {
                        tbName.Text = CurrCloud.Name;
                    }
                    else if ((TemplateTab.SelectedIndex == 1))
                    {
                        tbNameProp.Text = CurrCloud.Name;
                    }
                    else if ((TemplateTab.SelectedIndex == 2))
                    {
                        tbNameCOP.Text = CurrCloud.Name;
                    }
                }


                UpdateUI();
            }
            ignore = false;
        }

        private void tbName_TextChanged(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In tbName_TextChanged");
            //End added by huimei

            if (!initialized)
            {
                return;
            }
            if (ignore)
            {
                return;
            }
            ignore = true;
            if (!string.IsNullOrEmpty(tbName.Text))
            {
                ListBoxTemplates.SelectedIndices.Clear();
                Marker m = FindTemplateInSet(tbName.Text);
                //not found: create a new one
                if (m == null)
                {
                    CurrMarker = new Marker();
                    CurrMarker.Name = tbName.Text;
                }
                else
                {
                    CurrMarker = m;
                }
                UpdateUI();
            }
            ignore = false;
        }

        private Marker FindTemplateInSet(string markerName)
        {

            //Added by huimei
            log.Info("In FindTemplateInSet");
            //End added by huimei

            int i = 0;
            while (i < markers.Count)
            {
                if (markers[i].Name.ToUpper() == markerName.ToUpper())
                {
                    return markers[i];
                }
                i += 1;
            }
            return null;
        }

        private bool IsToolAndTooltipCalibratorDetected()
        {

            //Added by huimei
            log.Info("In IsToolAndTooltipCalibratorDetected");
            //End added by huimei

            bool b1 = false;
            bool b2 = false;
            IList<Marker> IdentifiedMarkers = markers.GetIdentifiedMarkers(null);
            //null means identified by all cameras
            foreach (Marker m in IdentifiedMarkers)
            {
                if (IsMarkerTooltipCalibrator(m))
                    b1 = true;
                if ((tbName.Text.ToUpper() == m.Name.ToUpper()))
                    b2 = true;
            }

            return (b1 & b2);
        }
        
        //OK
        private bool IsMarkerTooltipCalibrator(Marker m)
        {
            //Added by huimei
            log.Info("In IsMarkerTooltipCalibrator");
            //End added by huimei

            bool b = false;

            int CurrTTcalibratorIndex = cbTTCalibrator.SelectedIndex;
            if ((CurrTTcalibratorIndex >= 0 & CurrTTcalibratorIndex < markers.Count))
            {
                TTCal = markers[cbTTCalibrator.SelectedIndex];
                Debug.Assert((TTCal != null), "There are no markers in the list");
                if ((TTCal.Name.ToUpper() == m.Name.ToUpper()))
                {
                    b = true;
                }
            }
            else
            {
                if (m.Name.Length > 6)
                {
                    b = m.Name.Substring(0, 7).ToUpper() == "TTBLOCK" || m.Name.Substring(0, 7).ToUpper() == "TOOLCAL" || m.Name.Substring(0, 7).ToUpper() == "CALIBER";
                }
                if ((b == false & m.Name.Length > 7))
                {
                    b = m.Name.Substring(0, 8).ToUpper() == "UNICALIB";
                }
            }

            return b;
        }

        //OK
        private void SaveCurrMarkerTemplate(string TemplateName)
        {
            //Added by huimei
            log.Info("In SaveCurrMarkerTemplate");
            //End added by huimei

            Persistence P = new Persistence();
            CurrMarker.Name = TemplateName;
            P.Path = fMain.TemplatesFolderPath + "\\" + CurrMarker.Name;

            if (!(Utils.ReadyForWriting(P.Path, true)))
            {
                MessageBox.Show("File is read-only. Change permissions and try again");
                return;
            }
            CurrMarker.StoreTemplate(P, "");
        }

        //OK
        private void SaveNewCloudTemplate(string TemplateName)
        {

            //Added by huimei
            log.Info("In SaveNewCloudTemplate");
            //End added by huimei

            Persistence P = new Persistence();
            NewCloud.Name = TemplateName;
            P.Path = fMain.TemplatesFolderPath + "\\" + NewCloud.Name;

            if (!(Utils.ReadyForWriting(P.Path, true)))
            {
                MessageBox.Show("File is read-only. Change permissions and try again");
                return;
            }
            NewCloud.StoreTemplate(P, "");
        }

        private void btnAcceptSave_Click(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In btnAcceptSave_Click");
            //End added by huimei

            //completed the sampling: check it
            Xform3D FacetXf = new Xform3D();
            string CompletionStr = null;

            string name = "";
            if ((TemplateTab.SelectedIndex == 0))
            {
                name = tbName.Text;
            }
            else if ((TemplateTab.SelectedIndex == 1))
            {
                name = tbNameProp.Text;
            }
            else if ((TemplateTab.SelectedIndex == 2))
            {
                name = tbNameCOP.Text;
            }

            if (FormState == StateEnum.SamplingToolTip)
            {
                //Just save the marker with the tooltip
                SaveCurrMarkerTemplate(name);
                RefreshList(true);
            }
            else if (FormState == StateEnum.SamplingSlider)
            {
                //Just save the marker with the tooltip
                SaveCurrMarkerTemplate(name);
                RefreshList(true);
            }
            else if (FormState == StateEnum.SamplingCOP2Marker)
            {
                //Just save the marker with the tooltip
                SaveCurrMarkerTemplate(name);
                RefreshList(true);
            }
            else if (FormState == StateEnum.SamplingCOP){
                SaveNewCloudTemplate(name);
                RefreshList(true);
            }else{
                NewFacet = new Facet();
                Debug.Assert(VectorSamples.Count > 2);

                log.Debug("VectorSamples.Count > 2: " + (VectorSamples.Count > 2));
                //Here is where we try to create a new marker from the sampled vectors

                try
                {
                    NewFacet.SetTemplateVectorsFromSamples(VectorSamples, 0.3);
                    //success
                    if (FormState == StateEnum.SamplingNewMarker)
                    {
                        CurrMarker.AddTemplateFacet(NewFacet, FacetXf);
                        //unit xform, since first facet
                    }
                    else if (FormState == StateEnum.SamplingAdditionalFacet)
                    {
                        IList<Xform3D> Facet1ToNewFacetXfs = new List<Xform3D>();
                        Vector[] VectorPair = new Vector[2];
                        Xform3D Facet1ToNewFacetXf = default(Xform3D);
                        //identify the NewFacet->Sensor xform for each sample and, using
                        //the saved Facet1ToCameraXfs, compute and save the Facet1->NewFacet xform

                        //TODO check this!!! from huimei
                        int count = (VectorSamples.Count / 2);
                        bool IsUsingXforms3DAveragingMethod = false;
                        if (!IsUsingXforms3DAveragingMethod){
                            int i = 0;
                            double[] LVB = { 0, 0, 0 };
                            double[] LVH = { 0, 0, 0 };
                            double[] SVB = { 0, 0, 0 };
                            double[] SVH = { 0, 0, 0 };
                            while (i < count){
                                for (int j = 0; j < 3; j++){
                                    Xform3D Ft2CamXfInv = Facet1ToCameraXfs[i].Inverse();
                                    LVB[j] += Ft2CamXfInv.XformLocation(VectorSamples[2 * i].BasePos)[j];
                                    LVH[j] += Ft2CamXfInv.XformLocation(VectorSamples[2 * i].HeadPos)[j];
                                    SVB[j] += Ft2CamXfInv.XformLocation(VectorSamples[2 * i + 1].BasePos)[j];
                                    SVH[j] += Ft2CamXfInv.XformLocation(VectorSamples[2 * i + 1].HeadPos)[j];
                                }
                                i += 1;
                            }

                            Xform3D ref2CameraXform = Facet1ToCameraXfs[0];

                            for (int j = 0; j < 3; j++){
                                LVB[j] = LVB[j] / (double)count;
                                LVH[j] = LVH[j] / (double)count;
                                SVB[j] = SVB[j] / (double)count;
                                SVH[j] = SVH[j] / (double)count;
                            }

                            VectorPair[0] = VectorSamples[0];
                            VectorPair[1] = VectorSamples[1];

                            LVB = ref2CameraXform.XformLocation(LVB);
                            LVH = ref2CameraXform.XformLocation(LVH);
                            SVB = ref2CameraXform.XformLocation(SVB);
                            SVH = ref2CameraXform.XformLocation(SVH);

                            VectorPair[0].SetBasePos(LVB[0], LVB[1], LVB[2]);
                            VectorPair[0].SetHeadPos(LVH[0], LVH[1], LVH[2]);
                            VectorPair[1].SetBasePos(SVB[0], SVB[1], SVB[2]);
                            VectorPair[1].SetHeadPos(SVH[0], SVH[1], SVH[2]);

                            if (NewFacet.Identify(fMain.CurrCamera, VectorPair))
                            {
                                Facet1ToNewFacetXf = ref2CameraXform.Concatenate(NewFacet.GetFacet2CameraXf(fMain.CurrCamera).Inverse());
                            }
                        }
                        else
                        {
                            int i = 0;
                            while (i < count)
                            {
                                VectorPair[0] = VectorSamples[2 * i];
                                VectorPair[1] = VectorSamples[2 * i + 1];
                                //the sample matches the template
                                if (NewFacet.Identify(fMain.CurrCamera, VectorPair))
                                {
                                    //Compute the xform between the first marker facet and the new one
                                    //Facet1ToNewFacet = Facet1 -> Camera -> NewFacet
                                    Facet1ToNewFacetXf = Facet1ToCameraXfs[i].Concatenate(NewFacet.GetFacet2CameraXf(fMain.CurrCamera).Inverse());
                                    Facet1ToNewFacetXfs.Add(Facet1ToNewFacetXf);
                                }
                                i += 1;
                            }
                            Debug.Assert(Facet1ToNewFacetXfs.Count > 2);
                            log.Debug("Facet1ToNewFacetXfs.Count > 2: " + (Facet1ToNewFacetXfs.Count > 2));
                            //Combine the transforms accumulated to a new one and save it with the facet
                            //in the marker
                            Facet1ToNewFacetXf = Facet1ToNewFacetXfs[0];

                            i = 1;
                            while (i < Facet1ToNewFacetXfs.Count)
                            {
                                Facet1ToNewFacetXf.InBetween(Facet1ToNewFacetXfs[i], 1 / (1 + i));
                                //will result in equal contribution by all faces
                                i += 1;
                            }
                        }
                        if (Facet1ToNewFacetXf == null)
                        {
                            MessageBox.Show("Failed to register, please try again!");
                        }
                        else
                        {
                            CurrMarker.AddTemplateFacet(NewFacet, Facet1ToNewFacetXf);
                        }
                    }
                    NewFacet = null;
                    //not useful anymore
                    //If not same as another Marker, store the Marker info in the Markers directory
                    //under the name in txtName
                    Marker M = new Marker();

                    if (string.IsNullOrEmpty(name))
                    {
                        return;
                    }
                    //nothing to save
                    if (CurrMarker.TemplateFacets.Count == 0)
                    {
                        return;
                    }

                    CompletionCodes code = CurrMarker.ValidateTemplate();
                    if (!(CompletionCodes.OK == code))
                    {
                        MessageBox.Show("Save failed: " + code.ToString());
                        return;
                    }
                    //Here we're OK
                    SaveCurrMarkerTemplate(name);
                    RefreshList(true);
                    //failed to set the vectors - notify the reason
                }
                catch (MTException ex)
                {

                    FormState = StateEnum.Idling;

                    CompletionStr = ex.Message;
                    MessageBox.Show("Failed to record a new facet: " + CompletionStr, " Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            FormState = StateEnum.Idling;

            ResetSampling();
            UpdateUI();
            btnAcceptSave.Enabled = false;
            //prevent resaving the same Marker
            btnCOPAcceptSave.Enabled = false;
            //prevent resaving the same Marker

        }

        private void ResetSampling()
        {

            //Added by huimei
            log.Info("In ResetSampling");
            //End added by huimei

            VectorSamples = new List<Vector>();
            Facet1ToCameraXfs = new List<Xform3D>();
            Tip2ToolSamples = new List<Xform3D>();
            SliderXPointSamples = new List<XPoint>();
            for (int i = 1; i <= 100; i++)
            {
                COPsamples[i] = new List<XPoint>();
            }
            // remove the slider xpoints from the marker
            //For id As Integer = 1 To NumberofXPs
            //    Dim TempSliderXPoints As List(Of XPoint)
            //    TempSliderXPoints = CurrMarker.TemplateSliderControlledXpointsGet(id)
            //    For j As Integer = 0 To TempSliderXPoints.Count - 1
            //        CurrMarker.RemoveTemplateSliderControlledXpoint(id, TempSliderXPoints(j))
            //    Next
            //Next

            NumberofXPs = 0;

        }

        //OK
        private Xform3D GetCalibratorXform(Marker M){

            //Added by huimei
            log.Info("In GetCalibratorXform");
            //End added by huimei

            //Rotate the tooltip calibrator coordinates such that Z  is along the tooltip axis
            Xform3D CalibratorXform = new Xform3D();
            if (M.Name.Length > 6)
            {
                if (M.Name.Substring(0, 7).ToUpper() == "TOOLCAL")
                {
                    CalibratorXform.SetRotAnglesDegs(-90, 0, 0);
                    //tip Z axis is aligned with Y of calibrator
                }
                if (M.Name.Substring(0, 7).ToUpper() == "CALIBER")
                {
                    CalibratorXform.SetRotAnglesDegs(0.0233 * 180 / Math.PI, -0.0159 * 180 / Math.PI, -0.0524 * 180 / Math.PI);
                    //tip Z axis is aligned with Y of calibrator
                }
                if (M.Name.Length > 7)
                {
                    if (M.Name.Substring(0, 8).ToUpper() == "UNICALIB")
                    {
                        CalibratorXform.SetRotAnglesDegs(45, 0, 45);
                        //tip Z axis is aligned with Y of calibrator
                        double[] TTCALIB_shift = new double[3] {
                        -33.7,
                        -35.2,
                        -15.5
                    };
                        CalibratorXform.ShiftVector = TTCALIB_shift;
                    }
                }
                //ttblock - tooltip Z is 45 degs to all 3 axes
            }
            else
            {
                CalibratorXform.SetRotAnglesDegs(45, 0, 45);
            }
            return CalibratorXform;
        }

        private void chkSampleToolTip_CheckedChanged(System.Object sender, System.EventArgs e){

            //Added by huimei
            log.Info("In chkSampleToolTip_CheckedChanged");
            //End added by huimei

            if (!initialized)
            {
                return;
            }
            Persistence P = new Persistence();
            Marker M = null;
            labelInfo.Text = "";
            //activating sampling
            if (chkSampleToolTip.Checked == true)
            {
                if (fMain.CurrCamera.ThermalHazardCode != HazardCodes.None)
                {
                    if (MessageBox.Show("Camera not yet thermally stable. Proceed anyhow?", "Warning!", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        chkSampleToolTip.Checked = false;
                        //cancel (note: will recurse!)
                        return;
                    }
                }
                SamplingSuspended = false;
                //select the new form state
                //start sampling
                if (FormState == StateEnum.Idling)
                {
                    FormState = StateEnum.SamplingToolTip;
                }
                if (FormState == StateEnum.SamplingToolTip)
                {
                    IList<Marker> ColMarker = markers.GetIdentifiedMarkers(null);
                    foreach (Marker mm in ColMarker)
                    {
                        if (IsMarkerTooltipCalibrator(mm))
                            TTCal = mm;
                        if (mm.Name.ToUpper() == tbName.Text.ToUpper())
                            ToolName = mm.Name;
                    }
                    M = FindTemplateInSet(ToolName);
                    if (CurrMarker == null)
                    {
                        CurrMarker = M;
                        //check if the same (so we continue to accummulate samples)
                    }
                    else
                    {
                        if ((M != null))
                        {
                            if (M.Name != CurrMarker.Name)
                            {
                                CurrMarker = M;
                                //update current marker
                            }
                        }
                    }
                    Debug.Assert((CurrMarker != null));
                    log.Debug("CurrMarker != null: " + (CurrMarker != null));

                    //should not happen!
                    if ((TemplateTab.SelectedIndex == 0))
                    {
                        tbName.Text = CurrMarker.Name;
                    }
                    else if ((TemplateTab.SelectedIndex == 1))
                    {
                        tbNameProp.Text = CurrMarker.Name;
                    }
                    else if ((TemplateTab.SelectedIndex == 2))
                    {
                        tbNameCOP.Text = CurrMarker.Name;
                    }

                }
                //unchecked - suspend sampling
            }
            else
            {
                if (FormState == StateEnum.SamplingToolTip)
                {
                    //We average the tip->tool transforms sampled, then add it to the marker
                    Debug.Assert(CurrMarker.Name == ToolName);
                    log.Debug("CurrMarker.Name == ToolName: " + (CurrMarker.Name == ToolName));

                    if (Tip2ToolSamples.Count > 3)
                    {
                        //Average in all the tooltip xforms
                        Xform3D Tip2Tool = Tip2ToolSamples[0];

                        int i = 1;
                        while (i < Tip2ToolSamples.Count)
                        {
                            Tip2Tool = Tip2Tool.InBetween(Tip2ToolSamples[i], 1 / (i + 1));
                            i += 1;
                        }

                        float offset = 0;
                        if (float.TryParse(tbOffsetZ.Text, out offset))
                        {
                            if (!(offset == 0))
                            {
                                Xform3D Xf = null;

                                Xf = GetCalibratorXform(TTCal);
                                double[] tipOffset = new double[3];
                                double[] tipOffsetInCalibrator = null;
                                tipOffset[0] = offset;
                                tipOffsetInCalibrator = Xf.RotateLocation(tipOffset);
                                if (TTCal.Name.Length > 6)
                                {
                                    if (TTCal.Name.Substring(0, 7).ToUpper() == "TOOLCAL")
                                    {
                                        tipOffset[0] = offset;
                                        //tip Z axis is aligned with Y of calibrator
                                    }
                                    if (TTCal.Name.Substring(0, 7).ToUpper() == "CALIBER")
                                    {
                                        tipOffset[0] = offset;
                                        //tip Z axis is aligned with Y of calibrator
                                    }
                                    if (TTCal.Name.Length > 7)
                                    {
                                        if (TTCal.Name.Substring(0, 8).ToUpper() == "UNICALIB")
                                        {
                                            tipOffset[0] = offset;
                                            //tip Z axis is aligned with Y of calibrator
                                        }
                                    }
                                    //ttblock - tooltip Z is already aligned
                                }
                                else
                                {
                                    tipOffset[0] = offset;
                                }
                                for (i = 0; i <= 2; i++)
                                {
                                    Tip2Tool.ShiftVector[i] = Tip2Tool.ShiftVector[i] - tipOffset[i];
                                }
                            }
                        }
                        CurrMarker.Tooltip2MarkerXf = Tip2Tool;
                    }
                }
                SamplingSuspended = true;
            }
            UpdateUI();
        }

        //Slider XP registration
        private void chkSampleSlider_CheckedChanged(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In chkSampleSlider_CheckedChanged");
            //End added by huimei

            if (!initialized)
            {
                return;
            }
            if (WasXPointsProcessingEnabled == false)
            {
                fMain.XPointsProcessingEnabled = true;
                fMain.IsShowingXPoints = true;
            }

            Persistence P = new Persistence();
            Marker M = null;
            COPlabelInfo.Text = "";
            //activating sampling
            if (chkSampleSlider.Checked == true)
            {
                if (fMain.CurrCamera.ThermalHazardCode != HazardCodes.None)
                {
                    if (MessageBox.Show("Camera not yet thermally stable. Proceed anyhow?", "Warning!", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        chkSampleSlider.Checked = false;
                        //cancel (note: will recurse!)
                        return;
                    }
                }
                SamplingSuspended = false;
                //select the new form state
                //start sampling
                if (FormState == StateEnum.Idling)
                {
                    FormState = StateEnum.SamplingSlider;
                }
                if (FormState == StateEnum.SamplingSlider)
                {
                    IList<Marker> ColMarker = markers.GetIdentifiedMarkers(null);
                    if (IsToolAndTooltipCalibratorDetected())
                    {
                        foreach (Marker mm in ColMarker)
                        {
                            if (IsMarkerTooltipCalibrator(mm))
                                TTCal = mm;
                            if (mm.Name.ToUpper() == tbName.Text.ToUpper())
                                ToolName = mm.Name;
                        }
                        M = FindTemplateInSet(ToolName);
                        if (CurrMarker == null)
                        {
                            CurrMarker = M;
                            //check if the same (so we continue to accumulate samples)
                        }
                        else
                        {
                            if ((M != null))
                            {
                                if (M.Name != CurrMarker.Name)
                                {
                                    CurrMarker = M;
                                    //update current marker
                                }
                            }
                        }
                        Debug.Assert((CurrMarker != null));
                        log.Debug("CurrMarker != null" + (CurrMarker != null));

                        //should not happen!
                        if ((TemplateTab.SelectedIndex == 0))
                        {
                            tbName.Text = CurrMarker.Name;
                        }
                        else if ((TemplateTab.SelectedIndex == 1))
                        {
                            tbNameProp.Text = CurrMarker.Name;
                        }
                        else if ((TemplateTab.SelectedIndex == 2))
                        {
                            tbNameCOP.Text = CurrMarker.Name;
                        }
                    }
                }
                //unchecked - suspend sampling
            }
            else
            {
                if (FormState == StateEnum.SamplingSlider)
                {
                    //We average the tip->tool transforms sampled, then add it to the marker
                    Debug.Assert(CurrMarker.Name == ToolName);
                    log.Debug("CurrMarker.Name == ToolName: " + (CurrMarker.Name == ToolName));

                    if ((SliderXPointSamples.Count > 3))
                    {
                        //Average in all the Slider XPoint Samples
                        XPoint SliderXPoint = SliderXPointSamples[0];
                        double[] xp3d = new double[4];
                        double[] xp3d_avg = new double[4];
                        xp3d_avg = SliderXPointSamples[0].Position3D;

                        int i = 1;
                        while (i < SliderXPointSamples.Count)
                        {
                            xp3d = SliderXPointSamples[i].Position3D;
                            xp3d_avg[0] = ((i) * xp3d_avg[0] + xp3d[0]) / (i + 1);
                            xp3d_avg[1] = ((i) * xp3d_avg[1] + xp3d[1]) / (i + 1);
                            xp3d_avg[2] = ((i) * xp3d_avg[2] + xp3d[2]) / (i + 1);
                            i += 1;
                        }


                        // NOTE:
                        // SliderXpointId 
                        // Xpoint.Index = SliderControlPoint
                        SliderXPoint.Position3D = xp3d_avg;
                        SliderXPoint.Index = (int)nudSliderControlPoint.Value;
                        //CurrMarker.SliderXPointAt0 = SliderXPoint
                        CurrMarker.SliderControlledXpointsCount = (int)nudSliderXPointId.Value;
                        CurrMarker.AddTemplateSliderControlledXpoint((int)nudSliderXPointId.Value, SliderXPoint);
                    }

                    if (Tip2ToolSamples.Count > 3)
                    {
                        //Average in all the tooltip xforms
                        Xform3D Tip2Tool = Tip2ToolSamples[0];

                        int i = 1;
                        while (i < Tip2ToolSamples.Count)
                        {
                            Tip2Tool = Tip2Tool.InBetween(Tip2ToolSamples[i], 1 / (i + 1));
                            i += 1;
                        }

                        float offset = 0;
                        if (float.TryParse(tbOffsetZ.Text, out offset))
                        {
                            if (!(offset == 0))
                            {
                                Xform3D Xf = null;

                                Xf = GetCalibratorXform(TTCal);
                                double[] tipOffset = new double[3];
                                double[] tipOffsetInCalibrator = null;
                                tipOffset[0] = offset;
                                tipOffsetInCalibrator = Xf.RotateLocation(tipOffset);
                                if (TTCal.Name.Length > 6)
                                {
                                    if (TTCal.Name.Substring(0, 7).ToUpper() == "TOOLCAL")
                                    {
                                        tipOffset[0] = offset;
                                        //tip Z axis is aligned with Y of calibrator
                                    }
                                    if (TTCal.Name.Substring(0, 7).ToUpper() == "CALIBER")
                                    {
                                        tipOffset[0] = offset;
                                        //tip Z axis is aligned with Y of calibrator
                                    }
                                    //ttblock - tooltip Z is already aligned
                                }
                                else
                                {
                                    tipOffset[0] = offset;
                                }
                                for (i = 0; i <= 2; i++)
                                {
                                    Tip2Tool.ShiftVector[i] = Tip2Tool.ShiftVector[i] - tipOffset[i];
                                }
                            }
                        }
                        if ((nudSliderControlPoint.Value == 1))
                        {
                            CurrMarker.Tooltip2MarkerXf = Tip2Tool;
                        }
                        CurrMarker.AddTemplateSliderControlledTooltip2MarkerXf((int)nudSliderXPointId.Value, Tip2Tool);
                    }
                }
                SamplingSuspended = true;
                nudSliderControlPoint.Value = nudSliderControlPoint.Value + 1;
                ResetSampling();
            }
            UpdateUI();
        }

        private void chkSampleFacet_CheckedChanged(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In chkSampleFacet_CheckedChanged");
            //End added by huimei

            if (!initialized)
            {
                return;
            }
            Persistence P = new Persistence();
            Marker M = null;
            labelInfo.Text = "";

            //activating sampling
            if (chkSampleFacet.Checked == true)
            {
                if (fMain.CurrCamera.ThermalHazardCode != HazardCodes.None)
                {
                    if (MessageBox.Show("Camera not yet thermally stable. Proceed anyhow?", "Warning!", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        chkSampleFacet.Checked = false;
                        //cancel (note: will recurse!)
                        return;
                    }
                }
                SamplingSuspended = false;
                //select the new form state
                //start sampling
                if (FormState == StateEnum.Idling)
                {
                    if (chkAddFacet.Checked == true)
                    {
                        //Find the current marker in the set
                        string name = "";
                        if ((TemplateTab.SelectedIndex == 0))
                        {
                            name = tbName.Text;
                        }
                        else if ((TemplateTab.SelectedIndex == 1))
                        {
                            name = tbNameProp.Text;
                        }
                        else if ((TemplateTab.SelectedIndex == 2))
                        {
                            name = tbNameCOP.Text;
                        }

                        M = FindTemplateInSet(name);
                        if ((M != null))
                        {
                            CurrMarker = M;
                            FormState = StateEnum.SamplingAdditionalFacet;
                            CurrMarker.Name = name;
                            //cannot find it for some reason
                        }
                        else
                        {
                            MessageBox.Show("Error: cannot find existing marker " + name);
                            chkSampleFacet.Checked = false;
                        }
                    }
                    else
                    {
                        FormState = StateEnum.SamplingNewMarker;
                    }
                }
                if (FormState == StateEnum.SamplingToolTip)
                {
                    //save the tool name
                    IList<Marker> ColMarker = markers.GetIdentifiedMarkers(null);
                    foreach (Marker mm in ColMarker)
                    {
                        if (IsMarkerTooltipCalibrator(mm))
                            TTCal = mm;
                        if (mm.Name.ToUpper() == tbName.Text.ToUpper())
                            ToolName = mm.Name;
                    }
                    M = FindTemplateInSet(ToolName);
                    if (CurrMarker == null)
                    {
                        CurrMarker = M;
                        //check if the same (so we continue to accumulate samples)
                    }
                    else
                    {
                        if ((M != null))
                        {
                            if (M.Name != CurrMarker.Name)
                            {
                                CurrMarker = M;
                                //update current marker

                            }
                        }
                    }
                    Debug.Assert((CurrMarker != null));
                    //should not happen!
                    if ((TemplateTab.SelectedIndex == 0))
                    {
                        tbName.Text = CurrMarker.Name;
                    }
                    else if ((TemplateTab.SelectedIndex == 1))
                    {
                        tbNameProp.Text = CurrMarker.Name;
                    }
                    else if ((TemplateTab.SelectedIndex == 2))
                    {
                        tbNameCOP.Text = CurrMarker.Name;
                    }
                }
                //unchecked - suspend sampling
            }
            else
            {

                if (FormState == StateEnum.SamplingToolTip)
                {
                    //We average the tip->tool transforms sampled, then add it to the marker
                    Debug.Assert(CurrMarker.Name == ToolName);

                    if (Tip2ToolSamples.Count > 3)
                    {
                        //Average in all the tooltip xforms
                        Xform3D Tip2Tool = Tip2ToolSamples[0];

                        object tempFor1 = Tip2ToolSamples.Count;
                        int i = 1;
                        while (i < Tip2ToolSamples.Count)
                        {
                            Tip2Tool = Tip2Tool.InBetween(Tip2ToolSamples[i], 1 / (1 + i));
                            i += 1;
                        }

                        float offset = 0;
                        if (float.TryParse(tbOffsetZ.Text, out offset))
                        {
                            if (!(offset == 0))
                            {
                                Xform3D Xf = default(Xform3D);

                                Xf = GetCalibratorXform(TTCal);
                                double[] tipOffset = new double[3];
                                double[] tipOffsetInCalibrator = null;
                                tipOffset[0] = offset;

                                tipOffsetInCalibrator = Xf.RotateLocation(tipOffset);

                                if (TTCal.Name.Length > 6)
                                {
                                    if (TTCal.Name.Substring(0, 7).ToUpper() == "TOOLCAL")
                                    {
                                        tipOffset[0] = offset;
                                        //tip Z axis is aligned with Y of calibrator
                                    }
                                    if (TTCal.Name.Substring(0, 7).ToUpper() == "CALIBER")
                                    {
                                        tipOffset[0] = offset;
                                        //tip Z axis is aligned with Y of calibrator
                                    }
                                    if (TTCal.Name.Length > 7)
                                    {
                                        if (TTCal.Name.Substring(0, 8).ToUpper() == "UNICALIB")
                                        {
                                            tipOffset[0] = offset;
                                            //tip Z axis is aligned with Y of calibrator
                                        }
                                    }
                                    //ttblock - tooltip Z is alredy alligned
                                }
                                else
                                {
                                    tipOffset[0] = offset;
                                }
                                for (i = 0; i <= 2; i++)
                                {
                                    Tip2Tool.ShiftVector[i] = Tip2Tool.ShiftVector[i] - tipOffset[i];
                                }
                            }
                        }
                        CurrMarker.Tooltip2MarkerXf = Tip2Tool;
                    }
                }
                SamplingSuspended = true;
            }
            UpdateUI();

        }

        private void btnDelete_Click(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In btnDelete_Click");
            //End added by huimei

            string FilePath = null;
            string Name = null;
            if (ListBoxTemplates.SelectedIndices.Count == 0)
            {
                return;
            }

            int CurrTemplateIndex = ListBoxTemplates.SelectedIndices[0];
            if ((CurrTemplateIndex < markers.Count))
            {
                Name = markers[ListBoxTemplates.SelectedIndices[0]].Name;
            }
            else
            {
                Name = clouds[ListBoxTemplates.SelectedIndices[0] - markers.Count].Name;
            }

            FilePath = fMain.TemplatesFolderPath + "\\" + Name;
            // the delete button is disabled when the selection is Default
            // so there is no need for confirmation, since this is more an assertion than anything else
            if (!(File.Exists(FilePath)))
            {
                return;
            }

            DialogResult result = MessageBox.Show("Delete Marker \"" + Name + "\"?", "Confirm", MessageBoxButtons.OKCancel);
            if (result == DialogResult.Cancel)
            {
                return;
            }
            if (!(Utils.ReadyForWriting(FilePath, true)))
            {
                MessageBox.Show("File is read-only. Change permissions and try again");
                return;
            }
            File.Delete(FilePath);
            ResetSampling();
            if ((CurrTemplateIndex < markers.Count))
            {
                CurrMarker = null;
                NewFacet = null;
            }
            else
            {
                CurrCloud = null;
            }


            RefreshList(true);
            if ((TemplateTab.SelectedIndex == 0))
            {
                tbName.Text = "";
            }
            else if ((TemplateTab.SelectedIndex == 1))
            {
                tbNameProp.Text = "";
            }
            else if ((TemplateTab.SelectedIndex == 2))
            {
                tbNameCOP.Text = "";
            }
            UpdateUI();

        }

        private void btnRename_Click(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In btnRename_Click");
            //End added by huimei

            string OldName = null;
            string NewName = null;
            bool Valid = false;
            bool Exists = false;
            if (ListBoxTemplates.SelectedIndices.Count == 0)
            {
                return;
            }

            int CurrTemplateIndex = ListBoxTemplates.SelectedIndices[0];
            if ((CurrTemplateIndex < markers.Count))
            {
                OldName = markers[ListBoxTemplates.SelectedIndices[0]].Name;
            }
            else
            {
                OldName = clouds[ListBoxTemplates.SelectedIndices[0] - markers.Count].Name;
            }

            if ((TemplateTab.SelectedIndex == 0))
            {
                NewName = Microsoft.VisualBasic.Interaction.InputBox("Enter new name: ", "Rename Marker", tbName.Text, -1, -1);
            }
            else if ((TemplateTab.SelectedIndex == 1))
            {
                NewName = Microsoft.VisualBasic.Interaction.InputBox("Enter new name: ", "Rename Marker", tbNameProp.Text, -1, -1);
            }
            else if ((TemplateTab.SelectedIndex == 2))
            {
                NewName = Microsoft.VisualBasic.Interaction.InputBox("Enter new name: ", "Rename Marker", tbNameCOP.Text, -1, -1);
            }

            NewName = Microsoft.VisualBasic.Interaction.InputBox("Enter new name: ", "Rename Marker", tbName.Text, -1, -1);
            Valid = IsValidTemplateName(NewName, ref Exists);
            if (Exists)
            {
                MessageBox.Show("Failed to rename. " + NewName + " already exists.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (Valid)
            {
                //rename
                Marker M = default(Marker);
                Cloud Cl = default(Cloud);
                Persistence P = new Persistence();
                if ((CurrTemplateIndex < markers.Count))
                {
                    M = markers[ListBoxTemplates.SelectedIndices[0]];
                    M.Name = NewName;
                    P.Path = fMain.TemplatesFolderPath + "\\" + NewName;
                    M.StoreTemplate(P, "");
                }
                else
                {
                    Cl = clouds[ListBoxTemplates.SelectedIndices[0] - markers.Count];
                    Cl.Name = NewName;
                    P.Path = fMain.TemplatesFolderPath + "\\" + NewName;
                    Cl.StoreTemplate(P, "");
                }

                if ((TemplateTab.SelectedIndex == 0))
                {
                    tbName.Text = NewName;
                }
                else if ((TemplateTab.SelectedIndex == 1))
                {
                    tbNameProp.Text = NewName;
                }
                else if ((TemplateTab.SelectedIndex == 2))
                {
                    tbNameCOP.Text = NewName;
                }

                if (!(Utils.ReadyForWriting(fMain.TemplatesFolderPath + "\\" + OldName, true)))
                {
                    MessageBox.Show("Original File is read-only and could not be deleted");
                    RefreshList(true);
                    return;
                }
                File.Delete(fMain.TemplatesFolderPath + "\\" + OldName);
                RefreshList(true);
            }

        }

        private void btnOK_Click(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In btnOK_Click");
            //End added by huimei

            this.Close();
        }

        private void chkAddFacet_CheckedChanged(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In chkAddFacet_CheckedChanged");
            //End added by huimei

            if (!initialized)
            {
                return;
            }
            if (object.ReferenceEquals(this.ActiveControl, chkAddFacet))
            {
                UpdateUI();
            }
        }

        private void btnReset_Click(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In btnReset_Click");
            //End added by huimei

            ResetSampling();
            UpdateUI();
        }

        private void nudMinVectorLength_ValueChanged(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In nudMinVectorLength_ValueChanged");
            //End added by huimei

            if (!initialized)
            {
                return;
            }
            fMain.ShownVectorsMinLength = Convert.ToDouble(nudMinVectorLength.Value);
            //hide the vectors which would not be processed
        }

        private void MarkersForm_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
        {

            //Added by huimei
            log.Info("In MarkersForm_FormClosing");
            //End added by huimei

            if (VectorSamples.Count > 0 || Tip2ToolSamples.Count > 0)
            {
                if (MessageBox.Show("The samples were not saved. Exit anyhow?", "Warning...", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                {
                    e.Cancel = true;
                    return;
                }
            }
            ResetSampling();
            timerSample.Enabled = false;
            //restore previous fmain settings
            markers.PredictiveFramesInterleave = Convert.ToInt32(Conversion.Fix(SavedPredictiveFramesInterleave));
            fMain.ShownVectorsMinLength = SavedShownVectorsMinLength;
            fMain.IsShowingVectors = SavedIsShowingVectors;
            fMain.IsShowingVectorsForTemplateRegistration = false;
            fMain.XPointsProcessingEnabled = WasXPointsProcessingEnabled;
            markers.JitterFilterEnabled = WasJitterFilterEnabled;
            markers.KalmanFilterEnabled = WaskalmanFilterEnabled;
            markers.SmallerXPFootprint = WasSmallMarkerDetectionEnabled;

            double TtCalibOffsetX, TtCalibOffsetY, TtCalibOffsetZ;
            double TtCalibOffsetRX, TtCalibOffsetRY, TtCalibOffsetRZ;
            double.TryParse(tbOffsetX.Text, out TtCalibOffsetX);
            double.TryParse(tbOffsetY.Text, out TtCalibOffsetY);
            double.TryParse(tbOffsetZ.Text, out TtCalibOffsetZ);
            double.TryParse(tbOffsetRX.Text, out TtCalibOffsetRX);
            double.TryParse(tbOffsetRY.Text, out TtCalibOffsetRY);
            double.TryParse(tbOffsetRZ.Text, out TtCalibOffsetRZ);

            fMain.TTCalib_Offset_X = TtCalibOffsetX;
            fMain.TTCalib_Offset_Y = TtCalibOffsetY;
            fMain.TTCalib_Offset_Z = TtCalibOffsetZ;
            fMain.TTCalib_Offset_RX = TtCalibOffsetRX;
            fMain.TTCalib_Offset_RY = TtCalibOffsetRY;
            fMain.TTCalib_Offset_RZ = TtCalibOffsetRZ;

        }

        private void btnSetAsReference_Click(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In btnSetAsReference_Click");
            //End added by huimei

            fMain.RefreshMarkerTemplates();
            fMain.RefreshCloudTemplates();
            //markers.LoadTemplates(fMain.TemplatesFolderPath)
            string ReferenceName = null;
            if (ListBoxTemplates.SelectedIndices.Count == 0)
            {
                return;
            }
            ReferenceName = markers[ListBoxTemplates.SelectedIndices[0]].Name;

            // Use this method to assign a specific jitter coefficient for a marker.
            IList<Marker> markerList = markers.GetTemplateMarkers();
            int markernum = 0;
            Marker Ref_Marker = new Marker();
            while (markernum < markerList.Count)
            {
                Marker M = markerList[markernum];
                if (M.Name == ReferenceName)
                {
                    Ref_Marker = M;
                }
                markernum += 1;
            }
            markernum = 0;
            while (markernum < markerList.Count)
            {
                Marker M = markerList[markernum];
                if ((M.Name != Ref_Marker.Name))
                {
                    M.ReferenceMarker = Ref_Marker;
                }
                markernum += 1;
            }
            tbReferenceMarker.Text = "Camera-Space";
            tbReferenceMarker.Refresh();

            //ResetSampling()
            //CurrMarker = Nothing
            //NewFacet = Nothing
            //RefreshList(True)
            //tbName.Text = ""
            //tbNameCOP.Text = ""
            //tbNameProp.Text = ""
            UpdateUI();
        }

        private void btnUpdateProperties_Click(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In btnUpdateProperties_Click");
            //End added by huimei

            double JitterCoefficient = 0;
            if (double.TryParse(tbJitterCoefficient.Text, out JitterCoefficient))
            {
                CurrMarker.JitterFilterCoefficient = JitterCoefficient;
            }

            double ExtrapolatedFrames = 0;
            if (double.TryParse(tbExtrapolatedFrames.Text, out ExtrapolatedFrames))
            {
                CurrMarker.ExtrapolatedFrames = (int)ExtrapolatedFrames;
            }

        }

        private void chkSampleCOP2Marker_CheckedChanged(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In chkSampleCOP2Marker_CheckedChanged");
            //End added by huimei

            if (!initialized)
            {
                return;
            }
            if (WasXPointsProcessingEnabled == false)
            {
                fMain.XPointsProcessingEnabled = true;
                fMain.IsShowingXPoints = true;
            }

            Persistence P = new Persistence();
            Marker M = null;
            COPlabelInfo.Text = "";
            //activating sampling
            if (chkSampleCOP2Marker.Checked == true)
            {
                if (fMain.CurrCamera.ThermalHazardCode != HazardCodes.None)
                {
                    if (MessageBox.Show("Camera not yet thermally stable. Proceed anyhow?", "Warning!", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        chkSampleSlider.Checked = false;
                        //cancel (note: will recurse!)
                        return;
                    }
                }
                SamplingSuspended = false;
                //select the new form state
                //start sampling
                if (FormState == StateEnum.Idling)
                {
                    FormState = StateEnum.SamplingCOP2Marker;
                }
                if (FormState == StateEnum.SamplingCOP2Marker)
                {
                    IList<Marker> ColMarker = markers.GetIdentifiedMarkers(null);
                    if (ColMarker.Count == 1)
                    {
                        ToolName = ColMarker[0].Name;
                        M = FindTemplateInSet(ToolName);
                        if (CurrMarker == null)
                        {
                            CurrMarker = M;
                            //check if the same (so we continue to accumulate samples)
                        }
                        else
                        {
                            if ((M != null))
                            {
                                if (M.Name != CurrMarker.Name)
                                {
                                    CurrMarker = M;
                                    //update current marker
                                }
                            }
                        }
                        Debug.Assert((CurrMarker != null));
                        //should not happen!
                        tbNameCOP.Text = CurrMarker.Name;
                    }
                }
                //unchecked - suspend sampling
            }
            else
            {
                if (FormState == StateEnum.SamplingCOP2Marker)
                {
                    // for pure COP registration there is no need to any marker
                    if ((CurrMarker != null))
                    {
                        //We average the tip->tool transforms sampled, then add it to the marker
                        Debug.Assert(CurrMarker.Name == ToolName);

                        for (int j = 1; j <= NumberofXPs; j++)
                        {

                            if ((COPsamples[j].Count >= nudCOPSamplesNumber.Value))
                            {
                                //Average in all the Slider XPoint Samples
                                XPoint SliderXPoint = COPsamples[j][0];
                                double[] xp3d = new double[4];
                                double[] xp3d_avg = new double[4];
                                xp3d_avg = COPsamples[j][0].Position3D;

                                int i = 1;
                                while (i < COPsamples[j].Count)
                                {
                                    xp3d = COPsamples[j][i].Position3D;
                                    xp3d_avg[0] = ((i) * xp3d_avg[0] + xp3d[0]) / (i + 1);
                                    xp3d_avg[1] = ((i) * xp3d_avg[1] + xp3d[1]) / (i + 1);
                                    xp3d_avg[2] = ((i) * xp3d_avg[2] + xp3d[2]) / (i + 1);
                                    i += 1;
                                }

                                SliderXPoint.Position3D = xp3d_avg;
                                SliderXPoint.Index = 1;
                                // 10 * (nudSliderXPointId.Value) + nudSliderControlPoint.Value
                                //CurrMarker.SliderXPointAt0 = SliderXPoint
                                //CurrMarker.SliderControlledXpointsCount = nudCOPXpointsNumber.Value
                                int Id = Convert.ToInt32(Microsoft.VisualBasic.Interaction.InputBox("Enter an ID# for the slider XPoint(s)", "Slider XPoint", "1", 400, 400));
                                CurrMarker.AddTemplateSliderControlledXpoint(Id, SliderXPoint);
                            }
                        }
                    }

                }
                SamplingSuspended = true;
            }
            UpdateUI();


        }

        private void chkSampleCOP_CheckedChanged(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In chkSampleCOP_CheckedChanged");
            //End added by huimei

            if (!initialized)
            {
                return;
            }
            if (WasXPointsProcessingEnabled == false)
            {
                fMain.XPointsProcessingEnabled = true;
                fMain.CloudsProcessingEnabled = true;
                fMain.IsShowingXPoints = true;
            }

            Persistence P = new Persistence();
            COPlabelInfo.Text = "";
            //activating sampling
            if (chkSampleCOP.Checked == true)
            {
                if (fMain.CurrCamera.ThermalHazardCode != HazardCodes.None)
                {
                    if (MessageBox.Show("Camera not yet thermally stable. Proceed anyhow?", "Warning!", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    {
                        chkSampleSlider.Checked = false;
                        //cancel (note: will recurse!)
                        return;
                    }
                }
                SamplingSuspended = false;
                //select the new form state
                //start sampling
                if (FormState == StateEnum.Idling)
                {
                    FormState = StateEnum.SamplingCOP;
                    NewCloud = new Cloud();
                    NewCloud.IsCloudRegistering = true;
                }
                //unchecked - suspend sampling
            }
            else
            {
                NewCloud.IsCloudRegistering = false;
                chkSampleCOP.Checked = false;
                SamplingSuspended = true;
                UpdateUI();
            }


        }

        private void RemoveSXP_Click(System.Object sender, System.EventArgs e)
        {

            //Added by huimei
            log.Info("In RemoveSXP_Click");
            //End added by huimei

            int sliderXPtoberemovedID = 0;
            int.TryParse(tbRemoveSliderXpID.Text, out sliderXPtoberemovedID);
            // Code snippet for retrieving template Slider Controlled Xpoints
            int j = 0;
            XPoint XP = default(XPoint);
            IList<XPoint> TemplateSliderXPoints = null;
            TemplateSliderXPoints = CurrMarker.TemplateSliderControlledXpointsGet(sliderXPtoberemovedID);
            for (j = 0; j <= TemplateSliderXPoints.Count - 1; j++)
            {
                XP = TemplateSliderXPoints[j];
                if (MessageBox.Show("remove the slider xpoint from the marker list", "Warning!", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    CurrMarker.RemoveTemplateSliderControlledXpoint(sliderXPtoberemovedID, XP);

                    // delete the file and update it with a new one.
                    string FilePath = null;
                    string Name = null;
                    Name = markers[ListBoxTemplates.SelectedIndices[0]].Name;

                    FilePath = fMain.TemplatesFolderPath + "\\" + Name;
                    if (!(Utils.ReadyForWriting(FilePath, true)))
                    {
                        MessageBox.Show("File is read-only. Change permissions and try again");
                        return;
                    }
                    File.Delete(FilePath);
                    SaveCurrMarkerTemplate(Name);

                }

            }

        }

        private void knownVectorPresetsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            //Added by huimei
            log.Info("In knownVectorPresetsComboBox_SelectedIndexChanged");
            //End added by huimei

            if (knownVectorPresetsComboBox.SelectedIndex != 0)
            {
                tbName.Text = knownVectorPresetsComboBox.SelectedItem.ToString();
            }
        }

        private void MarkersForm_KeyDown(object sender, KeyEventArgs e)
        {

            //Added by huimei
            log.Info("In MarkersForm_KeyDown");
            //End added by huimei

            if (e.Control && e.KeyCode == Keys.F)
            {
                chkSampleFacet.Checked = !chkSampleFacet.Checked;
            }

            if (e.Control && e.KeyCode == Keys.T)
            {
                chkSampleToolTip.Checked = !chkSampleToolTip.Checked;
            }
        }

    }

}