using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;

namespace Roguegard.Device
{
    public class LoadingListMenu : RogueMenuScreen
    {
        private readonly string text;
        private readonly string buttonText;
        private readonly ListMenuSelectOption<RogueMenuManager, ReadOnlyMenuArg>.HandleClickAction buttonAction;
        private readonly ProgressBarViewWidget.GetProgressFunc<RogueMenuManager, ReadOnlyMenuArg> getProgress;
        private readonly object[] elms;

        private float oldProgress;

        private readonly DialogViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view = new()
        {
        };

        public LoadingListMenu(
            string text, string buttonText,
            ListMenuSelectOption<RogueMenuManager, ReadOnlyMenuArg>.HandleClickAction buttonAction,
            ProgressBarViewWidget.GetProgressFunc<RogueMenuManager, ReadOnlyMenuArg> updateAction = null)
        {
            this.text = text;
            this.buttonText = buttonText;
            this.buttonAction = buttonAction;
            this.getProgress = updateAction ?? delegate { return 1f; };
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            oldProgress = 0f;
            view.Show(text, manager, arg)
                ?.Append(ProgressBarViewWidget.CreateOption<RogueMenuManager, ReadOnlyMenuArg>((manager, arg) =>
                {
                    var progress = getProgress(manager, arg);
                    if (progress >= 1f && oldProgress < 1f) { manager.Done(); }
                    oldProgress = progress;

                    return progress;
                }))
                .Append(ListMenuSelectOption.Create(buttonText, buttonAction))
                .Build();
        }
    }
}
