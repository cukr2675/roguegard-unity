namespace Lysionium
{
    // 設計メモ: TMgr を継承したクラスでも有効にするため反変性を付与する
    public interface IBackOptionProviderListuiManager<in TMgr> : IListuiManager
    {
        ISelectOption<TMgr> BackOption { get; }
    }
}
