using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

namespace Roguegard
{
    /// <summary>
    /// 等価判定は <see cref="System.IEquatable{T}.Equals(T)"/> でおこなう。
    /// <see cref="System.IEquatable{T}.Equals(T)"/> で一致するものを同一インスタンスとしてシリアル化する。
    /// </summary>
    [Objforming.RequireRelationalComponent]
    public interface IRogueTile : IRogueSprite, System.IEquatable<IRogueTile>
    {
        /// <summary>
        /// ColoredRogueTile などのタイルを加工するタイルに、
        /// 常に第一ソースを参照させる強制力を持たせるためのプロパティ。
        /// （ColoredRogueTile が ColoredRogueTile を参照できないようにする）
        /// </summary>
        IRogueTileInfo Info { get; }

        // クローン生成は実装しない。
        // ・タイルのほとんどは面で処理（２重ループ処理）するため、一つ一つ複製すると重くなる
        // ・使いまわされたクローン可能タイルを複製するとインスタンス数が増大する
    }
}
