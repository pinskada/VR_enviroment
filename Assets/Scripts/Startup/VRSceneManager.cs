using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class VRSceneManager : MonoBehaviour
{
    public static VRSceneManager Instance;
    private string currentVRScene;

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
}
