using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System;

public class GuiHub : MonoBehaviour
{
    [SerializeField] private GuiInterface guiInterface;
    [SerializeField] private GuiRenderer guiRenderer;
    private TcpClientManager tcpClientManager;
    public static event Action OnGuiReady;
    private IPDConfig ipdConfig;
    private StereoCameraProjection stereoCameraProjection;
    void Awake()
    {
        OnGuiReady?.Invoke();
        tcpClientManager = FindFirstObjectByType<TcpClientManager>();
        if (tcpClientManager == null)
        {
            Debug.LogError("TcpClientManager not found in scene!");
        }
    }
   
    public void sendConfigToRpi(){
        guiInterface.ApplySettingsToRPI();
    }
    void OnEnable()
    {
        StereoRigHub.OnStereoRigReady += OnStereoReady;
        // If StereoRigHub already has cached → use it now
        if (StereoRigHub.GetCurrentProjection() != null)
        {
            OnStereoReady(StereoRigHub.GetCurrentProjection());
        }

        OnIPDReady(StereoRigHub.GetCurrentIPD());

    }

    void OnDisable()
    {
        StereoRigHub.OnStereoRigReady -= OnStereoReady;
        StereoRigHub.OnIPDconfigReady -= OnIPDReady;
    }

    void OnStereoReady(StereoCameraProjection projection)
    {
        stereoCameraProjection = projection;
    }
    void OnIPDReady(IPDConfig ipd)
    {
        ipdConfig = ipd;
    }
    public void SendMessage(Dictionary<string, object> message)
    {
        string json = JsonConvert.SerializeObject(message);
        tcpClientManager.SendMessageToPi(json);
    }

    public void SendTrackerConfig(string action, Dictionary<string, object> parameters)
    {
        var message = new Dictionary<string, object>
        {
            { "category", "eye_tracker" },
            { "action", action},
            { "params", parameters}
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

        UnityEngine.Debug.Log("Sending config message: " + JsonConvert.SerializeObject(message));
        SendMessage(message);
    }

    public void updateEyeSide(string newEyeSide)
    {
        guiRenderer.UpdateEyeSide(newEyeSide);
    }

    public void updateIPDconfig(string type, float var)
    {
        switch (type)
        {
            case "IPD":
                ipdConfig.setIPD(var);
                break;
            case "width":
                foreach (var projection in StereoRigHub.GetAllProjections())
                {
                    projection.setScreenWidth(var);
                }
                break;
            case "height":
                foreach (var projection in StereoRigHub.GetAllProjections())
                {
                    projection.setScreenHeight(var);
                }
                break;
            case "eyeToScreenDist":
                foreach (var projection in StereoRigHub.GetAllProjections())
                {
                    projection.setEyeToScreenDistance(var);
                }
                break;
            default:
                Debug.LogError("Invalid type for display settings update: " + type);
                break;
        }
    }

}
