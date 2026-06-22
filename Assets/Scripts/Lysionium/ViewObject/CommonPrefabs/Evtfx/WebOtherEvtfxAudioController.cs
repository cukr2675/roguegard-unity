using Lysionium.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Lysionium.Views
{
    [AddComponentMenu("UI/Lysionium/LUI Web Other Evtfx Audio Controller")]
    public class WebOtherEvtfxAudioController : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSourcePrefab = null;
        [SerializeField] private string _stopBgmEvtfxName = null;

#if UNITY_EDITOR || !UNITY_WEBGL
        [Space]
        [SerializeField] private EvtfxAudioTable _defaultEvtfxAudioTable = null;
#endif
#if UNITY_EDITOR || UNITY_WEBGL
        [Tooltip("Web プラットフォームで再生する " + nameof(EvtfxAudioTable))]
        [SerializeField] private EvtfxAudioTable _webEvtfxAudioTable = null;
#endif

#if UNITY_EDITOR
        [Header("Debug (Editor Only)")]
        [Tooltip("この値が true のとき Web 相当の動作をシミュレートする")]
        [SerializeField] private bool _simulateWeb = false;
#endif

        private Dictionary<string, Item> table;
        private int blankSamples;

        private Item waitSource;
        private Item bgmSource;

        // 命名メモ: IsInWaiting だとウェイトでこのコンポーネント全体が待機しているように見える
        /// <summary>
        /// 待機対象の音声が再生中のとき true
        /// </summary>
        public bool AudioWaitIsInProgress => waitSource?.IsPlaying ?? false;

        protected virtual void Awake()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            var evtfxAudioTable = _webAudioEvtfxTable;
#else
            var evtfxAudioTable = _defaultEvtfxAudioTable;
#endif
#if UNITY_EDITOR
            if (_simulateWeb) { evtfxAudioTable = _webEvtfxAudioTable; }
#endif

            blankSamples = evtfxAudioTable.BlankSamples;
            table = new Dictionary<string, Item>();
            foreach (var item in evtfxAudioTable.Items)
            {
                var source = new Item(this, item.AudioClip, item.AudioMixerGroup, item.PlayBehaviour);
                table.Add(item.EvtfxName, source);
            }
        }

        public void Play(string evtfxName, object sender)
        {
            Play(evtfxName, false);
        }

        public void Play(string evtfxName, bool wait)
        {
            if (evtfxName == _stopBgmEvtfxName)
            {
                StopBgm();
                return;
            }
            if (!table.TryGetValue(evtfxName, out var item)) return;

            waitSource = wait ? item : null;

            switch (item.PlayType)
            {
                case EvtfxAudioTable.PlayBehaviour.Sfx:
                    item.Play();
                    break;
                case EvtfxAudioTable.PlayBehaviour.SfxOneShot:
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
                case EvtfxAudioTable.PlayBehaviour.Bgm:
                    bgmSource?.StopAll();
                    bgmSource = item;
                    item.PlayLoop();
                    break;
                case EvtfxAudioTable.PlayBehaviour.BgmOnce:
                    bgmSource?.StopAll();
                    bgmSource = item;
                    item.Play();
                    break;
                case EvtfxAudioTable.PlayBehaviour.Manual:
                default:
                    break;
            }
        }

        public void PlayLoop(string evtfxName)
        {
            if (!table.TryGetValue(evtfxName, out var item)) return;

            item.PlayLoop();
        }

        public void SetLastLoop(string evtfxName)
        {
            if (!table.TryGetValue(evtfxName, out var item)) return;

            item.SetLastLoop();
        }

        public void StopBgm()
        {
            bgmSource?.StopAll();
            bgmSource = null;
        }

        private class Item
        {
            private readonly WebOtherEvtfxAudioController parent;
            private readonly AudioClip clip;
            private readonly AudioMixerGroup group;
            private readonly List<AudioSource> sources = new();

            public EvtfxAudioTable.PlayBehaviour PlayType { get; }

            public bool IsPlaying => sources[0].isPlaying;

            private double startLoopTime;
            private int loopCount;
            private bool isLoop;

            public Item(WebOtherEvtfxAudioController parent, AudioClip clip, AudioMixerGroup group, EvtfxAudioTable.PlayBehaviour playType)
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

            public void StopAll()
            {
                isLoop = false;
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
