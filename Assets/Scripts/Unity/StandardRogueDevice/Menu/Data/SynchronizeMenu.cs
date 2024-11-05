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
        };

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            beforeProgress = 0f;
            view.ShowTemplate("ê¢äEÇ∆ìØä˙íÜÅc", manager, arg)
                ?.Append(ProgressBarViewWidget.CreateOption<MMgr, MArg>((manager, arg) =>
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
