using System.Collections;
using System.Collections.Generic;

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

        protected ListViewData()
        {
            List = new ReadOnlyListConcat(headList, OriginalList, tailList);
        }

        public abstract class BaseListBuilder<TOut> : BaseBuilder<TOut>
            where TOut : BaseListBuilder<TOut>
        {
            private ListViewData<TItem, TMgr, TArg> parent;

            protected BaseListBuilder(ListViewData<TItem, TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public TOut Head(TItem item)
            {
                AssertNotBuilt();

                parent.headList.Add(item);
                return (TOut)this;
            }

            public TOut HeadRange(IEnumerable<TItem> items)
            {
                AssertNotBuilt();

                parent.tailList.AddRange(items);
                return (TOut)this;
            }

            public TOut HeadOption(string name, ClickItemHandler<TMgr, TArg> onClick, string style = null)
            {
                AssertNotBuilt();

                parent.headList.Add(SelectOption.Create(name, onClick, style));
                return (TOut)this;
            }

            public TOut Tail(TItem item)
            {
                AssertNotBuilt();

                parent.tailList.Add(item);
                return (TOut)this;
            }

            public TOut TailRange(IEnumerable<TItem> items)
            {
                AssertNotBuilt();

                parent.tailList.AddRange(items);
                return (TOut)this;
            }

            public TOut TailOption(string name, ClickItemHandler<TMgr, TArg> onClick, string style = null)
            {
                AssertNotBuilt();

                parent.tailList.Add(SelectOption.Create(name, onClick, style));
                return (TOut)this;
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
