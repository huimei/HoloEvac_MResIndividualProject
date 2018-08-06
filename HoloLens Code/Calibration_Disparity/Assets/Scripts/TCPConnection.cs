using System;
using System.Collections.Generic;
#if UNITY_UWP
using System.IO;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
#endif

public class TCPConnection
{

#if UNITY_UWP
    private StreamSocketListener streamsocketlistener;
    private List<StreamWriter> writer = new List<StreamWriter>();
#endif
    private bool ListenFlag = false;
    private bool connected = false;

    public int count = 0;

    public TCPConnection(int port)
    {
        ListenFlag = true;
#if UNITY_UWP
        Task.Run(async () =>
        {
            streamsocketlistener = new StreamSocketListener();
            streamsocketlistener.ConnectionReceived += ConnectionReceived;
            await streamsocketlistener.BindServiceNameAsync(port.ToString());
        });
#endif
    }

    public void DeleteManager()
    {
        ListenFlag = false;
#if UNITY_UWP
        writer.Clear();
        streamsocketlistener.Dispose();
#endif
    }

#if UNITY_UWP
    private async void ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
    {
        writer.Add(new StreamWriter(args.Socket.OutputStream.AsStreamForWrite()));
        while (ListenFlag)
        {
            connected = true;
        }
    }

    public async void sendDataAsync()
    {
        string data = "clicked" + count;

        if (connected)
        {
            for (int i = 0; i < writer.Count; i++)
            {
                await writer[i].WriteAsync(data);
                await writer[i].FlushAsync();
            }
        }

        count++;
    }
#endif
}
