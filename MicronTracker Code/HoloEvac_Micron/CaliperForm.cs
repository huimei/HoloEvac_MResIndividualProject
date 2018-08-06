using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace HoloEvac_Micron
{
  public partial class CaliperForm : Form
  {
    public CaliperForm()
    {
      InitializeComponent();
    }

    private double mDistOffset;
    public double DistOffset
    {
      get { return mDistOffset; }
      set
      {
        mDistOffset = value;
      }
    }

    private void btnDone_Click(object sender, EventArgs e)
    {
      this.Close();
      DistOffset = 0;// note VB6 :mDistOffset = 0#
    }

    private void CaliperForm_Load(object sender, EventArgs e)
    {
      DistOffset = 0;// note VB6 :mDistOffset = 0#
    }

    private void tbOffset_TextChanged(object sender, EventArgs e)
    {
      double temp;
      if (double.TryParse(tbOffset.Text, out temp))
      {
        DistOffset = temp;
      }
    }


  }
}