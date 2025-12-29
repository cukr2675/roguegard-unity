using UnityEngine.InputSystem;

namespace Lysionium
{
    // 誤って使用することを避けるため、省略版は実装しない
    ///// <inheritdoc/>
    //public class KeyOptionViewItemHandler : KeyOptionViewItemHandler<IListuiManager, IListuiArg>
    //{
    //}
    ///// <inheritdoc/>
    //public class KeyOptionViewItemHandler<TMgr> : KeyOptionViewItemHandler<TMgr, IListuiArg>
    //{
    //}

    public class KeyOptionViewItemHandler<TMgr, TArg> : IInputActionViewItemHandler
    {
        public static KeyOptionViewItemHandler<TMgr, TArg> Instance { get; } = new();

        public string GetName(object item, IListuiManager manager, IListuiArg arg)
        {
            if (LuiAssert.Type<IKeyOption<TMgr, TArg>>(item, out var keyOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetName(manager, arg);

            return keyOption.GetName(tMgr, tArg);
        }

        public string GetStyle(object item, IListuiManager manager, IListuiArg arg)
        {
            if (LuiAssert.Type<IKeyOption<TMgr, TArg>>(item, out var keyOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetStyle(manager, arg);

            return keyOption.GetStyle(tMgr, tArg);
        }

        public void Started(object item, IListuiManager manager, IListuiArg arg, InputAction.CallbackContext context)
        {
            if (LuiAssert.Type<IKeyOption<TMgr, TArg>>(item, out var keyOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            keyOption.Started(tMgr, tArg, context);
        }

        public void Performed(object item, IListuiManager manager, IListuiArg arg, InputAction.CallbackContext context)
        {
            if (LuiAssert.Type<IKeyOption<TMgr, TArg>>(item, out var keyOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            keyOption.Performed(tMgr, tArg, context);
        }

        public void Canceled(object item, IListuiManager manager, IListuiArg arg, InputAction.CallbackContext context)
        {
            if (LuiAssert.Type<IKeyOption<TMgr, TArg>>(item, out var keyOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            keyOption.Canceled(tMgr, tArg, context);
        }
    }
}
