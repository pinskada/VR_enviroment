using UnityEngine;
using Newtonsoft.Json.Linq;

public class Madgwick9DOFHandler : MonoBehaviour
{
    [Header("Target Transform to Rotate")]
    public Transform target; // Assign your Camera here

    [Header("Filter Settings")]
    public float sampleFreq = 100.0f; // Hz
    public float beta = 0.007f;        // Madgwick filter beta gain
    private Vector3 latestGyro;
    private Vector3 latestAccel;
    private Vector3 latestMag;
    private bool hasNewData = false;
    private MadgwickAHRS madgwick;
    private Quaternion initialRotation; 
    private bool resetRequested = false;

    private double lastPacketTime = 0f;

    void Start()
    {
        madgwick = new MadgwickAHRS(1.0f / sampleFreq, beta);
        if (target != null)
            initialRotation = target.rotation; // Save the starting rotation
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetOrientation();
        }

        if (hasNewData)
        {
            if (!resetRequested)
            {
                Vector3 gyroRad = latestGyro * Mathf.Deg2Rad;
                Vector3 accelNorm = latestAccel.normalized;
                Vector3 magNorm = latestMag.normalized;
                madgwick.Update(
                    gyroRad.x, gyroRad.y, gyroRad.z,
                    accelNorm.x, accelNorm.y, accelNorm.z,
                    magNorm.x, magNorm.y, magNorm.z
                );

                /*
                madgwick.UpdateIMU(
                    gyroRad.x, gyroRad.y, gyroRad.z,
                    accelNorm.x, accelNorm.y, accelNorm.z
                );
                */
                Quaternion q = new Quaternion(
                    madgwick.Quaternion[0],
                    madgwick.Quaternion[1],
                    madgwick.Quaternion[2],
                    madgwick.Quaternion[3]
                );

                if (target != null)
                    target.rotation = ConvertSensorToUnity(q);
            }

            resetRequested = false; // Clear reset flag after skipping one update
            hasNewData = false;
        }
    }

    public void Update9DOF(JToken data)
    {

        Vector3 gyroDegPerSec = ParseVector3(data["gyro"]);
        Vector3 accelRaw = ParseVector3(data["accel"]);
        Vector3 magRaw = ParseVector3(data["mag"]);

        double currentTime = (double)System.Diagnostics.Stopwatch.GetTimestamp() / System.Diagnostics.Stopwatch.Frequency;   

        if (lastPacketTime != 0f)
        {
            double deltaTime = currentTime - lastPacketTime;
            
            // Update sample period inside the Madgwick filter
            madgwick.samplePeriod = (float)deltaTime;
        }

        lastPacketTime = currentTime;

        latestGyro = gyroDegPerSec;
        latestAccel = accelRaw;
        latestMag = magRaw;
        hasNewData = true;
    }

    public void ResetOrientation()
    {
        if (target != null)
        {
            target.rotation = initialRotation;

            // Hard reset Madgwick filter quaternion to identity
            madgwick.Quaternion[0] = 0f;
            madgwick.Quaternion[1] = 0f;
            madgwick.Quaternion[2] = 0f;
            madgwick.Quaternion[3] = 1f;

            resetRequested = false; // No need to skip frames anymore

            Debug.Log("[Madgwick] Full reset: camera and filter set to default orientation.");
        }
    }

    private Quaternion ConvertSensorToUnity(Quaternion q)
    {
        // Final correct mapping: fixes roll, yaw, pitch
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    private Vector3 ParseVector3(Newtonsoft.Json.Linq.JToken token)
    {
        return new Vector3(
            token["x"]?.ToObject<float>() ?? 0f,
            token["y"]?.ToObject<float>() ?? 0f,
            token["z"]?.ToObject<float>() ?? 0f
        );
    }

}
