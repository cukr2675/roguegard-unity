namespace Lysionium
{
    // 設計メモ: IOptionsBuilder<ISelectOption<TMgr, TArg>, TBuilder> の拡張メソッドと
    // IOptionsBuilder<ITreeOption<TMgr, TArg>, TBuilder> の拡張メソッドは共存できない（builder.Option(...).Node(...) というチェーンができない）ため、
    // 可能な限り専用インターフェースで拡張する

    public interface ISelectOptionsBuilder<TMgr, TArg, TBuilder>
    {
        TBuilder Option();

        TBuilder Option(ISelectOption<TMgr, TArg> option);
    }
}
