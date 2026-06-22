using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Lysionium.Audio.Editor
{
    [CreateAssetMenu(menuName = "Lysionium/Evtfx Audio/Trim Seed", fileName = "AppAudioTableSfx")]
    public class TrimEvtfxAudioSeed : EvtfxAudioSeed
    {
        [SerializeField] private AudioMixerGroup _audioMixerGroup = null;

        [SerializeField] private EvtfxAudioTable.PlayBehaviour _playBehaviour = EvtfxAudioTable.PlayBehaviour.Sfx;

        [SerializeField] private bool _normalize = true;

        [SerializeField] private bool _isDirty = true;

        [SerializeField] private Item[] _items = null;

        public override void ClearSeedIsDirty()
        {
            _isDirty = false;
            EditorUtility.SetDirty(this);
        }

        public override EvtfxAudioTable.Item[] CreateEvtfxAudioItems(string directory, int blankSamples)
        {
            if (_isDirty)
            {
                var result = new EvtfxAudioTable.Item[_items.Length];
                for (int i = 0; i < _items.Length; i++)
                {
                    // wavファイルを生成
                    var item = _items[i];
                    var audioClip = item.CreateAudioClip(blankSamples);
                    var targetPath = $@"{directory}\{item.EvtfxName}.g.wav";
                    SaveAsWav(audioClip, targetPath, _normalize);
                    EditorUtility.SetDirty(audioClip);
                    AssetDatabase.ImportAsset(targetPath);

                    // 実際に使用する AudioClip を取得
                    result[i] = new EvtfxAudioTable.Item
                    {
                        EvtfxName = item.EvtfxName,
                        AudioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(targetPath),
                        AudioMixerGroup = _audioMixerGroup,
                        PlayBehaviour = _playBehaviour
                    };
                }
                return result;
            }
            else
            {

                var result = new EvtfxAudioTable.Item[_items.Length];
                for (int i = 0; i < _items.Length; i++)
                {
                    var item = _items[i];
                    var targetPath = $@"{directory}\{item.EvtfxName}.g.wav";

                    // 実際に使用する AudioClip を取得
                    result[i] = new EvtfxAudioTable.Item
                    {
                        EvtfxName = item.EvtfxName,
                        AudioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(targetPath),
                        AudioMixerGroup = _audioMixerGroup,
                        PlayBehaviour = _playBehaviour
                    };

                    if (result[i].AudioClip == null) throw new FileNotFoundException($"AudioClip ({targetPath}) が見つかりません。");
                }
                return result;
            }
        }

        private static void SaveAsWav(AudioClip audioClip, string filePath, bool normalize)
        {
            var samples = new float[audioClip.samples * audioClip.channels];
            audioClip.GetData(samples, 0);
            var peak = samples.Max(sample => Mathf.Abs(sample));

            using var fileStream = new FileStream(filePath, FileMode.Create);
            using var binaryWriter = new BinaryWriter(fileStream);

            // wavファイルのヘッダーぶん余白を作成
            binaryWriter.Write(new byte[44]);

            // 波形データを 16bit PCM で書き込む
            if (normalize)
            {
                foreach (var sample in samples)
                {
                    var value = (short)(sample / peak * short.MaxValue);
                    binaryWriter.Write(value);
                }
            }
            else
            {
                foreach (var sample in samples)
                {
                    var value = (short)(sample * short.MaxValue);
                    binaryWriter.Write(value);
                }
            }

            // 最終的なwavヘッダーを書き込む
            binaryWriter.Seek(0, SeekOrigin.Begin);
            WriteWavHeader(binaryWriter, audioClip, (int)fileStream.Length);
        }

        private static void WriteWavHeader(BinaryWriter writer, AudioClip clip, int dataSize)
        {
            var sampleRate = clip.frequency;
            var channels = (short)clip.channels;
            var byteRate = sampleRate * channels * 2; // 16bit PCM (2 bytes)

            writer.Write(Encoding.UTF8.GetBytes("RIFF"));   //  0   RIFF ヘッダー
                                                            //  2
            writer.Write(dataSize - 8);                     //  4       ファイルサイズ
                                                            //  6
            writer.Write(Encoding.UTF8.GetBytes("WAVE"));   //  8       "WAVE"
                                                            // 10
            writer.Write(Encoding.UTF8.GetBytes("fmt "));   // 12   fmt ヘッダー
                                                            // 14
            writer.Write(16);                               // 16       fmtチャンクサイズ
                                                            // 18
            writer.Write((short)1);                         // 20       PCMフォーマット
            writer.Write(channels);                         // 22       チャンネル数
            writer.Write(sampleRate);                       // 24       サンプリングレート
                                                            // 26
            writer.Write(byteRate);                         // 28       16bit PCM (2 bytes)
                                                            // 30
            writer.Write((short)(channels * 2));            // 32       ブロックアライン
            writer.Write((short)16);                        // 34       bps
            writer.Write(Encoding.UTF8.GetBytes("data"));   // 36   data ヘッダー
                                                            // 38
            writer.Write(dataSize - 44);                    // 40       データサイズ
                                                            // 42
                                                            // 44~      波形データ
        }



        [System.Serializable]
        internal class Item
        {
            [SerializeField] private string _evtfxName;
            public string EvtfxName => string.IsNullOrWhiteSpace(_evtfxName) ? _originalClip.name : _evtfxName;

            [SerializeField] private AudioClip _originalClip;
            public AudioClip OriginalClip => _originalClip;

            [SerializeField] private int _startSample;
            [SerializeField] private int _endSample;

            public AudioClip CreateAudioClip(int blankSamples)
            {
                // DecompressOnLoad でないと GetData で空になるため例外を投げる
                if (_originalClip.loadType != AudioClipLoadType.DecompressOnLoad) throw new System.InvalidOperationException(
                    $"{_originalClip} の {nameof(_originalClip.loadType)} が {AudioClipLoadType.DecompressOnLoad} ではありません。");

                var s = GetSubSample(_startSample);
                var e = GetSubSample(_endSample);
                var trimRange = new RangeInt(s, e - s);

                if (trimRange.length < 0) throw new System.InvalidOperationException(
                    $"{_originalClip} ({trimRange.start} - {trimRange.end}) の切り抜き範囲が不正です。 (長さ: {trimRange.length})");

                if (trimRange.start < 0 || _originalClip.samples <= trimRange.end) throw new System.InvalidOperationException(
                    $"{_originalClip} ({trimRange.start} - {trimRange.end}) の切り抜き範囲はサンプル数 (0 - {_originalClip.samples}) の範囲外です。");

                var sampleRate = _originalClip.frequency;
                var channels = _originalClip.channels;
                var startSample = trimRange.start * channels;
                var endSample = trimRange.end * channels;
                var length = endSample - startSample;
                var blankLength = blankSamples * channels;

                var data = new float[_originalClip.samples * channels];
                _originalClip.GetData(data, 0);

                var trimmedData = new float[blankLength + length];
                System.Array.Copy(data, startSample, trimmedData, blankLength, length);

                var result = AudioClip.Create(_originalClip.name + ".trim.wav", trimmedData.Length / channels, channels, sampleRate, false);
                result.SetData(trimmedData, 0);
                return result;
            }

            private int GetSubSample(int sample)
            {
                if (sample >= 0) return sample;
                else return _originalClip.samples + sample;
            }
        }
    }
}
