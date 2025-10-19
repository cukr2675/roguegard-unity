using UnityEngine.InputSystem;

namespace Lysionium
{
    public interface IInputActionViewItemHandler : IViewItemHandler
    {
        void Started(object item, IListMenuManager manager, IListMenuArg arg, InputAction.CallbackContext context);
        void Canceled(object item, IListMenuManager manager, IListMenuArg arg, InputAction.CallbackContext context);
        void Performed(object item, IListMenuManager manager, IListMenuArg arg, InputAction.CallbackContext context);
    }
}
