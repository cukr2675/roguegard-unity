using System.IO;
using UnityEditor;

namespace Lysionium.Editor
{
    internal class LuiProjectViewMenu : AssetModificationProcessor
    {
        private const string listuiStartupNewFileName = "NewListuiStartup.cs";
        private const string listuiStartupTemplatePath = "Temp/Lysionium__List UI Startup Script-NewListuiStartup.cs.txt";
        private const string listuiStartupTemplateContents =
@"using Lysionium;
using UnityEngine;

#ROOTNAMESPACEBEGIN#
public class #SCRIPTNAME# : MonoBehaviour
{
    protected virtual void Start()
    {
        var manager = FindAnyObjectByType<StandardListuiManager>();
        manager.Initialize();
        manager.PushInitialScreen(new InitialScreen(), null);
    }

    private class InitialScreen : ListuiScreen<StandardListuiManager>
    {
        private readonly MainMenuViewData<StandardListuiManager> view = new()
        {
        };

        public override void OpenScreen(StandardListuiManager manager, IListuiArg arg)
        {
            view.Show(manager, arg)
                ?

                .Option(""Hello"", new ChoicesScreen<StandardListuiManager>(""'Hello' was clicked."").Back())
                .Option(""World"", new ChoicesScreen<StandardListuiManager>(""'World' was clicked."").Back())

                .Build();
        }
    }
}
#ROOTNAMESPACEEND#
";

        private const string listuiScreenNewFileName = "NewListuiScreen.cs";
        private const string listuiScreenTemplatePath = "Temp/Lysionium__List UI Screen Script-NewListuiScreen.cs.txt";
        private const string listuiScreenTemplateContents =
@"using Lysionium;
using UnityEngine;

#ROOTNAMESPACEBEGIN#
public class #SCRIPTNAME# : ListuiScreen<StandardListuiManager>
{
    private readonly MainMenuViewData<StandardListuiManager> view = new()
    {
    };

    public override void OpenScreen(StandardListuiManager manager, IListuiArg arg)
    {
        view.Show(manager, arg)
            ?

            .Option(""Hello"", new ChoicesScreen<StandardListuiManager>(""'Hello' was clicked."").Back())
            .Option(""World"", new ChoicesScreen<StandardListuiManager>(""'World' was clicked."").Back())

            .Build();
    }
}
#ROOTNAMESPACEEND#
";

        private static System.Action<string> OnCreateClass;

        internal static void CreateListuiStartupScript(System.Action<string> onCreateClass)
        {
            File.WriteAllText(listuiStartupTemplatePath, listuiStartupTemplateContents); // テンプレートファイルを書き込む
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(listuiStartupTemplatePath, listuiStartupNewFileName); // スクリプト作成
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

        [MenuItem("Assets/Create/Lysionium/List-UI Startup Script")]
        private static void CreateListuiStartupScript()
        {
            File.WriteAllText(listuiStartupTemplatePath, listuiStartupTemplateContents); // テンプレートファイルを書き込む
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(listuiStartupTemplatePath, listuiStartupNewFileName); // スクリプト作成
        }

        [MenuItem("Assets/Create/Lysionium/List-UI Screen Script")]
        private static void CreateListuiScreenScript()
        {
            File.WriteAllText(listuiScreenTemplatePath, listuiScreenTemplateContents); // テンプレートファイルを書き込む
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(listuiScreenTemplatePath, listuiScreenNewFileName); // スクリプト作成
        }
    }
}