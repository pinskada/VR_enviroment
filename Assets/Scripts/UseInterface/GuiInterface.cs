using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class GuiInterface : MonoBehaviour
{
    [SerializeField] private GuiHub guiHub;

    public void SendTrackerModeFromDropdown(int index)

    {
        switch (index)
        {
            case 0:
                guiHub.SendTrackerMode("launch_tracker");
                break;
            case 1:
                guiHub.SendTrackerMode("setup_tracker_1");
                break;
            case 2:
                guiHub.SendTrackerMode("setup_tracker_2");
                break;
            case 3:
                guiHub.SendTrackerMode("stop_preview");
                break;
            default:
                Debug.LogError("Invalid dropdown index: " + index);
                break;
        }   

    }
}
