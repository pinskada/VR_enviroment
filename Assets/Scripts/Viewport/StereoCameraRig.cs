using UnityEngine;

// Manages the positioning of left and right cameras based on a configurable IPD value.
// Attach this script to a parent GameObject of the left and right eye cameras.

public class StereoCameraRig : MonoBehaviour
{
    [Header("Eye Transforms")]
    public Transform leftEye;
    public Transform rightEye;

    [Header("IPD Settings")]
    public IPDConfig ipdConfig;

    void Update()
    {
        // Convert IPD from millimeters to meters
        float halfIPD = ipdConfig.interpupillaryDistance / 2000f;

        // Offset each camera on the X-axis by half the IPD
        leftEye.localPosition = new Vector3(-halfIPD, 0f, 0f);
        rightEye.localPosition = new Vector3(halfIPD, 0f, 0f);
    }
}
