using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MicronTrackerVerification
{
  public partial class CameraProblemsForm : Form
  {

    private MainForm fMain;

    public CameraProblemsForm(MainForm mForm)
    {
      InitializeComponent();
      fMain = mForm;
    }

    private void CameraProblemsForm_Load(object sender, EventArgs e)
    {
      String file_path = Directory.GetCurrentDirectory() + "\\" + "CameraProblems.htm";
      FileStream source = new FileStream(file_path, FileMode.Open, FileAccess.Read);
      webBrowser1.DocumentStream = source;

    }
  }
}
