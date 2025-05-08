using UnityEngine;

/// <summary>
/// Applies an asymmetric projection matrix to a stereo camera to account for physical display size and IPD.
/// Attach this script to each eye camera (left and right) in the StereoCameraRig.
/// </summary>
[RequireComponent(typeof(Camera))]
public class StereoCameraProjection : MonoBehaviour
{
    public bool isLeftEye = true;
    public float screenWidth = 0.12f;
    public float screenHeight = 0.068f;
    public float eyeToScreenDistance = 0.06f;
    private Camera eyeCamera;
    void Start()
    {
        eyeCamera = GetComponent<Camera>();
        // Optional: Disable stereo rendering if using manual stereo rendering
        eyeCamera.stereoTargetEye = StereoTargetEyeMask.None;
        StereoRigHub.InvokeReadyProjection(this);

    }

    void Update()
    {
        ApplyAsymmetricFrustum();
    }

    /// Calculates and applies the custom projection matrix for this eye camera.
    void ApplyAsymmetricFrustum()
    {
        float near = eyeCamera.nearClipPlane;
        float far = eyeCamera.farClipPlane;

        // Calculate frustum edges (no IPD offset needed!)
        float left = (-screenWidth / 2f) * near / eyeToScreenDistance;
        float right = (screenWidth / 2f) * near / eyeToScreenDistance;
        float bottom = (-screenHeight / 2f) * near / eyeToScreenDistance;
        float top = (screenHeight / 2f) * near / eyeToScreenDistance;

        // Create the asymmetric projection matrix
        Matrix4x4 proj = Matrix4x4.Frustum(left, right, bottom, top, near, far);
        eyeCamera.projectionMatrix = proj;
    }

    public void setEyeToScreenDistance(float distance)
    {
        eyeToScreenDistance = distance;
    }
    public void setScreenWidth(float width)
    {
        screenWidth = width;
    }
    public void setScreenHeight(float height)
    {
        screenHeight = height;
    }
}
