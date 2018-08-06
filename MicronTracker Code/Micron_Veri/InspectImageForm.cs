using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MicronTrackerCSDemo
{
    public partial class InspectImageForm : Form
    {
        MainForm fMain;
        public InspectImageForm(MainForm mForm)
        {
            InitializeComponent();
            fMain = mForm;
        }

        //properties
        private int m_GridHalfSide;
        private bool m_StatsEnabled;
        //Public AveragingConvergenceEpsilon As Single
        private float LastMaxAverageChange;

        private int GridSide;

        private const int SRCCOPY = 0XCC0020;


        public PictureBox SourcePic;
        public long CenterX;
        public long CenterY;
        public bool LabelsEnabled;
        public bool XPEnabled;

        private long m_HistoryLength;
        private long HistCount;
        private byte[] PixHistory;
        private float[] PixAverageHistory;
        private long LastI;

        private double Avg5x5;
        private double STD5x5;
        private double STDofAvg5x5;

        private void btnBrk_Click(object sender, EventArgs e)
        {
            double[] XPPos = new double[2];
            XPPos[0] = CenterX * fMain.DisplayScale;
            XPPos[1] = CenterY * fMain.DisplayScale;
            SetTestVals(XPPos[0]);
        }


        public long HistoryLength
        {
            get { return m_HistoryLength; }
            set
            {
                value = Math.Max(2, value);
                value = Math.Min(value, 2000);
                if (value != m_HistoryLength)
                {
                    m_HistoryLength = value;
                    tbHistLen.Text = m_HistoryLength.ToString();
                    ResetStats();
                }
            }
        }

        public int SensorSide
        {
            get
            {
                if (SourcePic == fMain.pictureBox1)
	                return 0;
                else if(SourcePic == fMain.pictureBox2)
                    return 1;
                else
                    return -1;
            }
        }

        public bool StatsEnabled
        {
            get{ return m_StatsEnabled;}
            set
            {
                if(m_StatsEnabled != value)
                {   
                    m_StatsEnabled = value;
                    chkStats.Checked = value;
                    ResetStats();
                }
            }
        }

        public bool StatsValid
        {
            get
            {
                return (HistCount >= m_HistoryLength);
            }
        }

        public float GetPixelAverage(int RelativeX, int RelativeY)
        {
            long Total = 0;
            long j = 0;
            if (HistCount == 0) //just the most recent value
            {
	            PixelAverage = PixHistory(RelativeX, RelativeY, 0);
            }
            else
            {
	            Total = 0;
	            for (int h = 0; h < HistCount; h++)
	            {
	                j = (LastI - h + m_HistoryLength) % m_HistoryLength;
	                Total = Total + PixHistory(RelativeX, RelativeY, j);
	            }
	            PixelAverage = Total / HistCount;
            }
        }

        public double PixelAverageCenter5x5
        {
            get{return Avg5x5;}
        }

        public float PixelAverageAll
        {
            get
            {
                long Total = 0;
                long Count = 0;
                long X = 0;
                long Y = 0;
                long h = 0;
                long j = 0;
                long tempFor1 = m_GridHalfSide;
                for (Y = -tempFor1; Y <= tempFor1; Y++)
                {
                    long tempFor2 = m_GridHalfSide;
                    for (X = -tempFor2; X <= tempFor2; X++)
                    {
	                    if (HistCount == 0) //just the most recent value
	                    {
	                        Total = Total + PixHistory(X, Y, 0);
	                        Count = Count + 1;
	                    }
	                    else
	                    {
	                        long tempFor3 = HistCount;
	                        for (h = 0; h < tempFor3; h++)
	                        {
		                        j = (LastI - h + m_HistoryLength) % m_HistoryLength;
		                        Total = Total + PixHistory(X, Y, j);
		                        Count = Count + 1;
	                        }
	                    }
                    }
                }
                return (double)(Total) / Count;
            }
        }

        public double PixelSTDofAveragesCenter5x5
        {
            get{  return STDofAvg5x5;}
        }

        
        public float GetPixelSTD(int RelativeX, int RelativeY)
        {
            long Total = 0;
            long j = 0;
            double Avg = 0;
            double Diff = 0;
            if (HistCount == 0) //just the most recent value
            {
	            PixelSTD = 0;
            }
            else //root mean square difference
            {
	            Avg = PixelAverage(RelativeX, RelativeY);
	            Total = 0;
	            for (long h = 0; h < HistCount; h++)
	            {
	                j = (LastI - h + m_HistoryLength) % m_HistoryLength;
	                Diff = PixHistory(RelativeX, RelativeY, j) - Avg;
	                Total = Total + Diff * Diff;
	            }
	            return Math.Sqrt(Total / HistCount);
            }
        }


       
        public float GetSTDofAverage(int RelativeX, int RelativeY)
        {
            double Total = 0;
            long h = 0;
            long j = 0;
            double Avg = 0;
            double Diff = 0;
            if (HistCount == 0) //just the most recent value
            {
	            STDofAverage = 0;
            }
            else //root mean square difference
            {
	            //find the average of averages
	            Total = 0;
    
	            for (h = 0; h < HistCount; h++)
	            {
	                j = (LastI - h + m_HistoryLength) % m_HistoryLength;
	                Total = Total + PixAverageHistory(RelativeX, RelativeY, j);
	            }
	            Avg = Total / HistCount;
	            Total = 0;
                
	            for (h = 0; h < HistCount; h++)
	            {
	                j = (LastI - h + m_HistoryLength) % m_HistoryLength;
	                Diff = PixAverageHistory(RelativeX, RelativeY, j) - Avg;
	                Total = Total + Diff * Diff;
	            }
	            return Math.Sqrt(Total / HistCount);
            }
        }

        public int GridHalfSide
        {
            get{return m_GridHalfSide;}
            set
            {
                if (m_GridHalfSide != value)//TODO doubt
	            {
                    m_GridHalfSide = value;
                    GridSide = GridHalfSide * 2 + 1;
                    ResetStats();
                    Form_Resize();
	            }
                
               
            }
        }

        public void ResetStats()
        {
            if (m_HistoryLength >= 1)
            {
	            PixHistory = new byte[GridHalfSide + 1, GridHalfSide + 1, m_HistoryLength]; //TODO doubt
	            PixAverageHistory = new float[GridHalfSide + 1, GridHalfSide + 1, m_HistoryLength]; //TODO doubt
            }
            HistCount = 0;
            LastI = -1;
        }

        private void chkStats_CheckedChanged(object sender, EventArgs e)
        {
            if (! (chkStats == ActiveControl))
                return;
            StatsEnabled = chkStats.Checked;
        }

        private void chkXP_CheckedChanged(object sender, EventArgs e)
        {
            XPEnabled = chkXP.Checked;
            ShowRegion();
        }

        private void InspectImageForm_Load(object sender, EventArgs e)
        {
            //initializations
            HistoryLength = 50;
            GridHalfSide = 6;
            LabelsEnabled = true;
        }

        private void InspectImageForm_ResizeEnd(object sender, EventArgs e)
        {
        
        }

        private void InspectImageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            fMain.Show();
        }

        public void ShowRegion()
        {
            if (SourcePic == null)
                return;
            
            //TODO finish this line
            //StretchBlt Picture1.hDC, 0, 0, Picture1.ScaleWidth, Picture1.ScaleHeight, SourcePic.hDC, CenterX - GridHalfSide, CenterY - GridHalfSide, GridSide, GridSide, SRCCOPY; //Print image into picturebox with Stretch style
            //update pixel value labels
            long X = 0;
            long Y = 0;
            long i = 0;
            long h = 0;
            long j = 0;
            long color = 0;
            bool UseHistory = false;
            if (StatsEnabled & HistCount < m_HistoryLength)
            {
	            HistCount = HistCount + 1;
	            labelHistCount.ForeColor = Color.Red;
            }
            else
            {
                labelHistCount.ForeColor = Color.Black;
            }
            labelHistCount.Text = HistCount.ToString();
            
            LastI = (LastI + 1) % m_HistoryLength;
          
            for (Y = -GridHalfSide; Y <= GridHalfSide; Y++)
            {
	            for (X = -GridHalfSide; X <= GridHalfSide; X++)
	            {
                    // TODO replace the following Line
	                //PixHistory(X, Y, LastI) = GetPixel(SourcePic.hDC, CenterX + X, CenterY + Y) & 0XFF;
	                if (LabelsEnabled | lblPixVal(i).ForeColor == vbGreen)
	                {
		                if (StatsEnabled)
		                {
		                    lblPixVal(i) = System.Convert.ToInt32(PixelAverage(X, Y));
		                }
		                else
		                {
		                    lblPixVal(i) = PixHistory(X, Y, LastI);
		                }
		                lblPixVal(i).Visible = true;
	                }
	                else
	                {
		                lblPixVal(i).Visible = false;
	                }
	                i = i + 1;
	            }
            }

            lblStats.Visible = StatsEnabled;
            if (StatsEnabled)
            { 
	            //compute stats
	            for (Y = -2; Y <= 2; Y++)
	            {
	                for (X = -2; X <= 2; X++)
	                {
		                PixAverageHistory(X, Y, LastI) = PixelAverage(X, Y);
	                }
	            }
	            //lblStats = "Center Avg " & F(PixAverageHistory(0, 0, LastI), 1) & _
    
	            lblStats = "Avg all " + f(PixelAverageAll, 2) + ", STD " + f(PixelSTD(0, 0), 2) + ", STD(avg) " + f(STDofAverage(0, 0), 2);
	            if (GridHalfSide >= 2) //find the average stats of a 5x5 grid at the center
	            {
	                Avg5x5 = 0;
	                STD5x5 = 0;
	                STDofAvg5x5 = 0;
	                for (Y = -2; Y <= 2; Y++)
	                {
		                for (X = -2; X <= 2; X++)
		                {
		                    Avg5x5 = Avg5x5 + PixAverageHistory(X, Y, LastI) / 25; //average of pixel, including this one
		                    STD5x5 = STD5x5 + PixelSTD(X, Y) / 25;
		                    STDofAvg5x5 = STDofAvg5x5 + STDofAverage(X, Y) / 25;
		                }
	                }
	                lblStats = lblStats + System.Environment.NewLine + "5x5 Avg " + f(Avg5x5, 1) + ", STD " + f(STD5x5, 2) + ", STD(avg) " + f(f(STDofAvg5x5, 2), 2);
	            }
            }

            Picture1.Refresh();
            lblXY = "X: " + CenterX + "  Y:" + CenterY;
            if (XPEnabled)
            {
	            double[] Pos = new double[2];
	            double[] TCPos = new double[2];
	            double[] UnitV = null;
	            int[] Val = new int[3];
	            long XPI = 0;
	            object SideI = null;
	            if (SourcePic == fMain.PictureB(0))
	            {
	                SideI = 0;
	            }
	            else
	            {
	                SideI = 1;
	            }
	            XPI = NearbyXpointI(fMain.CurrCamera, SideI, CenterX / fMain.DisplayScale, CenterY / fMain.DisplayScale, 5);
	            if (XPI >= 0)
	            {
	                if (GetXpoint(fMain.CurrCamera, SideI, XPI, Pos[0], Pos[1], TCPos[0], TCPos[1], UnitV, Val[0], Val[1], Val[2]))
	                {
		                //draw the lines
		                double[] PC = new double[2];
		                double LineLen = 0;
		                PC[0] = 0.5 * Picture1.ScaleWidth + (Pos[0] * fMain.DisplayScale - CenterX) * Picture1.ScaleWidth / (2 * GridHalfSide + 1);
		                PC[1] = 0.5 * Picture1.ScaleHeight + (Pos[1] * fMain.DisplayScale - CenterY) * Picture1.ScaleHeight / (2 * GridHalfSide + 1);
		                LineLen = Min(Picture1.Width, Picture1.Height);
		                DrawLine PC[0] - LineLen * UnitV[0, 0], PC[1] - LineLen * UnitV[1, 0], PC[0] + LineLen * UnitV[0, 0], PC[1] + LineLen * UnitV[1, 0], vbBlue;
		                DrawLine PC[0] - LineLen * UnitV[0, 1], PC[1] - LineLen * UnitV[1, 1], PC[0] + LineLen * UnitV[0, 1], PC[1] + LineLen * UnitV[1, 1], vbBlue;
		                lblStats = f(Pos[0], 2) + " , " + f(Pos[1], 2) + " " + Val[0] + "," + Val[1] + "-" + Val[2];
		                lblStats.Visible = true;
	                }
	            }
            }
        }


    }
}