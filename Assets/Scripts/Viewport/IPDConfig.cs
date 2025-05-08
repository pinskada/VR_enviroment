using UnityEngine;

// Stores the interpupillary distance (IPD) between the user's eyes in millimeters.
// This value is used to correctly position the stereo cameras.

[CreateAssetMenu(fileName = "IPDConfig", menuName = "VR/IPD Configuration")]
public class IPDConfig : ScriptableObject
{
    [Range(50f, 75f)]
    public float interpupillaryDistance = 68f; // Default IPD in mm
    void OnEnable()
    {
        StereoRigHub.InvokeReadyIPD(this);
    }
    public void setIPD(float ipd)
    {
        interpupillaryDistance = ipd;
    }
}
