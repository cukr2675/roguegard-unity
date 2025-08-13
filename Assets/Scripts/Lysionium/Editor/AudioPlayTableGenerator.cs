using Lysionium.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Lysionium.Audio.Editor
{
    [CreateAssetMenu(menuName = "Lysionium/Play/Audio Play Table Generator")]
    public class AudioPlayTableGenerator : ScriptableGenerator
    {
        [SerializeField] private Platform[] _platforms = null;

        [SerializeField] private AudioPlaySeed[] _seeds = null;

        protected override string IconSearchFilter => "LUI_Icon";

        protected virtual void Reset()
        {
            _platforms = new[]
            {
                new Platform() { PlatformName = "Default", BlankSamples = 0 },
                new Platform() { PlatformName = "Web", BlankSamples = 4410 },
            };
        }

        public override void Generate()
        {
            var thisPath = AssetDatabase.GetAssetPath(this);
            var thisDirectory = Path.GetDirectoryName(thisPath);

            foreach (var platform in _platforms)
            {
                var targetName = $"{name}.{platform.PlatformName}";
                var targetPath = $@"{thisDirectory}\{targetName}.asset";
                if (targetPath == thisPath) throw new System.InvalidOperationException("生成によるジェネレータアセットの上書きは禁止です。");

                var target = AssetDatabase.LoadAssetAtPath<AudioPlayTable>(targetPath);
                if (target == null) { target = CreateInstance<AudioPlayTable>(); }

                var platformDirectory = $@"{thisDirectory}\{platform.PlatformName}";
                Directory.CreateDirectory(platformDirectory);
                SetTo(target, platform, platformDirectory);

                if (File.Exists(targetPath))
                {
                    EditorUtility.SetDirty(target);
                }
                else
                {
                    AssetDatabase.CreateAsset(target, targetPath);
                }
            }
            {
                // ダーティフラグを解除
                foreach (var seed in _seeds)
                {
                    seed.ClearSeedIsDirty();
                }

                // 最後にまとめてインポート
                AssetDatabase.ImportAsset(thisDirectory);
            }
        }

        private void SetTo(AudioPlayTable audioPlayTable, Platform platform, string directory)
        {
            audioPlayTable.BlankSamples = platform.BlankSamples;
            var items = new List<AudioPlayTable.Item>();
            foreach (var seed in _seeds)
            {
                items.AddRange(seed.CreatePlayItems(directory, platform.BlankSamples));
            }
            audioPlayTable.SetItems(items);
        }



        [System.Serializable]
        private class Platform
        {
            [SerializeField] private string _platformName;
            public string PlatformName { get => _platformName; set => _platformName = value; }

            [Tooltip("音声冒頭に挿入する無音区間のサイズ（WebGL向け）")]
            [SerializeField] private int _blankSamples;
            public int BlankSamples { get => _blankSamples; set => _blankSamples = value; }
        }
    }
}
