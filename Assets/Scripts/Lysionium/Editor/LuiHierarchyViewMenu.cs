using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace Lysionium.Editor
{
    internal class LuiHierarchyViewMenu : ScriptableObject
    {
        [SerializeField] private StandardListMenuManager _standardListMenuManagerPrefab;

        [MenuItem("GameObject/UI/Lysionium/List Menu Startup (and script)", false, 2675)]
        private static void CreateListMenuStartup()
        {
            // StandardListMenuManager が無ければ生成する
            EnsureStandardListMenuManagerExists();

            // ListMenuStartup コンポーネントをアタッチするためのプレースホルダーオブジェクトを生成する
            var parent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;
            var instance = new GameObject("[ListMenuStartupPlaceholder]");
            instance.transform.SetParent(parent, false);
            Undo.RegisterCreatedObjectUndo(instance, $"Create ListMenuStartup");

            // ListMenuStartup コンポーネントクラスを作成させる
            LuiProjectViewMenu.CreateListMenuStartupScript(className =>
            {
                // 作成後、アタッチするクラスを探すためのヒントオブジェクトを生成する
                var classNameHintObject = new GameObject(className);
                classNameHintObject.transform.SetParent(instance.transform, false);
            });
        }

        // コンパイル後に実行される
        [InitializeOnLoadMethod]
        private static void AutoAddListMenuStartupComponent()
        {
            // CreateListMenuStartup() で生成したプレースホルダーオブジェクトを取得する
            var instance = GameObject.Find("[ListMenuStartupPlaceholder]");
            if (instance == null || instance.transform.childCount == 0) return; // 存在しない/不正な状態であれば何もしない

            // CreateListMenuStartup() で生成したヒントオブジェクトを取得し、そこからクラスを取得する
            var classNameHintObject = instance.transform.GetChild(0).gameObject;
            var className = classNameHintObject.name;
            var classType = System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name == className);
            if (classType == null)
            {
                Debug.LogError($"\"{className}\" コンポーネントクラスが存在しません。"); // 見つからない場合はエラー終了
                return;
            }

            // プレースホルダーオブジェクトを Startup オブジェクトに整形する
            DestroyImmediate(classNameHintObject);
            instance.name = className;
            Undo.AddComponent(instance, classType);
            Debug.Log($"{instance} に {classType} コンポーネントを追加しました。");
        }

        [MenuItem("GameObject/UI/Lysionium/Standard List Menu Manager", false, 2676)]
        private static void CreateStandardListMenuManager()
        {
            // EventSystem が無ければ生成する
            EnsureEventSystemExists();

            // プレハブから StandardListMenuManager オブジェクトを生成する
            var hierarchyViewMenu = CreateInstance<LuiHierarchyViewMenu>();
            try
            {
                var parent = Selection.activeGameObject != null ? Selection.activeGameObject.transform : null;
                var instance = (StandardListMenuManager)PrefabUtility.InstantiatePrefab(hierarchyViewMenu._standardListMenuManagerPrefab, parent);
                instance.gameObject.name = "StandardListMenuManager";
                Undo.RegisterCreatedObjectUndo(instance.gameObject, $"Create {instance.name}");
            }
            finally
            {
                DestroyImmediate(hierarchyViewMenu);
            }
        }

        private static void EnsureEventSystemExists()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() != null) return; // Prefab モード中は何もしない
            if (FindAnyObjectByType<EventSystem>() != null) return; // 既にあれば何もしない

            // 存在しなければ EventSystem を作成
            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();
            Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
        }

        private static void EnsureStandardListMenuManagerExists()
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() != null) return; // Prefab モード中は何もしない
            if (FindAnyObjectByType<StandardListMenuManager>() != null) return; // 既にあれば何もしない

            // 存在しなければ StandardListMenuManager を作成
            CreateStandardListMenuManager();
        }
    }
}
