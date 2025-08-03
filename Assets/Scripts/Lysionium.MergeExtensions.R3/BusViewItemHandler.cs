using R3;
using UnityEngine;

namespace Lysionium.MergeExtensions.R3
{
    // 1つの Observer だけで名前取得やクリック等すべてのイベントを処理する ViewItemHandler の試作

    internal interface IBusViewItemHandler : IViewItemHandler
    {
        void OnNext(object item, IListMenuManager manager, IListMenuArg arg, MergedViewItemHandleContext context);

        string IViewItemHandler.GetName(object item, IListMenuManager manager, IListMenuArg arg)
        {
            return GetNameExtension.GetName(this, item, manager, arg);
        }

        string IViewItemHandler.GetStyle(object item, IListMenuManager manager, IListMenuArg arg)
        {
            return GetNameExtension.GetName(this, item, manager, arg);
        }
    }

    internal class BusViewItemHandler : IBusViewItemHandler, System.IDisposable
    {
        public Subject<MergedViewItemHandleArg<object, IListMenuManager, IListMenuArg, object, object>> Subject { get; } = new();

        public void OnNext(object item, IListMenuManager manager, IListMenuArg arg, MergedViewItemHandleContext context)
        {
            Subject.OnNext(new MergedViewItemHandleArg<object, IListMenuManager, IListMenuArg, object, object>(item, manager, arg, context));
        }

        public void Dispose()
        {
            Subject.Dispose();
        }
    }

    internal static class GetNameExtension
    {
        private static readonly Context context = new();

        public static string GetName(this IBusViewItemHandler handler, object item, IListMenuManager manager, IListMenuArg arg)
        {
            lock (context)
            {
                using var _ = context.OpenSelf();
                handler.OnNext(item, manager, arg, context);
                if (context.TryGetResult(out var result)) return result;
                else return item?.ToString() ?? "null";
            }
        }

        public static void NameFrom<TItem, TMgr, TArg>(
            this Observable<MergedViewItemHandleArg<object, IListMenuManager, IListMenuArg, object, object>> observable, ItemNameSelector<TItem, TMgr, TArg> getName)
        {
            observable.Subscribe(x =>
            {
                if (x.Context is not Context context) return;

                if (LocalAssert.Type<TItem>(x.Value, out var TItem) ||
                    LocalAssert.Type<TMgr>(x.Manager, out var tMgr) ||
                    LocalAssert.Type<TArg>(x.Arg, out var tArg)) return;

                context.Result = getName(TItem, tMgr, tArg);
            });
        }

        private class Context : MergedViewItemHandleContext<string> { }
    }

    internal static class OnClickExtension
    {
        private static readonly Context context = new();

        public static void OnClick(this IBusViewItemHandler handler, object item, IListMenuManager manager, IListMenuArg arg)
        {
            lock (context)
            {
                using var _ = context.OpenSelf();
                handler.OnNext(item, manager, arg, context);
            }
        }

        public static void OnClick<TItem, TMgr, TArg>(
            this Observable<MergedViewItemHandleArg<object, IListMenuManager, IListMenuArg, object, object>> observable, ClickItemHandler<TItem, TMgr, TArg> onClick)
        {
            observable.Subscribe(x =>
            {
                if (x.Context is not Context) return;

                if (LocalAssert.Type<TItem>(x.Value, out var TItem) ||
                    LocalAssert.Type<TMgr>(x.Manager, out var tMgr) ||
                    LocalAssert.Type<TArg>(x.Arg, out var tArg)) return;

                onClick(TItem, tMgr, tArg);
            });
        }

        private class Context : MergedViewItemHandleContext { }
    }

    internal static class LocalAssert
    {
        public static bool Type<T>(object instance, out T castedInstance, IListMenuManager manager = null)
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
                manager?.ErrorOption.Click(manager, null);

                castedInstance = default;
                return true;
            }
        }
    }
}
