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
            private ISubviewStateProvider subviewStateProvider;

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                manager.Dopesheet.Show(System.Array.Empty<object>(), null, manager, arg, ref subviewStateProvider);
            }
        }
    }
}
