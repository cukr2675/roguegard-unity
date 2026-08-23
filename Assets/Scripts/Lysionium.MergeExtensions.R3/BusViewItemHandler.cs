using R3;
using UnityEngine;

namespace Lysionium.MergeExtensions.R3
{
    // 1つの Observer だけで名前取得やクリック等すべてのイベントを処理する ViewItemHandler の試作

    internal interface IBusViewItemHandler : IViewItemHandler
    {
        void OnNext(object item, IListuiManager manager, MergedViewItemHandleContext context);

        string IViewItemHandler.GetName(object item, IListuiManager manager)
        {
            return GetName(item, manager);
        }

        string IViewItemHandler.GetStyle(object item, IListuiManager manager)
        {
            return string.Empty;
        }
    }

    internal class BusViewItemHandler : IBusViewItemHandler, System.IDisposable
    {
        public Subject<MergedViewItemHandleArg<object, IListuiManager, object, object>> Subject { get; } = new();

        public void OnNext(object item, IListuiManager manager, MergedViewItemHandleContext context)
        {
            Subject.OnNext(new MergedViewItemHandleArg<object, IListuiManager, object, object>(item, manager, context));
        }

        public void Dispose()
        {
            Subject.Dispose();
        }
    }

    internal static class GetNameExtensions
    {
        private static readonly Context context = new();

        public static string GetName(this IBusViewItemHandler handler, object item, IListuiManager manager)
        {
            lock (context)
            {
                using var _ = context.OpenSelf();
                handler.OnNext(item, manager, context);
                if (context.TryGetResult(out var result)) return result;
                else return item?.ToString() ?? "null";
            }
        }

        public static void NameFrom<TItem, TMgr>(
            this Observable<MergedViewItemHandleArg<object, IListuiManager, object, object>> observable, System.Func<TItem, TMgr, string> getName)
        {
            observable.Subscribe(x =>
            {
                if (x.Context is not Context context) return;

                if (LocalAssert.Type<TItem>(x.Value, out var TItem) ||
                    LocalAssert.Type<TMgr>(x.Manager, out var tMgr)) return;

                context.Result = getName(TItem, tMgr);
            });
        }

        private class Context : MergedViewItemHandleContext<string> { }
    }

    internal static class OnClickExtensions
    {
        private static readonly Context context = new();

        public static void OnClick(this IBusViewItemHandler handler, object item, IListuiManager manager)
        {
            lock (context)
            {
                using var _ = context.OpenSelf();
                handler.OnNext(item, manager, context);
            }
        }

        public static void OnClick<TItem, TMgr>(
            this Observable<MergedViewItemHandleArg<object, IListuiManager, object, object>> observable,
            SubmitItemHandler<TItem, TMgr> onClick)
        {
            observable.Subscribe(x =>
            {
                if (x.Context is not Context) return;

                if (LocalAssert.Type<TItem>(x.Value, out var TItem) ||
                    LocalAssert.Type<TMgr>(x.Manager, out var tMgr)) return;

                onClick(TItem, tMgr);
            });
        }

        private class Context : MergedViewItemHandleContext { }
    }

    internal static class LocalAssert
    {
        public static bool Type<T>(object instance, out T castedInstance, IListuiManager manager = null)
        {
            if (instance is T tInstance)
            {
                castedInstance = tInstance;
                return false;
            }
            else if (instance == null)
            {
                castedInstance = default;
                return false;
            }
            else
            {
                Debug.LogError($"{instance} を {typeof(T)} に変換できません。");
                manager?.ErrorOption.Click(manager);

                castedInstance = default;
                return true;
            }
        }
    }
}
