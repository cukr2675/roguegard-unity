using System.IO;
using UnityEditor;

namespace Lysionium.Editor
{
    internal static class MenuScreenScriptCreator
    {
        const string templatePath = "Temp/Lysionium__MenuScreen Script-NewMenuScreen.cs.txt";
        const string templateContents =
@"using Lysionium;

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

        [MenuItem("Assets/Create/Lysionium/MenuScreen Script")]
        private static void CreateMenuScreenScript()
        {
            File.WriteAllText(templatePath, templateContents);

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, "NewMenuScreen.cs");
        }
    }
}