using UnityEngine;

[DisallowMultipleComponent]
public class PrefabID : MonoBehaviour
{
    [SerializeField] private StringID value;

    public string Value => value.Value;

#if UNITY_EDITOR
    protected void Reset()
    {
        value = new StringID();
        value.Initialize();
    }
#endif
}