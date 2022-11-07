using System.Collections.Generic;
using UnityEngine;

public class PersistenceManager : MonoBehaviour
{
    private readonly Dictionary<string, string> staged = new Dictionary<string, string>();

    public void Stage<T>(string key, T data)
    {
        staged[key] = data.SaveRaw();
    }

    public void Commit()
    {
        Debug.Log("[PersistenceManager] Committing.");
    
        PlayerPrefs.SetInt("alive", 1);
        foreach (var pair in staged)
            PlayerPrefs.SetString(pair.Key, pair.Value);

        PlayerPrefs.Save();
        staged.Clear();
    }
}