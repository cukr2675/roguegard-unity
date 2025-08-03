using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class WalkDeviceCommand : IDeviceCommand
    {
        public static WalkDeviceCommand Instance { get; } = new WalkDeviceCommand();

        public bool Execute(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var method = self.Main.InfoSet.Walk;
            return RogueMethodAspectState.Invoke(MainInfoKw.Walk, method, self, user, activationDepth, arg);
        }
    }
}
