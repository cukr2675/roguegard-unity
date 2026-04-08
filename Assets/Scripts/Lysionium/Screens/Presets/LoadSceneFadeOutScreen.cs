using UnityEngine;
using UnityEngine.SceneManagement;

namespace Lysionium
{
    public class LoadSceneFadeOutScreen : DelegateListuiScreen<IListuiManager>
    {
        private readonly FadeOutInViewData<IListuiManager> view = new()
        {
        };

        public LoadSceneFadeOutScreen(string nextSceneName, System.Action<AsyncOperation> onLoadSceneCompleted = null, string style = null)
        {
            OnOpenScreen += (manager) =>
            {
                view.FadeOut(manager)
                ?
                .InitIf(
                    style != null, _ => _
                    .Append(StyleMetaWidgetOption.Create(style)))

                .OnFadeOutCompleted(_ =>
                {
                    var loadSceneOperation = SceneManager.LoadSceneAsync(nextSceneName);
                    loadSceneOperation.completed += onLoadSceneCompleted;
                })
                .Build();
            };
        }
    }
}
