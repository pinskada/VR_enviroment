using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Lock frame rate to match your HDMI display (60 Hz)
        Application.targetFrameRate = 60;

        // Optionally set VSync (redundant if set in Quality Settings)
        QualitySettings.vSyncCount = 1;

        Debug.Log("StartupSettings applied: Frame rate capped at 60 FPS with VSync.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
