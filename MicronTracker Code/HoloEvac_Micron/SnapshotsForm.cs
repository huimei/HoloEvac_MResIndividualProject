using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using MTInterfaceDotNet;

namespace HoloEvac_Micron
{
  public partial class SnapshotsForm : Form
  {
    private MainForm fMain;
    public SnapshotsForm(MainForm mainForm)
    {
      fMain = mainForm;
      InitializeComponent();
    }


    public List<Byte[]> LSnaps = new List<Byte[]>(); //left images
    public List<Byte[]> RSnaps = new List<Byte[]>(); //right images
    public List<Byte[]> MSnaps = new List<Byte[]>(); //Middle images
    //Various attributes that may affect measured poses values are captured,
    //stored and restored with each snapshot
    public List<double> gainS = new List<double>(); //exposure settings for each snap
    public List<double> shutterS = new List<double>();
    public List<double> DegreesC = new List<double>(); //camera temperature
    public List<double> LightCoolness = new List<double>();
    public List<Int16> HdrEnabledSnap = new List<Int16>();
    public List<Int32> HdrCycleLengthSnap = new List<Int32>();
    public List<Int32> HdrFrameIndexInCycleSnap = new List<Int32>();
    public List<double> HdrLightCoolnessSnap = new List<double>();

    public string SSnapFilePath;
    public int CurrSnap;
    private int replay_counter;
    private bool SnapsTakenAfterSave;
    private const Int32 SnapFileCode = 9867;
    private const Int32 SnapFileVersion = 5;

    private class SnapsHeaderType
    {
      public Int32 FileCode;
      public Int32 FileVersion;
      public Int16 IWidth;
      public Int16 IHeight;
      public Int16 ICount; //# of images in the file
      public Int32 CamSerialNum;
      public void ReadFromBinaryReader(BinaryReader reader)
      {
        FileCode = reader.ReadInt32();
        FileVersion = reader.ReadInt32();
        IWidth = reader.ReadInt16();
        IHeight = reader.ReadInt16();
        ICount = reader.ReadInt16();
        CamSerialNum = reader.ReadInt32();
      }
      public void WriteToBinaryWriter(BinaryWriter writer)
      {
        writer.Write(FileCode);
        writer.Write(FileVersion);
        writer.Write(IWidth);
        writer.Write(IHeight);
        writer.Write(ICount);
        writer.Write(CamSerialNum);
      }
    }

    private SnapsHeaderType Header = new SnapsHeaderType();

    public long SnappingCamSerialNum
    {
      get
      {
        return Header.CamSerialNum;
      }
    }

    private void btnSnap_Click(object sender, EventArgs e)
    {
      SnapFrame();
    }

    public void SnapFrame()
    {
      bool tempIgnore = ignore;
      ignore = true;
      SnapStereoPair();
      SnapsTakenAfterSave = true;
      CurrSnap = LSnaps.Count - 1;
      UpdateUI();
      ignore = tempIgnore;
    }

    private void btnLoadFile_Click(object sender, EventArgs e)
    {
      int Index = Convert.ToInt32(((Control)sender).Tag);
      string FilePath = null;


      //Get the path of the file to load
      openFileDialog1.Filter = "StereoSnap Files|*.ssnap";
      openFileDialog1.Title = "Load snapshots file";
      openFileDialog1.InitialDirectory = fMain.PP.RetrieveString("LastFilesDir", "");
      if (openFileDialog1.InitialDirectory == "")
        openFileDialog1.InitialDirectory = Application.StartupPath;
      if (openFileDialog1.ShowDialog(this) != DialogResult.OK)
        return;

      FilePath = openFileDialog1.FileName;
      if (Path.GetExtension(FilePath).ToUpper() == ".SSNAP")
      {
        if (LoadSSnapFile(FilePath, Index == 1, true))
        {
          fMain.PP.SaveString("LastFilesDir", Path.GetDirectoryName(FilePath));
          fMain.PP.SaveString("LastFilePath", FilePath);
          this.Text = FilePath;
          ShowFrame(0);
        }
        else
        {
          MessageBox.Show("Could not load");
        }
      }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      ClearCollections();
      SSnapFilePath = "";
      UpdateUI();
    }

    public void ClearCollections()
    {
      LSnaps = new List<Byte[]>();
      RSnaps = new List<Byte[]>();
      MSnaps = new List<Byte[]>();
      gainS = new List<double>();
      shutterS = new List<double>();
      DegreesC = new List<double>();
      LightCoolness = new List<double>();
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      if (LSnaps.Count == 0) //nothing to save
      {
        return;
      }
      //Get the path of the file to load
      saveFileDialog1.Filter = "StereoSnap Files (*.ssnap)|*.ssnap";
      saveFileDialog1.Title = "Save Stereo Snaps File";
      saveFileDialog1.InitialDirectory = fMain.PP.RetrieveString("LastFilesDir", Path.GetDirectoryName(Application.StartupPath));
      saveFileDialog1.DefaultExt = "ssnap";
      saveFileDialog1.FileName = "Snap File";
      if (saveFileDialog1.ShowDialog(this) == DialogResult.OK)
      {
        SaveSnaps(saveFileDialog1.FileName);
      }
    }

    private void Form_Load(object sender, EventArgs e)
    {
      UpdateUI();
    }
    private void SnapshotsForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (SnapsTakenAfterSave)
      {
        if (MessageBox.Show("Some snapshots were taken, but not saved. Exit anyhow?", string.Empty, MessageBoxButtons.YesNo) == DialogResult.No)
        {
          e.Cancel = true;
        }
        fMain.CaptureEnabled = true;
      }
    }


    private void scrImgNum_ValueChanged(object sender, EventArgs e)
    {
      if (ignore)
        return;
      ignore = true;
      ShowFrame(scrImgNum.Value);
      ignore = false;
    }


    private bool ignore = false;
    private void tbFrameNum_TextChanged(object sender, EventArgs e)
    {
      if (ignore)
        return;

      ignore = true;
      int frameNum;
      if (int.TryParse(tbFrameNum.Text, out frameNum))
      {
        ShowFrame(frameNum - 1);
      }
      ignore = false;
    }

    public void SnapStereoPair()
    {
      Byte[] Limage = null;
      Byte[] Rimage = null;
      Byte[] Mimage = null;
      bool PrevHistEq = false;
      //ensure that the image being snapped are the original ones (not histogram-equalized)
      PrevHistEq = MT.Cameras.HistogramEqualizeImages;
      MT.Cameras.HistogramEqualizeImages = false;
      if (fMain.CurrCamera.sensorsnum == 3)
      {
        fMain.CurrCamera.GetImagesArray3(out Limage, out Rimage, out Mimage); //get the images
      }
      else
      {
        fMain.CurrCamera.GetImagesArray(out Limage, out Rimage); //get the images
      }

      MT.Cameras.HistogramEqualizeImages = PrevHistEq; //restore

      LSnaps.Add(Limage);
      RSnaps.Add(Rimage);
      MSnaps.Add(Mimage);
      gainS.Add(fMain.CurrCamera.GainF);
      shutterS.Add(fMain.CurrCamera.ShutterMsecs);
      DegreesC.Add(fMain.CurrCamera.LastFrameTemperature);
      LightCoolness.Add(fMain.CurrCamera.LightCoolness);

      if (fMain.CurrCamera.HdrEnabled)
      {
        HdrEnabledSnap.Add(1);
        HdrFrameIndexInCycleSnap.Add(fMain.CurrCamera.HdrFrameIndexInCycle);
        HdrCycleLengthSnap.Add(fMain.CurrCamera.HdrCycleLength);
        HdrLightCoolnessSnap.Add(fMain.CurrCamera.HDRLightCoolnessOR);
      }
      else
      {
        HdrEnabledSnap.Add(0);
        HdrFrameIndexInCycleSnap.Add(0);
        HdrCycleLengthSnap.Add(0);
        HdrLightCoolnessSnap.Add(0);
      }

    }

    public bool LoadSSnapFile(string FilePath, bool Append, bool MustMatchCameraResolution)
    {
      bool tempLoadSSnapFile = false;
      if (!(Utils.ReadyForReading(FilePath)))
        return false;
      SSnapFilePath = FilePath;
      if (fMain.CurrCamera == null)
      {
        MessageBox.Show("No camera");
        return false;
      }

      Byte[] Img = null;
      int i = 0;
      //Dim ExposureUnavailable As Boolean, DegCUnavailable As Boolean, AnOldSsnap As Boolean
      FileStream fs = File.Open(FilePath, FileMode.Open, FileAccess.Read);
      BinaryReader reader = new BinaryReader(fs);
      Header.ReadFromBinaryReader(reader);
      if (Header.FileVersion != SnapFileVersion)
      {
        MessageBox.Show(FilePath + " is version " + Header.FileVersion + ". Can properly load only version " + SnapFileVersion + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        goto CloseAndExit;
      }
      if (Header.ICount <= 0)
      {
        goto CloseAndExit;
      }
      if (Header.CamSerialNum != fMain.CurrCamera.SerialNumber)
      {
        MessageBox.Show("Snapshots being loaded were taken with camera # " + Header.CamSerialNum + ", while current camera # is " + fMain.CurrCamera.SerialNumber + "." + "\n" + "Measurements from these snapshots are impossible or incorrect.", "Note...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
      if (MustMatchCameraResolution && (Header.IWidth != fMain.CurrCamera.Xres || Header.IHeight != fMain.CurrCamera.Yres))
      {
        MessageBox.Show("Resolution mismatch. Cannot load snapshots.", "Error...", MessageBoxButtons.OK, MessageBoxIcon.Error);
        goto CloseAndExit;
      }
      int imgWidth = (int)Header.IWidth;
      int imgheight = (int)Header.IHeight;
      int numImgBytes = imgWidth * imgheight;
      if (!Append)
      {
        ClearCollections();
      }
      double Gain = 0;
      double Shutter = 0;
      double DegC = 0;
      double LightC = 0;
      Int16 HdrEnabled = 0;
      Int32 HdrCycleLength = 0;
      Int32 HdrFrameIndexInCycle = 0;
      double HdrLightC = 0;

      try
      {
        for (i = 0; i < Header.ICount; i++)
        {
          Gain = reader.ReadDouble();
          Shutter = reader.ReadDouble();
          DegC = reader.ReadDouble();
          LightC = reader.ReadDouble();

          if (Header.FileVersion > 4)
          {
            HdrEnabled = reader.ReadInt16();
            HdrCycleLength = reader.ReadInt32();
            HdrFrameIndexInCycle = reader.ReadInt32();
            HdrLightC = reader.ReadDouble();
          }
          else
          {
            HdrEnabled = 0;
          }

          gainS.Add(Gain);
          shutterS.Add(Shutter);
          DegreesC.Add(DegC);
          LightCoolness.Add(LightC);
          HdrEnabledSnap.Add(HdrEnabled);
          HdrEnabledSnap.Add(HdrEnabled);
          if (Header.FileVersion > 4)
          {
            HdrCycleLengthSnap.Add(HdrCycleLength);
            HdrFrameIndexInCycleSnap.Add(HdrFrameIndexInCycle);
            HdrLightCoolnessSnap.Add(HdrLightC);
          }

          Img = reader.ReadBytes(numImgBytes);
          LSnaps.Add(Img);
          if (fMain.CurrCamera.sensorsnum == 3)
          {
            Img = reader.ReadBytes(numImgBytes);
            MSnaps.Add(Img);
          }
          Img = reader.ReadBytes(numImgBytes);
          RSnaps.Add(Img);
        }
        if (HdrEnabled == 1)
        {
          fMain.CurrCamera.HdrEnabled = true;
          fMain.CurrCamera.HdrCycleLength = HdrCycleLengthSnap[0];
          fMain.CurrCamera.HdrFrameIndexInCycle = HdrFrameIndexInCycleSnap[0];
          fMain.CurrCamera.HDRLightCoolnessOR = HdrLightCoolnessSnap[0];
          fMain.mnuHdrMode.Checked = true;
          fMain.HdrEnabled = true;
          fMain.mnuHistEqualize.Checked = true;
          fMain.HistogramEqualizeImages = true;
        }

        CurrSnap = 0;
        UpdateUI();
        ShowFrame(0);
        tempLoadSSnapFile = true;
      }
      catch
      {
        goto CloseAndExit;
      }
    CloseAndExit:
      reader.Close();
      return tempLoadSSnapFile;
    }

    public void SaveSnaps(string FilePath)
    {
      if (!(Utils.ReadyForWriting(FilePath)))
      {
        return;
      }

      byte[] Img = null;
      int i = 0;
      Camera Cam = null;
      double Gain = 0;
      double Shutter = 0;
      double DegC = 0;
      double LightC = 0;
      Int16 HdrEnabled = 0;
      Int32 HdrCycleLength = 0;
      Int32 HdrFrameIndexInCycle = 0;
      double HdrLightC = 0;

      FileStream fs = File.Open(FilePath, FileMode.Create, FileAccess.Write);
      using (BinaryWriter writer = new BinaryWriter(fs))
      {
        Cam = fMain.CurrCamera;
        Header.FileCode = SnapFileCode;
        Header.FileVersion = SnapFileVersion;
        Header.IWidth = (Int16)Cam.Xres;
        Header.IHeight = (Int16)Cam.Yres;
        Header.CamSerialNum = Cam.SerialNumber;
        Header.ICount = (Int16)LSnaps.Count;
        Header.WriteToBinaryWriter(writer);
        for (i = 0; i < LSnaps.Count; i++)
        {
          Gain = gainS[i];
          Shutter = shutterS[i];
          DegC = DegreesC[i];
          LightC = LightCoolness[i];
          HdrEnabled = HdrEnabledSnap[i];
          HdrCycleLength = HdrCycleLengthSnap[i];
          HdrFrameIndexInCycle = HdrFrameIndexInCycleSnap[i];
          HdrLightC = HdrLightCoolnessSnap[i];

          writer.Write(Gain);
          writer.Write(Shutter);
          writer.Write(DegC);
          writer.Write(LightC);
          if (SnapFileVersion > 4)
          {
            writer.Write(HdrEnabled);
            writer.Write(HdrCycleLength);
            writer.Write(HdrFrameIndexInCycle);
            writer.Write(HdrLightC);
          }

          Img = LSnaps[i];
          writer.Write(Img);
          if (fMain.CurrCamera.sensorsnum == 3)
          {
            Img = MSnaps[i];
            writer.Write(Img);
          }
          Img = RSnaps[i];
          writer.Write(Img);
        }
      }
      SnapsTakenAfterSave = false;
    }

    public double ShowFrame(int num)
    {
      return ShowFrame(num, -1);
    }

    private bool ShowFrame_inRecursion;

    public double ShowFrame(int num, int SideI)
    {
      double tempShowFrame = 0;
      try
      {
        //returns the "pure" processing time of the frame, in seconds (or 0, if there was no processing)
        if (ShowFrame_inRecursion)
          return 0;

        num = Utils.Constrain(scrImgNum.Minimum, num, scrImgNum.Maximum);
        if (LSnaps.Count < 1)
          return 0;
        if (LSnaps.Count < num + 1)
          return 0;
        ShowFrame_inRecursion = true;
        fMain.CaptureEnabled = false; //stop capturing while showing snapshots
        try
        {
          CurrSnap = (int)num;
          if (fMain.CurrCamera.sensorsnum == 3)
          {
            fMain.CurrCamera.SetImages3(LSnaps[num], RSnaps[num], MSnaps[num]);
          }
          else
          {
            fMain.CurrCamera.SetImages(LSnaps[num], RSnaps[num]);
          }

          fMain.CurrCamera.GainF = gainS[num];
          fMain.CurrCamera.ShutterMsecs = shutterS[num];
          fMain.CurrCamera.LastFrameTemperature = DegreesC[num];
          fMain.CurrCamera.LightCoolness = LightCoolness[num];
          Int16 HdrEnabled = 0;
          if ((HdrEnabledSnap[num] == 1) && (chkDisableHdr.Checked == false))
            HdrEnabled = 1;

          if (HdrEnabled == 1)
          {
            fMain.CurrCamera.HdrEnabled = true;
            fMain.CurrCamera.HdrCycleLength = (int)HdrCycleLengthSnap[num];
            fMain.CurrCamera.HdrFrameIndexInCycle = (int)HdrFrameIndexInCycleSnap[num];
            fMain.CurrCamera.HDRLightCoolnessOR = (double)HdrLightCoolnessSnap[num];
          }

        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
          ShowFrame_inRecursion = false;
          return 0;
        }
        if (fMain.MarkersProcessingEnabled)
        {
          tempShowFrame = fMain.ProcessCurrFrame();
        }
        else //display only
        {
          fMain.DisplayImages(true, SideI);
        }
        UpdateUI();
        ShowFrame_inRecursion = false;
      }

      catch
      {
      }
      return tempShowFrame;
    }

    public void UpdateUI()
    {
      if (LSnaps.Count == 0)
      {
        tbFrameNum.Text = "-";
        lblCount.Text = "no snaps";
      }
      else
      {
        scrImgNum.Maximum = LSnaps.Count - 1;
        tbFrameNum.Text = (CurrSnap + 1).ToString();
        scrImgNum.Value = CurrSnap;
        lblCount.Text = "of " + (scrImgNum.Maximum + 1).ToString();
      }
      scrImgNum.Enabled = (LSnaps.Count > 0);
      btnSave.Enabled = (LSnaps.Count > 0);
      btnLoadAppend.Enabled = (LSnaps.Count > 0);
      btnTimeIt.Enabled = (LSnaps.Count > 0);
      tbFrameNum.Enabled = (LSnaps.Count > 0);
      if (SSnapFilePath == "")
      {
        lblSnapPath.Text = "";
      }
      else
      {
        lblSnapPath.Text = Path.GetDirectoryName(SSnapFilePath) + System.Environment.NewLine + Path.GetFileName(SSnapFilePath);
      }
      toolTip1.SetToolTip(lblSnapPath, lblSnapPath.Text); //so it can be fully seen
    }

    public void ReplaceSnap(int i, byte[] Limg, byte[] Rimg, byte[] Mimg)
    {
      if (LSnaps.Count < 1)
      {
        return;
      }
      i = Utils.Constrain(0, i, LSnaps.Count - 1);
      LSnaps.RemoveAt(i);
      MSnaps.RemoveAt(i);
      RSnaps.RemoveAt(i);
      if (CurrSnap >= LSnaps.Count)
      {
        LSnaps.Add(Limg);
        MSnaps.Add(Mimg);
        RSnaps.Add(Rimg);
      }
      else
      {
        LSnaps.Insert(i, Limg);
        MSnaps.Insert(i, Mimg);
        RSnaps.Insert(i, Rimg);
      }
    }

    private void btnTimeIt_Click(object sender, EventArgs e)
    {
      //Process all the images in the snap sequence and shows the average processing time/frame
      if (LSnaps.Count < 1) //nothing to time
      {
        return;
      }
      int i = 0;
      double AccProcessingSecs = 0;

      for (i = scrImgNum.Minimum; i <= scrImgNum.Maximum; i++)
      {
        AccProcessingSecs = AccProcessingSecs + ShowFrame(i);
      }
      //Average timing per snap pair
      MessageBox.Show((1000 * AccProcessingSecs / (double)LSnaps.Count).ToString("0.00") + " ms / frame", string.Empty, MessageBoxButtons.OK);
    }


    private void btnReprocess_Click(object sender, EventArgs e)
    {
      ShowFrame(scrImgNum.Value);
    }

    private void button1_Click(object sender, EventArgs e)
    {
      int PrevFramesGrabbed;
      if (fMain.HdrEnabled == true)
      {	//wait for the first frame in the cycle
        do
        {
          fMain.mmTimer_Tick(sender, e);
        } while (fMain.CurrCamera.HdrFrameIndexInCycle != 0);
      }
      for (int taken = 0; taken < 16; taken++)
      {
        SnapFrame();
        PrevFramesGrabbed = fMain.CurrCamera.FramesGrabbed;
        do
        {
          fMain.mmTimer_Tick(sender, e);
        } while (PrevFramesGrabbed == fMain.CurrCamera.FramesGrabbed);
      }
    }

    private void Play_Timer_Tick(object sender, EventArgs e)
    {

      ShowFrame(replay_counter);
      replay_counter = replay_counter + 1;
      if (replay_counter > LSnaps.Count)
      {
        Play_CheckBox.Checked = false;
        Play_Timer.Enabled = false;
        replay_counter = 0;
      }

    }

    private void Play_CheckBox_CheckedChanged(object sender, EventArgs e)
    {
      Play_Timer.Enabled = Play_CheckBox.Checked;
    }


  }
}