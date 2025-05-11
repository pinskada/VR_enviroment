using UnityEngine;
using UnityEngine.SceneManagement;

public class CalibrationSceneTrigger : MonoBehaviour
{
    // You can change this to any key you prefer
    public KeyCode triggerKey = KeyCode.C;

    void Update()
    {
        if (Input.GetKeyDown(triggerKey))
        {
            Debug.Log("Loading CalibrationScene...");
            SceneManager.LoadScene("CalibScene");
        }
    }
}
