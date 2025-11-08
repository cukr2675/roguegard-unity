using UnityEngine.InputSystem;

namespace Lysionium
{
    public class KeyOptionViewItemHandler : IInputActionViewItemHandler
    {
        public static KeyOptionViewItemHandler Instance { get; } = new KeyOptionViewItemHandler();

        public string GetName(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (LuiAssert.Type<IKeyOption>(item, out var selectOption)) return string.Empty;

            return selectOption.GetName(manager, arg);
        }

        public string GetStyle(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (LuiAssert.Type<IKeyOption>(item, out var selectOption)) return string.Empty;

            return selectOption.GetStyle(manager, arg);
        }

        public void Started(object item, IListMenuManager manager, IListMenuArg arg, InputAction.CallbackContext context)
        {
            if (LuiAssert.Type<IKeyOption>(item, out var selectOption, manager)) return;

            selectOption.Started(manager, arg, context);
        }

        public void Performed(object item, IListMenuManager manager, IListMenuArg arg, InputAction.CallbackContext context)
        {
            if (LuiAssert.Type<IKeyOption>(item, out var selectOption, manager)) return;

            selectOption.Performed(manager, arg, context);
        }

        public void Canceled(object item, IListMenuManager manager, IListMenuArg arg, InputAction.CallbackContext context)
        {
            if (LuiAssert.Type<IKeyOption>(item, out var selectOption, manager)) return;

            selectOption.Canceled(manager, arg, context);
        }
    }
}
