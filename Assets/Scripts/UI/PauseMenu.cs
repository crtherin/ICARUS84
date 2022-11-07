using Rewired;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private UnityEvent pause;
    [SerializeField] private UnityEvent resume;
    [SerializeField] private GameObject abilityTree;
    [SerializeField] private ChallengeManager challengeManager;
    
    private Player player;
    private bool isPaused;
    private bool isTreeOpen;

    protected void Start()
    {
        player = ReInput.players.GetPlayer(0);
        player.AddInputEventDelegate(OnPauseInput, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased,
            "Start");
        player.AddInputEventDelegate(OnAbilitiesInput, UpdateLoopType.Update, InputActionEventType.ButtonJustReleased,
            "Abilities");
    }

    protected void OnDestroy()
    {
        if (isPaused || isTreeOpen)
            Time.timeScale = 1;
    }

    private void OnPauseInput(InputActionEventData data)
    {
        if (isTreeOpen)
        {
            OnAbilitiesInput(data);
            return;
        }

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1;
        
        player.controllers.maps.SetMapsEnabled(!isPaused, ControllerType.Mouse, 0);
        player.controllers.maps.SetMapsEnabled(!isPaused, ControllerType.Keyboard, 0);
        player.controllers.maps.SetMapsEnabled(!isPaused, ControllerType.Joystick, 0);

        if (isPaused)
            pause.Invoke();
        else
            resume.Invoke();
    }

    private void OnAbilitiesInput(InputActionEventData data)
    {
        if (isPaused)
            return;

        if (challengeManager.AggroCount() > 0)
            return;

        isTreeOpen = !isTreeOpen;
        Time.timeScale = isTreeOpen ? 0f : 1;

        player.controllers.maps.SetMapsEnabled(!isTreeOpen, ControllerType.Mouse, 0);
        player.controllers.maps.SetMapsEnabled(!isTreeOpen, ControllerType.Keyboard, 0);
        player.controllers.maps.SetMapsEnabled(!isTreeOpen, ControllerType.Joystick, 0);
        
        abilityTree.SetActive(isTreeOpen);
    }

    public void Resume()
    {
        if (!isPaused)
            return;

        isPaused = false;
        Time.timeScale = 1;
        player.controllers.maps.SetMapsEnabled(true, ControllerType.Mouse, 0);
        player.controllers.maps.SetMapsEnabled(true, ControllerType.Keyboard, 0);
        player.controllers.maps.SetMapsEnabled(true, ControllerType.Joystick, 0);

        resume.Invoke();
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        player.controllers.maps.SetMapsEnabled(true, ControllerType.Mouse, 0);
        player.controllers.maps.SetMapsEnabled(true, ControllerType.Keyboard, 0);
        player.controllers.maps.SetMapsEnabled(true, ControllerType.Joystick, 0);
        Settings.GetInstance().SaveSound();
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.Menu);
    }

    public void NewGame()
    {
        Time.timeScale = 1;
        player.controllers.maps.SetMapsEnabled(true, ControllerType.Mouse, 0);
        player.controllers.maps.SetMapsEnabled(true, ControllerType.Keyboard, 0);
        player.controllers.maps.SetMapsEnabled(true, ControllerType.Joystick, 0);
        Prefs.DeleteAll();
        Settings.GetInstance().SaveSound();
        SceneManager.GetInstance().Load(SceneManager.Game);
    }
}