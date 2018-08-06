using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MTInterfaceDotNet;


namespace HoloEvac_Micron
{
  public partial class OptionsForm : Form
  {
    private MainForm fMain;
    public OptionsForm(MainForm mForm)
    {
      InitializeComponent();
      rbArray[0] = radioCoolness1;
      rbArray[1] = radioCoolness2;
      rbArray[2] = radioCoolness3;
      rbArray[3] = radioCoolness4;
      rbArray[4] = radioCoolness5;
      rbArray[5] = radioCoolness6;
      rbArray[6] = radioCoolness7;
      rbArray[7] = radioCoolness8;

      rbArrayOR[0] = radioCoolnessOR1;
      rbArrayOR[1] = radioCoolnessOR2;
      rbArrayOR[2] = radioCoolnessOR3;
      rbArrayOR[3] = radioCoolnessOR4;
      rbArrayOR[4] = radioCoolnessOR5;
      rbArrayOR[5] = radioCoolnessOR6;
      rbArrayOR[6] = radioCoolnessOR7;
      rbArrayOR[7] = radioCoolnessOR8;

      fMain = mForm;
    }


    private RadioButton[] rbArray = new RadioButton[8];
    private RadioButton[] rbArrayOR = new RadioButton[8];
    private Cameras optCameras = MT.Cameras;
    private Markers optMarkers = MT.Markers;
    private XPoints optXPoints = MT.XPoints;
    private Clouds optClouds = MT.Clouds;

    private bool ignore = false;
    //factory default settings
    private double MarkersDefaultToleranceMM;
    //factory default settings
    private double CloudsDefaultToleranceMM;
    private int DefaultInterleave;
    private double DefaultShutterLimiter;
    private double DefaultStrobSignalDuration;
    private double DefaultStrobSignalDelay;
    private double DefaultGammaCorrectionValue;
    private double DefaultGainLimiter;
    private int DefaultOverExposureInterleave;
    private double DefaultCoolness;
    private int DefaultExtrapolatedFrames;
    private double DefaultAngulardotproductTolerance;
    private int DefaultJitterFilterHistoryLength;
    private bool DefaultJitterFilter;
    private bool DefaultKalmanFilter;
    private double DefaultJitterFilterCoefficient;
    private double DefaultAngularJitterFilterCoefficient;
    private bool DefaultSmallerFootprint;
    private bool DefaultTemplateBasedWarmupCorrection;
    private bool DefaultAutoAdjustHDRminLux;
    private bool DefaultHdrShortCycle;
    private bool initialized = false;
    private int DefaultXPointsSensitivity;
    private int DefaultMisalignmentSensitivity;
    private double DefaultHdrMinLux;


    private void OptionsForm_Load(System.Object sender, System.EventArgs e)
    {
      initialized = true;

      double ORlightCoolness = fMain.CurrCamera.HDRLightCoolnessOR;
      tbLightCoolnessOR.Text = ORlightCoolness.ToString();
      ORlightCoolness = Utils.Constrain(TrackBarLightCoolnessOR.Minimum / 100.0, ORlightCoolness, TrackBarLightCoolnessOR.Maximum / 100.0);
      TrackBarLightCoolnessOR.Value = TrackBarLightCoolnessOR.Minimum + TrackBarLightCoolnessOR.Maximum - Convert.ToInt32(Math.Round(ORlightCoolness * 100));
      TrackBarWarmupSensitivity.Value = (int)fMain.WarmupSensitivity;
      TextWarmupSensitivity.Text = fMain.WarmupSensitivity.ToString();

      //obtain factory defaults
      MarkersDefaultToleranceMM = optMarkers.TemplateMatchToleranceMMDefault;
      CloudsDefaultToleranceMM = optClouds.TemplateMatchToleranceMMDefault;
      DefaultInterleave = 0;
      DefaultOverExposureInterleave = 0;
      DefaultExtrapolatedFrames = 5;
      DefaultAngulardotproductTolerance = optMarkers.AngularDotProductToleranceDeg;
      DefaultJitterFilterHistoryLength = optMarkers.JitterFilterHistoryLength;
      DefaultXPointsSensitivity = 50;
      DefaultMisalignmentSensitivity = 35;
      TrackBarXpointsSensitivity.Value = optXPoints.Sensitivity;
      TrackBarMisalignmentSensitivity.Value = optXPoints.MisalignmentSensitivity;
      XpointsSensitivity.Text = optXPoints.Sensitivity.ToString();
      MisalignmentSensitivity.Text = optXPoints.MisalignmentSensitivity.ToString();
      DefaultShutterLimiter = Math.Round(optCameras.ShutterMsecsLimiterDefault, 1);
      DefaultStrobSignalDuration = Math.Round(fMain.StrobSignalDuration, 1);
      DefaultStrobSignalDelay = Math.Round(fMain.StrobSignalDelay, 1);
      DefaultGainLimiter = 5;
      DefaultCoolness = MT.GetLightCoolnessValue(LightType.SoftWhiteCFL_2700K);
      DefaultJitterFilter = true;
      DefaultSmallerFootprint = false;
      DefaultTemplateBasedWarmupCorrection = false;
      DefaultAutoAdjustHDRminLux = true;
      DefaultHdrShortCycle = true;
      DefaultGammaCorrectionValue = 0;
      DefaultHdrMinLux = 50;
      DefaultKalmanFilter = true;
      //initialize controls
      tbMarkersToleranceMM.Text = optMarkers.TemplateMatchToleranceMM.ToString();
      tbCloudsToleranceMM.Text = optClouds.TemplateMatchToleranceMM.ToString();
      nudPredictiveFramesInterleave.Value = optMarkers.PredictiveFramesInterleave;
      nudOverExposure.Value = optMarkers.OverExposureControlInterleave;
      nudExtrapolatedFrames.Value = optMarkers.ExtrapolatedFrames;
      nudAngulardotproductTolerance.Value = (int)optMarkers.AngularDotProductToleranceDeg;
      nudJitterFilterHistoryLength.Value = optMarkers.JitterFilterHistoryLength;
      txtHdrMinLux.Text = Math.Round((double)optCameras.HdrMinLux, 1).ToString();

      tbGainLimiter.Text = Math.Round(optCameras.GainFLimiter, 1).ToString();
      tbShutterLimiter.Text = Math.Round(optCameras.ShutterMsecsLimiter, 1).ToString();
      tbStrobSignalDuration.Text = Math.Round(fMain.StrobSignalDuration, 1).ToString();
      tbStrobSignalDelay.Text = Math.Round(fMain.StrobSignalDelay, 1).ToString();
      tbGammaCorrectionValue.Text = fMain.GammaValue.ToString();
      tbLightCoolness.Text = Math.Round(optCameras.LightCoolness, 2).ToString();
      DefaultJitterFilterCoefficient = 0.5;
      DefaultAngularJitterFilterCoefficient = 0.05;
      txtJitterFilterCoefficient.Text = optMarkers.JitterFilterCoefficient.ToString();
      txtAngularJitterFilterCoefficient.Text = optMarkers.AngularJitterFilterCoefficient.ToString();
      OptionForm_Timer.Enabled = true;

      if (fMain.CurrCamera.sensorsnum == 3)
      {
          cbMiddleSensorEnable.Enabled = true;
          cbMiddleSensorEnable.Visible = true;
          cbMiddleSensorEnable.Checked = fMain.CurrCamera.MiddleSensorEnabled;
      }
      UpdateUI();

    }

    private void OptionForm_Timer_Tick(System.Object sender, System.EventArgs e)
    {
      //regular coolness correction
      if (OptionsTabControl.SelectedIndex == 2)
      {
        if (fMain.AutoAdjustLightCoolness)
        {
          double lightCoolness = fMain.CurrCamera.LightCoolness;
          tbLightCoolness.Text = lightCoolness.ToString("0.0");
          lightCoolness = Utils.Constrain(TrackBarLightCoolness.Minimum / 100.0, lightCoolness, TrackBarLightCoolness.Maximum / 100.0);
          TrackBarLightCoolness.Value = TrackBarLightCoolness.Minimum + TrackBarLightCoolness.Maximum - Convert.ToInt32((Math.Round(lightCoolness * 100)));
        }
        //HDR coolness correction
      }
      else if (OptionsTabControl.SelectedIndex == 3)
      {
        if (fMain.AutoAdjustLightCoolness)
        {
          double ORlightCoolness = fMain.CurrCamera.LightCoolness;
          tbLightCoolnessOR.Text = ORlightCoolness.ToString("0.0");
          ORlightCoolness = Utils.Constrain(TrackBarLightCoolnessOR.Minimum / 100.0, ORlightCoolness, TrackBarLightCoolnessOR.Maximum / 100.0);
          TrackBarLightCoolnessOR.Value = TrackBarLightCoolnessOR.Minimum + TrackBarLightCoolnessOR.Maximum - Convert.ToInt32((Math.Round(ORlightCoolness * 100)));
        }
      }

    }


    private void btnApply_Click(System.Object sender, System.EventArgs e)
    {
      double MarkersToleranceMM = 0;
      if (double.TryParse(tbMarkersToleranceMM.Text, out MarkersToleranceMM))
      {
        optMarkers.TemplateMatchToleranceMM = MarkersToleranceMM;
        tbMarkersToleranceMM.Text = optMarkers.TemplateMatchToleranceMM.ToString();
        MarkersToleranceMM = optMarkers.TemplateMatchToleranceMM;
        //perhaps clipped
        //validate the new value against known templates
        string[] strArray = optMarkers.Validate();
        if (strArray.Length != 0)
        {
          StringBuilder strBuilder = new StringBuilder(strArray[0]);
          int i = 1;
          while (i < strArray.Length)
          {
            strBuilder.Append(Environment.NewLine).Append(strArray[i]);
            i += 1;
          }
          MessageBox.Show("WARNINGS: " + "\n" + strBuilder.ToString() + "\n" + "Please reduce tolerance or modify templates.");
        }
      }
      else
      {
        Console.Beep();
        //not acceptable
      }

      double CloudsToleranceMM = 0;
      if (double.TryParse(tbCloudsToleranceMM.Text, out CloudsToleranceMM))
      {
        optClouds.TemplateMatchToleranceMM = CloudsToleranceMM;
        tbCloudsToleranceMM.Text = optClouds.TemplateMatchToleranceMM.ToString();
        CloudsToleranceMM = optClouds.TemplateMatchToleranceMM;
        //perhaps clipped
        //validate the new value against known templates
        string[] strArray = optClouds.Validate();
        if (strArray.Length != 0)
        {
          StringBuilder strBuilder = new StringBuilder(strArray[0]);
          int i = 1;
          while (i < strArray.Length)
          {
            strBuilder.Append(Environment.NewLine).Append(strArray[i]);
            i += 1;
          }
          MessageBox.Show("WARNINGS: " + "\n" + strBuilder.ToString() + "\n" + "Please reduce tolerance or modify templates.");
        }
      }
      else
      {
        Console.Beep();
        //not acceptable
      }

      optMarkers.PredictiveFramesInterleave = Convert.ToInt32((nudPredictiveFramesInterleave.Value));
      //guaranteed numeric
      nudPredictiveFramesInterleave.Value = optMarkers.PredictiveFramesInterleave;
      //in case it was clipped

      optMarkers.OverExposureControlInterleave = Convert.ToInt32((nudOverExposure.Value));
      //guaranteed numeric
      nudOverExposure.Value = optMarkers.OverExposureControlInterleave;
      //in case it was clipped

      optMarkers.ExtrapolatedFrames = Convert.ToInt32((nudExtrapolatedFrames.Value));
      //guaranteed numeric
      nudExtrapolatedFrames.Value = optMarkers.ExtrapolatedFrames;
      //in case it was clipped

      optMarkers.AngularDotProductToleranceDeg = Convert.ToInt32((nudAngulardotproductTolerance.Value));
      //guaranteed numeric
      nudAngulardotproductTolerance.Value = (decimal)optMarkers.AngularDotProductToleranceDeg;

      optMarkers.JitterFilterHistoryLength = Convert.ToInt32((nudJitterFilterHistoryLength.Value));
      nudJitterFilterHistoryLength.Value = optMarkers.JitterFilterHistoryLength;

      double gainLimiter = 0;
      if (double.TryParse(tbGainLimiter.Text, out gainLimiter))
      {
        optCameras.GainFLimiter = gainLimiter;
        //add missing digits
      }
      else
      {
        Console.Beep();
        //not acceptable
      }

      gainLimiter = Math.Round(optCameras.GainFLimiter, 1);
      tbGainLimiter.Text = gainLimiter.ToString();

      double shutterLimiter = 0;
      if (double.TryParse(tbShutterLimiter.Text, out shutterLimiter))
      {
        optCameras.ShutterMsecsLimiter = shutterLimiter;
      }
      else
      {
        Console.Beep();
        //not acceptable
      }

      shutterLimiter = Math.Round(optCameras.ShutterMsecsLimiter, 1);
      tbShutterLimiter.Text = shutterLimiter.ToString();

      double StrobSignalDuration = 0;
      if (double.TryParse(tbStrobSignalDuration.Text, out StrobSignalDuration))
      {
        fMain.StrobSignalDuration = StrobSignalDuration;
      }
      else
      {
        Console.Beep();
        //not acceptable
      }
      StrobSignalDuration = Math.Round(fMain.StrobSignalDuration, 1);
      tbStrobSignalDuration.Text = StrobSignalDuration.ToString();

      double StrobSignalDelay = 0;
      if (double.TryParse(tbStrobSignalDelay.Text, out StrobSignalDelay))
      {
        fMain.StrobSignalDelay = StrobSignalDelay;
      }
      else
      {
        Console.Beep();
        //not acceptable
      }
      StrobSignalDelay = Math.Round(fMain.StrobSignalDelay, 1);
      tbStrobSignalDelay.Text = StrobSignalDelay.ToString();

      double GammaCorrectionValue = 0;
      if (double.TryParse(tbGammaCorrectionValue.Text, out GammaCorrectionValue))
      {
        fMain.GammaValue = GammaCorrectionValue;
      }
      else
      {
        Console.Beep();
        //not acceptable
      }
      GammaCorrectionValue = fMain.GammaValue;
      tbGammaCorrectionValue.Text = GammaCorrectionValue.ToString();


      int HdrMinLux = 0;
      if (int.TryParse(txtHdrMinLux.Text, out HdrMinLux))
      {
        optCameras.HdrMinLux = HdrMinLux;
      }
      else
      {
        Console.Beep();
        //not acceptable
      }

      double JitterFilterCoefficient = 0;
      if (double.TryParse(txtJitterFilterCoefficient.Text, out JitterFilterCoefficient))
      {
        optMarkers.JitterFilterCoefficient = JitterFilterCoefficient;
      }
      else
      {
        Console.Beep();
        //not acceptable
      }
      txtJitterFilterCoefficient.Text = Math.Round(optMarkers.JitterFilterCoefficient, 2).ToString();

      double AngularJitterFilterCoefficient = 0;
      if (double.TryParse(txtAngularJitterFilterCoefficient.Text, out AngularJitterFilterCoefficient))
      {
        optMarkers.AngularJitterFilterCoefficient = AngularJitterFilterCoefficient;
      }
      else
      {
        Console.Beep();
        //not acceptable
      }
      txtAngularJitterFilterCoefficient.Text = Math.Round(optMarkers.AngularJitterFilterCoefficient, 3).ToString();

      optXPoints.Sensitivity = int.Parse(XpointsSensitivity.Text);
      optXPoints.MisalignmentSensitivity = int.Parse(MisalignmentSensitivity.Text);

      // for the regular coolness
      if (OptionsTabControl.SelectedIndex == 2)
      {
        double lightCoolness = 0;
        if (double.TryParse(tbLightCoolness.Text, out lightCoolness))
        {
          optCameras.LightCoolness = lightCoolness;
        }
        else
        {
          Console.Beep();
          //not acceptable
        }
        lightCoolness = Math.Round(optCameras.LightCoolness, 2);
        tbLightCoolness.Text = lightCoolness.ToString();
      }
      else if (OptionsTabControl.SelectedIndex == 3)
      {
        double lightCoolnessOR = 0;
        if (double.TryParse(tbLightCoolnessOR.Text, out lightCoolnessOR))
        {
          optCameras.HDRLightCoolness = lightCoolnessOR;
        }
        else
        {
          Console.Beep();
          //not acceptable
        }
        lightCoolnessOR = Math.Round(optCameras.HDRLightCoolness, 2);
        tbLightCoolnessOR.Text = lightCoolnessOR.ToString();
      }

      UpdateUI();
      fMain.UpdatePP();
      //persist

    }

    private void btnResetDefaults_Click(System.Object sender, System.EventArgs e)
    {
      tbMarkersToleranceMM.Text = MarkersDefaultToleranceMM.ToString();
      nudPredictiveFramesInterleave.Value = DefaultInterleave;
      nudOverExposure.Value = DefaultOverExposureInterleave;

      nudExtrapolatedFrames.Value = DefaultExtrapolatedFrames;
      nudAngulardotproductTolerance.Value = (decimal)DefaultAngulardotproductTolerance;
      nudJitterFilterHistoryLength.Value = DefaultJitterFilterHistoryLength;

      tbGainLimiter.Text = Math.Round(DefaultGainLimiter, 1).ToString();
      tbShutterLimiter.Text = Math.Round(DefaultShutterLimiter, 1).ToString();
      tbStrobSignalDuration.Text = Math.Round(DefaultStrobSignalDuration, 1).ToString();
      tbStrobSignalDelay.Text = Math.Round(DefaultStrobSignalDelay, 1).ToString();
      tbGammaCorrectionValue.Text = DefaultGammaCorrectionValue.ToString();

      chkJitterFilter.Checked = DefaultJitterFilter;
      chkKalmanFilter.Checked = DefaultKalmanFilter;
      txtJitterFilterCoefficient.Text = DefaultJitterFilterCoefficient.ToString();
      txtAngularJitterFilterCoefficient.Text = DefaultAngularJitterFilterCoefficient.ToString();
      chkDetSmMarkers.Checked = DefaultSmallerFootprint;
      chkTemplateBasedWarmupCorrection.Checked = DefaultTemplateBasedWarmupCorrection;
      XpointsSensitivity.Text = DefaultXPointsSensitivity.ToString();
      TrackBarXpointsSensitivity.Value = DefaultXPointsSensitivity;
      MisalignmentSensitivity.Text = DefaultMisalignmentSensitivity.ToString();
      TrackBarMisalignmentSensitivity.Value = DefaultMisalignmentSensitivity;
      txtHdrMinLux.Text = DefaultHdrMinLux.ToString();

      UpdateUI();

    }

    private void btnOK_Click(System.Object sender, System.EventArgs e)
    {
      OptionForm_Timer.Enabled = false;
      this.Close();
    }

    private void OptionsForm_FormClosing(System.Object sender, System.Windows.Forms.FormClosingEventArgs e)
    {
      string s = null;
      bool b1 = optMarkers.TemplateMatchToleranceMM != MarkersDefaultToleranceMM;
      bool b2 = optMarkers.PredictiveFramesInterleave != DefaultInterleave;
      bool b3 = optMarkers.OverExposureControlInterleave != DefaultOverExposureInterleave;
      bool b4 = Math.Round(optCameras.GainFLimiter, 1) != DefaultGainLimiter;
      bool b5 = Math.Round(optCameras.ShutterMsecsLimiter, 1) != DefaultShutterLimiter;
      bool b6 = optMarkers.ExtrapolatedFrames > DefaultExtrapolatedFrames;
      bool b7 = optMarkers.JitterFilterEnabled != DefaultJitterFilter;
      bool b8 = optMarkers.SmallerXPFootprint != DefaultSmallerFootprint;
      bool b9 = optMarkers.JitterFilterCoefficient != DefaultJitterFilterCoefficient;
      bool b10 = optXPoints.Sensitivity != DefaultXPointsSensitivity;
      bool b11 = optXPoints.MisalignmentSensitivity != DefaultMisalignmentSensitivity;
      bool b12 = optMarkers.TemplateBasedWarmupCorrection != DefaultTemplateBasedWarmupCorrection;
      bool b13 = optMarkers.AutoAdjustHDRminLux != DefaultAutoAdjustHDRminLux;
      bool b14 = optMarkers.AngularDotProductToleranceDeg != DefaultAngulardotproductTolerance;
      bool b15 = optMarkers.JitterFilterHistoryLength != DefaultJitterFilterHistoryLength;
      bool b16 = optClouds.TemplateMatchToleranceMM != CloudsDefaultToleranceMM;
      bool b17 = optMarkers.AngularJitterFilterCoefficient != DefaultAngularJitterFilterCoefficient;
      bool b18 = fMain.StrobSignalDuration != DefaultStrobSignalDuration;
      bool b19 = fMain.StrobSignalDelay != DefaultStrobSignalDelay;
      bool b20 = fMain.CurrCamera.HdrShortCycleEnabled != DefaultHdrShortCycle;
      if (b1 || b2 || b3 || b4 || b5 || b6 || b7 || b8 || b9 || b10 || b11 || b12 || b13 || b14 || b15 || b16 || b17 || b18 || b19 || b20)
      {
        s = "Current settings are different from factory defaults." + Environment.NewLine;
        s = s + "This may result in sub-optimal performance." + Environment.NewLine;
        s = s + "Proceed anyway?";
        if (MessageBox.Show(s, "Warning...", MessageBoxButtons.YesNo) == DialogResult.No)
        {
          e.Cancel = true;
        }
      }

    }

    private void UpdateUI()
    {
      bool ToleranceSame = false;
      bool InterleaveSame = false;
      bool OverExposureSame = false;
      bool ShutterLimiterSame = false;
      bool StrobSignalDurationSame = false;
      bool StrobSignalDelaySame = false;
      bool GammaCorrectionValueSame = false;
      bool GainLimiterSame = false;
      bool HdrMinLuxSame = false;
      bool CoolnessSame = false;
      bool CoolnessSameOR = false;
      bool ExtrapolatedFramesSame = false;
      bool AngulardotproductSame = false;
      bool WarmupSensitivitySame = false;
      bool JitterFilterHistoryLengthSame = false;
      bool JitterFilterCoefficientSame = false;
      bool AngularJitterFilterCoefficientSame = false;

      double MarkerstoleranceMM = 0;
      if (double.TryParse(tbMarkersToleranceMM.Text, out MarkerstoleranceMM))
      {
        if (MarkerstoleranceMM == optMarkers.TemplateMatchToleranceMM)
        {
          ToleranceSame = true;
        }
      }

      double CloudstoleranceMM = 0;
      if (double.TryParse(tbCloudsToleranceMM.Text, out CloudstoleranceMM))
      {
        if (CloudstoleranceMM == optClouds.TemplateMatchToleranceMM)
        {
          ToleranceSame = true;
        }
      }

      if (ToleranceSame)
      {
        tbMarkersToleranceMM.ForeColor = Color.Black;
      }
      else
      {
        tbMarkersToleranceMM.ForeColor = Color.Red;
      }

      double gainLimiter = 0;
      if (double.TryParse(tbGainLimiter.Text, out gainLimiter))
      {
        if (gainLimiter == Math.Round(optCameras.GainFLimiter, 1))
        {
          GainLimiterSame = true;
        }
      }

      if (GainLimiterSame)
      {
        tbGainLimiter.ForeColor = Color.Black;
      }
      else
      {
        tbGainLimiter.ForeColor = Color.Red;
      }

      double JitterFilterCoefficient = 0;
      if (double.TryParse(txtJitterFilterCoefficient.Text, out JitterFilterCoefficient))
      {
        if (JitterFilterCoefficient == optMarkers.JitterFilterCoefficient)
        {
          JitterFilterCoefficientSame = true;
        }
      }
      txtJitterFilterCoefficient.ForeColor = (JitterFilterCoefficientSame ? Color.Black : Color.Red);

      double AngularJitterFilterCoefficient = 0;
      if (double.TryParse(txtAngularJitterFilterCoefficient.Text, out AngularJitterFilterCoefficient))
      {
        if (AngularJitterFilterCoefficient == optMarkers.AngularJitterFilterCoefficient)
        {
          AngularJitterFilterCoefficientSame = true;
        }
      }
      txtAngularJitterFilterCoefficient.ForeColor = (AngularJitterFilterCoefficientSame ? Color.Black : Color.Red);

      double shutterLimiter = 0;
      if (double.TryParse(tbShutterLimiter.Text, out shutterLimiter))
      {
        if (shutterLimiter == Math.Round(optCameras.ShutterMsecsLimiter, 1))
        {
          ShutterLimiterSame = true;
        }
      }
      if (ShutterLimiterSame)
      {
        tbShutterLimiter.ForeColor = Color.Black;
      }
      else
      {
        tbShutterLimiter.ForeColor = Color.Red;
      }

      double StrobSignalDuration = 0;
      if (double.TryParse(tbStrobSignalDuration.Text, out StrobSignalDuration))
      {
        if (StrobSignalDuration == Math.Round(fMain.StrobSignalDuration, 1))
        {
          StrobSignalDurationSame = true;
        }
      }
      if (StrobSignalDurationSame)
      {
        tbStrobSignalDuration.ForeColor = Color.Black;
      }
      else
      {
        tbStrobSignalDuration.ForeColor = Color.Red;
      }

      double StrobSignalDelay = 0;
      if (double.TryParse(tbStrobSignalDelay.Text, out StrobSignalDelay))
      {
        if (StrobSignalDelay == Math.Round(fMain.StrobSignalDelay, 1))
        {
          StrobSignalDelaySame = true;
        }
      }
      if (StrobSignalDelaySame)
      {
        tbStrobSignalDelay.ForeColor = Color.Black;
      }
      else
      {
        tbStrobSignalDelay.ForeColor = Color.Red;
      }

      double GammaCorrectionValue = 0;
      if (double.TryParse(tbGammaCorrectionValue.Text, out GammaCorrectionValue))
      {
        if (GammaCorrectionValue == fMain.GammaValue)
        {
          GammaCorrectionValueSame = true;
        }
      }
      if (GammaCorrectionValueSame)
      {
        tbGammaCorrectionValue.ForeColor = Color.Black;
      }
      else
      {
        tbGammaCorrectionValue.ForeColor = Color.Red;
      }


      InterleaveSame = nudPredictiveFramesInterleave.Value == optMarkers.PredictiveFramesInterleave;
      if (InterleaveSame)
      {
        nudPredictiveFramesInterleave.ForeColor = Color.Black;
      }
      else
      {
        nudPredictiveFramesInterleave.ForeColor = Color.Red;
      }

      OverExposureSame = nudOverExposure.Value == optMarkers.OverExposureControlInterleave;
      if (OverExposureSame)
      {
        nudOverExposure.ForeColor = Color.Black;
      }
      else
      {
        nudOverExposure.ForeColor = Color.Red;
      }

      ExtrapolatedFramesSame = nudExtrapolatedFrames.Value == optMarkers.ExtrapolatedFrames;
      if (ExtrapolatedFramesSame)
      {
        nudExtrapolatedFrames.ForeColor = Color.Black;
      }
      else
      {
        nudExtrapolatedFrames.ForeColor = Color.Red;
      }

      AngulardotproductSame = (nudAngulardotproductTolerance.Value == (int)optMarkers.AngularDotProductToleranceDeg);
      if (AngulardotproductSame)
      {
        nudAngulardotproductTolerance.ForeColor = Color.Black;
      }
      else
      {
        nudAngulardotproductTolerance.ForeColor = Color.Red;
      }

      JitterFilterHistoryLengthSame = nudJitterFilterHistoryLength.Value == optMarkers.JitterFilterHistoryLength;
      if (JitterFilterHistoryLengthSame)
      {
        nudJitterFilterHistoryLength.ForeColor = Color.Black;
      }
      else
      {
        nudJitterFilterHistoryLength.ForeColor = Color.Red;
      }

      int HdrMinLux = 0;
      if (int.TryParse(txtHdrMinLux.Text, out HdrMinLux))
      {
        if (HdrMinLux == optCameras.HdrMinLux)
        {
          HdrMinLuxSame = true;
        }
      }

      if (HdrMinLuxSame)
      {
        txtHdrMinLux.ForeColor = Color.Black;
      }
      else
      {
        txtHdrMinLux.ForeColor = Color.Red;
      }

      //regular coolness
      if (OptionsTabControl.SelectedIndex == 1)
      {
        double lightCoolness = 0;
        if (double.TryParse(tbLightCoolness.Text, out lightCoolness))
        {
          CoolnessSame = lightCoolness == optCameras.LightCoolness;

          lightCoolness = Utils.Constrain(TrackBarLightCoolness.Minimum / 100.0, lightCoolness, TrackBarLightCoolness.Maximum / 100.0);
          TrackBarLightCoolness.Value = TrackBarLightCoolness.Minimum + TrackBarLightCoolness.Maximum - Convert.ToInt32((Math.Round(lightCoolness * 100)));
          for (int i = 0; i <= 7; i++)
          {
            rbArray[i].Checked = (lightCoolness == MT.GetLightCoolnessValue((LightType)i));
          }
        }
        if (CoolnessSame)
        {
          tbLightCoolness.ForeColor = Color.Black;
        }
        else
        {
          tbLightCoolness.ForeColor = Color.Red;
        }
        // HDR coolness
      }
      else if (OptionsTabControl.SelectedIndex == 2)
      {

        double ORlightCoolness = 0;
        if (double.TryParse(tbLightCoolnessOR.Text, out ORlightCoolness))
        {
          CoolnessSameOR = ORlightCoolness == optCameras.HDRLightCoolness;

          ORlightCoolness = Utils.Constrain(TrackBarLightCoolnessOR.Minimum / 100.0, ORlightCoolness, TrackBarLightCoolnessOR.Maximum / 100.0);
          TrackBarLightCoolnessOR.Value = TrackBarLightCoolnessOR.Minimum + TrackBarLightCoolnessOR.Maximum - Convert.ToInt32((Math.Round(ORlightCoolness * 100)));
          for (int i = 0; i <= 7; i++)
          {
            rbArrayOR[i].Checked = (ORlightCoolness == MT.GetLightCoolnessValue((LightType)i));
          }
        }
        if (CoolnessSameOR)
        {
          tbLightCoolnessOR.ForeColor = Color.Black;
        }
        else
        {
          tbLightCoolnessOR.ForeColor = Color.Red;
        }
      }


      //btnApply.Enabled = Not (ToleranceSame AndAlso InterleaveSame AndAlso OverExposureSame AndAlso ShutterLimiterSame AndAlso GainLimiterSame AndAlso CoolnessSame AndAlso ExtrapolatedFramesSame AndAlso JitterFilterCoefficientSame andalso AngularJitterFilterCoefficientSame AndAlso HdrMinLuxSame andalso StrobSignalDurationSame andalso StrobSignalDelaySame)
      chkAutoAdjustLCOR.Checked = fMain.AutoAdjustLightCoolness;

      chkJitterFilter.Checked = optMarkers.JitterFilterEnabled;
      chkKalmanFilter.Checked = optMarkers.KalmanFilterEnabled;
      chkDetSmMarkers.Checked = optMarkers.SmallerXPFootprint;
      chkAutoAdjustHDRminLux.Checked = optMarkers.AutoAdjustHDRminLux;
      chkHdrShortCycle.Checked = fMain.CurrCamera.HdrShortCycleEnabled;
      chkHdrShortCycleLockedMarkers.Checked = optMarkers.AutoAdjustShortCycleHdrExposureLockedMarkers;
      chkTemplateBasedWarmupCorrection.Checked = optMarkers.TemplateBasedWarmupCorrection;

      btnApply.Enabled = !(ToleranceSame && InterleaveSame && OverExposureSame && ShutterLimiterSame && GainLimiterSame && CoolnessSame && CoolnessSameOR && ExtrapolatedFramesSame && JitterFilterCoefficientSame && AngularJitterFilterCoefficientSame && HdrMinLuxSame && AngulardotproductSame && WarmupSensitivitySame && JitterFilterHistoryLengthSame && StrobSignalDurationSame && StrobSignalDelaySame);
      chkAutoAdjustLC.Checked = fMain.AutoAdjustLightCoolness;

    }

    private void tbToleranceMM_TextChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      UpdateUI();
    }


    private void tbShutterLimiter_TextChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      UpdateUI();
    }

    private void tbGainLimiter_TextChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      UpdateUI();
    }

    private void nudPredictiveFramesInterleave_ValueChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      UpdateUI();
    }

    private void nudOverExposure_ValueChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      UpdateUI();
    }

    private void nudExtrapolatedFrames_ValueChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      UpdateUI();
    }

    private void nudAngulardotproductTolerance_ValueChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      UpdateUI();
    }

    private void chkJitterFilter_CheckedChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      optMarkers.JitterFilterEnabled = chkJitterFilter.Checked;
      if (optMarkers.JitterFilterEnabled)
      {
        txtJitterFilterCoefficient.Enabled = true;
        txtAngularJitterFilterCoefficient.Enabled = true;
      }
      else
      {
        txtJitterFilterCoefficient.Enabled = false;
        txtAngularJitterFilterCoefficient.Enabled = false;
      }
      fMain.UpdatePP();
      //persistance
    }

    private void chkDetSmMarkers_CheckedChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      optMarkers.SmallerXPFootprint = chkDetSmMarkers.Checked;
      if (optMarkers.SmallerXPFootprint)
      {
        fMain.FiducialVectorMaxLength = 8;
      }
      else
      {
        fMain.FiducialVectorMaxLength = 10;
      }
      if (optMarkers.SmallerXPFootprint)
      {
        fMain.ShownVectorsMinLength = 8;
      }
      else
      {
        fMain.ShownVectorsMinLength = 10;
      }
      fMain.UpdatePP();
      //persist
    }

    private void chkTemplateBasedWarmupCorrection_CheckedChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      optMarkers.TemplateBasedWarmupCorrection = chkTemplateBasedWarmupCorrection.Checked;
      fMain.UpdatePP();
      //persist
    }

    private void txtJitterFilterCoefficient_TextChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      UpdateUI();
    }

    private void tbLightCoolness_TextChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }

      if (ignore)
      {
        return;
      }
      ignore = true;
      UpdateUI();
      ignore = false;
    }
    private void TrackBarXpointsSensitivity_Scroll(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      XpointsSensitivity.Text = TrackBarXpointsSensitivity.Value.ToString();
      optXPoints.Sensitivity = int.Parse(XpointsSensitivity.Text);
      UpdateUI();

    }

    private void XpointsSensitivity_TextChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      optXPoints.Sensitivity = int.Parse(XpointsSensitivity.Text);
      TrackBarXpointsSensitivity.Value = optXPoints.Sensitivity;
      XpointsSensitivity.Text = optXPoints.Sensitivity.ToString();

      UpdateUI();
    }

    private void chkAutoAdjustLC_CheckedChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      fMain.AutoAdjustLightCoolness = chkAutoAdjustLC.Checked;
    }

    private void txtHdrMinLux_TextChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      UpdateUI();
    }

    private void chkAutoAdjustHDRminLux_CheckedChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      optMarkers.AutoAdjustHDRminLux = chkAutoAdjustHDRminLux.Checked;
      if ((chkAutoAdjustHDRminLux.Checked))
      {
        txtHdrMinLux.Enabled = false;
      }
      else
      {
        double HdrMinLux = 0;
        double.TryParse(txtHdrMinLux.Text, out HdrMinLux);
        txtHdrMinLux.Enabled = true;
        optCameras.HdrMinLux = (int)Math.Round(HdrMinLux, 1);
      }
      fMain.UpdatePP();
      //persist
    }


    private void radioCoolness_CheckedChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      if (ignore)
      {
        return;
      }

      RadioButton rb = (RadioButton)sender;
      if ((!rb.Checked))
      {
        return;
      }
      ignore = true;
      int index = Convert.ToInt32(rb.Tag);
      LightType lightType = (LightType)index;

      tbLightCoolness.Text = MT.GetLightCoolnessValue(lightType).ToString();
      //The indexes match the enum
      UpdateUI();
      ignore = false;

    }
    private void TrackBarLightCoolness_Scroll(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      if (ignore)
      {
        return;
      }
      ignore = true;
      tbLightCoolness.Text = ((TrackBarLightCoolness.Maximum + TrackBarLightCoolness.Minimum - TrackBarLightCoolness.Value) / 100.0).ToString();
      UpdateUI();
      ignore = false;
    }

    private void TrackBarFrameRate_Scroll(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      TxtFrameRate.Text = TrackBarFrameRate.Value.ToString();
      fMain.FrameRatePercentage = int.Parse(TxtFrameRate.Text);
    }

    private void TxtFrameRate_TextChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      fMain.FrameRatePercentage = int.Parse(TxtFrameRate.Text);
      TrackBarFrameRate.Value = (int)fMain.FrameRatePercentage;
      TxtFrameRate.Text = TrackBarFrameRate.Value.ToString();
      //if the textbox value was set out of boundary
    }

    private void chkAutoAdjustLCOR_CheckedChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      fMain.AutoAdjustLightCoolness = chkAutoAdjustLCOR.Checked;
    }

    private void radioCoolnessOR_CheckedChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      if (ignore)
      {
        return;
      }

      RadioButton rb = (RadioButton)sender;
      if ((!rb.Checked))
      {
        return;
      }
      ignore = true;
      int index = Convert.ToInt32(rb.Tag);
      LightType lightType = (LightType)index;

      tbLightCoolnessOR.Text = MT.GetLightCoolnessValue(lightType).ToString();
      //The indexes match the enum
      UpdateUI();
      ignore = false;
    }

    private void TrackBarLightCoolnessOR_Scroll(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      if (ignore)
      {
        return;
      }
      ignore = true;
      tbLightCoolnessOR.Text = ((TrackBarLightCoolnessOR.Maximum + TrackBarLightCoolnessOR.Minimum - TrackBarLightCoolnessOR.Value) / 100.0).ToString();
      UpdateUI();
      ignore = false;
    }



    private void btnLoadCoolness_Click(System.Object sender, System.EventArgs e)
    {
      int Index = Convert.ToInt32(((Control)sender).Tag);
      string FilePath = null;

      //Get the path of the file to load
      OpenFileDialog1.Filter = "Coolness Calibration Files|*.mtcool";
      OpenFileDialog1.Title = "Load custom coolness file";
      OpenFileDialog1.InitialDirectory = fMain.PP.RetrieveString("LastFilesDir", "");
      if (string.IsNullOrEmpty(OpenFileDialog1.InitialDirectory))
      {
        OpenFileDialog1.InitialDirectory = Application.StartupPath;
      }
      if (OpenFileDialog1.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
      {
        return;
      }

      FilePath = OpenFileDialog1.FileName;
      if (Path.GetExtension(FilePath).ToUpper() == ".MTCOOL")
      {
        optCameras.LoadLightCoolnessCoefficients(FilePath);
        bool loaded = optCameras.CustomCoolnessLoaded;
        if (loaded)
        {
          fMain.PP.SaveString("LastFilesDir", Path.GetDirectoryName(FilePath));
          fMain.PP.SaveString("LastFilePath", FilePath);
          this.Text = FilePath;
          MessageBox.Show("Custom coolness loaded successfully", "Custom Coolness ...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
          MessageBox.Show("Could not load", "Error ... ", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }

    }

    private void nudJitterFilterHistoryLength_ValueChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      UpdateUI();
    }

    private void TrackBarWarmupSensitivity_Scroll(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      TextWarmupSensitivity.Text = TrackBarWarmupSensitivity.Value.ToString();
      fMain.WarmupSensitivity = int.Parse(TextWarmupSensitivity.Text);

    }

    private void TextWarmupSensitivity_TextChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      fMain.WarmupSensitivity = int.Parse(TextWarmupSensitivity.Text);
      TrackBarWarmupSensitivity.Value = (int)fMain.WarmupSensitivity;
      TextWarmupSensitivity.Text = TrackBarWarmupSensitivity.Value.ToString();
      //if the textbox value was set out of boundary

    }

    private void txtAngularJitterFilterCoefficient_TextChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      UpdateUI();
    }

    private void tbStrobSignalDuration_TextChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      UpdateUI();
    }

    private void tbStrobSignalDelay_TextChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      UpdateUI();
    }

    private void TrackBarMisalignmentSensitivity_Scroll(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      MisalignmentSensitivity.Text = TrackBarMisalignmentSensitivity.Value.ToString();
      optXPoints.MisalignmentSensitivity = int.Parse(MisalignmentSensitivity.Text);
      UpdateUI();
    }

    private void MisalignmentSensitivity_TextChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      optXPoints.MisalignmentSensitivity = int.Parse(MisalignmentSensitivity.Text);
      TrackBarMisalignmentSensitivity.Value = optXPoints.MisalignmentSensitivity;
      MisalignmentSensitivity.Text = optXPoints.MisalignmentSensitivity.ToString();

      UpdateUI();
    }


    private void chkKalmanFilter_CheckedChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      optMarkers.KalmanFilterEnabled = chkKalmanFilter.Checked;
      if (optMarkers.KalmanFilterEnabled)
      {
        //txtJitterFilterCoefficient.Enabled = True
        //txtAngularJitterFilterCoefficient.Enabled = True
      }
      else
      {
        //txtJitterFilterCoefficient.Enabled = False
        //txtAngularJitterFilterCoefficient.Enabled = False
      }
      //fMain.UpdatePP() 'persistance
    }

    private void tbGammaCorrectionValue_TextChanged(System.Object sender, System.EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      UpdateUI();
    }

    private void txtHdrMaxLux_TextChanged(object sender, EventArgs e)
    {

    }

    private void chkHdrShortCycle_CheckedChanged(object sender, EventArgs e)
    {
      if (!initialized)
      {
        return;
      }
      fMain.CurrCamera.HdrShortCycleEnabled = chkHdrShortCycle.Checked;
      fMain.UpdatePP();
    }

    private void cbMiddleSensorEnable_CheckedChanged(object sender, EventArgs e)
    {
        fMain.CurrCamera.MiddleSensorEnabled = cbMiddleSensorEnable.Checked;
    }

    private void chkHdrShortCycleLockedMarkers_CheckedChanged(object sender, EventArgs e)
    {
      if (!initialized)
      {
        return;
      }

      optMarkers.AutoAdjustShortCycleHdrExposureLockedMarkers = chkHdrShortCycleLockedMarkers.Checked;
      string MarkerName1 = tbLockedMarkerName1.Text;
      string MarkerName2 = tbLockedMarkerName2.Text;
      optMarkers.AutoAdjustHdrExposureLockMarkersNamesSet(MarkerName1, MarkerName2);

      // code snippet to use hdr mode with exposures locked to specific markers.
      //markers.AutoAdjustShortCycleHdrExposureLockedMarkers = true;
      //markers.AutoAdjustHdrExposureLockMarkersNamesSet(MarkerName1, MarkerName2);
      //markers.LockedMarkersExposuresRatioThreshold = 5;
      /*
      // More API
      bool islockedmodeenabled = markers.AutoAdjustShortCycleHdrExposureLockedMarkers;
      string name1= "";
      string name2 = "";
      markers.AutoAdjustHdrExposureLockMarkersNamesGet(out name1, out name2);
      double MinExpVal, MaxExpVal;
      CurrCamera.MinMaxAllowedExposureForShortCycleHdrModeGet(out MinExpVal, out MaxExpVal);
      MinExpVal = 0.1;
      MaxExpVal = 40;
      CurrCamera.MinMaxAllowedExposureForShortCycleHdrModeSet(MinExpVal, MaxExpVal);    
      int LockedFramesCount;
      LockedFramesCount = CurrCamera.LockedMarkersNotDetectedFramesThreshold;
      CurrCamera.LockedMarkersNotDetectedFramesThreshold = 2;
      */
      
      fMain.UpdatePP();
    }


  }

}