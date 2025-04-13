using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Diagnostics;

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
        SetStaticIP(); // Set static IP for the PC
        ConnectToServer(); // Connect to the Raspberry Pi server
    }

    void Update()
    {
        // Press Enter to send a message
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendMessageToPi(lastMessage);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
        SendMessageToPi("launch_eyeloop");
        }

        // Press R to change the message (for testing)
        if (Input.GetKeyDown(KeyCode.R))
        {
            lastMessage = "Ping " + UnityEngine.Random.Range(0, 999);
            UnityEngine.Debug.Log("Message set to: " + lastMessage);
        }
    }

    void OnApplicationQuit()
    {
        Disconnect(); // Disconnect from the server when the application quits
        SetDynamicIP(); // Reset the IP to DHCP
    }

    public void SetStaticIP()
    {
        // This method executes a powershell script to set a static IP address for the PC.
        // Ensure the script is located in the correct path relative to the Unity project.
        // The script should be named "setStaticIP.ps1" and should be placed in the "Assets/Scripts/Networking" directory.


        // Path to the PowerShell script to set a static IP address
        string scriptPath = $"{Application.dataPath}\\Scripts\\Networking\\setStaticIP.ps1";

        
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        try
        {
            Process.Start(psi);
            UnityEngine.Debug.Log("Static IP set.");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"Failed to run static IP script: {e.Message}");
        }
    }

    public void SetDynamicIP()
    {
        // This method executes a powershell script to reset the IP to DHCP.
        // Ensure the script is located in the correct path relative to the Unity project.
        // The script should be named "resetDynamicIP.ps1" and should be placed in the "Assets/Scripts/Networking" directory.

        // Path to the PowerShell script to reset the IP address to DHCP
        string scriptPath = $"{Application.dataPath}\\Scripts\\Networking\\resetDynamicIP";

        // ProcessStartInfo is used to start a process with specific settings
        ProcessStartInfo psi = new ProcessStartInfo 
        {
            FileName = "powershell.exe",
            Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"",
            UseShellExecute = false,
            CreateNoWindow = true
        };

        
        try // Ensure the script is executable
        {
            Process.Start(psi);
            UnityEngine.Debug.Log("IP set backto DHCP.");
        }
        catch (System.Exception e) // Catch any exceptions that occur during the process start
        {
            UnityEngine.Debug.LogError($"Failed to run dynamic IP script: {e.Message}");
        }
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

            UnityEngine.Debug.LogError("Connected to Raspberry Pi.");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Connection failed: " + e.Message);
        }
    }

    public void SendMessageToPi(string message)
    {
        if (!isConnected || stream == null) return;

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(data, 0, data.Length);
            UnityEngine.Debug.LogError("Sent: " + message);
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Send failed: " + e.Message);
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
                UnityEngine.Debug.LogError("Received: " + message);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Receive error: " + e.Message);
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

        UnityEngine.Debug.LogError("Disconnected from RPI.");
    }
}
