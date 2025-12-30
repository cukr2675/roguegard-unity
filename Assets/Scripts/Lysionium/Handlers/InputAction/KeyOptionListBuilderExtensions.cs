using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Lysionium
{
    public static class KeyOptionListBuilderExtensions
    {
        // 命名メモ: メソッド名を Option にすると UnityEngine.InputSystem をインポートしていないアセンブリで SelectOptionListBuilderExtensions が使えなくなる

        public static TBuilder KeyOptionRange<TMgr, TArg, TBuilder>(
            this IKeyOptionListBuilder<TMgr, TArg, TBuilder> builder, IEnumerable<IKeyOption<TMgr, TArg>> options)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            foreach (var option in options)
            {
                builder.Option(option);
            }
            return builder.Option();
        }

        // input 指定子を想定して style は必須にする
        // name と style は近いほうが見やすいので onDo よりも左にする
        public static TBuilder KeyOption<TMgr, TArg, TBuilder>(
            this IKeyOptionListBuilder<TMgr, TArg, TBuilder> builder,
            string name, string style,
            InputItemHandler<TMgr, TArg> onDo,
            KeybindPhase phase = KeybindPhase.Performed | KeybindPhase.Canceled)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            return builder.Option(new Implement<TMgr, TArg>(
                name, style,
                (phase & KeybindPhase.Started) != 0 ? onDo : null,
                (phase & KeybindPhase.Performed) != 0 ? onDo : null,
                (phase & KeybindPhase.Canceled) != 0 ? onDo : null));
        }

        public static TBuilder KeyOption<TMgr, TArg, TBuilder>(
            this IKeyOptionListBuilder<TMgr, TArg, TBuilder> builder,
            string name, string style,
            System.Action<InputAction.CallbackContext> onDo,
            KeybindPhase phase = KeybindPhase.Performed | KeybindPhase.Canceled)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            return builder.Option(new Implement<TMgr, TArg>(
                name, style,
                (phase & KeybindPhase.Started) != 0 ? onDo : null,
                (phase & KeybindPhase.Performed) != 0 ? onDo : null,
                (phase & KeybindPhase.Canceled) != 0 ? onDo : null));
        }

        public static TBuilder KeyOption<TMgr, TArg, TBuilder>(
            this IKeyOptionListBuilder<TMgr, TArg, TBuilder> builder,
            string name, string style,
            InputItemHandler<TMgr, TArg> onStart,
            InputItemHandler<TMgr, TArg> onPerform,
            InputItemHandler<TMgr, TArg> onCancel)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            return builder.Option(new Implement<TMgr, TArg>(name, style, onStart, onPerform, onCancel));
        }

        public static TBuilder KeyOption<TMgr, TArg, TBuilder>(
            this IKeyOptionListBuilder<TMgr, TArg, TBuilder> builder,
            string name, string style,
            System.Action<InputAction.CallbackContext> onStart,
            System.Action<InputAction.CallbackContext> onPerform,
            System.Action<InputAction.CallbackContext> onCancel)
            where TMgr : IListuiManager
            where TArg : IListuiArg
        {
            return builder.Option(new Implement<TMgr, TArg>(name, style, onStart, onPerform, onCancel));
        }

        private class Implement<TMgr, TArg> : IKeyOption<TMgr, TArg>
        {
            private string name;
            private System.Func<TMgr, TArg, string> getName;

            private string style;
            private System.Func<TMgr, TArg, string> getStyle;

            public InputItemHandler<TMgr, TArg> Started { get; set; }
            public InputItemHandler<TMgr, TArg> Performed { get; set; }
            public InputItemHandler<TMgr, TArg> Canceled { get; set; }

            public Implement(
                string name, string style,
                InputItemHandler<TMgr, TArg> onStart,
                InputItemHandler<TMgr, TArg> onPerform,
                InputItemHandler<TMgr, TArg> onCancel)
            {
                this.name = name;
                this.style = style;
                Started = onStart;
                Performed = onPerform;
                Canceled = onCancel;
            }

            public Implement(
                string name, string style,
                System.Action<InputAction.CallbackContext> onStart,
                System.Action<InputAction.CallbackContext> onPerform,
                System.Action<InputAction.CallbackContext> onCancel)
            {
                this.name = name;
                this.style = style;
                if (onStart != null) { Started = (_, _, ctx) => onStart(ctx); }
                if (onPerform != null) { Performed = (_, _, ctx) => onPerform(ctx); }
                if (onCancel != null) { Canceled = (_, _, ctx) => onCancel(ctx); }
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
            void IKeyOption<TMgr, TArg>.Performed(TMgr manager, TArg arg, InputAction.CallbackContext context) => Performed?.Invoke(manager, arg, context);
            void IKeyOption<TMgr, TArg>.Canceled(TMgr manager, TArg arg, InputAction.CallbackContext context) => Canceled?.Invoke(manager, arg, context);
        }
    }
}
