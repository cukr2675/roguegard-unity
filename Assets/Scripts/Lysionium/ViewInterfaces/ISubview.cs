using System.Collections.Generic;

namespace Lysionium
{
    // 設計メモ: Subview はビューであり ListMenuManager に依存しない（どの IListMenuManager でも同じ Subview Prefab が使いまわせる）ことが望ましい。
    // 具象 ListMenuManager に特化したビューを作るのは避けるか慎重になるべきなので、このインターフェースは型引数 <TMgr, TArg> を取らない。
    // （あるいは DI で具象クラスではなくインターフェースを使うように）
    // 型安全性はラッパークラスや ViewData が担う　ラッパークラス使うにしても共通処理である Show, Hide はあるほうが便利

    /// <summary>
    /// 設定された <see cref="IViewItemHandler"/> とリストをもとに UI を表示するインターフェース。
    /// </summary>
    public interface ISubview
    {
        void CommonInit();

        void Show(ListMenuEventHandler onEndAnimation = null, ListMenuEventHandler onHide = null);

        void Hide(bool back, ListMenuEventHandler onEndAnimation = null);
    }
}
