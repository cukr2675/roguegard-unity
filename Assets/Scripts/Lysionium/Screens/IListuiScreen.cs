// 命名メモ:
// ViewData と合わせて Presenter のような働きをするが、モデルに埋め込む運用を想定するため Presenter ではない。
// そのため namespace Lysionium.Presenters にはしない。
namespace Lysionium
{
    // 設計メモ: 画面遷移ナビゲーションを拡張できるようにするため ListuiScreen と ViewData に分離する

    // 設計メモ: 複数の IListuiManager で共通のメニュー（クイックメニューなど）を作りたい場合、反変性があると便利なので付与する

    /// <summary>
    /// メニューの画面単位のインターフェース
    /// </summary>
    public interface IListuiScreen
    {
        // 命名メモ: 前画面に対する増分 (Incremental) として振る舞うため。単純に重ねる方法とは異なるため添加物 (Additive) ではない
        /// <summary>
        /// このメニューを表示中のメニューに重ねて表示するかを取得する。
        /// ダイアログなどを実装する際は true でオーバーライドしたうえで <see cref="CloseScreenView(TMgr, bool)"/> も実装する
        /// </summary>
        bool IsIncremental => false;
    }

    /// <inheritdoc/>
    public interface IListuiScreen<in TMgr> : IListuiScreen
        where TMgr : IListuiManager
    {
        /// <summary>
        /// 画面を開くメソッド。画面の初期化処理とUIの表示を行う。
        /// </summary>
        void OpenScreen(TMgr manager);

        /// <summary>
        /// 画面UIを閉じるメソッド。 <see cref="IsIncremental"/> によって実行されないことがあるためビジネスロジックの記述は禁止。
        /// <para>
        /// メモ: この画面のUI表示前に独自の遷移アニメーションをトリガーしたい場合や <see cref="IsIncremental"/> == true のときオーバーライドする
        /// </para>
        /// </summary>
        void CloseScreenView(TMgr manager, bool back) // 命名メモ：表示処理のみ扱うことを推奨するため View をつける
        {
            manager.HideAll(back);
        }



        // コラム: CloseScreenView と IsIncremental を同時に行う没案
        //
        // CloseScreenView で設定した AnimationController パラメータがリセットされてしまう
        // ・回避するとそれはそれで問題
        // ・CloseScreenView を2回呼び出せば最適だがそこまでするなら IsIncremental プロパティにする
        //
        // IsIncremental が隠れるので可読性が下がる
        //
        // IsIncremental == false の画面から IsIncremental == true の画面に戻れるように拡張しづらくなる
        //
        //bool CloseScreenViewIsIncremental(TMgr manager, bool back);
    }

    /// <inheritdoc/>
    public interface IListuiScreen<in TMgr, in TArg> : IListuiScreen
        where TMgr : IListuiManager
    {
        /// <summary>
        /// 画面を開くメソッド。画面の初期化処理とUIの表示を行う。
        /// </summary>
        void OpenScreen(TMgr manager, TArg arg);

        /// <summary>
        /// 画面UIを閉じるメソッド。 <see cref="IsIncremental"/> によって実行されないことがあるためビジネスロジックの記述は禁止。
        /// <para>
        /// メモ: この画面のUI表示前に独自の遷移アニメーションをトリガーしたい場合や <see cref="IsIncremental"/> == true のときオーバーライドする
        /// </para>
        /// </summary>
        void CloseScreenView(TMgr manager, bool back, TArg arg)
        {
            manager.HideAll(back);
        }
    }
}
