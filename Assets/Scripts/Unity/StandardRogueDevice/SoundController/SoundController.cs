using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    internal class SoundController
    {
        private Dictionary<IKeyword, AudioSource> table;

        private AudioSource waitSource;

        public bool Wait => waitSource?.isPlaying ?? false;

        public void Open(Transform parent, AudioSource seAudioSourcePrefab, IReadOnlyDictionary<IKeyword, AudioClip> table)
        {
            var name = "SoundController";
            var soundParent = new GameObject($"{name} - Parent").transform;
            soundParent.SetParent(parent, false);
            this.table = new Dictionary<IKeyword, AudioSource>();
            foreach (var pair in table)
            {
                if (pair.Key == null || pair.Value == null) continue;

                var source = Object.Instantiate(seAudioSourcePrefab, soundParent);
                source.clip = pair.Value;
                this.table.Add(pair.Key, source);
            }
        }

        public void Play(IKeyword name, bool wait)
        {
            if (!table.TryGetValue(name, out var source)) return;

            waitSource = wait ? source : null;
            source.loop = false;
            source.Play();
        }

        public void PlayLoop(IKeyword name)
        {
            if (!table.TryGetValue(name, out var source)) return;

            source.loop = true;
            source.Play();
        }

        public void SetLastLoop(IKeyword name)
        {
            if (!table.TryGetValue(name, out var source)) return;

            source.loop = false;
        }
    }
}
