using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;
using UnityEngine.Audio;
using UnityEditor;

namespace ListingMF.Audio.Editor
{
    [CreateAssetMenu(menuName = "Listing Menu Foundation/Play/Trimmed Audio Play Seed")]
    public class TrimmedAudioPlaySeed : AudioPlaySeed
    {
        [SerializeField] private AudioMixerGroup _audioMixerGroup = null;

        [SerializeField] private AudioPlayTable.PlayBehaviour _playBehaviour = AudioPlayTable.PlayBehaviour.SE;

        [SerializeField] private Item[] _items = null;

        public override AudioPlayTable.Item[] CreatePlayItems(string directory, int blankSamples)
        {
            var result = new AudioPlayTable.Item[_items.Length];
            for (int i = 0; i < _items.Length; i++)
            {
                // wavファイルを生成
                var item = _items[i];
                var audioClip = item.CreateAudioClip(blankSamples);
                var targetPath = $@"{directory}\{item.PlayName}.wav";
                SaveAsWav(audioClip, targetPath);
                EditorUtility.SetDirty(audioClip);
                AssetDatabase.ImportAsset(targetPath);

                // 実際に使用する AudioClip を取得
                var resultItem = new AudioPlayTable.Item();
                resultItem.AudioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(targetPath);
                resultItem.AudioMixerGroup = _audioMixerGroup;
                resultItem.PlayBehaviour = _playBehaviour;
                result[i] = resultItem;
            }
            return result;
        }

        private static void SaveAsWav(AudioClip audioClip, string filePath)
        {
            var samples = new float[audioClip.samples * audioClip.channels];
            audioClip.GetData(samples, 0);

            using var fileStream = new FileStream(filePath, FileMode.Create);
            using var binaryWriter = new BinaryWriter(fileStream);

            // wavファイルのヘッダーぶん余白を作成
            binaryWriter.Write(new byte[44]);

            // 波形データを 16bit PCM で書き込む
            foreach (var sample in samples)
            {
                var value = (short)(sample * short.MaxValue);
                binaryWriter.Write(value);
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
        private class Item
        {
            [SerializeField] private string _playName;
            public string PlayName => string.IsNullOrWhiteSpace(_playName) ? _originalClip.name : _playName;

            [SerializeField] private AudioClip _originalClip;
            public AudioClip OriginalClip => _originalClip;

            [SerializeField] private int _startSample;
            [SerializeField] private int _endSample;
            private RangeInt TrimRange => new RangeInt(_startSample, _endSample - _startSample);

            public AudioClip CreateAudioClip(int blankSamples)
            {
                // DecompressOnLoad でないと GetData で空になるため例外を投げる
                if (_originalClip.loadType != AudioClipLoadType.DecompressOnLoad) throw new System.InvalidOperationException(
                    $"{_originalClip} の {nameof(_originalClip.loadType)} が {AudioClipLoadType.DecompressOnLoad} ではありません。");

                if (TrimRange.end > _originalClip.samples) throw new System.InvalidOperationException(
                    $"{_originalClip} ({TrimRange.start} - {TrimRange.end}) の切り抜き範囲はサンプル数 ({_originalClip.samples}) の範囲外です。");

                var sampleRate = _originalClip.frequency;
                var channels = _originalClip.channels;
                var startSample = TrimRange.start * channels;
                var endSample = TrimRange.end * channels;
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
        }
    }
}
