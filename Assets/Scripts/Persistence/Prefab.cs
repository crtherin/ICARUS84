using System;
using UnityEngine;

[Serializable]
public class Prefab
{
    [SerializeField] private GameObject value;
    public GameObject Value => value;
}