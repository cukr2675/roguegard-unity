using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    internal class SoundController
    {
        private Dictionary<IKeyword, Item> table;
        private AudioSource seAudioSourcePrefab;
        private int blankTimeSamples;
        private Transform sourceParent;

        private Item waitSource;

        public bool Wait => waitSource?.IsPlaying ?? false;

        public void Open(Transform parent, AudioSource seAudioSourcePrefab, SoundTable soundTable)
        {
            var name = "SoundController";
            sourceParent = new GameObject($"{name} - Parent").transform;
            sourceParent.SetParent(parent, false);
            table = new Dictionary<IKeyword, Item>();
            this.seAudioSourcePrefab = seAudioSourcePrefab;
            blankTimeSamples = soundTable.BlankTimeSamples;
            foreach (var pair in soundTable)
            {
                if (pair.Key == null || pair.Value == null) continue;

                var source = new Item(this, pair.Value);
                this.table.Add(pair.Key, source);
            }
        }

        public void Play(IKeyword name, bool wait)
        {
            if (!table.TryGetValue(name, out var item)) return;

            waitSource = wait ? item : null;
            item.Play();
        }

        public void PlayLoop(IKeyword name)
        {
            if (!table.TryGetValue(name, out var item)) return;

            item.PlayLoop();
        }

        public void SetLastLoop(IKeyword name)
        {
            if (!table.TryGetValue(name, out var item)) return;

            item.SetLastLoop();
        }

        private class Item
        {
            private readonly SoundController parent;
            private readonly AudioClip clip;
            private readonly List<AudioSource> sources = new List<AudioSource>();

            public bool IsPlaying => sources[0].isPlaying;

            private double startLoopTime;
            private int loopCount;
            private bool isLoop;

            public Item(SoundController parent, AudioClip clip)
            {
                this.parent = parent;
                this.clip = clip;
                Add();
            }

            private void Add()
            {
                var source = Object.Instantiate(parent.seAudioSourcePrefab, parent.sourceParent);
                source.name = $"{clip.name} {sources.Count}";
                source.clip = clip;
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
                source.timeSamples = parent.blankTimeSamples;
                source.Play();
            }

            public void PlayLoop()
            {
                if (parent.blankTimeSamples == 0)
                {
                    // 音声冒頭の無音がないとき AudioSource ひとつだけでループ再生する
                    var source = sources[0];
                    source.loop = true;
                    source.Play();
                    return;
                }

                StopAll();

                // ループ再生に必要な AudioSource の数を計算して追加する
                var clipSingleSamples = clip.samples - parent.blankTimeSamples;
                var count = Mathf.CeilToInt((float)parent.blankTimeSamples / clipSingleSamples);
                while (sources.Count < count) { Add(); }

                startLoopTime = AudioSettings.dspTime;
                loopCount = 0;
                isLoop = true;
                foreach (var source in sources)
                {
                    source.timeSamples = parent.blankTimeSamples;
                    var delaySamples = clipSingleSamples * loopCount;
                    source.PlayScheduled(startLoopTime + (double)delaySamples / AudioSettings.outputSampleRate);
                    loopCount++;
                }
                FadeCanvas.StartCanvasCoroutine(AudioLoopCoroutine());
            }

            public void SetLastLoop()
            {
                if (parent.blankTimeSamples == 0)
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
                    if (source.timeSamples <= parent.blankTimeSamples) { source.Stop(); }
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
                        source.timeSamples = parent.blankTimeSamples;
                        var clipSingleSamples = clip.samples - parent.blankTimeSamples;
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
