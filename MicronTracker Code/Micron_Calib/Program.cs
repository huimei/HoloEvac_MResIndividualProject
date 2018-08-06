using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MicronTrackerCalibration
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      try
      {
        Application.Run(new MainForm());
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message + Environment.NewLine + ex.StackTrace);
      }

    }

    static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
      MessageBox.Show(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace);
    }
  }
}