using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UIElements;
using System.Reflection;
using System;
using System.Globalization;

public class GuiInterface : MonoBehaviour
{
    [SerializeField] private TMP_InputField widthField;
    [SerializeField] private TMP_InputField heightField;
    [SerializeField] private TMP_InputField focusField;
    [SerializeField] private TMP_InputField expTimeField;
    [SerializeField] private TMP_InputField gainField;
    [SerializeField] private TMP_InputField jpegQualField;
    [SerializeField] private TMP_InputField prewFpsField;

    [SerializeField] private TMP_InputField leftMinTrackRField;
    [SerializeField] private TMP_InputField leftMaxTrackRField;
    [SerializeField] private TMP_InputField leftSrchStepField;
    [SerializeField] private TMP_InputField rightMinTrackRField;
    [SerializeField] private TMP_InputField rightMaxTrackRField;
    [SerializeField] private TMP_InputField rightSrchStepField;

    [SerializeField] private TMP_InputField alphaValField;
    [SerializeField] private TMP_InputField bufferCropFacField;
    [SerializeField] private TMP_InputField dataStdThrField;
    [SerializeField] private TMP_InputField gyroThrField;

    [SerializeField] private TMP_InputField camIPDField;
    [SerializeField] private TMP_InputField dispWidthField;
    [SerializeField] private TMP_InputField dispHeightField;
    [SerializeField] private TMP_InputField eyeToScreenField;

    [SerializeField] private GuiHub guiHub;

    [SerializeField] private GameObject cameraPreviewPanel;
    [SerializeField] private GameObject trackerPreviewPanel;
    [SerializeField] private GameObject gazeProcPanel;
    [SerializeField] private GameObject displayPanel;
    [SerializeField] private GUISettingsManager guiSettingsManager;

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
        GUISettingsManager.LoadSettings();
        PopulateInputFields();
    }
    public void PopulateInputFields()
    {
        var s = GUISettingsManager.CurrentSettings;

        widthField.text = s.resWidth;
        heightField.text = s.resHeight;
        focusField.text = s.focus;
        expTimeField.text = s.expTime;
        gainField.text = s.gain;
        jpegQualField.text = s.jpegQual;
        prewFpsField.text = s.prewFps;

        leftMinTrackRField.text = s.leftMinTrackR;
        leftMaxTrackRField.text = s.leftMaxTrackR;
        leftSrchStepField.text = s.leftSrchStep;
        rightMinTrackRField.text = s.rightMinTrackR;
        rightMaxTrackRField.text = s.rightMaxTrackR;
        rightSrchStepField.text = s.rightSrchStep;

        alphaValField.text = s.alphaVal;
        bufferCropFacField.text = s.bufferCropFac;
        dataStdThrField.text = s.dataStdThr;
        gyroThrField.text = s.gyroThr;

        camIPDField.text = s.camIPD;
        dispWidthField.text = s.dispWidth;
        dispHeightField.text = s.dispHeight;
        eyeToScreenField.text = s.eyeToScreen;
    }

    public void ApplySettingsToRPI()
    {
        StartCoroutine(ApplySettingsCoroutine());
    }
    private IEnumerator ApplySettingsCoroutine()
    {
        // This method applies the settings to the RPI by invoking the methods in the GUISettingsManager class
        var settings = GUISettingsManager.CurrentSettings;
        Type type = typeof(GuiInterface);

        foreach (FieldInfo field in settings.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
        {
            string methodName = field.Name;
            string value = field.GetValue(settings)?.ToString();
            if (methodName == "leftMinTrackR" ||
                methodName == "leftMaxTrackR" ||
                methodName == "leftSrchStep" ||
                methodName == "rightMinTrackR" ||
                methodName == "rightMaxTrackR" ||
                methodName == "rightSrchStep")
            {
                // Skip these fields as they are handled separately
                continue;
            }


            MethodInfo method = type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);

            if (method != null && method.GetParameters().Length == 1 &&
                method.GetParameters()[0].ParameterType == typeof(string))
            {
                method.Invoke(this, new object[] { value });
                //Debug.Log($"[GUI] Applied {methodName}({value})");
            }
            else
            {
                Debug.LogWarning($"[GUI] No matching method for {methodName} or signature mismatch");
            }
            yield return new WaitForSeconds(0.1f);        }
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

    public void loadCalibScene(){
        VRSceneManager.Instance.SwitchVRScene("CalibScene");
    }

    public void loadScene1(){
        VRSceneManager.Instance.SwitchVRScene("SampleScene");
    }

    public void loadScene2(){

    }

    public void loadScene3(){

    }

    public void quitGame(){
        guiHub.dissconect();
        Application.Quit();
    }

    // CAMERA SETTINGS=========================================================
    public void resWidth(string input)
    {
        className = "camera_manager_config";
        actionName = " width";

        action = className + actionName;
        guiHub.SendConfig(action, input);
        GUISettingsManager.CurrentSettings.resWidth = input;
    }
    public void resHeight(string input)
    {
        className = "camera_manager_config";
        actionName = " height";

        action = className + actionName;
        guiHub.SendConfig(action, input);
        GUISettingsManager.CurrentSettings.resHeight = input;
    }
    public void focus(string input)
    {
        className = "camera_manager_config";
        actionName = " focus";

        action = className + actionName;
        guiHub.SendConfig(action, input);
        GUISettingsManager.CurrentSettings.focus = input;
    }
    public void expTime(string input)
    {
        className = "camera_manager_config";
        actionName = " exposure_time";

        action = className + actionName;
        guiHub.SendConfig(action, input);
        GUISettingsManager.CurrentSettings.expTime = input;
    }
    public void gain(string input)
    {
        className = "camera_manager_config";
        actionName = " analogue_gain";

        action = className + actionName;
        guiHub.SendConfig(action, input);
        GUISettingsManager.CurrentSettings.gain = input;
    }
    public void jpegQual(string input)
    {
        className = "tracker_config";
        actionName = " jpeg_quality";

        action = className + actionName;
        guiHub.SendConfig(action, input);
        GUISettingsManager.CurrentSettings.jpegQual = input;
    }
    public void prewFps(string input)
    {
        className = "tracker_config";
        actionName = " preview_fps";

        action = className + actionName;
        guiHub.SendConfig(action, input);
        GUISettingsManager.CurrentSettings.prewFps = input;
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
        GUISettingsManager.CurrentSettings.leftMinTrackR = input;
        
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
        GUISettingsManager.CurrentSettings.leftMaxTrackR = input;
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
        GUISettingsManager.CurrentSettings.leftSrchStep = input;
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
        GUISettingsManager.CurrentSettings.rightMinTrackR = input;
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
        GUISettingsManager.CurrentSettings.rightMaxTrackR = input;
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
        GUISettingsManager.CurrentSettings.rightSrchStep = input;
    }

    // GAZE PROCESSING SETTINGS================================================
    public void alphaVal(string input)
    {
        className = "eye_processing_config";
        actionName = " filter_alpha";

        action = className + actionName;
        guiHub.SendConfig(action, input);
        GUISettingsManager.CurrentSettings.alphaVal = input;
    }
    public void bufferCropFac(string input)
    {
        className = "eye_processing_config";
        actionName = " buffer_crop_factor";

        action = className + actionName;
        guiHub.SendConfig(action, input);
        GUISettingsManager.CurrentSettings.bufferCropFac = input;
    }
    public void dataStdThr(string input)
    {
        className = "eye_processing_config";
        actionName = " std_threshold";

        action = className + actionName;
        guiHub.SendConfig(action, input);
        GUISettingsManager.CurrentSettings.dataStdThr = input;
    }
    public void gyroThr(string input)
    {
        className = "eye_processing_config";
        actionName = " gyro_threshold";

        action = className + actionName;
        guiHub.SendConfig(action, input);
        GUISettingsManager.CurrentSettings.gyroThr = input;
    }

    // DISPLAY SETTINGS========================================================
    public void camIPD(string input)
    {
        guiHub.updateIPDconfig("IPD", float.Parse(input, CultureInfo.InvariantCulture));
        GUISettingsManager.CurrentSettings.camIPD = input;
    }
    public void dispWidth(string input)
    {
        guiHub.updateIPDconfig("width", float.Parse(input, CultureInfo.InvariantCulture));
        GUISettingsManager.CurrentSettings.dispWidth = input;
    }
    public void dispHeight(string input)
    {
        guiHub.updateIPDconfig("height", float.Parse(input, CultureInfo.InvariantCulture));
        GUISettingsManager.CurrentSettings.dispHeight = input;
    }
    public void eyeToScreen(string input)
    {
        guiHub.updateIPDconfig("eyeToScreenDist", float.Parse(input, CultureInfo.InvariantCulture));
        GUISettingsManager.CurrentSettings.eyeToScreen = input;
    }
}
