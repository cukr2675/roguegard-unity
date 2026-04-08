namespace Lysionium
{
    // 設計メモ: IOptionsBuilder<ISelectOption<TMgr>, TBuilder> の拡張メソッドと
    // IOptionsBuilder<ITreeOption<TMgr>, TBuilder> の拡張メソッドは共存できない（builder.Option(...).Node(...) というチェーンができない）ため、
    // 可能な限り専用インターフェースで拡張する

    public interface ISelectOptionsBuilder<TMgr, TBuilder>
    {
        TBuilder Option();

        TBuilder Option(ISelectOption<TMgr> option);
    }

    public interface ISelectOptionsBuilder<TMgr, TArg, TBuilder> : ISelectOptionsBuilder<TMgr, TBuilder>
    {
        TBuilder Option(ISelectOption<TMgr, TArg> option);
    }
}
