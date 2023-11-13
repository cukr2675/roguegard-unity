using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard.Device
{
    /// <summary>
    /// <see cref="IRogueDevice"/> から <see cref="RogueObj"/> を操作するインターフェース
    /// </summary>
    public interface IDeviceCommandAction : IActiveRogueMethodCaller
    {
        /// <summary>
        /// コマンドを実行するメソッド。
        /// <see cref="RogueMethodAspectState"/> から実行される <see cref="IRogueMethod"/> と違い、
        /// このメソッドは <see cref="RogueMethodAspectState"/> の実行が目的。
        /// </summary>
        bool CommandInvoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg);
    }
}
