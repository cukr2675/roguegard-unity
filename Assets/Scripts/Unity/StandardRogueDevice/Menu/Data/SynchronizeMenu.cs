using Lysionium;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class SynchronizeMenu : RogueMenuScreen
    {
        public bool Interrupt { get; private set; }
        public float Progress { get; set; }

        private float beforeProgress;

        private readonly DialogViewData<MMgr, MArg> view = new()
        {
            DialogSubviewName = StandardSubviewTable.OverlayName,
            BackAnchorSubviewName = null,
        };

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            beforeProgress = 0f;
            view.Show("世界と同期中…", manager, arg)
                ?
                .Tail(ProgressBarWidgetOption.Create<MMgr, MArg>((manager, arg) =>
                {
                    if (Progress >= 1f && beforeProgress < 1f) { manager.Done(); }
                    beforeProgress = Progress;

                    return Progress;
                }))

                .Option("同期を中止", (manager, arg) => Interrupt = true)

                .Build();
        }
    }
}
