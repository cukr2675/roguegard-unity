using UnityEngine.InputSystem;

namespace Lysionium
{
    // 設計メモ: delegate void SubmitItemHandler<in TItem>(TItem item) は作らない
    // 既存 SubmitItemHandler の引数省略版として使用するだけなら必要性はさほどない

    // 設計メモ: 複数の IListuiManager で共通のメニュー（クイックメニューなど）を作りたい場合、反変性があると便利なので付与する

    public delegate void SubmitItemHandler<in TItem, in TMgr>(TItem item, TMgr manager);

    public delegate void SubmitOptionHandler<in TMgr>(TMgr manager);
    public delegate void SubmitOptionHandler<in TMgr, in TArg>(TMgr manager, TArg arg);

    public delegate void InputOptionHandler<in TMgr>(TMgr manager, InputAction.CallbackContext context);
}
