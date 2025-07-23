using UnityEngine;

namespace Roguegard
{
    // 命名メモ: IDescribable だとゲームと関係するのか分かりづらい。出現頻度も IKeyword に比べると少ない
    public interface IRogueDescribable
    {
        string Name { get; }
        Sprite Icon { get; }
        Color Color { get; }
        string Caption { get; }
        IRogueDetails Details { get; }
    }
}
