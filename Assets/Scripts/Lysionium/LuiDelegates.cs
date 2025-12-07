using UnityEngine.InputSystem;

namespace Lysionium
{
    // 設計メモ: delegate void ClickItemHandler<in TItem>(TItem item) は作らない
    // 既存 ClickItemHandler の引数省略版として使用するだけなら必要性はさほどない
    // むしろ System.Action<T> や System.Action<InputAction.CallbackContext> のほうが外部モジュールとの連携に向いている

    // 設計メモ: 複数の IListMenuManager で共通のメニュー（クイックメニューなど）を作りたい場合、反変性があると便利なので付与する

    public delegate void ClickItemHandler<in TItem, in TMgr, in TArg>(TItem item, TMgr manager, TArg arg);

    public delegate void ClickItemHandler<in TMgr, in TArg>(TMgr manager, TArg arg);

    public delegate void InputItemHandler<in TItem, in TMgr, in TArg>(TItem item, TMgr manager, TArg arg, InputAction.CallbackContext context);

    public delegate void InputItemHandler<in TMgr, in TArg>(TMgr manager, TArg arg, InputAction.CallbackContext context);

    public delegate void ListMenuEventHandler(IListMenuManager manager, IListMenuArg arg);

    public delegate void ListMenuEventHandler<in TMgr, in TArg>(TMgr manager, TArg arg);
}
