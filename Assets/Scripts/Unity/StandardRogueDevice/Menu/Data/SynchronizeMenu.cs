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

        private readonly DialogViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
        {
            DialogSubViewName = StandardSubViewTable.OverlayName,
        };

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            beforeProgress = 0f;
            view.Show("世界と同期中…", manager, arg)
                ?.Append(ProgressBarViewWidget.CreateOption<RogueMenuManager, ReadOnlyMenuArg>((manager, arg) =>
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
