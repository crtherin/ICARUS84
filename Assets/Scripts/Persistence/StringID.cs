using System;
using UnityEngine;

[Serializable]
public class StringID
{
    [SerializeField] private string value;

    public string Value => value;

    public void Initialize()
    {
        value = Guid.NewGuid().ToString();
    }

    public void Clear()
    {
        value = string.Empty;
    }
}