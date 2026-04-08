using Lysionium;
using Roguegard.Device;

namespace RoguegardUnity
{
    internal class SynchronizeMenu : RogueListuiScreen
    {
        public bool Interrupt { get; private set; }
        public float Progress { get; set; }

        private float beforeProgress;

        private readonly DialogViewData<MMgr> view = new()
        {
            DialogSubviewSelector = m => m.Overlay,
            BackAnchorSubviewSelector = null,
        };

        public SynchronizeMenu()
        {
            OnOpenScreen += (manager) =>
            {
                beforeProgress = 0f;
                view.Show("世界と同期中…", manager)
                ?
                .Tail.Append(ProgressBarWidgetOption.Create<MMgr>((manager) =>
                {
                    if (Progress >= 1f && beforeProgress < 1f) { manager.Done(); }
                    beforeProgress = Progress;

                    return Progress;
                }))

                .Tail.Option("同期を中止", _ => Interrupt = true)

                .Build();
            };
        }
    }
}
