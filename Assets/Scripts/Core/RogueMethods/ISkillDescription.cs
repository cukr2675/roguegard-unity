using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface ISkillDescription
    {
        IRogueMethodTarget Target { get; }
        IRogueMethodRange Range { get; }
        int RequiredMp { get; }
        Spanning<IKeyword> AmmoCategories { get; }

        /// <param name="additionalEffect">状態異常などダメージ以外の追加効果</param>
        int GetAtk(RogueObj self, out bool additionalEffect);
    }
}
