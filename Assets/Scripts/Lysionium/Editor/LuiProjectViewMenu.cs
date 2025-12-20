using System.IO;
using UnityEditor;

namespace Lysionium.Editor
{
    internal class LuiProjectViewMenu : AssetModificationProcessor
    {
        private const string listMenuStartupNewFileName = "NewListMenuStartup.cs";
        private const string listMenuStartupTemplatePath = "Temp/Lysionium__ListMenuStartup Script-NewListMenuStartup.cs.txt";
        private const string listMenuStartupTemplateContents =
@"using Lysionium;
using UnityEngine;

#ROOTNAMESPACEBEGIN#
public class #SCRIPTNAME# : MonoBehaviour
{
    protected virtual void Start()
    {
        var manager = FindAnyObjectByType<StandardListMenuManager>();
        manager.Initialize();
        manager.PushInitialMenuScreen(new MenuScreen(), null);
    }

    private class MenuScreen : MenuScreen<StandardListMenuManager>
    {
        private readonly MainMenuViewData<StandardListMenuManager> view = new()
        {
        };

        public override void OpenScreen(StandardListMenuManager manager, IListMenuArg arg)
        {
            view.Show(manager, arg)
                ?

                .Option(""Hello"", new ChoicesMenuScreen<StandardListMenuManager>(""'Hello' was clicked."").Back())
                .Option(""World"", new ChoicesMenuScreen<StandardListMenuManager>(""'World' was clicked."").Back())

                .Build();
        }
    }
}
#ROOTNAMESPACEEND#
";

        private const string menuScreenNewFileName = "NewMenuScreen.cs";
        private const string menuScreenTemplatePath = "Temp/Lysionium__MenuScreen Script-NewMenuScreen.cs.txt";
        private const string menuScreenTemplateContents =
@"using Lysionium;
using UnityEngine;

#ROOTNAMESPACEBEGIN#
public class #SCRIPTNAME# : MenuScreen<StandardListMenuManager>
{
    private readonly MainMenuViewData<StandardListMenuManager> view = new()
    {
    };

    public override void OpenScreen(StandardListMenuManager manager, IListMenuArg arg)
    {
        view.Show(manager, arg)
            ?

            .Option(""Hello"", new ChoicesMenuScreen<StandardListMenuManager>(""'Hello' was clicked."").Back())
            .Option(""World"", new ChoicesMenuScreen<StandardListMenuManager>(""'World' was clicked."").Back())

            .Build();
    }
}
#ROOTNAMESPACEEND#
";

        private static System.Action<string> OnCreateClass;

        internal static void CreateListMenuStartupScript(System.Action<string> onCreateClass)
        {
            File.WriteAllText(listMenuStartupTemplatePath, listMenuStartupTemplateContents); // テンプレートファイルを書き込む
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(listMenuStartupTemplatePath, listMenuStartupNewFileName); // スクリプト作成
            OnCreateClass += onCreateClass;
        }

        // アセットが作成される直前に呼ばれる
        internal static string OnWillCreateAsset(string assetPath)
        {
            if (Path.GetExtension(assetPath) == ".cs")
            {
                OnCreateClass?.Invoke(Path.GetFileNameWithoutExtension(assetPath));
            }
            return assetPath;
        }

        [MenuItem("Assets/Create/Lysionium/ListMenuStartup Script")]
        private static void CreateListMenuStartupScript()
        {
            File.WriteAllText(listMenuStartupTemplatePath, listMenuStartupTemplateContents); // テンプレートファイルを書き込む
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(listMenuStartupTemplatePath, listMenuStartupNewFileName); // スクリプト作成
        }

        [MenuItem("Assets/Create/Lysionium/MenuScreen Script")]
        private static void CreateMenuScreenScript()
        {
            File.WriteAllText(menuScreenTemplatePath, menuScreenTemplateContents); // テンプレートファイルを書き込む
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(menuScreenTemplatePath, menuScreenNewFileName); // スクリプト作成
        }
    }
}