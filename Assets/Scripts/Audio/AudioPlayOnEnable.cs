using UnityEngine;
using Data;

public class AudioPlayOnEnable : MonoBehaviour
{
    /*private StringData clipLabel = new StringData("Clip Label", "");

    private AudioClipContainer clipContainer;
    [SerializeField] private AudioClipContainer.NamedClip namedClip;

    //private AudioSource source;

    public void Initialize()
    {
        clipContainer = GetComponentInParent<AudioClipContainer>();

        if (clipContainer == null)
        {
            Debug.LogError("Audio Clip Container not found.");
            return;
        }

        namedClip = clipContainer.GetClip(clipLabel.Get());
    }

    protected void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    protected void OnEnable()
    {
        AudioClip clip = namedClip.Clip;

        if (clip == null)
        {
            Debug.LogWarning("[AUDIO] [AUDIO CLIP NOT FOUND] " + namedClip.Name);
            return;
        }

        Debug.Log("[AUDIO] " + clip.name);
        source.PlayOneShot(clip);
    }*/

    [SerializeField] private StringData clipLabel = new StringData("Clip Label", "");

    private AudioClipContainer clipContainer;
    private AudioClipContainer.NamedClip namedClip;

    public void Awake()
    {
        clipContainer = GetComponentInParent<AudioClipContainer>();

        if (clipContainer == null)
        {
            Debug.LogError("Audio Clip Container not found.");
            return;
        }

        namedClip = clipContainer.GetClip(clipLabel.Get());
    }

    public void OnEnable()
    {
        if (namedClip == null)
        {
            Debug.Log("[AUDIO] [NAMED CLIP NOT FOUND] " + clipLabel.Get());
            return;
        }

        clipContainer.PlayClip(namedClip);
    }
}

