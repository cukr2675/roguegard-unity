namespace Roguegard
{
    // 命名メモ: ICachedValue や IValueCache なども考えられるが高速で動けばキャッシュの有無は関係ないため IRogueCalculator

    /// <summary>
    /// <see cref="IValueEffect"/> の計算結果をキャッシュするインターフェース
    /// </summary>
    public interface IRogueCalculator
    {
        float MainBaseValue { get; }
        float MainValue { get; }

        float SubValues(IKeyword key);

        void Update(RogueObj self);
    }
}
