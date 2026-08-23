using System.Collections.Generic;

namespace Lysionium
{
    // 誤って使用することを避けるため、省略版は実装しない
    ///// <inheritdoc/>
    //public class SelectOptionViewItemHandler : SelectOptionViewItemHandler<IListuiManager>
    //{
    //}

    /// <summary>
    /// モデルのリストではなく選択肢を扱いたいときに使用する <see cref="IViewItemHandler"/> 。
    /// </summary>
    public class SelectOptionViewItemHandler<TMgr> : IEventGestureViewItemHandler
    {
        public static SelectOptionViewItemHandler<TMgr> Instance { get; } = new();

        public string GetName(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<ISelectOption<TMgr>>(item, out var selectOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetName(manager);

            return selectOption.GetName(tMgr);
        }

        public string GetStyle(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<ISelectOption<TMgr>>(item, out var selectOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetStyle(manager);

            return selectOption.GetStyle(tMgr);
        }

        public IReadOnlyList<string> GetCandidateEventGestureNames(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<ISelectOption<TMgr>>(item, out var selectOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return System.Array.Empty<string>();

            return selectOption.GetCandidateEventGestureNames(tMgr);
        }

        public void EventGestureConfirmed(object item, IListuiManager manager, string eventGestureName)
        {
            if (LuiAssert.Type<ISelectOption<TMgr>>(item, out var selectOption, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            selectOption.EventGestureConfirmed(tMgr, eventGestureName);
        }
    }
}
