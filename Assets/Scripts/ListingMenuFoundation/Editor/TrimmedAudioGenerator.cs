using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;
using UnityEditor;

namespace ListingMF.Editor
{
    [CreateAssetMenu(menuName = "Listing Menu Foundation/Trimmed Audio Generator")]
    public class TrimmedAudioGenerator : ScriptableGenerator
    {
        [Tooltip("音声冒頭に挿入する無音区間のサイズ（WebGL 向け）")]
        [SerializeField] private int _blankSamples = 4410;

        [SerializeField] private Item[] _items = null;

        protected override string IconSearchFilter => "LMF_Icon";

        public override void Generate()
        {
            foreach (var item in _items)
            {
                var targetName = item.OriginalClip.name;

                var thisPath = AssetDatabase.GetAssetPath(this);
                var thisDirectory = Path.GetDirectoryName(thisPath);
                var targetPath = $@"{thisDirectory}\{targetName}.trim.wav";
                if (targetPath == thisPath) throw new System.InvalidOperationException("生成によるジェネレータアセットの上書きは禁止です。");

                var audioClip = GetAudioClip(item);
                AudioClip2Wav(audioClip, targetPath);
                EditorUtility.SetDirty(audioClip);
                AssetDatabase.ImportAsset(targetPath);
            }
        }

        private AudioClip GetAudioClip(Item item)
        {
            var originalClip = item.OriginalClip;
            var trimRange = item.TrimRange;

            // DecompressOnLoad でないと GetData で空になるため例外を投げる
            if (originalClip.loadType != AudioClipLoadType.DecompressOnLoad) throw new System.InvalidOperationException(
                $"{originalClip} の {nameof(originalClip.loadType)} が {AudioClipLoadType.DecompressOnLoad} ではありません。");

            if (trimRange.end > originalClip.samples) throw new System.InvalidOperationException(
                $"{originalClip} ({trimRange.start} - {trimRange.end}) の切り抜き範囲はサンプル数 ({originalClip.samples}) の範囲外です。");

            var sampleRate = originalClip.frequency;
            var channels = originalClip.channels;
            var startSample = trimRange.start * channels;
            var endSample = trimRange.end * channels;
            var length = endSample - startSample;
            var blankLength = _blankSamples * channels;

            var data = new float[originalClip.samples * channels];
            originalClip.GetData(data, 0);

            var trimmedData = new float[blankLength + length];
            System.Array.Copy(data, startSample, trimmedData, blankLength, length);

            var result = AudioClip.Create(originalClip.name + ".trim.wav", trimmedData.Length / channels, channels, sampleRate, false);
            result.SetData(trimmedData, 0);
            return result;
        }

        private static void AudioClip2Wav(AudioClip audioClip, string filePath)
        {
            var samples = new float[audioClip.samples * audioClip.channels];
            audioClip.GetData(samples, 0);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            using (var binaryWriter = new BinaryWriter(fileStream))
            {
                // WAVファイルのヘッダーぶん余白を作成
                binaryWriter.Write(new byte[44]);

                // 波形データを 16bit PCM で書き込む
                foreach (var sample in samples)
                {
                    var value = (short)(sample * short.MaxValue);
                    binaryWriter.Write(value);
                }

                // 最終的なWAVヘッダーを書き込む
                binaryWriter.Seek(0, SeekOrigin.Begin);
                WriteWavHeader(binaryWriter, audioClip, (int)fileStream.Length);
            }
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
            [SerializeField] private AudioClip _originalClip;
            public AudioClip OriginalClip => _originalClip;

            [SerializeField] private int _startSample;
            [SerializeField] private int _endSample;
            public RangeInt TrimRange => new RangeInt(_startSample, _endSample - _startSample);
        }
    }
}
