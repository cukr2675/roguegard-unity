using UnityEngine.InputSystem;

namespace Lysionium
{
    public interface IInputActionViewItemHandler : IViewItemHandler
    {
        void Started(object item, IListuiManager manager, IListuiArg arg, InputAction.CallbackContext context);
        void Canceled(object item, IListuiManager manager, IListuiArg arg, InputAction.CallbackContext context);
        void Performed(object item, IListuiManager manager, IListuiArg arg, InputAction.CallbackContext context);
    }
}
