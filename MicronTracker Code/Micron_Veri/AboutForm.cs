using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using MTInterfaceDotNet;
using System.IO;
using System.Runtime.InteropServices;

namespace MicronTrackerVerification
{
  public partial class AboutForm : Form
  {
    MainForm fMain;
    public AboutForm(MainForm mForm)
    {
      InitializeComponent();
      fMain = mForm;
    }

    private void btnSysInfo_Click(object sender, EventArgs e)
    {
      String strSysInfo = @"C:\Program Files\Common Files\Microsoft Shared\MSInfo\msinfo32.exe";
      StartSysInfo(strSysInfo);
    }

    private void StartSysInfo(string strSysInfo)
    {
      try
      {
        Process.Start(strSysInfo);// it starts the msinfo32.exe which is stored process
      }
      catch (Win32Exception ex)//Exception handled for any error in file location 
      {
        MessageBox.Show(this, ex.Message, Application.ProductName,
            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      string target = "www.claronav.com";
      Process.Start(target);
    }

    private void AboutForm_Load(object sender, EventArgs e)
    {
      this.Text = "About MicronTracker demo";
      string app = Assembly.GetExecutingAssembly().FullName;
      String s = "Application: " + Regex.Match(app, @"\d+[.]\d+[.]\d+[.]\d+").Value + System.Environment.NewLine;
      app = Assembly.GetAssembly(typeof(MT)).FullName;
      s = s + "MTInterfaceDotNet.dll: " + Regex.Match(app, @"\d+[.]\d+[.]\d+[.]\d+").Value + System.Environment.NewLine;
      s = s + "MTC.dll: " + MT.Version + System.Environment.NewLine;
      if (fMain.CurrCamera != null) { 
        s = s + "Camera Firmware version: " + fMain.CurrCamera.FirmwareVersion;
      }
      labelVersion.Text = s;
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      this.Close();
      //DialogResult = DialogResult.OK;
    }
  }
}