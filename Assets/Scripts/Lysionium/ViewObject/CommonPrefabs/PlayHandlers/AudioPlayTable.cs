using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;

namespace Lysionium.Audio
{
    //[CreateAssetMenu(menuName = "Lysionium/Play/Audio Play Table")]
    public class AudioPlayTable : ScriptableObject
    {
        [Tooltip("音声冒頭に挿入する無音区間のサイズ（WebGL向け）")]
        [SerializeField] private int _blankSamples = 0;
        public int BlankSamples { get => _blankSamples; set => _blankSamples = value; }

        [SerializeField] private Item[] _items = null;
        public IReadOnlyList<Item> Items => _items;

        public void SetItems(List<Item> items)
        {
            _items = items.ToArray();
        }

        [System.Serializable]
        public class Item
        {
            [SerializeField] private string _playName;
            public string PlayName { get => _playName; set => _playName = value; }

            [SerializeField] private AudioClip _audioClip;
            public AudioClip AudioClip { get => _audioClip; set => _audioClip = value; }

            [SerializeField] private AudioMixerGroup _audioMixerGroup;
            public AudioMixerGroup AudioMixerGroup { get => _audioMixerGroup; set => _audioMixerGroup = value; }

            [SerializeField] private PlayBehaviour _playBehaviour;
            public PlayBehaviour PlayBehaviour { get => _playBehaviour; set => _playBehaviour = value; }
        }

        public enum PlayBehaviour
        {
            /// <summary>
            /// 音が重ならない一回再生
            /// </summary>
            Sfx,

            /// <summary>
            /// 音が重なる一回再生（WebGLでは <see cref="Sfx"/> と同様）
            /// </summary>
            SfxOneShot,

            /// <summary>
            /// BGMとしてループ再生する。二つ目のBGMを再生すると前のBGMは停止する
            /// </summary>
            Bgm,

            /// <summary>
            /// BGMとして一回再生する。二つ目のBGMを再生すると前のBGMは停止する
            /// </summary>
            BgmOnce,

            /// <summary>
            /// 何もしない
            /// </summary>
            Manual,
        }
    }
}
