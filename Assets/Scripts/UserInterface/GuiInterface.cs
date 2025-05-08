using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;

public class GuiInterface : MonoBehaviour
{
    [SerializeField] private GuiHub guiHub;

    [SerializeField] private GameObject cameraPreviewPanel;
    [SerializeField] private GameObject trackerPreviewPanel;
    [SerializeField] private GameObject gazeProcPanel;
    [SerializeField] private GameObject displayPanel;

    string className;
    string actionName;
    string action;
    string eyeSide;
    Dictionary<string, object> actionDict;
    void Start()
    {
        cameraPreviewPanel.SetActive(false);
        trackerPreviewPanel.SetActive(false);
        gazeProcPanel.SetActive(false);
        displayPanel.SetActive(false);
    }

    // DROPDOWN MENU===========================================================
    public void SendTrackerModeFromDropdown(int index)
    {
        switch (index)
        {
            case 0:
                cameraPreviewPanel.SetActive(false);
                trackerPreviewPanel.SetActive(false);
                gazeProcPanel.SetActive(false);
                displayPanel.SetActive(false);
                guiHub.SendTrackerMode("launch_tracker");
                break;
            case 1:
                cameraPreviewPanel.SetActive(true);
                trackerPreviewPanel.SetActive(false);
                gazeProcPanel.SetActive(false);
                displayPanel.SetActive(false);
                guiHub.SendTrackerMode("setup_tracker_1");
                break;
            case 2:
                cameraPreviewPanel.SetActive(false);
                trackerPreviewPanel.SetActive(true);
                gazeProcPanel.SetActive(false);
                displayPanel.SetActive(false);
                guiHub.SendTrackerMode("setup_tracker_2");
                break;
            case 3:
                cameraPreviewPanel.SetActive(false);
                trackerPreviewPanel.SetActive(false);
                gazeProcPanel.SetActive(true);
                displayPanel.SetActive(false);
                break;
            case 4:
                cameraPreviewPanel.SetActive(false);
                trackerPreviewPanel.SetActive(false);
                gazeProcPanel.SetActive(false);
                displayPanel.SetActive(true);
                break;
            case 5:
                cameraPreviewPanel.SetActive(false);
                trackerPreviewPanel.SetActive(false);
                gazeProcPanel.SetActive(false);
                displayPanel.SetActive(false);
                guiHub.SendTrackerMode("stop_preview");
                break;
            default:
                Debug.LogError("Invalid dropdown index: " + index);
                break;
        }   

    }

    // SAVE BUTTON=============================================================
    public void saveConfig()
    {
    }

    // CAMERA SETTINGS=========================================================
    public void resWidth(string input)
    {
        className = "camera_manager_config";
        actionName = " width";

        action = className + actionName;
        guiHub.SendConfig(action, input);
    }
    public void resHeight(string input)
    {
        className = "camera_manager_config";
        actionName = " height";

        action = className + actionName;
        guiHub.SendConfig(action, input);
    }
    public void focus(string input)
    {
        className = "camera_manager_config";
        actionName = " focus";

        action = className + actionName;
        guiHub.SendConfig(action, input);
    }
    public void expTime(string input)
    {
        className = "camera_manager_config";
        actionName = " exposure_time";

        action = className + actionName;
        guiHub.SendConfig(action, input);
    }
    public void gain(string input)
    {
        className = "camera_manager_config";
        actionName = " analogue_gain";

        action = className + actionName;
        guiHub.SendConfig(action, input);
    }
    public void jpegQual(string input)
    {
        className = "tracker_config";
        actionName = " jpeg_quality";

        action = className + actionName;
        guiHub.SendConfig(action, input);
    }
    public void prewFps(string input)
    {
        className = "tracker_config";
        actionName = " preview_fps";

        action = className + actionName;
        guiHub.SendConfig(action, input);
    }

    // TRACKER SETTINGS========================================================
    public void leftThrUp()
    {
        eyeSide = "L";
        actionDict = new Dictionary<string, object>
        {
            { "type", "config" },
            { "param", "threshold_up" },
            { "value", "" }
        };
        guiHub.SendTrackerConfig(eyeSide, actionDict);
    }
    public void leftThrDown()
    {
        eyeSide = "L";
        actionDict = new Dictionary<string, object>
        {
            { "type", "config" },
            { "param", "threshold_down" },
            { "value", "" }
        };
        guiHub.SendTrackerConfig(eyeSide, actionDict);
    }
    public void leftBlurUp()
    {
        eyeSide = "L";
        actionDict = new Dictionary<string, object>
        {
            { "type", "config" },
            { "param", "blur_up" },
            { "value", "" }
        };
        guiHub.SendTrackerConfig(eyeSide, actionDict);
    }
    public void leftBlurDown()
    {
        eyeSide = "L";
        actionDict = new Dictionary<string, object>
        {
            { "type", "config" },
            { "param", "blur_down" },
            { "value", "" }
        };
        guiHub.SendTrackerConfig(eyeSide, actionDict);
    }
    public void leftMinTrackR(string input)
    {
        eyeSide = "L";
        actionDict = new Dictionary<string, object>
        {
            { "type", "config" },
            { "param", "minThrRad" },
            { "value", input }
        };
        guiHub.SendTrackerConfig(eyeSide, actionDict);
    }
    public void leftMaxTrackR(string input)
    {
        eyeSide = "L";
        actionDict = new Dictionary<string, object>
        {
            { "type", "config" },
            { "param", "maxThrRad" },
            { "value", input }
        };
        guiHub.SendTrackerConfig(eyeSide, actionDict);
    }
    public void leftSrchStep(string input)
    {
        eyeSide = "L";
        actionDict = new Dictionary<string, object>
        {
            { "type", "config" },
            { "param", "search_step" },
            { "value", input }
        };
        guiHub.SendTrackerConfig(eyeSide, actionDict);
    }
    public void rightThrUp()
    {
        eyeSide = "R";
        actionDict = new Dictionary<string, object>
        {
            { "type", "config" },
            { "param", "threshold_up" },
            { "value", "" }
        };
        guiHub.SendTrackerConfig(eyeSide, actionDict);
    }
    public void rightThrDown()
    {
        eyeSide = "R";
        actionDict = new Dictionary<string, object>
        {
            { "type", "config" },
            { "param", "threshold_down" },
            { "value", "" }
        };
        guiHub.SendTrackerConfig(eyeSide, actionDict);
    }
    public void rightBlurUp()
    {
        eyeSide = "R";
        actionDict = new Dictionary<string, object>
        {
            { "type", "config" },
            { "param", "blur_up" },
            { "value", "" }
        };
        guiHub.SendTrackerConfig(eyeSide, actionDict);
    }
    public void rightBlurDown()
    {
        eyeSide = "R";
        actionDict = new Dictionary<string, object>
        {
            { "type", "config" },
            { "param", "blur_down" },
            { "value", "" }
        };
        guiHub.SendTrackerConfig(eyeSide, actionDict);
    }
    public void rightMinTrackR(string input)
    {
        eyeSide = "R";
        actionDict = new Dictionary<string, object>
        {
            { "type", "config" },
            { "param", "minThrRad" },
            { "value", input }
        };
        guiHub.SendTrackerConfig(eyeSide, actionDict);
    }
    public void rightMaxTrackR(string input)
    {
        eyeSide = "R";
        actionDict = new Dictionary<string, object>
        {
            { "type", "config" },
            { "param", "maxThrRad" },
            { "value", input }
        };
        guiHub.SendTrackerConfig(eyeSide, actionDict);
    }
    public void rightSrchStep(string input)
    {
        eyeSide = "R";
        actionDict = new Dictionary<string, object>
        {
            { "type", "config" },
            { "param", "search_step" },
            { "value", input }
        };
        guiHub.SendTrackerConfig(eyeSide, actionDict);
    }

    // GAZE PROCESSING SETTINGS================================================
    public void alphaVal(string input)
    {
        className = "eye_processing_config";
        actionName = " filter_alpha";

        action = className + actionName;
        guiHub.SendConfig(action, input);
    }
    public void bufferCropFac(string input)
    {
        className = "eye_processing_config";
        actionName = " crop_buffer_factor";

        action = className + actionName;
        guiHub.SendConfig(action, input);
    }
    public void dataStdThr(string input)
    {
        className = "eye_processing_config";
        actionName = " std_threshold";

        action = className + actionName;
        guiHub.SendConfig(action, input);
    }
    public void gyroThr(string input)
    {
        className = "eye_processing_config";
        actionName = " gyro_threshold";

        action = className + actionName;
        guiHub.SendConfig(action, input);
    }

    // DISPLAY SETTINGS========================================================
    public void camIPD(string input)
    {
        guiHub.updateIPDconfig("IPD", float.Parse(input));
    }
    public void dispWidth(string input)
    {
        guiHub.updateIPDconfig("width", float.Parse(input)/1000);
    }
    public void dispHeight(string input)
    {
        guiHub.updateIPDconfig("height", float.Parse(input)/1000);
    }
    public void eyeToScreen(string input)
    {
        guiHub.updateIPDconfig("eyeToScreenDist", float.Parse(input)/1000);
    }
}
