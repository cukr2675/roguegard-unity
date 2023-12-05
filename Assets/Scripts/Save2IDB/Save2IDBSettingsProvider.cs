#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace Save2IDB
{
    internal class Save2IDBSettingsProvider : AssetSettingsProvider
    {
        private static Save2IDBSettings instance;

        private Save2IDBSettingsProvider(string path, System.Func<Editor> editorCreator, IEnumerable<string> keywords = null)
            : base(path, editorCreator, keywords)
        {
        }

        [SettingsProvider]
        private static SettingsProvider Create()
        {
            var provider = new Save2IDBSettingsProvider("Project/Save2IDB", () => GetEditor(), new[] { "WebGL" });
            return provider;
        }

        private static Editor GetEditor()
        {
            if (instance == null)
            {
                instance = Save2IDBSettings.Load();
                AssemblyReloadEvents.beforeAssemblyReload += () => BeforeAssemblyReload();
            }
            return Editor.CreateEditor(instance);
        }

        private static void BeforeAssemblyReload()
        {
            // 再読み込みがかかったとき、開いていたオブジェクトを削除しないと見えない所でオブジェクトが溜まってしまう
            if (instance) { Object.DestroyImmediate(instance); }

            instance = null;
        }

		public override void OnGUI(string searchContext)
		{
            EditorGUI.BeginChangeCheck();

            base.OnGUI(searchContext);

            if (EditorGUI.EndChangeCheck())
            {
                // instance.OnValidate は base.OnGUI 内で呼び出される
                instance.Save();
            }
		}
	}
}
#endif
