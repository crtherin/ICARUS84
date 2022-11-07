using UnityEngine;

public class SpearChargeAudio : MonoBehaviour
{
    [SerializeField] private float baseVolume = 0.3f;
    [SerializeField] private float volumePerSecond = 0.5f;
    [SerializeField] private AudioClip staminaDrainLoop;
    [SerializeField] private AudioClip healthDrainLoop;

    private bool isPlaying;
    private AudioSource audioSource;

    protected void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    protected void Start()
    {
        audioSource.clip = staminaDrainLoop;
    }

    public void SetUsingHealthAsStamina(bool isUsingHealthAsStamina)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = !isUsingHealthAsStamina ? staminaDrainLoop : healthDrainLoop;
        }
        else
        {
            audioSource.Stop();
            audioSource.clip = !isUsingHealthAsStamina ? staminaDrainLoop : healthDrainLoop;
            audioSource.Play();
        }

        if (isUsingHealthAsStamina)
            Debug.Log("[AUDIO] Spear Charge Using Health (Volume: " + audioSource.volume + ")");
        else
            Debug.Log("[AUDIO] Spear Charge Using Stamina (Volume: " + audioSource.volume + ")");
    }

    public void StartLoop()
    {
        audioSource.volume = baseVolume;
        audioSource.Play();
        Debug.Log("[AUDIO] Spear Charge Start (Volume: " + audioSource.volume + ")");
    }

    public void SetTime(float time)
    {
        audioSource.volume = baseVolume + time * volumePerSecond;
    }

    public void StopLoop()
    {
        audioSource.Stop();
        Debug.Log("[AUDIO] Spear Charge Stop (Volume: " + audioSource.volume + ")");
    }
}