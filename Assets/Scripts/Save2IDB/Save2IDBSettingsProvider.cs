#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace Save2IDB
{
    internal class Save2IDBSettingsProvider : SettingsProvider
    {
        private Save2IDBSettings instance;
        private Editor editor;

        private Save2IDBSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null)
            : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        private static SettingsProvider Create()
        {
            var provider = new Save2IDBSettingsProvider("Project/Save2IDB", SettingsScope.Project, new[] { "WebGL" });
            AssemblyReloadEvents.beforeAssemblyReload += provider.BeforeAssemblyReload;
            return provider;
        }

        private void BeforeAssemblyReload()
        {
            // 再読み込みがかかったとき、開いていたオブジェクトを削除しないと見えない所でオブジェクトが溜まってしまう
            if (instance) { Object.DestroyImmediate(instance); }

            instance = null;
            editor = null;
        }

        public override void OnGUI(string searchContext)
        {
            if (instance == null)
            {
                instance = Save2IDBSettings.Load();
                editor = Editor.CreateEditor(instance);
            }

            EditorGUI.BeginChangeCheck();

            instance._overrideDatabaseName = EditorGUILayout.Toggle("OverrideDatabaseName", instance._overrideDatabaseName);

            if (!instance._overrideDatabaseName) { EditorGUI.BeginDisabledGroup(true); }
            instance._databaseName = EditorGUILayout.TextField("DatabaseName", instance._databaseName);
            if (!instance._overrideDatabaseName) { EditorGUI.EndDisabledGroup(); }

            instance._filesObjectStoreName = EditorGUILayout.TextField("FilesObjectStoreName", instance._filesObjectStoreName);

            if (EditorGUI.EndChangeCheck())
            {
                instance.OnValidate();
                instance.Save();
            }
        }
    }
}
#endif
