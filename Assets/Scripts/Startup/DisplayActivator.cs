using UnityEngine;

public class DisplayActivator : MonoBehaviour
{
    void Start()
    {
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();  // Enable Display 2
        }
        else
        {
            Debug.LogWarning("Only one display connected.");
        }
    }
}
