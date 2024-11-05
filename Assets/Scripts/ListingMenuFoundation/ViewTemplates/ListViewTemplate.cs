using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public abstract class ListViewTemplate<TElm, TMgr, TArg> : ViewTemplate<TMgr, TArg>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly List<TElm> beforeList = new();
        protected List<TElm> OriginalList { get; } = new();
        private readonly List<TElm> afterList = new();
        protected IReadOnlyList<TElm> List { get; }

        protected ListViewTemplate()
        {
            List = new ReadOnlyListConcat(beforeList, OriginalList, afterList);
        }

        public abstract class BaseListBuilder<TOut> : BaseBuilder<TOut>
            where TOut : BaseListBuilder<TOut>
        {
            private ListViewTemplate<TElm, TMgr, TArg> parent;

            protected BaseListBuilder(ListViewTemplate<TElm, TMgr, TArg> parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
                this.parent = parent;
            }

            public TOut InsertNext(TElm element)
            {
                AssertNotBuilded();

                parent.beforeList.Add(element);
                return (TOut)this;
            }

            public TOut InsertNextRange(IEnumerable<TElm> elements)
            {
                AssertNotBuilded();

                parent.afterList.AddRange(elements);
                return (TOut)this;
            }

            public TOut Append(TElm element)
            {
                AssertNotBuilded();

                parent.afterList.Add(element);
                return (TOut)this;
            }

            public TOut AppendRange(IEnumerable<TElm> elements)
            {
                AssertNotBuilded();

                parent.afterList.AddRange(elements);
                return (TOut)this;
            }
        }

        private class ReadOnlyListConcat : IReadOnlyList<TElm>
        {
            private readonly IReadOnlyList<TElm>[] lists;

            public TElm this[int index]
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

            public ReadOnlyListConcat(params IReadOnlyList<TElm>[] lists)
            {
                this.lists = lists;
            }

            public IEnumerator<TElm> GetEnumerator()
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
