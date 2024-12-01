using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using Roguegard.Device;
using Roguegard;

namespace RoguegardUnity
{
    internal class SynchronizeMenu : RogueMenuScreen
    {
        public bool Interrupt { get; private set; }
        public float Progress { get; set; }

        private float beforeProgress;

        private readonly DialogViewTemplate<MMgr, MArg> view = new()
        {
            DialogSubViewName = StandardSubViewTable.OverlayName,
            BackAnchorSubViewName = null,
        };

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            beforeProgress = 0f;
            view.ShowTemplate("世界と同期中…", manager, arg)
                ?
                .Append(ProgressBarViewWidget.CreateOption<MMgr, MArg>((manager, arg) =>
                {
                    if (Progress >= 1f && beforeProgress < 1f) { manager.Done(); }
                    beforeProgress = Progress;

                    return Progress;
                }))

                .AppendSelectOption("同期を中止", (manager, arg) => Interrupt = true)

                .Build();
        }
    }
}
