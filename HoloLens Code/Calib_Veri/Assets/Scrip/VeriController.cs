using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_UWP
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
#endif

public class VeriController : MonoBehaviour
{

    public GameObject leftCube;
    public GameObject rightCube;

    public Camera leftCam;
    public Camera rightCam;

    public static int flag = 0;
    public static int processFlag = 1;
    public static string ms = "";

    private static GameObject dummyGameObjectLeft;
    private static GameObject dummyGameObjectRight;

    private static Matrix4x4 latestTransMatrixLeft;
    private static Matrix4x4 latestTransMatrixRight;

    private int port = 8080;
#if UNITY_UWP
    private DatagramSocket socket = null;
#endif

    // Use this for initialization
    void Start()
    {
        //Start UDP Connection part
        UDPConnection udpConnection = new UDPConnection(port);
        //End UDP Connection part

        if (leftCam == null || rightCam == null)
        {
            Debug.Log("Main Camera does not exist in the scene");
            Application.Quit();
        }
        else
        {
            dummyGameObjectLeft = new GameObject("DummyLeft");
            dummyGameObjectRight = new GameObject("DummyRight");

            dummyGameObjectLeft.transform.SetParent(leftCam.transform);
            dummyGameObjectRight.transform.SetParent(rightCam.transform);
        }
    }

    // Update is called once per frame
    void Update()
    { 
        processMessage(ms);
    }

    public void transformCube(Matrix4x4 latestTransMatrixLeft, Matrix4x4 latestTransMatrixRight)
    {
        if (leftCube != null && rightCube != null)
        {
            if (latestTransMatrixLeft != null && latestTransMatrixRight != null)
            {
                // World anchored
                dummyGameObjectLeft.transform.localRotation = VeriUtils.QuaternionFromMatrix(latestTransMatrixLeft);
                dummyGameObjectLeft.transform.position = leftCam.ScreenToWorldPoint(VeriUtils.PositionFromMatrix(latestTransMatrixLeft));

                dummyGameObjectRight.transform.localRotation = VeriUtils.QuaternionFromMatrix(latestTransMatrixRight);
                dummyGameObjectRight.transform.position = rightCam.ScreenToWorldPoint(VeriUtils.PositionFromMatrix(latestTransMatrixRight));

                VeriUtils.SetMatrix4x4ToGameObject(ref leftCube, dummyGameObjectLeft.transform.localToWorldMatrix);
                VeriUtils.SetMatrix4x4ToGameObject(ref rightCube, dummyGameObjectRight.transform.localToWorldMatrix);

                // !World anchored
                //leftCube.transform.position = leftCam.ScreenToWorldPoint(VeriUtils.PositionFromMatrix(latestTransMatrixLeft));
                //leftCube.transform.rotation = VeriUtils.QuaternionFromMatrix(latestTransMatrixLeft);

                //rightCube.transform.position = rightCam.ScreenToWorldPoint(VeriUtils.PositionFromMatrix(latestTransMatrixRight));
                //rightCube.transform.rotation = VeriUtils.QuaternionFromMatrix(latestTransMatrixRight);

                Debug.Log("transformed leftCube position: " + leftCam.WorldToScreenPoint(leftCube.transform.position));
                Debug.Log("transformed rightCube position: " + rightCam.WorldToScreenPoint(rightCube.transform.position));
            }
            else
            {
                Debug.Log("Empty matrix!!!");
            }
        }
        else
        {
            Debug.Log("Cube is empty!!!");
        }
    }

    public void processMessage(string ms)
    {
        processFlag = 0;

        Debug.Log("In processMessage, ms = " + ms);

        char delimiter = ';';

        bool leftFlag = false;
        bool rightFlag = false;

        if (ms != null)
        {
            string[] msSubString = ms.Split(delimiter);

            if (msSubString.Length == 2)
            {
                int count = 0;
                foreach (string substring in msSubString)
                {
                    string[] array = substring.Split(',');

                    if (array.Length == 12)
                    {
                        float[] matrxArray = new float[12];

                        for (int i = 0; i < matrxArray.Length; i++)
                        {
                            matrxArray[i] = float.Parse(array[i]);
                        }

                        if (count == 0)
                        {
                            latestTransMatrixLeft = VeriUtils.ConvertARUWPFloatArrayToMatrix4x4(matrxArray);
                            leftFlag = true;
                        }
                        else if (count == 1)
                        {
                            latestTransMatrixRight = VeriUtils.ConvertARUWPFloatArrayToMatrix4x4(matrxArray);
                            rightFlag = true;
                        }
                        else
                        {
                            Debug.Log("Wrong Count!!!");
                        }
                    }
                    else
                    {
                        Debug.Log("Array size != 12!!!");
                    }

                    count++;
                }//End of foreach (string substring in msSubString)

                if (leftFlag  && rightFlag )
                {
                    transformCube(latestTransMatrixLeft, latestTransMatrixRight);
                    leftFlag = false;
                    rightFlag = false;
                }
            }
            else
            {
                Debug.Log("Message larger than 2 array!!!");
            }
        }//End of if (ms != null)

    }
}
