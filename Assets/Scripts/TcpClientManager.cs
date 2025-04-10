using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TcpClientManager : MonoBehaviour
{
    public string raspberryPiIP = "192.168.2.2";
    public int port = 65432;

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool isConnected = false;

    private string lastMessage = "Hello from Unity!";

    void Start()
    {
        ConnectToServer();
    }

    void Update()
    {
        // Press Enter to send a message
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendMessageToPi(lastMessage);
        }

        // Press R to change the message (for testing)
        if (Input.GetKeyDown(KeyCode.R))
        {
            lastMessage = "Ping " + UnityEngine.Random.Range(0, 999);
            Debug.Log("Message set to: " + lastMessage);
        }
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }

    public void ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            client.Connect(raspberryPiIP, port);
            stream = client.GetStream();
            isConnected = true;

            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log("✅ Connected to Raspberry Pi.");
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Connection failed: " + e.Message);
        }
    }

    public void SendMessageToPi(string message)
    {
        if (!isConnected || stream == null) return;

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(data, 0, data.Length);
            Debug.Log("📤 Sent: " + message);
        }
        catch (Exception e)
        {
            Debug.LogError("❌ Send failed: " + e.Message);
        }
    }

    private void ReceiveData()
    {
        byte[] buffer = new byte[1024];

        while (isConnected)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log("📥 Received: " + message);
            }
            catch (Exception e)
            {
                Debug.LogError("❌ Receive error: " + e.Message);
                break;
            }
        }

        Disconnect();
    }

    public void Disconnect()
    {
        if (!isConnected) return;

        isConnected = false;
        receiveThread?.Abort();
        stream?.Close();
        client?.Close();

        Debug.Log("🔌 Disconnected.");
    }
}
