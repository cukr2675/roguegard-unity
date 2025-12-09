using UnityEngine.InputSystem;

namespace Lysionium
{
    /// <inheritdoc/>
    public class KeyOption : KeyOption<IListMenuManager, IListMenuArg>
    {
        public KeyOption()
        {
        }

        public KeyOption(
            string name, InputItemHandler<IListMenuManager, IListMenuArg> onPerform, string style = null,
            InputItemHandler<IListMenuManager, IListMenuArg> onStart = null, InputItemHandler<IListMenuManager, IListMenuArg> onCancel = null)
            : base(name, onPerform, style, onStart, onCancel)
        {
        }

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

    /// <inheritdoc/>
    public class KeyOption<TMgr> : KeyOption<TMgr, IListMenuArg>
    {
        public KeyOption()
        {
        }

        public KeyOption(
            string name, InputItemHandler<TMgr, IListMenuArg> onPerform, string style = null,
            InputItemHandler<TMgr, IListMenuArg> onStart = null, InputItemHandler<TMgr, IListMenuArg> onCancel = null)
            : base(name, onPerform, style, onStart, onCancel)
        {
        }
    }

    public class KeyOption<TMgr, TArg> : IKeyOption<TMgr, TArg>
    {
        private string name;
        private System.Func<TMgr, TArg, string> getName;

        private string style;
        private System.Func<TMgr, TArg, string> getStyle;

        public InputItemHandler<TMgr, TArg> Started { get; set; }
        public InputItemHandler<TMgr, TArg> Canceled { get; set; }
        public InputItemHandler<TMgr, TArg> Performed { get; set; }

        public KeyOption()
        {
        }

        public KeyOption(
            string name, InputItemHandler<TMgr, TArg> onPerform, string style = null,
            InputItemHandler<TMgr, TArg> onStart = null, InputItemHandler<TMgr, TArg> onCancel = null)
        {
            this.name = name;
            this.style = style;
            Performed = onPerform;
            Started = onStart;
            Canceled = onCancel;
        }

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

        string IKeyOption<TMgr, TArg>.GetName(TMgr manager, TArg arg) => getName?.Invoke(manager, arg) ?? name;
        string IKeyOption<TMgr, TArg>.GetStyle(TMgr manager, TArg arg) => getStyle?.Invoke(manager, arg) ?? style;
        void IKeyOption<TMgr, TArg>.Started(TMgr manager, TArg arg, InputAction.CallbackContext context) => Started?.Invoke(manager, arg, context);
        void IKeyOption<TMgr, TArg>.Canceled(TMgr manager, TArg arg, InputAction.CallbackContext context) => Canceled?.Invoke(manager, arg, context);
        void IKeyOption<TMgr, TArg>.Performed(TMgr manager, TArg arg, InputAction.CallbackContext context) => Performed?.Invoke(manager, arg, context);
    }
}
