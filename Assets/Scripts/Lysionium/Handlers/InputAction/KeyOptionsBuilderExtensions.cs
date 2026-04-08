using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Lysionium
{
    public static class KeyOptionsBuilderExtensions
    {
        // 命名メモ: メソッド名を Option にすると UnityEngine.InputSystem をインポートしていないアセンブリで SelectOptionListBuilderExtensions が使えなくなる

        public static TBuilder KeyOptionRange<TMgr, TBuilder>(
            this IKeyOptionsBuilder<TMgr, TBuilder> builder, IEnumerable<IKeyOption<TMgr>> options)
            where TMgr : IListuiManager
        {
            foreach (var option in options)
            {
                builder.Option(option);
            }
            return builder.Option();
        }

        // input 指定子を想定して style は必須にする
        // name と style は近いほうが見やすいので onDo よりも左にする
        public static TBuilder KeyOption<TMgr, TBuilder>(
            this IKeyOptionsBuilder<TMgr, TBuilder> builder,
            string name, string style,
            InputOptionHandler<TMgr> onDo,
            KeybindPhase phase = KeybindPhase.Performed | KeybindPhase.Canceled)
            where TMgr : IListuiManager
        {
            return builder.Option(new Implement<TMgr>(
                name, style,
                (phase & KeybindPhase.Started) != 0 ? onDo : null,
                (phase & KeybindPhase.Performed) != 0 ? onDo : null,
                (phase & KeybindPhase.Canceled) != 0 ? onDo : null));
        }

        public static TBuilder KeyOption<TMgr, TBuilder>(
            this IKeyOptionsBuilder<TMgr, TBuilder> builder,
            string name, string style,
            System.Action<InputAction.CallbackContext> onDo,
            KeybindPhase phase = KeybindPhase.Performed | KeybindPhase.Canceled)
            where TMgr : IListuiManager
        {
            return builder.Option(new Implement<TMgr>(
                name, style,
                (phase & KeybindPhase.Started) != 0 ? onDo : null,
                (phase & KeybindPhase.Performed) != 0 ? onDo : null,
                (phase & KeybindPhase.Canceled) != 0 ? onDo : null));
        }

        public static TBuilder KeyOption<TMgr, TBuilder>(
            this IKeyOptionsBuilder<TMgr, TBuilder> builder,
            string name, string style,
            InputOptionHandler<TMgr> onStart,
            InputOptionHandler<TMgr> onPerform,
            InputOptionHandler<TMgr> onCancel)
            where TMgr : IListuiManager
        {
            return builder.Option(new Implement<TMgr>(name, style, onStart, onPerform, onCancel));
        }

        public static TBuilder KeyOption<TMgr, TBuilder>(
            this IKeyOptionsBuilder<TMgr, TBuilder> builder,
            string name, string style,
            System.Action<InputAction.CallbackContext> onStart,
            System.Action<InputAction.CallbackContext> onPerform,
            System.Action<InputAction.CallbackContext> onCancel)
            where TMgr : IListuiManager
        {
            return builder.Option(new Implement<TMgr>(name, style, onStart, onPerform, onCancel));
        }

        private class Implement<TMgr> : IKeyOption<TMgr>
        {
            private string name;
            private System.Func<TMgr, string> getName;

            private string style;
            private System.Func<TMgr, string> getStyle;

            public InputOptionHandler<TMgr> Started { get; set; }
            public InputOptionHandler<TMgr> Performed { get; set; }
            public InputOptionHandler<TMgr> Canceled { get; set; }

            public Implement(
                string name, string style,
                InputOptionHandler<TMgr> onStart,
                InputOptionHandler<TMgr> onPerform,
                InputOptionHandler<TMgr> onCancel)
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
                if (onStart != null) { Started = (_, ctx) => onStart(ctx); }
                if (onPerform != null) { Performed = (_, ctx) => onPerform(ctx); }
                if (onCancel != null) { Canceled = (_, ctx) => onCancel(ctx); }
            }

            public void SetName(string name)
            {
                this.name = name ?? throw new System.ArgumentNullException(nameof(name));
                getName = null;
            }

            public void SetName(System.Func<TMgr, string> selector)
            {
                getName = selector ?? throw new System.ArgumentNullException(nameof(selector));
                name = null;
            }

            public void SetStyle(string style)
            {
                this.style = style;
                getStyle = null;
            }

            public void SetStyle(System.Func<TMgr, string> selector)
            {
                getStyle = selector;
                style = null;
            }

            string IKeyOption<TMgr>.GetName(TMgr manager) => getName?.Invoke(manager) ?? name;
            string IKeyOption<TMgr>.GetStyle(TMgr manager) => getStyle?.Invoke(manager) ?? style;
            void IKeyOption<TMgr>.Started(TMgr manager, InputAction.CallbackContext context) => Started?.Invoke(manager, context);
            void IKeyOption<TMgr>.Performed(TMgr manager, InputAction.CallbackContext context) => Performed?.Invoke(manager, context);
            void IKeyOption<TMgr>.Canceled(TMgr manager, InputAction.CallbackContext context) => Canceled?.Invoke(manager, context);
        }
    }
}
