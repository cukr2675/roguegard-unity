using UnityEngine.InputSystem;

namespace Lysionium
{
    // 設計メモ: ボタンではない FlickButton は認めないので IButtonViewItemHandler を必須にする
    public interface IFlickButtonViewItemHandler : IButtonViewItemHandler
    {
        /// <summary>
        /// フリックボタンの操作が開始されたとき
        /// </summary>
        void Press(object item, IListuiManager manager);

        /// <summary>
        /// フリックボタンのコンテンツが展開されたとき
        /// </summary>
        void Expand(object item, IListuiManager manager);

        /// <summary>
        /// フリックボタンの操作が中断されたとき。
        /// <see cref="IButtonViewItemHandler"/> と同時実行されることがあるので同時購読は非推奨。
        /// (<see cref="InputAction.canceled"/> と同様に、コンテンツ展開後に中断した場合も実行される)
        /// </summary>
        void Release(object item, IListuiManager manager);
    }
}
