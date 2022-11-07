using UnityEngine;

public class GameObjectEvents : MonoBehaviour
{
    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}