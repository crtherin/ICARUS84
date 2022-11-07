using Data;
using UnityEngine;

namespace Procedures
{
    public class PlaySound : Process, IInitialize, IStart
    {
        private StringData clipLabel = new StringData("Clip Label", "");

        private AudioClipContainer clipContainer;
        private AudioClipContainer.NamedClip namedClip;

        public void Initialize()
        {
            clipContainer = Procedure.GetComponentInParent<AudioClipContainer>();

            if (clipContainer == null)
            {
                Debug.LogError("Audio Clip Container not found.", Procedure);
                return;
            }

            namedClip = clipContainer.GetClip(clipLabel.Get());
        }

        public void Start()
        {
            if (namedClip == null)
            {
                Debug.Log("[AUDIO] [NAMED CLIP NOT FOUND] " + clipLabel.Get());
                return;
            }

            if (clipLabel.Get() == "Melee_0")
            {
                namedClip = clipContainer.GetClip(clipLabel.Get());
            }

            clipContainer.PlayClip(namedClip);
        }
    }
}