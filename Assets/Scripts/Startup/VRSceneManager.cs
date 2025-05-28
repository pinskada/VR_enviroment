using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class VRSceneManager : MonoBehaviour
{
    public static VRSceneManager Instance;
    private string currentVRScene;
    public List<string> availableScenes = new() { "SampleScene"};
    private int currentSceneIndex = 0;
    private string previousSceneBeforeCalibration = null;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SwitchVRScene(string newScene)
    {
        if (newScene == currentVRScene)
        {
            Debug.Log($"Scene '{newScene}' already active. Skipping load.");
            return;
        }
        StartCoroutine(SwitchRoutine(newScene));
    }

    private IEnumerator SwitchRoutine(string newScene)
    {
        if (!string.IsNullOrEmpty(currentVRScene))
            yield return SceneManager.UnloadSceneAsync(currentVRScene);

        yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
        currentVRScene = newScene;
    }
    public void SetCurrentScene(string sceneName)
    {
        currentVRScene = sceneName;
    }

    public void GoToCalibration()
    {
        previousSceneBeforeCalibration = availableScenes[currentSceneIndex];
        SwitchVRScene("CalibScene");
    }

    public void NextScene()
    {
        if (currentVRScene == "CalibScene")
        {
            SwitchVRScene(previousSceneBeforeCalibration ?? availableScenes[0]);
            return;
        }

        currentSceneIndex = (currentSceneIndex + 1) % availableScenes.Count;
        SwitchVRScene(availableScenes[currentSceneIndex]);
    }

    public void PreviousScene()
    {
        if (currentVRScene == "CalibScene")
        {
            SwitchVRScene(previousSceneBeforeCalibration ?? availableScenes[0]);
            return;
        }

        currentSceneIndex = (currentSceneIndex - 1 + availableScenes.Count) % availableScenes.Count;
        SwitchVRScene(availableScenes[currentSceneIndex]);
    }

}
