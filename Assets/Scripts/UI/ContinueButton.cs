using UnityEngine;

public class ContinueButton : MenuButton
{
    protected override bool IsEnabled()
    {
        return PlayerPrefs.GetInt("alive", 0) == 1;
    }

    protected override void Click()
    {
        SceneManager.GetInstance().Load(1);
    }
}