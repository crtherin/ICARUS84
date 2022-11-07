using UnityEngine;

public class SpearHitAudio : MonoBehaviour
{
    [SerializeField] private float stackBaseVolume = 0.3f;
    [SerializeField] private float stackVolumeInrement = 0.3f;

    [Header("References")]
    [SerializeField] private AudioClip hit_A;
    [SerializeField] private AudioClip hit_B_0;
    [SerializeField] private AudioClip hit_B_1;
    [SerializeField] private AudioClip hit_B_2;

    private int variation;
    private int upgrade;
    private int stacks;

    private AudioSource audioSource;

    protected void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void AddStack()
    {
        stacks++;
    }

    public void SetVariation(int variation)
    {
        this.variation = variation;
        upgrade = 0;
    }

    public void SetUpgrade(int upgrade)
    {
        if (this.upgrade >= upgrade)
            return;

        this.upgrade = upgrade;
    }

    public void Play()
    {
        if (variation == 2 && upgrade >= 2)
        {
            audioSource.volume = stackBaseVolume + stacks * stackVolumeInrement;
        }

        switch (variation)
        {
            case 0:
                PlaySafe(hit_A, "A");
                break;

            case 1:

                switch (upgrade)
                {
                    case 0:
                        PlaySafe(hit_B_0, "B_0");
                        break;

                    case 1:
                        PlaySafe(hit_B_1, "B_1");
                        break;

                    case 2:
                        PlaySafe(hit_B_2, "B_2");
                        break;
                }

                break;
        }
    }

    private void PlaySafe(AudioClip clip, string message)
    {
        Debug.Log("[AUDIO] Spear Hit " + message);
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}