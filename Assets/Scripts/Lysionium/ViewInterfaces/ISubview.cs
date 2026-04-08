namespace Lysionium
{
    // 設計メモ: Subview はビューであり ListuiManager に依存しない（どの IListuiManager でも同じ Subview Prefab が使いまわせる）ことが望ましい。
    // 具象 ListuiManager に特化したビューを作るのは避けるか慎重になるべきなので、このインターフェースは型引数 <TMgr> を取らない。
    // （あるいは DI で具象クラスではなくインターフェースを使うように）
    // 型安全性はラッパークラスや ViewData が担う　ラッパークラス使うにしても共通処理である Show, Hide はあるほうが便利

    /// <summary>
    /// 設定された <see cref="IViewItemHandler"/> とリストをもとに UI を表示するインターフェース。
    /// </summary>
    public interface ISubview
    {
        void Show(ListuiEventHandler onEndAnimation = null, ListuiEventHandler onHide = null);

        void Hide(bool back, ListuiEventHandler onEndAnimation = null);
    }
}
