using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// <see cref="RogueObj"/> に情報を付与するインターフェース。
    /// リストで保存する <see cref="IRogueEffect"/> や <see cref="IStatusEffect"/> と違い <see cref="object.GetType"/> をキーとする連想配列で保存される。
    /// </summary>
    [Objforming.RequireRelationalComponent]
    public interface IRogueObjInfo
    {
        /// <summary>
        /// この値が true のときシリアル化から除外される。
        /// 常に除外し、かつ <see cref="IRogueEffect"/> を実装しないクラスでは
        /// <see cref="Objforming.IgnoreRequireRelationalComponentAttribute"/> と併用する。
        /// </summary>
        bool IsExclusedWhenSerialize { get; }

        /// <summary>
        /// <paramref name="other"/> == null のときも true を返す場合、スタック判定には関わらない。
        /// </summary>
        bool CanStack(IRogueObjInfo other);

        /// <summary>
        /// <paramref name="self"/> を <paramref name="clonedSelf"/> に置き換えたクローンにあたるインスタンスを取得する。
        /// </summary>
        IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf);

        /// <summary>
        /// <see cref="DeepOrShallowCopy(RogueObj, RogueObj)"/> で複製したあと、
        /// このメソッドで複製前 <see cref="RogueObj"/> を複製後 <see cref="RogueObj"/> に置き換えることで、
        /// <see cref="RogueObj"/> の親子関係を保つ。
        /// </summary>
        IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj);
    }
}
