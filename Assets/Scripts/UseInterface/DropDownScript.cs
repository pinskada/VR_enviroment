using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;

public class DropDownScript : MonoBehaviour
{
    [SerializeField] private UIMessageSender uiMessageSender;

    public void SendTrackerModeFromDropdown(int index)
    {
        switch (index)
        {
            case 0:
                uiMessageSender.SendTrackerMode("launch_tracker");
                break;
            case 1:
                uiMessageSender.SendTrackerMode("setup_tracker_1");
                break;
            case 2:
                uiMessageSender.SendTrackerMode("setup_tracker_2");
                break;
            default:
                Debug.LogError("Invalid dropdown index: " + index);
                break;
        }   

    }
}
