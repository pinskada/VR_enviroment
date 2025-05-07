using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

// This script manages the TCP connection to a Raspberry Pi (RPI) server.
// It sets a static IP address for the Unity application to communicate with the RPI,
// connects to the RPI server, sends messages, and receives data from the server.
// It also handles the disconnection process and resets the IP address back to DHCP when the application quits.

public class TcpClientManager : MonoBehaviour
{
    
    [SerializeField] private string raspberryPiIP = "192.168.2.2";
    [SerializeField] private int port = 65432;
    private TcpClient client;
    public Madgwick9DOFHandler madgwickHandler; 
    private GuiRenderer guiRenderer; // Reference to the GuiRenderer script
    private GuiHub guiHub; // Reference to the GuiHub script
    private NetworkStream stream;
    private Thread receiveThread;
    private bool isConnected = false;
    private volatile bool isShuttingDown = false;
    private MemoryStream incomingStream = new MemoryStream();

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
    }

    void OnApplicationQuit()
    {
        isShuttingDown = true; // Set the flag to true to prevent errors during shutdown
        Disconnect(); // Disconnect from the server when the application quits
        ResetToDHCP(); // Reset the IP to DHCP
    }

    void OnEnable()
    {
        GuiRenderer.OnGuiReady += InitGuiReference;
    }

    void InitGuiReference()
    {
        guiRenderer = FindFirstObjectByType<GuiRenderer>();
        guiHub = FindFirstObjectByType<GuiHub>();
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

                // Decode the incoming packet
                HandleIncomingData(buffer, bytesRead);
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
    private void HandleIncomingData(byte[] data, int length)
    {
        // Write new incoming bytes
        incomingStream.Seek(0, SeekOrigin.End);
        incomingStream.Write(data, 0, length);

        incomingStream.Position = 0; // Start reading from beginning of buffer

        while (true)
        {
            if (incomingStream.Length - incomingStream.Position < 4)
            {
                // Not enough bytes for header
                //UnityEngine.Debug.Log("Not enough bytes for header, waiting for more data...");
                break;
            }

            long packetStartPos = incomingStream.Position;

            byte typeByte = (byte)incomingStream.ReadByte();
            char packetType = (char)typeByte;

            byte[] lengthBytes = new byte[3];
            incomingStream.Read(lengthBytes, 0, 3);
            int payloadLength = (lengthBytes[0] << 16) | (lengthBytes[1] << 8) | lengthBytes[2];

            if (incomingStream.Length - incomingStream.Position < payloadLength)
            {
                // Not enough bytes for full payload
                incomingStream.Position = packetStartPos; // rewind back, wait for more data
                //UnityEngine.Debug.Log("Not enough bytes for full payload, waiting for more data...");
                break;
            }


            // Read full payload
            byte[] payload = new byte[payloadLength];
            incomingStream.Read(payload, 0, payloadLength);

            // Handle based on packet type
            switch (packetType)
            {
                case 'J':
                    string json = Encoding.UTF8.GetString(payload);      
                    //UnityEngine.Debug.Log("Incoming JSON");
                    HandleJson(json);
                    break;
                case 'G':
                    //UnityEngine.Debug.Log("Incoming JPEG image");
                    guiRenderer.OnImageReceived(payload, "JPEG");
                    break;
                case 'P':
                    //UnityEngine.Debug.Log("Incoming PNG image");
                    guiRenderer.OnImageReceived(payload, "PNG");
                    break;
                default:
                    UnityEngine.Debug.LogWarning($"Unknown packet type: {packetType}");
                    break;
            }
        }

        // Clean up already processed bytes
        long leftoverBytes = incomingStream.Length - incomingStream.Position;
        if (leftoverBytes > 0)
        {
            byte[] leftover = new byte[leftoverBytes];
            incomingStream.Read(leftover, 0, (int)leftoverBytes);
            incomingStream.SetLength(0);
            incomingStream.Write(leftover, 0, leftover.Length);
        }
        else
        {
            incomingStream.SetLength(0);
        }
    }

    private void HandleJson(string json)
    {

        JToken gyroData;
        string eyeSideData;

        try
        {
            JObject message = JObject.Parse(json);

            string type = message["type"]?.ToString();

            // Optionally: special handling based on type
            switch (type)
            {
                case "9dof":
                    //UnityEngine.Debug.Log($"9DOF data recieved");
                     
                    if (madgwickHandler != null)
                    {
                        gyroData= message["data"];
                        // Parse the JSON data and update the Madgwick filter
                        madgwickHandler.Update9DOF(gyroData);
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning("MadgwickHandler is not assigned!");
                    }
                    
                    break;
                case "distance":
                    //UnityEngine.Debug.Log($"Distance: {data}");
                    break;
                case "control":
                    //UnityEngine.Debug.Log($"Control signal: {data}");
                    break;
                case "STATUS":
                    //UnityEngine.Debug.Log($"Status: {data}");
                    break;
                case "FAILURE":
                    //UnityEngine.Debug.Log($"Failure: {data}");
                    break;
                case "imageInfo":
                    eyeSideData = message["data"].ToString();
                    guiHub.updateEyeSide(eyeSideData);
                    break;
                default:
                    UnityEngine.Debug.LogWarning($"Unknown message type: {type}");
                    break;
            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.LogError($"Failed to parse JSON: {ex.Message}");
        }
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
