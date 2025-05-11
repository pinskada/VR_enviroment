using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartupManager : MonoBehaviour
{
    private IEnumerator Start()
    {
        Application.targetFrameRate = 50;
        QualitySettings.vSyncCount = 1;

        if (Display.displays.Length > 1)
        {
            // Activate second display (external)
            Display.displays[1].Activate();
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.SetResolution(2560, 1440, true);
        }
        else
        {
            Debug.LogWarning("Only one display found – GUI fallback to Display 1.");
        }

        // Delay to ensure CoreScene is fully initialized
        yield return null;

        // Load GUI
        SceneManager.LoadScene("UI_EditorScene", LoadSceneMode.Additive);
        yield return null;

        // Load initial VR scene (SampleScene)
        VRSceneManager.Instance.SwitchVRScene("SampleScene");

    }
}
