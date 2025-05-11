using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StereoRigAligner : MonoBehaviour
{
    [SerializeField] private string anchorName = "StereoRigTarget";

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(AlignToAnchorNextFrame());
    }

    private IEnumerator AlignToAnchorNextFrame()
    {
        yield return null; // Wait one frame for objects in the new scene to initialize

        GameObject anchor = GameObject.Find(anchorName);
        if (anchor != null)
        {
            transform.position = anchor.transform.position;
            transform.rotation = anchor.transform.rotation;
            Debug.Log($"StereoRig aligned to anchor '{anchorName}' in scene.");
        }
        else
        {
            Debug.LogWarning($"StereoRigAligner: No anchor '{anchorName}' found in scene.");
        }
    }
}
