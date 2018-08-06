using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
using UnityEngine.XR.WSA.WebCam;
using System.Linq;
using System.Net;

public class UserClick : MonoBehaviour, IInputClickHandler
{
    int count;
    int totalCount;

    int screenpointCount = 5;
    int worldpointCount = 4;

    public GameObject crossL1;
    public GameObject crossL2;
    public GameObject crossL3;
    public GameObject crossL4;
    public GameObject crossL5;

    public GameObject crossR1;
    public GameObject crossR2;
    public GameObject crossR3;
    public GameObject crossR4;
    public GameObject crossR5;

    public Camera camL;
    public Camera camR;

    private TCPConnection tcpConnection;
    private int port = 8080;

    // Use this for initialization
    void Start()
    {
        Debug.Log("In Start");
        Initialize();

        //Start TCP server
        tcpConnection = new TCPConnection(port);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDestroy()
    {
        tcpConnection.DeleteManager();
    }

    public void Initialize()
    {
        Debug.Log("In Initialize");

        crossL1.SetActive(true);
        crossL2.SetActive(false);
        crossL3.SetActive(false);
        crossL4.SetActive(false);
        crossL5.SetActive(false);

        crossR1.SetActive(true);
        crossR2.SetActive(false);
        crossR3.SetActive(false);
        crossR4.SetActive(false);
        crossR5.SetActive(false);

        count = 0;
        totalCount = 0;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
#if UNITY_UWP
        tcpConnection.sendDataAsync();
#endif
        if (eventData.TapCount == 1)
        {
            if (totalCount > (screenpointCount * worldpointCount) - 1)
            {
                Debug.Log("Done!!!");
            }
            else
            {
                count++;
                totalCount++;

                if (count > 4)
                {
                    count = 0;
                }

                Debug.Log("Current count: " + count);

                displayCross(count);
            }
        }
        else if (eventData.TapCount == 2)
        {
            count--;

            Debug.Log("TapCount == 2");
            Debug.Log("Current count: " + count);
        }
        else
        {
            Debug.Log("Error clicking");
        }
    }

    public void displayCross(int count)
    {
        if (count == 0)
        {
            crossL5.SetActive(false);
            crossL1.SetActive(true);

            crossR5.SetActive(false);
            crossR1.SetActive(true);

            Vector3 screenPosL = camL.WorldToScreenPoint(crossL1.transform.position);
            Debug.Log("crossL1 screenPos = " + screenPosL.ToString());

            Vector3 screenPosR = camR.WorldToScreenPoint(crossR1.transform.position);
            Debug.Log("crossR1 screenPos = " + screenPosR.ToString());
        }
        else if (count == 1)
        {
            crossL1.SetActive(false);
            crossL2.SetActive(true);

            crossR1.SetActive(false);
            crossR2.SetActive(true);

            Vector3 screenPosL = camL.WorldToScreenPoint(crossL2.transform.position);
            Debug.Log("crossL2 screenPos = " + screenPosL.ToString());

            Vector3 screenPosR = camR.WorldToScreenPoint(crossR2.transform.position);
            Debug.Log("crossR2 screenPos = " + screenPosR.ToString());
        }
        else if (count == 2)
        {
            crossL2.SetActive(false);
            crossL3.SetActive(true);

            crossR2.SetActive(false);
            crossR3.SetActive(true);

            Vector3 screenPosL = camL.WorldToScreenPoint(crossL3.transform.position);
            Debug.Log("crossL3 screenPos = " + screenPosL.ToString());

            Vector3 screenPosR = camR.WorldToScreenPoint(crossR3.transform.position);
            Debug.Log("crossR3 screenPos = " + screenPosR.ToString());
        }
        else if (count == 3)
        {
            crossL3.SetActive(false);
            crossL4.SetActive(true);

            crossR3.SetActive(false);
            crossR4.SetActive(true);

            Vector3 screenPosL = camL.WorldToScreenPoint(crossL4.transform.position);
            Debug.Log("crossL4 screenPos = " + screenPosL.ToString());

            Vector3 screenPosR = camR.WorldToScreenPoint(crossR4.transform.position);
            Debug.Log("crossR4 screenPos = " + screenPosR.ToString());
        }
        else if (count == 4)
        {
            crossL4.SetActive(false);
            crossL5.SetActive(true);

            crossR4.SetActive(false);
            crossR5.SetActive(true);

            Vector3 screenPosL = camL.WorldToScreenPoint(crossL5.transform.position);
            Debug.Log("crossL5 screenPos = " + screenPosL.ToString());

            Vector3 screenPosR = camR.WorldToScreenPoint(crossR5.transform.position);
            Debug.Log("crossR5 screenPos = " + screenPosR.ToString());
        }
        else
        {
            count = 0;
        }
    }
}
