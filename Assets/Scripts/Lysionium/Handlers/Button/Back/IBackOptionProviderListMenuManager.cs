namespace Lysionium
{
    // 設計メモ: TMgr を継承したクラスでも有効にするため反変性を付与する
    public interface IBackOptionProviderListMenuManager<in TMgr, in TArg> : IListMenuManager
    {
        ISelectOption<TMgr, TArg> BackOption { get; }
    }
}
