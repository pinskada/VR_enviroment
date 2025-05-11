using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class CalibrationController : MonoBehaviour
{
    public TextMeshProUGUI instructionText;
    public GameObject targetPrefab;
    public Transform cameraOrigin;
    public float[] distances = { 0.5f, 0.7f, 1.0f, 1.3f, 1.5f }; // meters
    public float targetDuration = 3f; // seconds
    //public TCPClient tcpClient; // Reference to your working TCP client

    private int currentStep = 0;

    void Start()
    {
        instructionText.text = "Calibration starting.\nPlease look at the targets as they appear.\nPress SPACE to begin.";
        SendTCPMessage("{\"type\":\"calib_prepare\"}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentStep == 0)
        {
            instructionText.gameObject.SetActive(false);
            StartCoroutine(CalibrationSequence());
            currentStep++;
        }
    }

    IEnumerator CalibrationSequence()
    {
        for (int i = 0; i < distances.Length; i++)
        {
            float d = distances[i];
            Vector3 targetPosition = cameraOrigin.position + cameraOrigin.forward * d;
            GameObject target = Instantiate(targetPrefab, targetPosition, Quaternion.identity);

            SendTCPMessage($"{{\"type\":\"calib_start\", \"distance\": {d}}}");

            yield return new WaitForSeconds(targetDuration);

            Destroy(target);
        }

        SendTCPMessage("{\"type\":\"calib_done\"}");
        instructionText.text = "Calibration complete.";
        instructionText.gameObject.SetActive(true);
    }

    void SendTCPMessage(string msg)
    {
        /*
        if (tcpClient != null)
            tcpClient.SendMessage(msg);
        else
            Debug.LogWarning("TCPClient not assigned!");
            */
    }
}
