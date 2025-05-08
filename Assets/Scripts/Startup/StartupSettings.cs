using UnityEngine;
using UnityEngine.SceneManagement;

public class StartupSettings : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
{
    SceneManager.LoadScene("UI_EditorScene", LoadSceneMode.Additive);

    // Set framerate and VSync
    Application.targetFrameRate = 50;
    QualitySettings.vSyncCount = 1;

    //Debug.Log($"StartupSettings: Capped to {Application.targetFrameRate} FPS");

    // Log connected displays
    //Debug.Log($"Total displays: {Display.displays.Length}");
    for (int i = 0; i < Display.displays.Length; i++)
    {
        //Debug.Log($"Display {i}: {Display.displays[i].systemWidth}x{Display.displays[i].systemHeight}");
    }

    if (Display.displays.Length > 1)
    {
        // Activate second display (external)
        Display.displays[1].Activate();

        //Debug.Log("Display 2 (external HDMI) activated");

        // Optional: apply fullscreen and resolution only to primary display
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        Screen.SetResolution(Display.displays[0].systemWidth, Display.displays[0].systemHeight, true);
    }
    else
    {
        Debug.LogWarning("Only one display found – GUI fallback to Display 1.");
    }
}



    // Update is called once per frame
    void Update()
    {
        
    }

    void ConnectToDisplay()
    {
        // This method checks for external display and makes it a default output for render

        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate(); // HDMI headset display
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.SetResolution(2560, 1440, true);
        } 
    }

}
