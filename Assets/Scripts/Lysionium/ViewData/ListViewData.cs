using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    public abstract class ListViewData<TItem, TMgr, TArg> : ViewData<TMgr, TArg>
        where TItem : class
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly List<object> headList = new();
        protected List<TItem> OriginalList { get; } = new();
        private readonly List<object> tailList = new();
        protected IReadOnlyList<object> List { get; }

        // 並べ替えとフィルタは Head(TItem) や Tail(TItem) とは別のほうが実用的
        private System.Func<TItem, TMgr, TArg, bool> filter;

        protected ListViewData()
        {
            List = new ReadOnlyListConcat(headList, OriginalList, tailList);
        }

        protected void SetOriginalList(TItem[] list, TMgr manager, TArg arg)
        {
            if (list == null) throw new System.ArgumentNullException(nameof(list));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            OriginalList.Clear();
            foreach (var item in list)
            {
                if (filter?.Invoke(item, manager, arg) ?? true) { OriginalList.Add(item); }
            }
        }

        protected void SetOriginalList(IReadOnlyList<TItem> list, TMgr manager, TArg arg)
        {
            if (list == null) throw new System.ArgumentNullException(nameof(list));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            OriginalList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                if (filter?.Invoke(list[i], manager, arg) ?? true) { OriginalList.Add(list[i]); }
            }
        }

        protected void SetOriginalList(System.ReadOnlySpan<TItem> list, TMgr manager, TArg arg)
        {
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            OriginalList.Clear();
            foreach (var item in list)
            {
                if (filter?.Invoke(item, manager, arg) ?? true) { OriginalList.Add(item); }
            }
        }

        public abstract class BaseListBuilder<TViewData, TOut> : BaseBuilder<TViewData, TOut>, IViewItemFilterBuilder<TItem, TMgr, TArg, TOut>
            where TViewData : ListViewData<TItem, TMgr, TArg>
            where TOut : BaseListBuilder<TViewData, TOut>
        {
            protected BaseListBuilder(TViewData parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
            }

            public TOut Head(object item)
            {
                AssertNotBuilt();

                Parent.headList.Add(item);
                return (TOut)this;
            }

            public TOut HeadRange(IEnumerable<object> items)
            {
                AssertNotBuilt();

                Parent.tailList.AddRange(items);
                return (TOut)this;
            }

            public TOut HeadOption(string name, ClickItemHandler<TMgr, TArg> onClick, string style = null)
            {
                AssertNotBuilt();

                Parent.headList.Add(SelectOption.Create(name, onClick, style));
                return (TOut)this;
            }

            public TOut Tail(object item)
            {
                AssertNotBuilt();

                Parent.tailList.Add(item);
                return (TOut)this;
            }

            public TOut TailRange(IEnumerable<object> items)
            {
                AssertNotBuilt();

                Parent.tailList.AddRange(items);
                return (TOut)this;
            }

            public TOut TailOption(string name, ClickItemHandler<TMgr, TArg> onClick, string style = null)
            {
                AssertNotBuilt();

                Parent.tailList.Add(SelectOption.Create(name, onClick, style));
                return (TOut)this;
            }

            public TOut Filter(System.Func<TItem, TMgr, TArg, bool> predicate)
            {
                AssertNotBuilt();

                if (Parent.filter != null) { Debug.LogWarning($"{nameof(Filter)} が多重購読されました。"); }

                Parent.filter += predicate;
                return (TOut)this;
            }

            public override void Build()
            {
                // IsBuilt == false 時の Show ではフィルタ未設定状態で SetOriginalList を実行しているため、フィルタ設定後であるここで再実行する
                for (int i = Parent.OriginalList.Count - 1; i >= 0 ; i--)
                {
                    if (!(Parent.filter?.Invoke(Parent.OriginalList[i], Manager, Arg)) ?? false) { Parent.OriginalList.RemoveAt(i); }
                }

                base.Build();
            }

            protected override void Unload()
            {
                base.Unload();
                Parent.headList.Clear();
                Parent.tailList.Clear();
                Parent.filter = null;
            }
        }

        private class ReadOnlyListConcat : IReadOnlyList<object>
        {
            private readonly IReadOnlyList<object>[] lists;

            public object this[int index]
            {
                get
                {
                    foreach (var list in lists)
                    {
                        if (index < list.Count) { return list[index]; }
                        else { index -= list.Count; }
                    }
                    throw new System.IndexOutOfRangeException();
                }
            }

            public int Count
            {
                get
                {
                    var sumCount = 0;
                    foreach (var list in lists)
                    {
                        sumCount += list.Count;
                    }
                    return sumCount;
                }
            }

            public ReadOnlyListConcat(params IReadOnlyList<object>[] lists)
            {
                this.lists = lists;
            }

            public IEnumerator<object> GetEnumerator()
            {
                foreach (var list in lists)
                {
                    foreach (var item in list)
                    {
                        yield return item;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
