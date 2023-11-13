using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    [CreateAssetMenu(menuName = "RoguegardData/Settings/SoundTable")]
    public class SoundTable : ScriptableObject
    {
        [SerializeField] private Item[] _items = null;

        public Dictionary<IKeyword, AudioClip> ToTable()
        {
            var table = new Dictionary<IKeyword, AudioClip>();
            foreach (var item in _items)
            {
                if (!item.Clip) continue;
                if (!item.Clip.LoadAudioData() || item.Clip.loadState != AudioDataLoadState.Loaded) throw new RogueException($"{item.Clip} の読み込みに失敗しました。");

                table.Add(item.Name, item.Clip);
            }
            return table;
        }

        [System.Serializable]
        private class Item
        {
            [SerializeField] private KeywordData _name;
            public IKeyword Name => _name;

            [SerializeField] private AudioClip _clip;
            public AudioClip Clip => _clip;
        }
    }
}
