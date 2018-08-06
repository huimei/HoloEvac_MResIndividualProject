using System;
using UnityEngine;
#if UNITY_UWP
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
#endif

public class UDPConnection
{
#if UNITY_UWP
    private DatagramSocket socket = null;
#endif
    HoloEvacController holoEvacController = new HoloEvacController();

    public UDPConnection(int port)
    {
        Debug.Log("In UDPConnection with port number: " + port);
#if UNITY_UWP
        Task.Run(async () =>
        {
            socket = new DatagramSocket();
            socket.MessageReceived += MessageReceived;
            await socket.BindServiceNameAsync(port.ToString());
        });
#endif
    }

#if UNITY_UWP
    async void MessageReceived(DatagramSocket sender, DatagramSocketMessageReceivedEventArgs args)
    {
        StreamReader reader = new StreamReader(args.GetDataStream().AsStreamForRead());
        string ms = await reader.ReadLineAsync();

        Debug.Log("Message received: " + ms);

        if (ms != null && ms.Trim() != ""){
            holoEvacController.flag = 1;
            holoEvacController.ms = ms;
        }
    }
#endif
}