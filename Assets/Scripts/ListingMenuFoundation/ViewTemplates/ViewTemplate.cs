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

        private bool isBuilded;

        /// <summary>
        /// このメソッドが失敗する（false を返す）ときのみ FluentBuilder を返すように実装する
        /// </summary>
        protected bool TryShowSubViews(TMgr manager, TArg arg)
        {
            if (isBuilded)
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

        public abstract class BaseFluentBuilder
        {
            private readonly ViewTemplate<TMgr, TArg> parent;
            private readonly TMgr manager;
            private readonly TArg arg;

            protected BaseFluentBuilder(ViewTemplate<TMgr, TArg> parent, TMgr manager, TArg arg)
            {
                this.parent = parent;
                this.manager = manager;
                this.arg = arg;
            }

            protected void AssertNotBuilded()
            {
                if (parent.isBuilded) throw new System.InvalidOperationException($"{parent} はビルド済みです。");
            }

            public void Build()
            {
                AssertNotBuilded();
                parent.isBuilded = true;
                parent.ShowSubViews(manager, arg);
            }
        }
    }
}
