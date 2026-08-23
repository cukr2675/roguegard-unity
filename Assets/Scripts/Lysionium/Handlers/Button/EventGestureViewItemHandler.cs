using System.Collections.Generic;

namespace Lysionium
{
    public class EventGestureViewItemHandler<TItem, TMgr> : IEventGestureViewItemHandler
        where TMgr : IListuiManager
    {
        public System.Func<TItem, TMgr, string> GetName { get; set; }
        public System.Func<TItem, TMgr, string> GetStyle { get; set; }
        private System.Action<TItem, TMgr, string> eventGestureConfirmed;

        /// <summary>
        /// このインスタンスのデリゲート実行前に <see cref="SelectOptionViewItemHandler{TMgr}"/> の処理を挟む
        /// (リストの前後に <see cref="ISelectOption{TMgr}"/> を入れる場合を想定)
        /// </summary>
        public bool EnableSelectOptionProxy { get; set; }

        private readonly List<string> candidateEventGestureNames = new();

        public EventGestureViewItemHandler(bool enableSelectOptionProxy = true)
        {
            EnableSelectOptionProxy = enableSelectOptionProxy;
        }

        public void SubscribeEventGestureConfirmed(
            string eventGestureName, SubmitItemHandler<TItem, TMgr> onEventGestureConfirmed)
        {
            if (!candidateEventGestureNames.Contains(eventGestureName))
            {
                candidateEventGestureNames.Add(eventGestureName);
            }

            eventGestureConfirmed += (item, manager, currentEventGestureName) =>
            {
                if (currentEventGestureName == eventGestureName)
                {
                    onEventGestureConfirmed(item, manager);
                }
            };
        }

        public void ClearEventGestureConfirmed()
        {
            candidateEventGestureNames.Clear();
            eventGestureConfirmed = null;
        }

        string IViewItemHandler.GetName(object item, IListuiManager manager)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr>)
            {
                return SelectOptionViewItemHandler<TMgr>.Instance.GetName(item, manager);
            }

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetName(manager);

            if (GetName != null) return GetName(tItem, tMgr);
            else return item?.ToString() ?? "null";
        }

        string IViewItemHandler.GetStyle(object item, IListuiManager manager)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr>)
            {
                return SelectOptionViewItemHandler<TMgr>.Instance.GetStyle(item, manager);
            }

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetStyle(manager);

            return GetStyle?.Invoke(tItem, tMgr) ?? string.Empty;
        }

        IReadOnlyList<string> IEventGestureViewItemHandler.GetCandidateEventGestureNames(
            object item, IListuiManager manager)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr>)
            {
                return SelectOptionViewItemHandler<TMgr>.Instance.GetCandidateEventGestureNames(item, manager);
            }

            if (LuiAssert.Type<TItem>(item, out var tItem) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return System.Array.Empty<string>();

            return candidateEventGestureNames;
        }

        void IEventGestureViewItemHandler.EventGestureConfirmed(
            object item, IListuiManager manager, string eventGestureName)
        {
            if (EnableSelectOptionProxy && item is ISelectOption<TMgr>)
            {
                SelectOptionViewItemHandler<TMgr>.Instance.EventGestureConfirmed(item, manager, eventGestureName);
                return;
            }

            if (eventGestureConfirmed == null) throw new System.InvalidOperationException(
                $"{eventGestureConfirmed} が null です。");
            if (LuiAssert.Type<TItem>(item, out var tItem, manager) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

            eventGestureConfirmed(tItem, tMgr, eventGestureName);
        }
    }
}
