using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using UnityEditor;

namespace Roguegard.Editor
{
    public abstract class ScriptableObjectGenerator<T> : ScriptableGenerator
        where T : ScriptableObject
    {
        [SerializeField] private string _generationNameFormat = null;
        public string GenerationNameFormat { get => _generationNameFormat; set => _generationNameFormat = value; }

        protected virtual int Start => 1;
        protected virtual int Length => 999;

        protected sealed override void Generate()
        {
            string oldName = null;
            for (int i = Start; i < Start + Length; i++)
            {
                var targetName = string.Format(_generationNameFormat, i);
                if (targetName == oldName) throw new RogueException("生成したアセットを次の生成で上書きしようとしました。");
                oldName = targetName;

                var thisPath = AssetDatabase.GetAssetPath(this);
                var thisDirectory = Path.GetDirectoryName(thisPath);
                var targetPath = $@"{thisDirectory}\{targetName}.asset";
                if (targetPath == thisPath) throw new RogueException("生成によるジェネレータアセットの上書きは禁止です。");

                var target = AssetDatabase.LoadAssetAtPath<T>(targetPath);
                if (target == null) { target = CreateInstance<T>(); }
                var result = TrySetObject(target, i);
                if (!result) break;

                if (File.Exists(targetPath))
                {
                    EditorUtility.SetDirty(target);
                    AssetDatabase.ImportAsset(targetPath);
                }
                else
                {
                    AssetDatabase.CreateAsset(target, targetPath);
                }
            }
        }

        protected abstract bool TrySetObject(T target, int index);
    }
}
