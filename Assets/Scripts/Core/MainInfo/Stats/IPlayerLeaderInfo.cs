using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// プレイヤーパーティのリーダー専用のエフェクト。
    /// ターン経過でリーダーの満腹度を消費させたり、パーティメンバーを自然回復させたりするために使う。
    /// </summary>
    public interface IPlayerLeaderInfo
    {
        void Move(RogueObj from, RogueObj to);

        void Close(RogueObj self);
    }
}
