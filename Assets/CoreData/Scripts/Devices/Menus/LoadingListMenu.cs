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
        private readonly HandleClickElement<MMgr, MArg> buttonAction;
        private readonly ProgressBarViewWidget.GetProgress<MMgr, MArg> getProgress;
        private readonly object[] elms;

        private float oldProgress;

        private readonly DialogViewTemplate<MMgr, MArg> view = new()
        {
        };

        public LoadingListMenu(
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
                ?.Append(ProgressBarViewWidget.CreateOption<MMgr, MArg>((manager, arg) =>
                {
                    var progress = getProgress(manager, arg);
                    if (progress >= 1f && oldProgress < 1f) { manager.Done(); }
                    oldProgress = progress;

                    return progress;
                }))
                .Append(SelectOption.Create(buttonText, buttonAction))
                .Build();
        }
    }
}
