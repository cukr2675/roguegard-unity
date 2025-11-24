using UnityEngine.InputSystem;

namespace Lysionium
{
    public delegate void ClickItemHandler<in TItem, in TMgr, in TArg>(TItem item, TMgr manager, TArg arg);

    public delegate void ClickItemHandler<in TMgr, in TArg>(TMgr manager, TArg arg);

    public delegate void InputItemHandler<in TItem, in TMgr, in TArg>(TItem item, TMgr manager, TArg arg, InputAction.CallbackContext context);

    public delegate void InputItemHandler<in TMgr, in TArg>(TMgr manager, TArg arg, InputAction.CallbackContext context);

    public delegate void ListMenuEventHandler(IListMenuManager manager, IListMenuArg arg);

    public delegate void ListMenuEventHandler<in TMgr, in TArg>(TMgr manager, TArg arg);
}
