using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using UnityEditor;

namespace OchalikeSprites.Editor
{
    public abstract class ScriptableObjectGenerator<T> : ScriptableGenerator
        where T : ScriptableObject
    {
        [SerializeField] private string _generationNameFormat = null;
        public string GenerationNameFormat { get => _generationNameFormat; set => _generationNameFormat = value; }

        // Start = 0 よりも 1 のほうが TrySetObject の戻り値 false によってデータが一つも出力されないといったことが起こりにくい。
        // 一件も出力されないのは様々な原因が考えられるが、一つ足りないだけなら比較的直しやすい。
        protected virtual int Start => 1;

        // 実装ミス対策でループ回数に制限をつける
        protected virtual int Length => 99;

        public sealed override void Generate()
        {
            string oldName = null;
            for (int i = Start; i < Start + Length; i++)
            {
                var targetName = string.Format(_generationNameFormat, i);
                if (targetName == oldName) throw new System.InvalidOperationException("生成したアセットを次の生成で上書きしようとしました。");
                oldName = targetName;

                var thisPath = AssetDatabase.GetAssetPath(this);
                var thisDirectory = Path.GetDirectoryName(thisPath);
                var targetPath = $@"{thisDirectory}\{targetName}.asset";
                if (targetPath == thisPath) throw new System.InvalidOperationException("生成によるジェネレータアセットの上書きは禁止です。");

                var target = AssetDatabase.LoadAssetAtPath<T>(targetPath);
                if (target == null) { target = CreateInstance<T>(); }
                var result = TrySetObject(target, i);
                if (!result) break;

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
                // 最後にまとめてインポート
                var thisPath = AssetDatabase.GetAssetPath(this);
                var thisDirectory = Path.GetDirectoryName(thisPath);
                AssetDatabase.ImportAsset(thisDirectory);
            }
        }

        protected abstract bool TrySetObject(T target, int index);
    }
}
