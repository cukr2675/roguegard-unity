using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace Lysionium
{
    public static class KeyOptionListBuilderExtensions
    {
        public static TBuilder KeyOptionRange<TMgr, TArg, TBuilder>(
            this IKeyOptionListBuilder<TMgr, TArg, TBuilder> builder, IEnumerable<IKeyOption<TMgr, TArg>> options)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            foreach (var option in options)
            {
                builder.Option(option);
            }
            return (TBuilder)builder;
        }

        public static TBuilder KeyOption<TMgr, TArg, TBuilder>(
            this IKeyOptionListBuilder<TMgr, TArg, TBuilder> builder,
            string name, InputItemHandler<TMgr, TArg> onPerform, string style,
            InputItemHandler<TMgr, TArg> onStart = null, InputItemHandler<TMgr, TArg> onCancel = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            return builder.Option(Lysionium.KeyOption.Create(name, onPerform, style, onStart, onCancel));
        }

        public static TBuilder KeyOption<TMgr, TArg, TBuilder>(
            this IKeyOptionListBuilder<TMgr, TArg, TBuilder> builder,
            string name, System.Action<InputAction.CallbackContext> onPerform, string style,
            System.Action<InputAction.CallbackContext> onStart = null, System.Action<InputAction.CallbackContext> onCancel = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            return builder.Option(Lysionium.KeyOption.Create<TMgr, TArg>(name, onPerform, style, onStart, onCancel));
        }

        public static TBuilder KeyOptionPerformOrCancel<TMgr, TArg, TBuilder>(
            this IKeyOptionListBuilder<TMgr, TArg, TBuilder> builder,
            string name, InputItemHandler<TMgr, TArg> onPerformOrCancel, string style,
            InputItemHandler<TMgr, TArg> onStart = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            return builder.Option(Lysionium.KeyOption.Create(name, onPerformOrCancel, style, onStart, onPerformOrCancel));
        }

        public static TBuilder KeyOptionPerformOrCancel<TMgr, TArg, TBuilder>(
            this IKeyOptionListBuilder<TMgr, TArg, TBuilder> builder,
            string name, System.Action<InputAction.CallbackContext> onPerformOrCancel, string style,
            System.Action<InputAction.CallbackContext> onStart = null)
            where TMgr : IListMenuManager
            where TArg : IListMenuArg
        {
            return builder.Option(Lysionium.KeyOption.Create<TMgr, TArg>(name, onPerformOrCancel, style, onStart, onPerformOrCancel));
        }
    }
}
