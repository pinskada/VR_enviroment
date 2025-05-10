using System.IO;
using UnityEngine;

[System.Serializable]
public class GUISettings
{
    // Camera settings
    public string resWidth = "1080";
    public string resHeight = "720";
    public string focus = "30";
    public string expTime = "10000";
    public string gain = "2";
    public string jpegQual = "20";
    public string prewFps = "5";

    // Tracker settings
    public string leftMinTrackR = "5";
    public string leftMaxTrackR = "20";
    public string leftSrchStep = "10";

    public string rightMinTrackR = "5";
    public string rightMaxTrackR = "20";
    public string rightSrchStep = "10";

    // Gaze processor settings
    public string alphaVal = "0.5";
    public string bufferCropFac = "0.1";
    public string dataStdThr = "0.01";
    public string gyroThr = "5";

    // Display settings
    public string camIPD = "63";
    public string dispWidth = "120";
    public string dispHeight = "68";
    public string eyeToScreen = "0.05";
}

public class GUISettingsManager : MonoBehaviour
{
    public static GUISettings CurrentSettings = new GUISettings();

    private static string settingsPath => Path.Combine(Application.persistentDataPath, "gui_settings.json");

    /// Load settings from file, or create defaults if not found.
    public static void LoadSettings()
    {
        if (File.Exists(settingsPath))
        {
            string json = File.ReadAllText(settingsPath);
            CurrentSettings = JsonUtility.FromJson<GUISettings>(json);
            Debug.Log($"[Settings] Loaded from {settingsPath}");
        }
        else
        {
            CurrentSettings = new GUISettings(); // defaults
            Debug.LogWarning("[Settings] File not found. Using defaults.");
        }
    }

    /// Save current settings to file.
    public static void SaveSettings()
    {
        string json = JsonUtility.ToJson(CurrentSettings, true);
        File.WriteAllText(settingsPath, json);
        Debug.Log($"[Settings] Saved to {settingsPath}");
    }

    /// Optional: for debugging, opens the folder where the file is saved.
    public static void OpenSettingsFolder()
    {
        Application.OpenURL(Application.persistentDataPath);
    }

    // Auto-save on quit (optional)
    private void OnApplicationQuit()
    {
        SaveSettings();
    }
}
