using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;
public class UIMessageSender : MonoBehaviour
{
    private TcpClientManager tcpClientManager;
    void Awake()
    {
        tcpClientManager = FindFirstObjectByType<TcpClientManager>();
        if (tcpClientManager == null)
        {
            Debug.LogError("TcpClientManager not found in scene!");
        }
    }

    public void SendMessage(Dictionary<string, object> message)
    {
        string json = JsonConvert.SerializeObject(message);
        tcpClientManager.SendMessageToPi(json);
    }

    public void SendTrackerConfig(string action)
    {
        var message = new Dictionary<string, object>
        {
            { "category", "eye_tracker" },
            { "action", action }
        };

        SendMessage(message);
    }

    public void SendTrackerMode(string action)
    {
        var message = new Dictionary<string, object>
        {
            { "category", "tracker_mode" },
            { "action", action }
        };

        SendMessage(message);
    }

    public void SendCalibration(string action)
    {
        var message = new Dictionary<string, object>
        {
            { "category", "calibration" },
            { "action", action }
        };

        SendMessage(message);
    }

    public void SendConfig(string action)
    {
        var message = new Dictionary<string, object>
        {
            { "category", "config" },
            { "action", action }
        };

        SendMessage(message);
    }


}
