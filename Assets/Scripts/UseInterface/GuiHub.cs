using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System;

public class GuiHub : MonoBehaviour
{
    [SerializeField] private GuiRenderer guiRenderer;
    private TcpClientManager tcpClientManager;

    public static event Action OnGuiReady;

    void Awake()
    {
        OnGuiReady?.Invoke();
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

    public void SendConfig(string action, object parameters)
    {
        var message = new Dictionary<string, object>
        {
            { "category", "config" },
            { "action", action },
            { "params", parameters}
        };

        SendMessage(message);
    }

    public void updateEyeSide(string newEyeSide)
    {
        guiRenderer.UpdateEyeSide(newEyeSide);
    }

}
