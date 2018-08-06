using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MTInterfaceDotNet;
using System.Drawing.Drawing2D;

namespace HoloEvac_Micron
{
  public partial class TraceForm : Form
  {
    private MainForm fMain;
    public TraceForm(MainForm mainForm)
    {
      fMain = mainForm;
      InitializeComponent();
    }



    private const long MaxPoints = 2000;
    private double[,] Trace = new double[3, MaxPoints];
    private long PointsCount;

    private const int UnknownX = int.MaxValue;

    public Marker ReferenceMarker; //if nothing, then it is in the camera's coordinates
    public Marker TracerMarker; //if nothing, then it is in the camera's coordinates

    private bool[] AutoUpdateMarkerName = new bool[2];

    private Color LineColor;
    private bool MarkersLocked;
    private bool SavedMagnifiedToolTip;


    private void chkShowMagnified_CheckChanged(object sender, EventArgs e)
    {
      if (chkShowMagnified.Checked)
      {
        fMain.IsShowingMagnifiedToolTip = false; //cannot have at the same time
      }
      else
      {
        fMain.IsShowingMagnifiedToolTip = SavedMagnifiedToolTip; //restore to prev status
      }
    }

    private void chkTrace_CheckChanged(object sender, EventArgs e)
    {
      MarkDiscontinuity();
    }

    private void Form_Activate(object sender, EventArgs e)
    {
      SavedMagnifiedToolTip = fMain.IsShowingMagnifiedToolTip;
    }

    private void Form_Load(object sender, EventArgs e)
    {
      AutoUpdateMarkerName[0] = true;
      AutoUpdateMarkerName[1] = true;
      tmrCheckFOM.Enabled = true;
      LineColor = Color.FromArgb(255, 150, 150); //pink
      TracerMarker = null; //forget previous settings
      ReferenceMarker = null;
      PointsCount = 0;
      UpdateUI();
    }

    private void TraceForm_FormClosing(object sender, FormClosingEventArgs e)
    {
      tmrCheckFOM.Enabled = false;
      fMain.IsShowingMagnifiedToolTip = SavedMagnifiedToolTip; //restore
    }

    private void tbMarkerName_TextChanged(object sender, EventArgs e)
    {
      TextBox tb = (TextBox)sender;
      int Index = Convert.ToInt32(tb.Tag);
      if (tb == ActiveControl) //manual modification.
      {
        AutoUpdateMarkerName[Index] = false; //no more auto updates
      }
      TracerMarker = FindMarkerByName(tbTracerMarker.Text);
      ReferenceMarker = FindMarkerByName(tbRefMarker.Text);
    }


    private long PrevFrameCounter;

    private void tmrCheckFOM_Tick(object sender, EventArgs e)
    {
      int i = 0;
      Marker M = null;
      double[] Svec = null;
      if (!Visible)
      {
        return;
      }

      if (Control.MouseButtons == MouseButtons.Right)
      {
        btnReset_Click(btnReset, EventArgs.Empty);
      }
      if (PrevFrameCounter == fMain.CurrCamera.FramesGrabbed) //ignore: same frame as before
      {
        return;
      }
      PrevFrameCounter = fMain.CurrCamera.FramesGrabbed; //update for next time

      IList<Marker> IMarkers = MT.Markers.GetIdentifiedMarkers(null);
      if (!MarkersLocked && AutoUpdateMarkerName[0]) //try to find trace marker automatically
      {
        //Find a marker with a calibrated tooltip
        for (i = 0; i < IMarkers.Count; i++)
        {
          M = IMarkers[i];
          Svec = M.Tooltip2MarkerXf.ShiftVector;
          if (Svec[0] != 0 || Svec[1] != 0 || Svec[2] != 0) //non-null transform -tooltip calibrated
          {
            if (M.Name.ToUpper() != tbTracerMarker.Text.ToUpper())
            {
              TracerMarker = M;
              tbTracerMarker.Text = M.Name;
              UpdateUI();
              if (chkTrace.Enabled)
              {
                chkTrace.Focus();
              }
              break;
            }
          }
        }
      }
      if (!MarkersLocked & AutoUpdateMarkerName[1])
      {
        //Find a marker with more than one facet identified and without a tooltip
        for (i = 0; i < IMarkers.Count; i++)
        {
          M = IMarkers[i];
          Svec = M.Tooltip2MarkerXf.ShiftVector;
          if (Svec[0] == 0 || Svec[1] == 0 || Svec[2] == 0)
          {
            if (M.GetIdentifiedFacets(null, false).Count > 1)
            {
              tbRefMarker.Text = M.Name;
              ReferenceMarker = M;
              break;
            }
          }
        }
      }

      Xform3D Xf = null;
      Xform3D Ref2Cam = null;
      Xform3D ToolTip2Cam = null;
      double[] NewPos = null;
      if ((chkTrace.Checked || Control.MouseButtons == MouseButtons.Left) && TracerMarker != null)
      {
        //Is the tracer marker tracked?
        Xf = TracerMarker.GetMarker2CameraXf(fMain.CurrCamera);
        if (Xf != null)
        {
          //ToolTip2Cam = tooltip->tracer->camera xform
          ToolTip2Cam = TracerMarker.Tooltip2MarkerXf.Concatenate(Xf);
          if (ReferenceMarker != null)
          {
            if (ReferenceMarker.WasIdentified(null)) //can trace in the reference marker's coordinates
            {
              Ref2Cam = ReferenceMarker.GetMarker2CameraXf(fMain.CurrCamera); //ref->camera
              if (Ref2Cam != null & ToolTip2Cam != null)
              {
                Xf = ToolTip2Cam.Concatenate(Ref2Cam.Inverse()); //tracer->camera->ref
                NewPos = Xf.ShiftVector;
              }
            }
          }
          else //in camera coordinates
          {
            NewPos = ToolTip2Cam.ShiftVector;
          }
        }
        else //not tracked. A discontinuity
        {
          MarkDiscontinuity();
        }
      }
      if (NewPos != null) //a new point was captured
      {
        //Add point only if moved at least 1mm since the previous point
        double DistanceMoved = 0;
        DistanceMoved = 1000; //init
        if (PointsCount > 0)
        {
          if (Trace[0, PointsCount - 1] != UnknownX)
          {
            DistanceMoved = Utils.Distance(NewPos, new double[] { Trace[0, PointsCount - 1], Trace[1, PointsCount - 1], Trace[2, PointsCount - 1] });
          }
        }
        if (DistanceMoved > 1)
        {
          Trace[0, PointsCount] = NewPos[0];
          Trace[1, PointsCount] = NewPos[1];
          Trace[2, PointsCount] = NewPos[2];
          PointsCount = PointsCount + 1;
        }
      }
      UpdateUI();
    }

    public void ShowTraceOverlay()
    {
      //To be called by the main form to add an overlay on the images
      int i = 0;
      int SideI = 0;
      double[] PosInCam = null;
      Xform3D Ref2Cam = null;
      Camera CurrCam = null;
      double[] CurrPosInImg = new double[2];
      double[] PrevPosInImg = new double[2];
      bool ShouldShowMagnified = false;
      CurrCam = fMain.CurrCamera; //convenience
      if (ReferenceMarker != null)
      {
        if (!(ReferenceMarker.WasIdentified(CurrCam))) //no reference frame
        {
          return; //do not show
        }
        else
        {
          Ref2Cam = ReferenceMarker.GetMarker2CameraXf(CurrCam); //ref->camera
        }
      }
      ShouldShowMagnified = chkShowMagnified.Checked;

      //Show in main image areas
      double[] XYBounds = new double[4]; //xmin,xmax,ymin,ymax
      List<double[]> Lines = new List<double[]>();
      DashStyle LineStyle;

      LineStyle = chkEmphasizeLine.Checked ? DashStyle.Solid : DashStyle.Dot;
      PosInCam = new double[3];
      for (SideI = 0; SideI < 2; SideI++)
      {
        PrevPosInImg[0] = UnknownX;
        XYBounds[0] = 100000; //init
        XYBounds[1] = 0;
        XYBounds[2] = 100000;
        XYBounds[3] = 0;

        for (i = 0; i < PointsCount; i++)
        {
          if (Trace[0, i] != UnknownX)
          {
            PosInCam[0] = Trace[0, i];
            PosInCam[1] = Trace[1, i];
            PosInCam[2] = Trace[2, i];
            if (ReferenceMarker != null) //transform from reference to camera space
            {
              PosInCam = Ref2Cam.XformLocation(PosInCam);
            }
            if (CurrCam.ProjectionOnImage(SideI, PosInCam, out CurrPosInImg[0], out CurrPosInImg[1]))
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
              if (PrevPosInImg[0] != UnknownX) //draw a line between prev and curr
              {
                fMain.DrawLine(PrevPosInImg[0], PrevPosInImg[1], CurrPosInImg[0], CurrPosInImg[1], LineColor, SideI, LineStyle, true, false, fMain.DisplayMirrorImage);
                Lines.Add(new double[] { PrevPosInImg[0], PrevPosInImg[1], CurrPosInImg[0], CurrPosInImg[1] });

              }
            }
            else //current position unknown
            {
              CurrPosInImg[0] = UnknownX;
            }
            PrevPosInImg[0] = CurrPosInImg[0];
            PrevPosInImg[1] = CurrPosInImg[1];
          }
          else //unknown
          {
            PrevPosInImg[0] = UnknownX;
          }
        }
        if (ShouldShowMagnified & XYBounds[1] > XYBounds[0] & XYBounds[3] > XYBounds[2])
        {
          //show magnified area, with overlaid trace
          int OutRegionHalfSide = 0;
          int InRegionHalfSide = 0;
          double ZoomF = 0;
          //PictureBox pictBox = SideI == 0 ? fMain.picturebox0 : fMain.picturebox1;
          PictureBox pictBox = SideI == 2 ? fMain.PicBox2 : SideI == 0 ? fMain.picBox0 : fMain.picBox1;
          OutRegionHalfSide = Math.Min(100, pictBox.Height / 5);
          InRegionHalfSide = (int)Math.Max(XYBounds[1] - XYBounds[0], XYBounds[3] - XYBounds[2]) / 2 + 1;
          ZoomF = Utils.Constrain(8.0, OutRegionHalfSide / (double)InRegionHalfSide, 1.5); //no sense minifying
          fMain.ShowMagnifiedRegion(SideI, (XYBounds[1] + XYBounds[0]) / 2, (XYBounds[3] + XYBounds[2]) / 2, OutRegionHalfSide, ((SideI == 0) ? 2 : 1), ZoomF, false, Lines, LineColor, LineStyle);
        }
      }

    }

    private void btnReset_Click(object sender, EventArgs e)
    {
      PointsCount = 0;
      UnlockMarkers();
      UpdateUI();
    }

    private void LockMarkers()
    {
      tbTracerMarker.Enabled = false;
      tbRefMarker.Enabled = false;
      MarkersLocked = true;
    }

    private void UnlockMarkers()
    {
      tbTracerMarker.Enabled = true;
      tbRefMarker.Enabled = true;
      MarkersLocked = false;
    }

    public void UpdateUI()
    {
      if (!chkTrace.Checked)
      {
        chkTrace.Enabled = TracerMarker != null;
      }
      if (ReferenceMarker != null)
      {
        lblInfo.Text = PointsCount + " pts in \"" + ((ReferenceMarker == null) ? "camera" : ReferenceMarker.Name) + "\" space";
      }
      else
      {
        lblInfo.Text = PointsCount + " pts in camera Space";
      }
    }

    private Marker FindMarkerByName(string MarkerName)
    {
      for (int i = 0; i < MT.Markers.Count; i++)
      {
        if (MT.Markers[i].Name.ToUpper() == MarkerName.ToUpper())
        {
          return MT.Markers[i];
        }
      }
      return null;
    }

    public void MarkDiscontinuity()
    {
      if (PointsCount > 0)
      {
        if (Trace[0, PointsCount - 1] != UnknownX)
        {
          Trace[0, PointsCount] = UnknownX;
          PointsCount = PointsCount + 1;
        }
      }
    }

  }
}