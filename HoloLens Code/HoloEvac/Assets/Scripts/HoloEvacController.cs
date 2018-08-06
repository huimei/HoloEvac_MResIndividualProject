using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloEvacController : MonoBehaviour
{

    private Object skullPrefab;
    private GameObject skullObject;
    public GameObject toolObject;

    public static int flag = 0;
    private static int processFlag = 1;
    public static string ms = "";

    private int port = 8080;

    private static Matrix4x4 latestSkullTransformationMatrix;
    private static Matrix4x4 latestToolTransformationMatrix;

    private Transform skullCachedTransform;
    private Vector3 skullCachedPosition;

    //TODO: set method to trigger overlayFlag (maybe using double tapping)
    private bool overlayFlag = true;

    // Use this for initialization
    void Start()
    {
        //Start UDP Connection part
        UDPConnection udpConnection = new UDPConnection(port);
        //End UDP Connection part

        skullPrefab = Resources.Load("Skull Prefabs/skullCT"); // Assets/Resources/Skull Prefabs/skullCT.FBX
        skullObject = (GameObject)Instantiate(skullPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        skullCachedTransform = GetComponent<Transform>();
        if (skullObject.transform)
        {
            skullCachedPosition = skullObject.transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Process data received from MicronTracker
        processMessage(ms);

        // Keep track of skull's position (In case being dragged by user), only update if the position has changed
        if (skullObject.transform && skullCachedPosition != skullObject.transform.position)
        {
            skullCachedPosition = skullObject.transform.position;
            transform.position = skullCachedPosition;
        }
    }

    public void processMessage(string ms)
    {
        processFlag = 0;

        Debug.Log("In processMessage, ms = " + ms);

        char delimiter = ';';

        bool skullFlag = false;
        bool toolFlag = false;

        if (ms != null)
        {
            string[] msSubString = ms.Split(delimiter);

            // if two data exists, skull position has changed with position grater than 3mm
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
                            latestSkullTransformationMatrix = HoloEvacUtils.ConvertFloatArrayToMatrix4x4(matrxArray);
                            skullFlag = true;
                        }
                        else if (count == 1)
                        {
                            latestToolTransformationMatrix = HoloEvacUtils.ConvertFloatArrayToMatrix4x4(matrxArray);
                            toolFlag = true;
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
            }
            else if (msSubString.Length == 1)
            {
                string[] array = msSubString[0].Split(',');

                if (array.Length == 12)
                {
                    float[] matrxArray = new float[12];

                    for (int i = 0; i < matrxArray.Length; i++)
                    {
                        matrxArray[i] = float.Parse(array[i]);
                    }

                    latestToolTransformationMatrix = HoloEvacUtils.ConvertFloatArrayToMatrix4x4(matrxArray);
                    toolFlag = true;
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
        }
        else
        {
            Debug.Log("Message arrived in wrong size, check MicronTracker code!!!");
        }

        if (skullFlag && toolFlag && overlayFlag)
        {
            transformSkull(latestSkullTransformationMatrix);
            transformTool(latestSkullTransformationMatrix, latestToolTransformationMatrix);
            skullFlag = false;
            toolFlag = false;
        }
        else
        {
            transformTool(Matrix4x4.TRS(skullCachedPosition, skullCachedTransform.rotation, skullCachedTransform.localScale), latestToolTransformationMatrix);
            skullFlag = false;
            toolFlag = false;
        }
    }

    private void transformSkull(Matrix4x4 latestSkullTransformationMatrix)
    {
        if (latestSkullTransformationMatrix != null)
        {
            skullObject.transform.position = HoloEvacUtils.PositionFromMatrix(latestSkullTransformationMatrix);
            skullObject.transform.rotation = HoloEvacUtils.QuaternionFromMatrix(latestSkullTransformationMatrix);
        }
    }

    private void transformTool (Matrix4x4 latestSkullTransformationMatrix, Matrix4x4 latestToolTransformationMatrix)
    {
        if (latestSkullTransformationMatrix != null && latestToolTransformationMatrix != null)
        {
            // Transform tool's position from HoloLens marker coordinate space to skull's coordinate space
            Matrix4x4 toolPositionInSkullSpace = latestSkullTransformationMatrix * latestToolTransformationMatrix;

            toolObject.transform.position = HoloEvacUtils.PositionFromMatrix(toolPositionInSkullSpace);
            toolObject.transform.rotation = HoloEvacUtils.QuaternionFromMatrix(toolPositionInSkullSpace);
        } 
    }
}



