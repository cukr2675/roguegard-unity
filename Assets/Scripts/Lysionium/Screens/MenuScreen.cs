namespace Lysionium
{
    /// <summary>
    /// <see cref="IMenuScreen{TMgr, TArg}"/> の標準実装クラス
    /// </summary>
    public abstract class MenuScreen<TMgr, TArg> : IMenuScreen<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        public virtual bool IsIncremental => false;

        public abstract void OpenScreen(in TMgr manager, in TArg arg);
        void IMenuScreen<TMgr, TArg>.OpenScreen(TMgr manager, TArg arg) => OpenScreen(manager, arg);

        public virtual void CloseScreenView(TMgr manager, bool back)
        {
            manager.HideAll(back);
        }

        // 設計メモ: 通常であれば暗黙の変換を使用せずに IMenuScreen のオーバーロードを実装すべきだが、
        // IMenuScreen は画面遷移機能のサンプルという位置づけのため、余計な実装や依存関係を作らないよう暗黙の変換にしている。
        // また、オーバーロードにする場合だと、使用者が独自の画面インターフェースを追加するとき
        // 拡張メソッド（+ 拡張用インターフェース）と静的メソッドの実装が必要で面倒。ソースジェネレータを用意するとパッケージの複雑度が増す
        /// <summary>
        /// このメニューを開くクリックアクションに変換する。単純な遷移ならこれで楽できる
        /// </summary>
        public static implicit operator ClickItemHandler<TMgr, TArg>(MenuScreen<TMgr, TArg> menuScreen)
        {
            return (manager, arg) => manager.PushMenuScreenFromExtension(menuScreen, arg);
        }
    }
}
