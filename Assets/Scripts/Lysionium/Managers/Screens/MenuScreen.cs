namespace Lysionium
{
    // 画面遷移ナビゲーションを拡張できるようにするため MenuScreen と ViewData に分離する

    /// <summary>
    /// メニューの画面単位のクラス
    /// </summary>
    public abstract class MenuScreen<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        /// <summary>
        /// このメニューを表示中のメニューに重ねて表示するかを取得する。
        /// ダイアログなどを実装する際は true でオーバーライドしたうえで <see cref="CloseScreenView(TMgr, bool)"/> も実装する
        /// </summary>
        public virtual bool IsIncremental => false;

        /// <summary>
        /// 画面を開くメソッド。画面の初期化処理とUIの表示を行う。
        /// <para>メモ: 引数が in なのは ViewData クラスのメソッドチェーンで誤って使用しないようにするため</para>
        /// </summary>
        public abstract void OpenScreen(in TMgr manager, in TArg arg);

        /// <summary>
        /// 画面UIを閉じるメソッド。 <see cref="IsIncremental"/> によって実行されないことがあるためビジネスロジック関連の処理は禁止。
        /// <para>メモ: この画面のUI表示前に独自の遷移アニメーションをトリガーしたい場合や <see cref="IsIncremental"/> == true のときオーバーライドする</para>
        /// </summary>
        public virtual void CloseScreenView(TMgr manager, bool back) // 命名メモ：表示処理のみ扱うことを推奨するため View をつける
        {
            manager.HideAll(back);
        }

        /// <summary>
        /// このメニューを開くクリックアクションに変換する。単純な遷移ならこれで楽できる
        /// </summary>
        public static implicit operator ClickItemHandler<TMgr, TArg>(MenuScreen<TMgr, TArg> menuScreen)
        {
            return (manager, arg) => manager.PushMenuScreenFromExtension(menuScreen, arg);
        }
    }
}
