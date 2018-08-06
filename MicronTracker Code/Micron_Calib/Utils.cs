using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace MicronTrackerCalibration
{
  public static class Utils
  {
    static public double Distance(double[] v1, double[] v2) //V1 and V2 must be vectors with same # of dimensions
    {
      double Acc = 0;
      for (int i = 0; i < v1.Length; i++)
      {
        Acc = Acc + (v1[i] - v2[i]) * (v1[i] - v2[i]);
      }
      return Math.Sqrt(Acc);
    }

    static public bool ParseStringToBool(string str)
    {
      if (str == "1") return true;
      if (str == "0") return false;
      return bool.Parse(str);
    }

    static public bool ReadyForWriting(string FilePath)
    {
      bool SilentUI = false;
      return ReadyForWriting(FilePath, SilentUI);
    }

    static public bool ReadyForWriting(string FilePath, bool SilentUI)
    {
      //Checks if the file is ready for writing and informs the user if it is not ready
      //Returns true iff the file is ready
      Debug.Assert(FilePath != "");
      FileStream fileStream = null;
      try
      {
        fileStream = File.Open(FilePath, FileMode.Append);
        BinaryWriter binaryWriter = new BinaryWriter(fileStream);
      }
      catch
      {
        if (!SilentUI)
        {
          MessageBox.Show("Cannot open file " + FilePath + " for writing. The file may be missing or read-only.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          if (fileStream != null)
          {
            fileStream.Close();
            fileStream = null;
          }
        }
        return false;
      }
      if (fileStream != null)
        fileStream.Close();
      return true;
    }

    static public bool ReadyForReading(string FilePath)
    {
      bool SilentUI = false;
      return ReadyForReading(FilePath, SilentUI);
    }

    static public bool ReadyForReading(string FilePath, bool SilentUI)
    {
      //Checks if the file is ready for reading, and informs the user if it is not ready
      //Returns true iff the file is ready for reading
      Debug.Assert(FilePath != "");
      FileStream fileStream = null;

      if (Path.GetDirectoryName(FilePath) == "") //not found
      {
        if (!SilentUI)
        {
          MessageBox.Show("File " + FilePath + " doesn't exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        return false;
      }
      try
      {
        fileStream = File.Open(FilePath, FileMode.Open);
        BinaryReader binaryReader = new BinaryReader(fileStream);
      }
      catch (Exception ex)
      {
        if (!SilentUI)
        {
          MessageBox.Show("Cannot open file " + FilePath + " for reading. " + ex.Message + ".", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
          if (fileStream != null)
          {
            fileStream.Close();
            fileStream = null;
          }
        }
        return false;
      }
      if (fileStream != null)
        fileStream.Close();
      return true;
    }

    static public T Constrain<T>(T Min, T num, T Max) where T : IComparable<T>
    {
      T tempConstrain = num;
      if (tempConstrain.CompareTo(Min) < 0)
      {
        tempConstrain = Min;
      }
      if (tempConstrain.CompareTo(Max) > 0)
      {
        tempConstrain = Max;
      }
      return tempConstrain;
    }
  }

  public static class PathHelper
  {
    public static string GetFolderNameFromFolderPath(string folderPath)
    {
      if (folderPath == null) return null;
      int beg = folderPath.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
      return folderPath.Substring(beg + 1);
    }
    public static string GetUpFolderPathFromFolderPath(string folderPath)
    {
      if (folderPath == null) return null;
      int beg = folderPath.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
      return folderPath.Substring(0, beg);
    }
    public static string[] GetFolderNamesInUpFolder(string folderPath)
    {
      if (folderPath == null) return null;
      string upFolderPath = GetUpFolderPathFromFolderPath(folderPath);
      string[] folderNames = Directory.GetDirectories(upFolderPath);
      for (int i = 0; i < folderNames.Length; i++)
      {
        folderNames[i] = GetFolderNameFromFolderPath(folderNames[i]);
      }
      return folderNames;
    }
    public static string[] GetSubFolderNames(string folderPath)
    {
      if (folderPath == null) return null;
      string[] folderNames = Directory.GetDirectories(folderPath);
      for (int i = 0; i < folderNames.Length; i++)
      {
        folderNames[i] = GetFolderNameFromFolderPath(folderNames[i]);
      }
      return folderNames;
    }
  }
}
