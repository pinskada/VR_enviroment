using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Diagnostics;
using System.Collections;


// This script manages the TCP connection to a Raspberry Pi (RPI) server.
// It sets a static IP address for the Unity application to communicate with the RPI,
// connects to the RPI server, sends messages, and receives data from the server.
// It also handles the disconnection process and resets the IP address back to DHCP when the application quits.

public class TcpClientManager : MonoBehaviour
{
    
    public string raspberryPiIP = "192.168.2.2";
    public int port = 65432;

    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool isConnected = false;
    private volatile bool isShuttingDown = false;

    void Start()
    {
        StartCoroutine(StartupSequence());
    }

    IEnumerator StartupSequence()
    {

        SetStaticIP(); // Set static IP to communicate with the RPI on a local network.
        yield return new WaitForSeconds(3f); // Wait for a few seconds to ensure the IP is set

        UnityEngine.Debug.Log("Attempting to connect to server...");
        ConnectToServer(); // Connect to the RPI TCP server

        SendMessageToPi("launch_eyeloop"); // Send a message to the RPI to launch the eyeloop program
    }

    void Update()
    {
        
    }

    void OnApplicationQuit()
    {
        isShuttingDown = true; // Set the flag to true to prevent errors during shutdown
        Disconnect(); // Disconnect from the server when the application quits
        ResetToDHCP(); // Reset the IP to DHCP
    }

    public static void SetStaticIP()
    {
        // This method sets static IP to be able to communicate with the RPI on a local network.
        // IN ORDER TO WORK THESE SETTINGS MUST BE ENSURED: Edit -> Project Settings -> Player -> 
        // -> Configuration -> Scripting Backend: Mono; Api Compatibility Level: .NET Framework


        // Adapter parameters
        string adapterName = "Ethernet";
        string ipAddress = "192.168.2.1";
        string subnetMask = "255.255.255.0";
        string gateway = "192.168.2.2";

        // Arguments for netsh command
        string args = $"interface ip set address name=\"{adapterName}\" static {ipAddress} {subnetMask} {gateway} 1";

  
        // Start the netsh process with elevated privileges
        Process setStaticIProcess = new Process();
        // Set the process start info
        setStaticIProcess.StartInfo = new ProcessStartInfo
        {
            FileName = @"C:\Windows\System32\netsh.exe", // Path to netsh executable
            Arguments = args, // Arguments for the command
            Verb = "runas", // This is required to run as administrator
            UseShellExecute = false, // This is required to redirect output
            RedirectStandardOutput = true, // Redirect output to read it
            RedirectStandardError = true, // Redirect error to read it
            CreateNoWindow = true // Don't create a window
        };

        // Try to start the process
        try
        {
            setStaticIProcess.Start();
            setStaticIProcess.WaitForExit();
            UnityEngine.Debug.Log("Static IP has been set successfully.");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Failed to set static IP: " + ex.Message);
        }
    }

    public static void ResetToDHCP()
    {
        // This method sets resets the IP back to DHCP.
        // IN ORDER TO WORK THESE SETTINGS MUST BE ENSURED: Edit -> Project Settings -> Player -> 
        // -> Configuration -> Scripting Backend: Mono; Api Compatibility Level: .NET Framework

        // Adapter parameters
        string adapterName = "Ethernet"; 

        // Arguments for netsh command
        string args = $"interface ip set address name=\"{adapterName}\" source=dhcp";

        // Start the netsh process with elevated privileges
        Process setStaticIProcess = new Process();
        // Set the process start info
        setStaticIProcess.StartInfo = new ProcessStartInfo
        {
            FileName = @"C:\Windows\System32\netsh.exe", // Path to netsh executable
            Arguments = args, // Arguments for the command
            Verb = "runas", // This is required to run as administrator
            UseShellExecute = false, // This is required to redirect output
            RedirectStandardOutput = true, // Redirect output to read it
            RedirectStandardError = true, // Redirect error to read it
            CreateNoWindow = true // Don't create a window
        };

        // Try to start the process
        try
        {
            setStaticIProcess.Start();
            setStaticIProcess.WaitForExit();
            UnityEngine.Debug.Log("IP has been reset to DHCP sucessfully.");
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError("Failed to reset IP back to DHCP: " + ex.Message);
        }
    }

    public void ConnectToServer()
    {
        // This method connects to the RPI TCP server.

    
        try
        {
            // Create a new TcpClient and connect to the server
            client = new TcpClient(); 
            client.Connect(raspberryPiIP, port);

            // Get the network stream for reading and writing data
            stream = client.GetStream();
            isConnected = true;

            // Start the receive thread to listen for incoming messages
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
        // This method sends a message to the RPI TCP server.

        // Check if the client is connected and the stream is not null
        if (!isConnected || stream == null) return;

        try
        {
            // Convert the message to bytes and send it over the stream
            // Append a newline character to the message to indicate the end of the message
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
        // This method receives data from the RPI TCP server.


        // Buffer for incoming data
        byte[] buffer = new byte[1024];

        try
        {
            // Keep listening for incoming messages until the connection is closed
            while (isConnected)
            {
                // Read data from the stream into the buffer
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                // Convert the bytes to a string and log it
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                UnityEngine.Debug.Log("Received: " + message);
            }
        }
        catch (Exception e)
        {
            // Prevent errors during shutdown
            if (!isShuttingDown)
                UnityEngine.Debug.LogError("Receive error: " + e.Message);
        }

        Disconnect();
    }

    public void Disconnect()
    {
        // This method disconnects from the RPI TCP server.


        // Check if the client is connected and the stream is not null
        if (!isConnected) return;

        // Close the stream and client, and set the isConnected flag to false
        isConnected = false;
        receiveThread?.Abort();
        stream?.Close();
        client?.Close();

        UnityEngine.Debug.Log("Disconnected from RPI.");
    }

}
