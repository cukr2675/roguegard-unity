using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    [CreateAssetMenu(menuName = "RoguegardData/Settings/SoundTable")]
    public class SoundTable : ScriptableObject, IEnumerable<KeyValuePair<IKeyword, AudioClip>>
    {
        [SerializeField, Tooltip("音声冒頭の無音の長さを指定する")] private int _blankTimeSamples = 44100 / 10; // 0.1秒
        public int BlankTimeSamples => _blankTimeSamples;

        [SerializeField] private Item[] _items = null;

        public IEnumerator<KeyValuePair<IKeyword, AudioClip>> GetEnumerator()
        {
            foreach (var item in _items)
            {
                if (!item.Clip) continue;
                if (!item.Clip.LoadAudioData() || item.Clip.loadState != AudioDataLoadState.Loaded) throw new RogueException(
                    $"{item.Clip} の読み込みに失敗しました。");

                yield return new KeyValuePair<IKeyword, AudioClip>(item.Name, item.Clip);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
