using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Lock frame rate to match your HDMI display (50 Hz)
        Application.targetFrameRate = 50;

        // Optionally set VSync (redundant if set in Quality Settings)
        QualitySettings.vSyncCount = 1;

        Debug.Log($"StartupSettings applied: Frame rate capped at {Application.targetFrameRate} FPS with VSync.");

        Debug.Log($"Total displays: {Display.displays.Length}");
            for (int i = 0; i < Display.displays.Length; i++)
            {
                Debug.Log($"Display {i}: {Display.displays[i].systemWidth}x{Display.displays[i].systemHeight}");
            }

            if (Display.displays.Length > 1)
            {
                Display.displays[1].Activate();
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Screen.SetResolution(2560, 1440, true);
                Debug.Log("Activated Display 2");
            }
            else
            {
                Debug.LogWarning("Only 1 display found — HDMI screen not detected!");
            }


        // Connect external VR display
        //ConnectToDisplay();
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
