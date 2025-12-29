using Lysionium;
using Roguegard.Device;

namespace Roguegard
{
    public class MotionGrapherBeApplied : BaseApplyRogueMethod
    {
        private static MotionGrapherScreen motionGrapherScreen;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            motionGrapherScreen ??= new();
            var choreographerInfo = MotionGrapherInfo.Get(self);
            if (choreographerInfo == null)
            {
                MotionGrapherInfo.SetTo(self);
                choreographerInfo = MotionGrapherInfo.Get(self);
            }

            RogueDevice.Primary.AddScreen(motionGrapherScreen, user, null, new(other: choreographerInfo));
            return false;
        }

        private class MotionGrapherScreen : RogueListuiScreen
        {
            private ISubviewStateProvider subviewStateProvider;

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                manager.Dopesheet.Show(System.Array.Empty<object>(), null, manager, arg, ref subviewStateProvider);
            }
        }
    }
}
