using Lysionium;

namespace Roguegard.Device
{
    public class LoadingScreen : RogueListuiScreen
    {
        private readonly string text;
        private readonly string buttonText;
        private readonly System.Action<MMgr> buttonAction;
        private readonly System.Func<MMgr, float> getProgress;

        private float oldProgress;

        private readonly DialogViewData<MMgr> view = new()
        {
            BackAnchorSubviewSelector = null,
        };

        public LoadingScreen(
            string text, string buttonText,
            System.Action<MMgr> buttonAction,
            System.Func<MMgr, float> updateAction = null)
        {
            this.text = text;
            this.buttonText = buttonText;
            this.buttonAction = buttonAction;
            this.getProgress = updateAction ?? delegate { return 0f; };
        }

        public LoadingScreen()
        {
            OnOpenScreen += (manager) =>
            {
                oldProgress = 0f;
                view.Show(text, manager)
                ?
                .Tail.Append(ProgressBarWidgetOption.Create<MMgr>((manager) =>
                {
                    var progress = getProgress(manager);
                    if (progress >= 1f && oldProgress < 1f) { manager.Done(); }
                    oldProgress = progress;

                    return progress;
                }))

                .Tail.Option(buttonText, buttonAction)

                .Build();
            };
        }
    }
}
