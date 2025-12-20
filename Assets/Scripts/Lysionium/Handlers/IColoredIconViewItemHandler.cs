using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// <see cref="Sprite"/> を同期取得するため非推奨
    /// スタイル名 (<see cref="IViewItemHandler.GetStyle"/>) を使用した AnimationLayer 切り替え・非同期読み込みを推奨
    /// </summary>
    internal interface IColoredIconViewItemHandler : IViewItemHandler
    {
        void GetIcon(object item, IListMenuManager manager, IListMenuArg arg, out Sprite sprite, out Color color);
    }
}
