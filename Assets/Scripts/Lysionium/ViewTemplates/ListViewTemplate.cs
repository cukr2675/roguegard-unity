using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    public abstract class ListViewTemplate<TElm, TMgr, TArg> : ViewTemplate<TMgr, TArg>
        where TElm : class
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly List<object> headList = new();
        protected List<TElm> OriginalList { get; } = new();
        private readonly List<object> tailList = new();
        protected IReadOnlyList<object> List { get; }

        protected ListViewTemplate()
        {
            List = new ReadOnlyListConcat(headList, OriginalList, tailList);
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

            public TOut Head(TElm element)
            {
                AssertNotBuilt();

                parent.headList.Add(element);
                return (TOut)this;
            }

            public TOut HeadRange(IEnumerable<TElm> elements)
            {
                AssertNotBuilt();

                parent.tailList.AddRange(elements);
                return (TOut)this;
            }

            public TOut HeadOption(string name, HandleClickElement<TMgr, TArg> onClick, string style = null)
            {
                AssertNotBuilt();

                parent.headList.Add(SelectOption.Create(name, onClick, style));
                return (TOut)this;
            }

            public TOut Tail(TElm element)
            {
                AssertNotBuilt();

                parent.tailList.Add(element);
                return (TOut)this;
            }

            public TOut TailRange(IEnumerable<TElm> elements)
            {
                AssertNotBuilt();

                parent.tailList.AddRange(elements);
                return (TOut)this;
            }

            public TOut TailOption(string name, HandleClickElement<TMgr, TArg> onClick, string style = null)
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
