using Roguegard;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class TurnDeviceCommand : IDeviceCommand
    {
        public static TurnDeviceCommand Instance { get; } = new TurnDeviceCommand();

        private static readonly TurnRogueMethod turnRogueMethod = new();

        public bool Execute(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            return turnRogueMethod.Invoke(self, user, activationDepth, arg);
            //return RogueMethodAspectState.Invoke(StdKw.Turn, turnRogueMethod, self, user, activationDepth, arg);
        }

        private class TurnRogueMethod : IActiveRogueMethod
        {
            public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                self.Main.Stats.Direction = RogueMethodUtility.GetTargetDirection(self, arg);
                MainCharacterWorkUtility.TryAddTurn(self);
                return false;
            }
        }
    }
}
