using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class C3AnimationHandler : MonoBehaviour
    {
        [Serializable]
        private class Clip
        {
            [SerializeField] private string name;
            [SerializeField] private Sprite[] tints;
            [SerializeField] private Sprite[] frames;
            [SerializeField] private Vector2[] boxSizes;
            [SerializeField] private Vector2[] boxOffsets;
            [SerializeField] private Vector2[] spawnOffsets;

            public string Name => name;
            public Sprite[] Tints => tints;
            public Sprite[] Frames => frames;
            public Vector2[] BoxSizes => boxSizes;
            public Vector2[] BoxOffsets => boxOffsets;
            public Vector2[] SpawnOffsets => spawnOffsets;
        }

        [SerializeField] private AudioClipContainer audioContainer;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TintHandler tintHandler;
        [SerializeField] private BoxCollider2D boxCollider2D;
        [SerializeField] private Transform spawnOrigin;
        [SerializeField] private Clip[] clips;

#if UNITY_EDITOR    
        [Header("Testing")]
        [SerializeField] private string testClipName;
        [SerializeField] private int testClipIndex;
#endif    

        private Dictionary<string, Clip> clipsDict;
        private Dictionary<string, AudioClipContainer.NamedClip> audioDict;
    
#if UNITY_EDITOR
        protected void OnValidate()
        {
            CreateDictionary();
            Sample(testClipName, testClipIndex);
        }
#endif

        protected void Start()
        {
            CreateDictionary();
        }

        private void CreateDictionary()
        {
            clipsDict = new Dictionary<string, Clip>();
            audioDict = new Dictionary<string, AudioClipContainer.NamedClip>();

            foreach (var clip in clips)
            {
                clipsDict[clip.Name] = clip;
                audioDict[clip.Name] = audioContainer.GetClip(clip.Name);
            }
        }

        public void PlayAudio(string clipName)
        {
            audioContainer.PlayClip(audioDict[clipName]);
        }

        public void Sample(string clipName, float normalizedTime)
        {
            if (string.IsNullOrEmpty(clipName) || !clipsDict.ContainsKey(clipName))
                return;
        
            Clip clip = clipsDict[clipName];
        
            int index = Mathf.FloorToInt(normalizedTime * clip.Frames.Length);
            
            Debug.Log(clipName + " :: " + index + " :: " + normalizedTime);

            Sample(clipName, index);
        }
    
        public void Sample(string clipName, int index)
        {
            if (string.IsNullOrEmpty(clipName) || !clipsDict.ContainsKey(clipName))
                return;
        
            Clip clip = clipsDict[clipName];
        
            if (index < 0)
                index = 0;
            if (index >= clip.Frames.Length)
                index = clip.Frames.Length - 1;

            tintHandler.SetTintMap(clip.Tints[index]);
            spriteRenderer.sprite = clip.Frames[index];
            boxCollider2D.size = clip.BoxSizes[index];
            boxCollider2D.offset = clip.BoxOffsets[index];
            spawnOrigin.localPosition = clip.SpawnOffsets[index];
        }
    }
}
