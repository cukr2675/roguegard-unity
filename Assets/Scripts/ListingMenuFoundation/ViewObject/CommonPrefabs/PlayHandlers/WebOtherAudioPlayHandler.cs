using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;

namespace ListingMF.Audio
{
    [AddComponentMenu("UI/Listing Menu Foundation/LMF Web Other Audio Play Handler")]
    public class WebOtherAudioPlayHandler : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSourcePrefab = null;

#if UNITY_EDITOR || !UNITY_WEBGL
        [Space]
        [SerializeField] private AudioPlayTable _defaultAudioPlayTable = null;
#endif
#if UNITY_EDITOR || UNITY_WEBGL
        [Tooltip("Web プラットフォームで再生する " + nameof(AudioPlayTable))]
        [SerializeField] private AudioPlayTable _webAudioPlayTable = null;
#endif

#if UNITY_EDITOR
        [Header("Debug (Editor Only)")]
        [Tooltip("この値が true のとき Web 相当の動作をシミュレートする")]
        [SerializeField] private bool _simulateWeb = false;
#endif

        private Dictionary<string, Item> table;
        private int blankSamples;

        private Item waitSource;

        public bool Wait => waitSource?.IsPlaying ?? false;

        private void Awake()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            var audioPlayTable = _webAudioPlayTable;
#else
            var audioPlayTable = _defaultAudioPlayTable;
#endif
#if UNITY_EDITOR
            if (_simulateWeb) { audioPlayTable = _webAudioPlayTable; }
#endif

            blankSamples = audioPlayTable.BlankSamples;
            table = new Dictionary<string, Item>();
            foreach (var item in audioPlayTable.Items)
            {
                var source = new Item(this, item.AudioClip, item.AudioMixerGroup, item.PlayBehaviour);
                table.Add(item.AudioClip.name, source);
            }
        }

        public void Play(string name, object sender)
        {
            Play(name, false);
        }

        public void Play(string name, bool wait)
        {
            if (!table.TryGetValue(name, out var item)) return;

            switch (item.PlayType)
            {
                case AudioPlayTable.PlayBehaviour.SE:
                    waitSource = wait ? item : null;
                    item.Play();
                    break;
                case AudioPlayTable.PlayBehaviour.SEOneShot:
                    waitSource = wait ? item : null;
#if UNITY_EDITOR
                    if (_simulateWeb)
                    {
                        item.Play();
                        break;
                    }
#endif
#if !UNITY_EDITOR && UNITY_WEBGL
                    item.Play();
#else
                    item.PlayOneShot();
#endif
                    break;
                case AudioPlayTable.PlayBehaviour.BGM:
                case AudioPlayTable.PlayBehaviour.BGMOnce:
                    throw new System.NotImplementedException();
                case AudioPlayTable.PlayBehaviour.Manual:
                default:
                    break;
            }
        }

        public void PlayLoop(string name)
        {
            if (!table.TryGetValue(name, out var item)) return;

            item.PlayLoop();
        }

        public void SetLastLoop(string name)
        {
            if (!table.TryGetValue(name, out var item)) return;

            item.SetLastLoop();
        }

        private class Item
        {
            private readonly WebOtherAudioPlayHandler parent;
            private readonly AudioClip clip;
            private readonly AudioMixerGroup group;
            private readonly List<AudioSource> sources = new();

            public AudioPlayTable.PlayBehaviour PlayType { get; }

            public bool IsPlaying => sources[0].isPlaying;

            private double startLoopTime;
            private int loopCount;
            private bool isLoop;

            public Item(WebOtherAudioPlayHandler parent, AudioClip clip, AudioMixerGroup group, AudioPlayTable.PlayBehaviour playType)
            {
                this.parent = parent;
                this.clip = clip;
                this.group = group;
                PlayType = playType;
                Add();
            }

            private void Add()
            {
                var source = Instantiate(parent._audioSourcePrefab, parent.transform);
                source.name = $"{clip.name} {sources.Count}";
                source.clip = clip;
                source.outputAudioMixerGroup = group;
                sources.Add(source);
            }

            private void StopAll()
            {
                foreach (var source in sources)
                {
                    source.Stop();
                }
            }

            public void Play()
            {
                StopAll();
                var source = sources[0];
                source.loop = false;
                source.timeSamples = parent.blankSamples;
                source.Play();
            }

            public void PlayOneShot()
            {
                var source = sources[0];
                source.PlayOneShot(clip);
            }

            public void PlayLoop()
            {
                if (parent.blankSamples == 0)
                {
                    // 音声冒頭の無音がないとき AudioSource ひとつだけでループ再生する
                    var source = sources[0];
                    source.loop = true;
                    source.Play();
                    return;
                }

                StopAll();

                // ループ再生に必要な AudioSource の数を計算して追加する
                var clipSingleSamples = clip.samples - parent.blankSamples;
                var count = Mathf.CeilToInt((float)parent.blankSamples / clipSingleSamples);
                while (sources.Count < count) { Add(); }

                startLoopTime = AudioSettings.dspTime;
                loopCount = 0;
                isLoop = true;
                foreach (var source in sources)
                {
                    source.timeSamples = parent.blankSamples;
                    var delaySamples = clipSingleSamples * loopCount;
                    source.PlayScheduled(startLoopTime + (double)delaySamples / AudioSettings.outputSampleRate);
                    loopCount++;
                }
                parent.StartCoroutine(AudioLoopCoroutine());
            }

            public void SetLastLoop()
            {
                if (parent.blankSamples == 0)
                {
                    // 音声冒頭の無音がないとき AudioSource ひとつだけでループ再生する
                    var source = sources[0];
                    source.loop = false;
                    return;
                }

                isLoop = false;
                foreach (var source in sources)
                {
                    // 再生中の AudioSource 以外を停止する
                    if (source.timeSamples <= parent.blankSamples) { source.Stop(); }
                }
            }

            private IEnumerator AudioLoopCoroutine()
            {
                while (isLoop)
                {
                    var index = loopCount % sources.Count;
                    var source = sources[index];
                    if (!source.isPlaying)
                    {
                        source.timeSamples = parent.blankSamples;
                        var clipSingleSamples = clip.samples - parent.blankSamples;
                        var delaySamples = clipSingleSamples * loopCount;
                        source.PlayScheduled(startLoopTime + (double)delaySamples / AudioSettings.outputSampleRate);
                        loopCount++;
                        continue;
                    }

                    yield return null;
                }
            }
        }
    }
}
