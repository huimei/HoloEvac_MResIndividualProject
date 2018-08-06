//Code taken from longqian's HoloLensARToolKit "https://github.com/qian256/HoloLensARToolKit"

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HoloEvacUtils
{

    private static string TAG = "HoloEvacUtils";

    /// <summary>
    /// Convert row major 3x4 matrix returned by ARToolKitUWP to Matrix4x4 used in Unity.
    /// Right-hand coordinates to left-hand coordinates conversion is performed.
    /// That is, the Y-axis is flipped. Unit is changed from millimeter to meter. [internal use]
    /// </summary>
    /// <param name="t">Flat float array with length 12, obtained from ARToolKitUWP</param>
    /// <returns>The Matrix4x4 object representing the transformation in Unity</returns>
    public static Matrix4x4 ConvertFloatArrayToMatrix4x4(float[] t)
    {
        Debug.Log("In ConvertARUWPFloatArrayToMatrix4x4");

        Matrix4x4 m = new Matrix4x4();
        m.SetRow(0, new Vector4(t[0], -t[1], t[2], t[3]));
        m.SetRow(1, new Vector4(-t[4], t[5], -t[6], t[7]));
        m.SetRow(2, new Vector4(t[8], -t[9], t[10], t[11]));
        m.SetRow(3, new Vector4(0, 0, 0, 1));
        return m;
    }

    /// <summary>
    /// Extract Quaternion representation from Matrix4x4 object, that is more robust against
    /// singularity. [public use]
    /// </summary>
    /// <param name="m">Matrix4x4 object</param>
    /// <returns>Quaternion extracted from the 3x3 submatrix</returns>
    public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
    {
        Debug.Log("In QuaternionFromMatrix");

        // Trap the case where the matrix passed in has an invalid rotation submatrix.
        if (m.GetColumn(2) == Vector4.zero)
        {
            Debug.Log(TAG + ": Quaternion got zero matrix");
            return Quaternion.identity;
        }
        return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
    }

    /// <summary>
    /// Extract Vector3 representation of translation from Matrix4x4 object. [public use]
    /// </summary>
    /// <param name="m">Matrix4x4 object</param>
    /// <returns>Translation represented in Vector3</returns>
    public static Vector3 PositionFromMatrix(Matrix4x4 m)
    {
        Debug.Log("In PositionFromMatrix");

        return m.GetColumn(3);
    }

    /// <summary>
    /// Extract Vector3 representation of scale from Matrix4x4 object. [public use]
    /// </summary>
    /// <param name="m">Matrix4x4 object</param>
    /// <returns>Scale represented in Vector3</returns>
    public static Vector3 ScaleFromMatrix(Matrix4x4 m)
    {
        Debug.Log("In ScaleFromMatrix");

        var x = Mathf.Sqrt(m.m00 * m.m00 + m.m01 * m.m01 + m.m02 * m.m02);
        var y = Mathf.Sqrt(m.m10 * m.m10 + m.m11 * m.m11 + m.m12 * m.m12);
        var z = Mathf.Sqrt(m.m20 * m.m20 + m.m21 * m.m21 + m.m22 * m.m22);

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Logging Matrix4x4 using Debug.Log support. [public use]
    /// </summary>
    /// <param name="mat">Matrix4x4 object</param>
    public static void LogMatrix4x4(Matrix4x4 mat)
    {
        var str1 = string.Format("{0:0.0000}, {1:0.0000}, {2:0.0000}, {3:0.0000}\n", mat[0], mat[4], mat[8], mat[12]);
        var str2 = string.Format("{0:0.0000}, {1:0.0000}, {2:0.0000}, {3:0.0000}\n", mat[1], mat[5], mat[9], mat[13]);
        var str3 = string.Format("{0:0.0000}, {1:0.0000}, {2:0.0000}, {3:0.0000}\n", mat[2], mat[6], mat[10], mat[14]);
        var str4 = string.Format("{0:0.0000}, {1:0.0000}, {2:0.0000}, {3:0.0000}\n", mat[3], mat[7], mat[11], mat[15]);
        Debug.Log(str1 + str2 + str3 + str4);
    }

    /// <summary>
    /// Set a transformation represented by Matrix4x4 to a GameObject localtransform. [public use]
    /// </summary>
    /// <param name="o">The GameObject to set</param>
    /// <param name="m">The Matrix4x4 object representing the target transformation</param>
    public static void SetMatrix4x4ToGameObject(ref GameObject o, Matrix4x4 m)
    {
        Debug.Log("In SetMatrix4x4ToGameObject");

        if (o != null)
        {
            o.transform.localPosition = PositionFromMatrix(m);
            o.transform.localRotation = QuaternionFromMatrix(m);
            o.transform.localScale = ScaleFromMatrix(m);

            Debug.Log("Position = " + PositionFromMatrix(m));
            Debug.Log("Rotation = " + QuaternionFromMatrix(m));
        }
        else
        {
            Debug.Log("Object is null!!");
        }

    }
}
