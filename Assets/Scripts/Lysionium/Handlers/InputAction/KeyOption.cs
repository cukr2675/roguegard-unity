using UnityEngine;
using UnityEngine.InputSystem;

namespace Lysionium
{
    public static class KeyOption
    {
        public static KeyOption<TMgr, TArg> Create<TMgr, TArg>(
            string name, InputItemHandler<TMgr, TArg> onPerform, string style, // input 指定子を想定して style は必須にする
            InputItemHandler<TMgr, TArg> onStart = null, InputItemHandler<TMgr, TArg> onCancel = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new KeyOption<TMgr, TArg>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.Performed = onPerform;
            instance.Started = onStart;
            instance.Canceled = onCancel;
            return instance;
        }

        public static KeyOption<TMgr, TArg> Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> getName, InputItemHandler<TMgr, TArg> onPerform, string style,
            InputItemHandler<TMgr, TArg> onStart = null, InputItemHandler<TMgr, TArg> onCancel = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new KeyOption<TMgr, TArg>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.Performed = onPerform;
            instance.Started = onStart;
            instance.Canceled = onCancel;
            return instance;
        }

        public static KeyOption<TMgr, TArg> Create<TMgr, TArg>(
            string name, InputItemHandler<TMgr, TArg> onPerform, System.Func<TMgr, TArg, string> style,
            InputItemHandler<TMgr, TArg> onStart = null, InputItemHandler<TMgr, TArg> onCancel = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new KeyOption<TMgr, TArg>();
            instance.SetName(name);
            instance.SetStyle(style);
            instance.Performed = onPerform;
            instance.Started = onStart;
            instance.Canceled = onCancel;
            return instance;
        }

        public static KeyOption<TMgr, TArg> Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> getName, InputItemHandler<TMgr, TArg> onPerform, System.Func<TMgr, TArg, string> style,
            InputItemHandler<TMgr, TArg> onStart = null, InputItemHandler<TMgr, TArg> onCancel = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new KeyOption<TMgr, TArg>();
            instance.SetName(getName);
            instance.SetStyle(style);
            instance.Performed = onPerform;
            instance.Started = onStart;
            instance.Canceled = onCancel;
            return instance;
        }

        public static KeyOption<TMgr, TArg> Create<TMgr, TArg>(
            string name, System.Action<InputAction.CallbackContext> onPerform, string style,
            System.Action<InputAction.CallbackContext> onStart = null, System.Action<InputAction.CallbackContext> onCancel = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new KeyOption<TMgr, TArg>();
            instance.SetName(name);
            instance.SetStyle(style);
            if (onPerform != null) { instance.Performed = (_, _, ctx) => onPerform(ctx); }
            if (onStart != null) { instance.Started = (_, _, ctx) => onStart(ctx); }
            if (onCancel != null) { instance.Canceled = (_, _, ctx) => onCancel(ctx); }
            return instance;
        }

        public static KeyOption<TMgr, TArg> Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> getName, System.Action<InputAction.CallbackContext> onPerform, string style,
            System.Action<InputAction.CallbackContext> onStart = null, System.Action<InputAction.CallbackContext> onCancel = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new KeyOption<TMgr, TArg>();
            instance.SetName(getName);
            instance.SetStyle(style);
            if (onPerform != null) { instance.Performed = (_, _, ctx) => onPerform(ctx); }
            if (onStart != null) { instance.Started = (_, _, ctx) => onStart(ctx); }
            if (onCancel != null) { instance.Canceled = (_, _, ctx) => onCancel(ctx); }
            return instance;
        }

        public static KeyOption<TMgr, TArg> Create<TMgr, TArg>(
            string name, System.Action<InputAction.CallbackContext> onPerform, System.Func<TMgr, TArg, string> style,
            System.Action<InputAction.CallbackContext> onStart = null, System.Action<InputAction.CallbackContext> onCancel = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new KeyOption<TMgr, TArg>();
            instance.SetName(name);
            instance.SetStyle(style);
            if (onPerform != null) { instance.Performed = (_, _, ctx) => onPerform(ctx); }
            if (onStart != null) { instance.Started = (_, _, ctx) => onStart(ctx); }
            if (onCancel != null) { instance.Canceled = (_, _, ctx) => onCancel(ctx); }
            return instance;
        }

        public static KeyOption<TMgr, TArg> Create<TMgr, TArg>(
            System.Func<TMgr, TArg, string> getName, System.Action<InputAction.CallbackContext> onPerform, System.Func<TMgr, TArg, string> style,
            System.Action<InputAction.CallbackContext> onStart = null, System.Action<InputAction.CallbackContext> onCancel = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            var instance = new KeyOption<TMgr, TArg>();
            instance.SetName(getName);
            instance.SetStyle(style);
            if (onPerform != null) { instance.Performed = (_, _, ctx) => onPerform(ctx); }
            if (onStart != null) { instance.Started = (_, _, ctx) => onStart(ctx); }
            if (onCancel != null) { instance.Canceled = (_, _, ctx) => onCancel(ctx); }
            return instance;
        }
    }

    public class KeyOption<TMgr, TArg> : IKeyOption
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private string name;
        private System.Func<TMgr, TArg, string> getName;

        private string style;
        private System.Func<TMgr, TArg, string> getStyle;

        public InputItemHandler<TMgr, TArg> Started { get; set; }
        public InputItemHandler<TMgr, TArg> Canceled { get; set; }
        public InputItemHandler<TMgr, TArg> Performed { get; set; }

        public void SetName(string name)
        {
            this.name = name ?? throw new System.ArgumentNullException(nameof(name));
            getName = null;
        }

        public void SetName(System.Func<TMgr, TArg, string> selector)
        {
            getName = selector ?? throw new System.ArgumentNullException(nameof(selector));
            name = null;
        }

        public void SetStyle(string style)
        {
            this.style = style;
            getStyle = null;
        }

        public void SetStyle(System.Func<TMgr, TArg, string> selector)
        {
            getStyle = selector;
            style = null;
        }

        string IKeyOption.GetName(IListMenuManager manager, IListMenuArg arg)
        {
            if (getName != null)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return null;

                return getName(tMgr, tArg);
            }
            else return name;
        }

        string IKeyOption.GetStyle(IListMenuManager manager, IListMenuArg arg)
        {
            if (getStyle != null)
            {
                if (LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                    LuiAssert.Type<TArg>(arg, out var tArg)) return null;

                return getStyle(tMgr, tArg);
            }
            else return style;
        }

        void IKeyOption.Started(IListMenuManager manager, IListMenuArg arg, InputAction.CallbackContext context)
        {
            if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            Started?.Invoke(tMgr, tArg, context);
        }

        void IKeyOption.Canceled(IListMenuManager manager, IListMenuArg arg, InputAction.CallbackContext context)
        {
            if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            Canceled?.Invoke(tMgr, tArg, context);
        }

        void IKeyOption.Performed(IListMenuManager manager, IListMenuArg arg, InputAction.CallbackContext context)
        {
            if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager) ||
                LuiAssert.Type<TArg>(arg, out var tArg, manager)) return;

            Performed?.Invoke(tMgr, tArg, context);
        }
    }
}
