using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChallengeManager : MonoBehaviour
{
    [Header("Win")]
    [SerializeField] private CanvasGroup winScreen;
    [SerializeField] private float fadeDuration = 1;
    [SerializeField] private float menuDelay = 1;
    [SerializeField] private float unfadeDuration = 0.5f;

    [Header("State")]
    [ShowIfPlayMode] [SerializeField] private int queuedSkillPoints;
    [ShowIfPlayMode] [SerializeField] private int queuedEnemies;
    [ShowIfPlayMode] [SerializeField] private int aggroEnemies;
    [SerializeField] private AbilityHandler abilityHandler;

    [Header("Persistence")]
    [SerializeField] private PersistenceManager persistenceManager;
    [SerializeField] private PersistenceBase[] stageTargets;

    public void RegisterEnemy()
    {
        queuedEnemies++;
    }

    public void UnregisterEnemy()
    {
        if (aggroEnemies > 0)
        {
            aggroEnemies--;
            if (aggroEnemies == 0)
                Reward();
        }

        if (queuedEnemies > 0)
        {
            queuedEnemies--;
            if (queuedEnemies == 0)
                Win();
        }
    }

    public void AggroEnemy()
    {
        aggroEnemies++;
    }

    public int AggroCount()
    {
        return aggroEnemies;
    }

    public void QueueSkillPoint()
    {
        queuedSkillPoints++;
    }

    private void Reward()
    {
        Debug.Log("[ChallengeManager] Rewarding skill point.");
        
        if (queuedSkillPoints > 0)
        {
            abilityHandler.AddSkillPoints(queuedSkillPoints);
            queuedSkillPoints = 0;
        }

        foreach (PersistenceBase stageTarget in stageTargets)
            stageTarget.Stage();

        persistenceManager.Commit();
    }

    private void Win()
    {
        if (winScreen)
            StartCoroutine(WinFade());
    }

    private IEnumerator WinFade()
    {
        DontDestroyOnLoad(gameObject);

        winScreen.alpha = 0;
        winScreen.gameObject.SetActive(true);

        for (float t = 0; t < 1; t += Time.deltaTime / fadeDuration)
        {
            winScreen.alpha = t;
            yield return null;
        }

        winScreen.alpha = 1;
        yield return null;

        Scene active = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(SceneManager.Dummy, LoadSceneMode.Additive);
        yield return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(active);
        
        yield return new WaitForSeconds(menuDelay);

        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.Menu);
        
        for (float t = 0; t < 1; t += Time.deltaTime / unfadeDuration)
        {
            winScreen.alpha = 1 - t;
            yield return null;
        }

        Destroy(gameObject);
    }

#if UNITY_EDITOR
    protected void Reset()
    {
        abilityHandler = FindObjectOfType<AbilityHandler>();
        persistenceManager = FindObjectOfType<PersistenceManager>();
    }
#endif
}