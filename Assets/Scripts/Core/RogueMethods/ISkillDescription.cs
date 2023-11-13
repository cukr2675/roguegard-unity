using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface ISkillDescription
    {
        IRogueMethodTarget Target { get; }
        IRogueMethodRange Range { get; }
        int RequiredMP { get; }
        Spanning<IKeyword> AmmoCategories { get; }

        /// <param name="additionalEffect">状態異常などダメージ以外の追加効果</param>
        int GetATK(RogueObj self, out bool additionalEffect);
    }
}
