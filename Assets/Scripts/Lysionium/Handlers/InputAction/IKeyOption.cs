using UnityEngine;
using UnityEngine.InputSystem;

namespace Lysionium
{
    /// <summary>
    /// <see cref="KeyOptionViewItemHandler"/> のモデルとして扱うインターフェース。
    /// </summary>
    public interface IKeyOption
    {
        string GetName(IListMenuManager manager, IListMenuArg arg);

        string GetStyle(IListMenuManager manager, IListMenuArg arg);

        void Started(IListMenuManager manager, IListMenuArg arg, InputAction.CallbackContext context);
        void Canceled(IListMenuManager manager, IListMenuArg arg, InputAction.CallbackContext context);
        void Performed(IListMenuManager manager, IListMenuArg arg, InputAction.CallbackContext context);
    }
}
