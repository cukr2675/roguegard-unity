using Roguegard.Extensions;

namespace Roguegard.Device
{
    /// <summary>
    /// <see cref="IRogueDevice"/> から <see cref="RogueObj"/> を操作するインターフェース
    /// </summary>
    public interface IDeviceCommand : IActiveRogueMethodCaller
    {
        /// <summary>
        /// コマンドを実行するメソッド。
        /// <see cref="RogueMethodAspectState"/> から実行される <see cref="IRogueMethod"/> と違い、
        /// このメソッドは <see cref="RogueMethodAspectState"/> の実行が目的。
        /// </summary>
        bool Execute(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);
    }
}
