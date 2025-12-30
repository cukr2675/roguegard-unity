using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lysionium
{
    public class LoadSceneFadeOutScreen : DelegateListuiScreen<IListuiManager, IListuiArg>
    {
        private readonly FadeOutInViewData<IListuiManager, IListuiArg> view = new()
        {
        };

        public LoadSceneFadeOutScreen(string nextSceneName, System.Action<AsyncOperation> onLoadSceneCompleted = null, string style = null)
        {
            OnOpenScreen += (manager, arg) =>
            {
                view.FadeOut(manager, arg)
                ?
                .InitIf(
                    style != null, _ => _
                    .Append(StyleMetaWidgetOption.Create(style)))

                .OnFadeOutCompleted((manager, arg) =>
                {
                    var loadSceneOperation = SceneManager.LoadSceneAsync(nextSceneName);
                    loadSceneOperation.completed += onLoadSceneCompleted;
                })
                .Build();
            };
        }
    }
}
