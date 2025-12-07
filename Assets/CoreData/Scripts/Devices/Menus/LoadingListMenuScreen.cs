using Lysionium;

namespace Roguegard.Device
{
    public class LoadingListMenuScreen : RogueMenuScreen
    {
        private readonly string text;
        private readonly string buttonText;
        private readonly ClickItemHandler<MMgr, MArg> buttonAction;
        private readonly System.Func<MMgr, MArg, float> getProgress;

        private float oldProgress;

        private readonly DialogViewData<MMgr, MArg> view = new()
        {
            BackAnchorSubviewSelector = null,
        };

        public LoadingListMenuScreen(
            string text, string buttonText,
            ClickItemHandler<MMgr, MArg> buttonAction,
            System.Func<MMgr, MArg, float> updateAction = null)
        {
            this.text = text;
            this.buttonText = buttonText;
            this.buttonAction = buttonAction;
            this.getProgress = updateAction ?? delegate { return 0f; };
        }

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            oldProgress = 0f;
            view.Show(text, manager, arg)
                ?
                .Tail.Append(ProgressBarWidgetOption.Create<MMgr, MArg>((manager, arg) =>
                {
                    var progress = getProgress(manager, arg);
                    if (progress >= 1f && oldProgress < 1f) { manager.Done(); }
                    oldProgress = progress;

                    return progress;
                }))

                .Tail.Option(buttonText, buttonAction)

                .Build();
        }
    }
}
