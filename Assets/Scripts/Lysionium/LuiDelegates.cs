using UnityEngine.InputSystem;

namespace Lysionium
{
    // 設計メモ: delegate void ClickItemHandler<in TItem>(TItem item) は作らない
    // 既存 ClickItemHandler の引数省略版として使用するだけなら必要性はさほどない
    // むしろ System.Action<T> や System.Action<InputAction.CallbackContext> のほうが外部モジュールとの連携に向いている

    public delegate void ClickItemHandler<in TItem, in TMgr, in TArg>(TItem item, TMgr manager, TArg arg);

    public delegate void ClickItemHandler<in TMgr, in TArg>(TMgr manager, TArg arg);

    public delegate void InputItemHandler<in TItem, in TMgr, in TArg>(TItem item, TMgr manager, TArg arg, InputAction.CallbackContext context);

    public delegate void InputItemHandler<in TMgr, in TArg>(TMgr manager, TArg arg, InputAction.CallbackContext context);

    public delegate void ListMenuEventHandler(IListMenuManager manager, IListMenuArg arg);

    public delegate void ListMenuEventHandler<in TMgr, in TArg>(TMgr manager, TArg arg);
}
