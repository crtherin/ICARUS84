using UnityEngine;
using System.Collections;
using Miscellaneous;

public class SceneManager : Singleton<SceneManager>
{
    public static int Menu => 0;
    public static int Game => 1;
    public static int Dummy => 2;

    private bool isWaitingForLoad;

    public void Load(int scene, float delay = -1)
    {
        if (isWaitingForLoad)
        {
            Debug.LogError("Attempting to load scene while another is in queue.");
            return;
        }

        if (delay > 0)
        {
            StartCoroutine(LoadDelayedCoroutine(scene, delay));
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    private IEnumerator LoadDelayedCoroutine(int scene, float delay)
    {
        isWaitingForLoad = true;

        float triggerTime = Time.realtimeSinceStartup + delay;

        while (triggerTime > Time.realtimeSinceStartup)
            yield return null;

        isWaitingForLoad = false;

        Load(scene);
    }
}