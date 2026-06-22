using Lysionium.Editor;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Lysionium.Audio.Editor
{
    [CreateAssetMenu(menuName = "Lysionium/Evtfx Audio/Table Generator", fileName = "AppAudioTable")]
    public class EvtfxAudioTableGenerator : ScriptableGenerator
    {
        [SerializeField] private Platform[] _platforms = null;

        [SerializeField] private EvtfxAudioSeed[] _seeds = null;

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

                var target = AssetDatabase.LoadAssetAtPath<EvtfxAudioTable>(targetPath);
                if (target == null) { target = CreateInstance<EvtfxAudioTable>(); }

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

        private void SetTo(EvtfxAudioTable evtfxAudioTable, Platform platform, string directory)
        {
            evtfxAudioTable.BlankSamples = platform.BlankSamples;
            var items = new List<EvtfxAudioTable.Item>();
            foreach (var seed in _seeds)
            {
                items.AddRange(seed.CreateEvtfxAudioItems(directory, platform.BlankSamples));
            }
            evtfxAudioTable.SetItems(items);
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
