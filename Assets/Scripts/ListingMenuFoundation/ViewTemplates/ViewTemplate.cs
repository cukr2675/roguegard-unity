using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public abstract class ViewTemplate<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly string[] _titleSingle = new string[1];
        protected IReadOnlyList<string> TitleSingle => _titleSingle;
        public string Title
        {
            get => _titleSingle[0];
            set => _titleSingle[0] = value;
        }

        public bool IsBuilt { get; private set; }

        /// <summary>
        /// このメソッドが失敗する（false を返す）ときのみ FluentBuilder を返すように実装する
        /// </summary>
        protected bool TryShowSubViews(TMgr manager, TArg arg)
        {
            if (IsBuilt)
            {
                ShowSubViews(manager, arg);
                return true;
            }
            else
            {
                return false;
            }
        }

        protected abstract void ShowSubViews(TMgr manager, TArg arg);

        /// <summary>
        /// フルエントビルダークラス
        /// </summary>
        public abstract class BaseBuilder<TOut>
            where TOut : BaseBuilder<TOut>
        {
            private readonly ViewTemplate<TMgr, TArg> parent;
            private readonly TMgr manager;
            private readonly TArg arg;

            protected BaseBuilder(ViewTemplate<TMgr, TArg> parent, TMgr manager, TArg arg)
            {
                this.parent = parent;
                this.manager = manager;
                this.arg = arg;
            }

            protected void AssertNotBuilded()
            {
                if (parent.IsBuilt) throw new System.InvalidOperationException($"{parent} はビルド済みです。");
            }

            public TOut VariableOnce<T>(out T variable, T defaultValue = default)
            {
                variable = defaultValue;
                return (TOut)this;
            }

            public TOut DoOnce(System.Action action)
            {
                if (action == null) throw new System.ArgumentNullException(nameof(action));

                action();
                return (TOut)this;
            }

            public TOut DoOnce<T1>(System.Func<T1> func, out T1 result)
            {
                if (func == null) throw new System.ArgumentNullException(nameof(func));

                result = func();
                return (TOut)this;
            }

            public TOut DoOnce<T1, T2>(System.Func<(T1, T2)> func, out T1 result1, out T2 result2)
            {
                (result1, result2) = func?.Invoke() ?? throw new System.ArgumentNullException(nameof(func));
                return (TOut)this;
            }

            public TOut DoOnce<T1, T2, T3>(System.Func<(T1, T2, T3)> func, out T1 result1, out T2 result2, out T3 result3)
            {
                (result1, result2, result3) = func?.Invoke() ?? throw new System.ArgumentNullException(nameof(func));
                return (TOut)this;
            }

            public TOut DoOnce<T1, T2, T3, T4>(System.Func<(T1, T2, T3, T4)> func, out T1 result1, out T2 result2, out T3 result3, out T4 result4)
            {
                (result1, result2, result3, result4) = func?.Invoke() ?? throw new System.ArgumentNullException(nameof(func));
                return (TOut)this;
            }

            public TOut IfOnce(bool condition, System.Action<TOut> action)
            {
                if (action == null) throw new System.ArgumentNullException(nameof(action));

                if (condition) { action((TOut)this); }
                return (TOut)this;
            }

            public void Build()
            {
                AssertNotBuilded();
                parent.IsBuilt = true;
                parent.ShowSubViews(manager, arg);
            }
        }
    }
}
