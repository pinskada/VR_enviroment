using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Diagnostics;
using System.Collections;

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
        StartCoroutine(StartupSequence());
    }

    IEnumerator StartupSequence()
    {
        SetStaticIP(); // This waits for the .bat file and PowerShell to finish

        // ✅ Wait to allow OS and adapter to apply the new settings
        yield return new WaitForSeconds(3f);

        UnityEngine.Debug.Log("Attempting to connect to server...");
        ConnectToServer();
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
        string batPath = $"{Application.dataPath}\\Scripts\\Networking\\run_ip_config.bat";
        
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = batPath,
            Arguments = "static",        // or "dhcp"
            UseShellExecute = true,      // Required for elevation
            Verb = "runas",              // Triggers admin prompt
            CreateNoWindow = false    
        };

        try
        {
            Process SetStaticIP = Process.Start(psi);
            SetStaticIP.WaitForExit();
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
        string batPath = $"{Application.dataPath}\\Scripts\\Networking\\run_ip_config.bat";
        
        // ProcessStartInfo is used to start a process with specific settings
        ProcessStartInfo psi = new ProcessStartInfo 
        {
            FileName = batPath,
            Arguments = "dhcp",        // or "dhcp"
            UseShellExecute = true,      // Required for elevation
            Verb = "runas",              // Triggers admin prompt
            CreateNoWindow = false    
        };

        
        try // Ensure the script is executable
        {
            Process SetDynamicIP = Process.Start(psi);
            SetDynamicIP.WaitForExit();
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

            UnityEngine.Debug.Log("Connected to Raspberry Pi.");
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
            UnityEngine.Debug.Log("Sent: " + message);
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
                UnityEngine.Debug.Log("Received: " + message);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Receive error: " + e.Message);
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

        UnityEngine.Debug.Log("Disconnected from RPI.");
    }
}
