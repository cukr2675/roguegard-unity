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
            view.Show("ê¢äEÇ∆ìØä˙íÜÅc", manager, arg)
                ?.Append(ProgressBarViewWidget.CreateOption<RogueMenuManager, ReadOnlyMenuArg>((manager, arg) =>
                {
                    if (Progress >= 1f && beforeProgress < 1f) { manager.Done(); }
                    beforeProgress = Progress;

                    return Progress;
                }))
                .AppendSelectOption("ìØä˙ÇíÜé~", (manager, arg) => Interrupt = true)
                .Build();
        }
    }
}
