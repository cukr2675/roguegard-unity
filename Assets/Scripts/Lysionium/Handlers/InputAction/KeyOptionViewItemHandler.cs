using UnityEngine.InputSystem;

namespace Lysionium
{
    // 誤って使用することを避けるため、省略版は実装しない
    ///// <inheritdoc/>
    //public class KeyOptionViewItemHandler : KeyOptionViewItemHandler<IListuiManager>
    //{
    //}

    public class KeyOptionViewItemHandler<TMgr> : IInputActionViewItemHandler
    {
        public static KeyOptionViewItemHandler<TMgr> Instance { get; } = new();

        public string GetName(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IKeyOption<TMgr>>(item, out var keyOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetName(manager);

            return keyOption.GetName(tMgr);
        }

        public string GetStyle(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<IKeyOption<TMgr>>(item, out var keyOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetStyle(manager);

            return keyOption.GetStyle(tMgr);
        }

        public void Started(object item, IListuiManager manager, InputAction.CallbackContext context)
        {
            if (LuiAssert.Type<IKeyOption<TMgr>>(item, out var keyOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            keyOption.Started(tMgr, context);
        }

        public void Performed(object item, IListuiManager manager, InputAction.CallbackContext context)
        {
            if (LuiAssert.Type<IKeyOption<TMgr>>(item, out var keyOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            keyOption.Performed(tMgr, context);
        }

        public void Canceled(object item, IListuiManager manager, InputAction.CallbackContext context)
        {
            if (LuiAssert.Type<IKeyOption<TMgr>>(item, out var keyOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            keyOption.Canceled(tMgr, context);
        }
    }
}
