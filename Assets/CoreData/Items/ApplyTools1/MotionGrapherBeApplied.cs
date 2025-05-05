using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Lysionium;
using Roguegard.Device;

namespace Roguegard
{
    public class MotionGrapherBeApplied : BaseApplyRogueMethod
    {
        private static Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            menu ??= new();
            var choreographerInfo = MotionGrapherInfo.Get(self);
            if (choreographerInfo == null)
            {
                MotionGrapherInfo.SetTo(self);
                choreographerInfo = MotionGrapherInfo.Get(self);
            }

            RogueDevice.Primary.AddMenu(menu, user, null, new(other: choreographerInfo));
            return false;
        }

        private class Menu : RogueMenuScreen
        {
            private IElementsSubviewStateProvider subviewStateProvider;

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var dopesheetSubview = RoguegardSubviews.GetDopesheet(manager);
                dopesheetSubview.Show(System.Array.Empty<object>(), null, manager, arg, ref subviewStateProvider);
            }
        }
    }
}
