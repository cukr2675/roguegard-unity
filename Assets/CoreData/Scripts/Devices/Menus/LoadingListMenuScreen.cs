using Lysionium;

namespace Roguegard.Device
{
    public class LoadingListMenuScreen : RogueMenuScreen
    {
        private readonly string text;
        private readonly string buttonText;
        private readonly HandleClickElement<MMgr, MArg> buttonAction;
        private readonly ProgressBarViewWidget.GetProgress<MMgr, MArg> getProgress;
        private readonly object[] elms;

        private float oldProgress;

        private readonly DialogViewTemplate<MMgr, MArg> view = new()
        {
            BackAnchorSubviewName = null,
        };

        public LoadingListMenuScreen(
            string text, string buttonText,
            HandleClickElement<MMgr, MArg> buttonAction,
            ProgressBarViewWidget.GetProgress<MMgr, MArg> updateAction = null)
        {
            this.text = text;
            this.buttonText = buttonText;
            this.buttonAction = buttonAction;
            this.getProgress = updateAction ?? delegate { return 0f; };
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            oldProgress = 0f;
            view.ShowTemplate(text, manager, arg)
                ?.Tail(ProgressBarViewWidget.CreateOption<MMgr, MArg>((manager, arg) =>
                {
                    var progress = getProgress(manager, arg);
                    if (progress >= 1f && oldProgress < 1f) { manager.Done(); }
                    oldProgress = progress;

                    return progress;
                }))
                .Tail(SelectOption.Create(buttonText, buttonAction))
                .Build();
        }
    }
}
