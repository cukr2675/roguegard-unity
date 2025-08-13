using Roguegard;
using UnityEngine;

namespace RoguegardUnity
{
    /// <summary>
    /// <see cref="RogueObj"/> と移動先を見て移動を停止するインターフェース。
    /// </summary>
    public interface IPositionedWalkStopper
    {
        bool GetStop(RogueObj self, Vector2Int targetPosition);
    }
}
