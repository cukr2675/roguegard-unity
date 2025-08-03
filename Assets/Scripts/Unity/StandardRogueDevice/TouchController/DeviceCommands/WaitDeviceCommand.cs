using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class WaitDeviceCommand : IDeviceCommand
    {
        public static WaitDeviceCommand Instance { get; } = new WaitDeviceCommand();

        public bool Execute(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var method = self.Main.InfoSet.Wait;
            return RogueMethodAspectState.Invoke(MainInfoKw.Wait, method, self, user, activationDepth, arg);
        }
    }
}
