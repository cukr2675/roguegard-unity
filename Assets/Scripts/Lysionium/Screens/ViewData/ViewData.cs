using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    // 命名メモ: データ駆動メニュービュー
    // ViewFrame はUI要素としてのフレームと被る
    // ViewMarkup は markup (マークをつける) というよりは builder や query のほうが近い
    // ViewSetup は Initialize を連想させる
    // ViewPresenter, ViewController はビューに参照されるわけではないので不適切
    public abstract class ViewData<TMgr>
        where TMgr : IListuiManager
    {
        public string Title { get; set; }

        public bool IsBuilt { get; private set; }

        private TMgr callerManager;

        /// <summary>
        /// <see cref="ISubview.Show"/> によって実行される <see cref="OnHide"/> の後にこのデリゲートを呼び出す
        /// </summary>
        private ListuiEventHandler<TMgr> onShow;
        protected ListuiEventHandler OnHide { get; private set; }

        /// <summary>
        /// このメソッドが失敗する（false を返す）ときのみ FluentBuilder を返すように実装する
        /// </summary>
        protected bool TryShowSubviews(TMgr manager)
        {
            if (IsBuilt)
            {
                if (!ReferenceEquals(manager, callerManager))
                {
                    Debug.LogWarning($"{GetType()} をビルドしたマネージャーは {callerManager} です。 {manager} に表示することはできません。");
                }

                ShowSubviews(manager);
                onShow?.Invoke(manager);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// <see cref="ISubview"/> 群を表示するメソッド。
        /// このメソッド単体では <see cref="onShow"/> が実行されないため、呼び出しは非推奨（<see cref="TryShowSubviews"/> の使用を検討）
        /// </summary>
        protected abstract void ShowSubviews(TMgr manager);

        /// <summary>
        /// フルエントビルダークラス
        /// </summary>
        public abstract class BaseBuilder<TViewData, TOut>
            where TViewData : ViewData<TMgr>
            where TOut : BaseBuilder<TViewData, TOut>
        {
            protected TViewData Parent { get; }
            protected TMgr Manager { get; }
            private readonly List<System.IDisposable> disposables;

            protected BaseBuilder(TViewData parent, TMgr manager)
            {
                Parent = parent;
                Manager = manager;
                disposables = new List<System.IDisposable>();
            }

            protected void AssertNotBuilt()
            {
                if (Parent.IsBuilt) throw new System.InvalidOperationException($"{Parent} はビルド済みです。");
            }

            public TOut VarOnce<T>(out T variable, T defaultValue = default)
            {
                variable = defaultValue;
                return (TOut)this;
            }

            public TOut Init(System.Action action)
            {
                if (action == null) throw new System.ArgumentNullException(nameof(action));

                action();
                return (TOut)this;
            }

            public TOut Init(System.Func<System.IDisposable> func)
            {
                if (func == null) throw new System.ArgumentNullException(nameof(func));

                var disposable = func();
                if (disposable != null)
                {
                    disposables.Add(disposable);
                }
                return (TOut)this;
            }

            public TOut InitIf(bool condition, System.Action<TOut> action)
            {
                if (action == null) throw new System.ArgumentNullException(nameof(action));

                if (condition) { action((TOut)this); }
                return (TOut)this;
            }

            internal TOut DoOnce<T1>(System.Func<T1> func, out T1 result)
            {
                if (func == null) throw new System.ArgumentNullException(nameof(func));

                result = func();
                return (TOut)this;
            }

            internal TOut DoOnce<T1, T2>(System.Func<(T1, T2)> func, out T1 result1, out T2 result2)
            {
                (result1, result2) = func?.Invoke() ?? throw new System.ArgumentNullException(nameof(func));
                return (TOut)this;
            }

            internal TOut DoOnce<T1, T2, T3>(System.Func<(T1, T2, T3)> func, out T1 result1, out T2 result2, out T3 result3)
            {
                (result1, result2, result3) = func?.Invoke() ?? throw new System.ArgumentNullException(nameof(func));
                return (TOut)this;
            }

            internal TOut DoOnce<T1, T2, T3, T4>(System.Func<(T1, T2, T3, T4)> func, out T1 result1, out T2 result2, out T3 result3, out T4 result4)
            {
                (result1, result2, result3, result4) = func?.Invoke() ?? throw new System.ArgumentNullException(nameof(func));
                return (TOut)this;
            }

            public TOut OnShow(ListuiEventHandler<TMgr> handler)
            {
                AssertNotBuilt();

                Parent.onShow += handler;
                return (TOut)this;
            }

            public TOut OnHide(ListuiEventHandler<TMgr> handler)
            {
                AssertNotBuilt();

                Parent.OnHide += (manager) =>
                {
                    if (LuiAssert.Type<TMgr>(manager, out var tMgr, manager)) return;

                    handler(tMgr);
                };
                return (TOut)this;
            }

            public virtual void Build()
            {
                AssertNotBuilt();
                Manager.OnUnload += Unload;
                Parent.IsBuilt = true;
                Parent.callerManager = Manager;
                Parent.ShowSubviews(Manager);
                Parent.onShow?.Invoke(Manager);
            }

            protected virtual void Unload()
            {
                foreach (var disposable in disposables)
                {
                    disposable.Dispose();
                }
                disposables.Clear();
                Parent.IsBuilt = false;
                Parent.callerManager = default;
                Parent.onShow = null;
                Parent.OnHide = null;
                Manager.OnUnload -= Unload;
            }
        }
    }
}
