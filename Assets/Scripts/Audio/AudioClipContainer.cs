using System;
using UnityEngine;

public class AudioClipContainer : MonoBehaviour
{
    [Serializable]
    public class NamedClip
    {
        [SerializeField] [ReadOnlyPlayMode] private string name;
        [SerializeField] private AudioClip clip;

        public string Name => name;

        public AudioClip Clip => clip;
    }

    [SerializeField] private AudioSource source;
    [SerializeField] private NamedClip[] clips;
    [SerializeField] private NamedClip[] meleeAudio;
    [SerializeField] private int meleeAudioRandom;

#if UNITY_EDITOR
    private void Reset()
    {
        source = GetComponent<AudioSource>();
    }
#endif

    public NamedClip GetClip(string label)
    {
        //this works but it only gets called once, so every time the game gets initiated it
        //passes a random melee clip which stays for the entire instance of the game
        //UPDATE:now works correctly, rotates through the array; could generalize for any array if needed I think;

        if (label == "Melee_0")
        {
            //int meleeAudioRandom = UnityEngine.Random.Range(0, 9);
            var namedClip = meleeAudio[meleeAudioRandom];
            if(meleeAudioRandom<9)
            {
                meleeAudioRandom++;
            }
            else
            {
                meleeAudioRandom = 0;
            }
            return namedClip;
        }
        else
        {
            for (var i = 0; i < clips.Length; i++)
            {
                var namedClip = clips[i];
                if (namedClip.Name == label)
                    return namedClip;
            }
        }

        return null;
    }

    public void PlayClip(NamedClip namedClip)
    {
        AudioClip clip = namedClip.Clip;

        if (clip == null)
        {
            Debug.Log("[AUDIO] [AUDIO CLIP NOT FOUND] " + namedClip.Name);
            return;
        }
        /*else
            if (namedClip.Name == "Melee_0")
            {
                int meleeAudioRandom = UnityEngine.Random.Range(1, 11);
                Debug.Log("[AUDIO]" + clip.name + meleeAudioRandom);
                AudioClip clip = meleeAudio[meleeAudioRandom];
                //source.PlayOneShot(meleeAudio[meleeAudioRandom]);
            }*/
        Debug.Log("[AUDIO] " + clip.name);
        source.PlayOneShot(clip);       
    }
}