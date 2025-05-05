using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using R3;

namespace Lysionium.R3
{
    internal interface IObserverElementHandler : IElementHandler
    {
        void OnNext(object element, IListMenuManager manager, IListMenuArg arg, R3RuleContext ctx);

        string IElementHandler.GetName(object element, IListMenuManager manager, IListMenuArg arg)
        {
            return GetNameExtension.GetName(this, element, manager, arg);
        }

        string IElementHandler.GetStyle(object element, IListMenuManager manager, IListMenuArg arg)
        {
            return GetNameExtension.GetName(this, element, manager, arg);
        }
    }

    internal class ObserverElementHandler : IObserverElementHandler, System.IDisposable
    {
        public Subject<R3RuleArg<object, IListMenuManager, IListMenuArg, object, object>> Subject { get; } = new();

        public void OnNext(object element, IListMenuManager manager, IListMenuArg arg, R3RuleContext ctx)
        {
            Subject.OnNext(new R3RuleArg<object, IListMenuManager, IListMenuArg, object, object>(element, manager, arg, ctx));
        }

        public void Dispose()
        {
            Subject.Dispose();
        }
    }

    internal static class GetNameExtension
    {
        private static Context ctx = new();

        public static string GetName(this IObserverElementHandler handler, object element, IListMenuManager manager, IListMenuArg arg)
        {
            using var _ = ctx;
            handler.OnNext(element, manager, arg, ctx);
            if (ctx.IsHandled) return ctx.ReturnValue;
            else return manager.ErrorOption.GetName(manager, arg);
        }

        public static void OnClick<TElm, TMgr, TArg>(
            this Observable<R3RuleArg<object, IListMenuManager, IListMenuArg, object, object>> observable, GetElementName<TElm, TMgr, TArg> getName)
        {
            observable.Subscribe(x =>
            {
                if (!(x.Ctx is Context ctx)) return;

                if (LocalAssert.Type<TElm>(x.Value, out var tElm) ||
                    LocalAssert.Type<TMgr>(x.Manager, out var tMgr) ||
                    LocalAssert.Type<TArg>(x.Arg, out var tArg)) return;

                ctx.ReturnValue = getName(tElm, tMgr, tArg);
            });
        }

        private class Context : R3RuleContext<string> { }
    }

    internal static class OnClickExtension
    {
        private static Context ctx = new();

        public static void OnClick(this IObserverElementHandler handler, object element, IListMenuManager manager, IListMenuArg arg)
        {
            using var _ = ctx;
            handler.OnNext(element, manager, arg, ctx);
        }

        public static void OnClick<TElm, TMgr, TArg>(
            this Observable<R3RuleArg<object, IListMenuManager, IListMenuArg, object, object>> observable, HandleClickElement<TElm, TMgr, TArg> onClick)
        {
            observable.Subscribe(x =>
            {
                if (!(x.Ctx is Context)) return;

                if (LocalAssert.Type<TElm>(x.Value, out var tElm) ||
                    LocalAssert.Type<TMgr>(x.Manager, out var tMgr) ||
                    LocalAssert.Type<TArg>(x.Arg, out var tArg)) return;

                onClick(tElm, tMgr, tArg);
            });
        }

        private class Context : R3RuleContext { }
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
                manager?.ErrorOption.HandleClick(manager, null);

                castedInstance = default;
                return true;
            }
        }
    }
}
