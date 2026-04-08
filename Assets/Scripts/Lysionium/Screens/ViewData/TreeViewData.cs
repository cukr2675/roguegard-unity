using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    public abstract class TreeViewData<TItem, TMgr> : ViewData<TMgr>
        where TItem : class
        where TMgr : IListuiManager
    {
        private readonly List<object> headList = new();
        protected List<TItem> OriginalList { get; } = new();
        private readonly List<object> tailList = new();
        protected IReadOnlyList<object> List { get; }

        // フィルタは headList や tailList には効かないほうが実用的
        private System.Func<TItem, TMgr, bool> filter;

        // 並べ替えや Map メソッドは実装しない（ビルダーの責務が増大して可読性が落ちるため）

        protected TreeViewData()
        {
            List = new ReadOnlyListConcat(headList, OriginalList, tailList);
        }

        protected void SetOriginalList(TItem[] list, TMgr manager)
        {
            if (list == null) throw new System.ArgumentNullException(nameof(list));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            OriginalList.Clear();
            foreach (var item in list)
            {
                if (filter?.Invoke(item, manager) ?? true) { OriginalList.Add(item); }
            }
        }

        protected void SetOriginalList(IReadOnlyList<TItem> list, TMgr manager)
        {
            if (list == null) throw new System.ArgumentNullException(nameof(list));
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            OriginalList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                if (filter?.Invoke(list[i], manager) ?? true) { OriginalList.Add(list[i]); }
            }
        }

        protected void SetOriginalList(System.ReadOnlySpan<TItem> list, TMgr manager)
        {
            if (manager == null) throw new System.ArgumentNullException(nameof(manager));

            OriginalList.Clear();
            foreach (var item in list)
            {
                if (filter?.Invoke(item, manager) ?? true) { OriginalList.Add(item); }
            }
        }

        public abstract class BaseListBuilder<TViewData, TOut> : BaseBuilder<TViewData, TOut>, IViewItemFilterBuilder<TItem, TMgr, TOut>
            where TViewData : TreeViewData<TItem, TMgr>
            where TOut : BaseListBuilder<TViewData, TOut>
        {
            protected BaseListBuilder(TViewData parent, TMgr manager)
                : base(parent, manager)
            {
            }

            // 命名メモ: Prepend/Append (Linq風) ではなく Head/Tail
            // そもそも Linq ではないのとぱっと見の見分けやすさ重視

            public HeadBuilder Head => new((TOut)this);
            public TailBuilder Tail => new((TOut)this);

            public TOut Filter(System.Func<TItem, TMgr, bool> predicate)
            {
                AssertNotBuilt();

                if (Parent.filter != null) { Debug.LogWarning($"{nameof(Filter)} が多重購読されました。"); }

                Parent.filter += predicate;
                return (TOut)this;
            }

            public override void Build()
            {
                // IsBuilt == false 時の Show ではフィルタ未設定状態で SetOriginalList を実行しているため、フィルタ設定後であるここで再実行する
                for (int i = Parent.OriginalList.Count - 1; i >= 0; i--)
                {
                    if (!(Parent.filter?.Invoke(Parent.OriginalList[i], Manager)) ?? false) { Parent.OriginalList.RemoveAt(i); }
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

            public readonly struct HeadBuilder : ISelectOptionsBuilder<TMgr, TOut>, ITreeOptionsBuilder<TMgr, TOut>
            {
                private readonly TOut parent;
                public HeadBuilder(TOut parent) => this.parent = parent;

                public TOut Append(object item)
                {
                    parent.AssertNotBuilt();

                    parent.Parent.headList.Add(item);
                    return parent;
                }

                public TOut Option(ISelectOption<TMgr> option)
                {
                    parent.AssertNotBuilt();

                    parent.Parent.headList.Add(option);
                    return parent;
                }

                public TOut Option(ITreeOption<TMgr> option)
                {
                    parent.AssertNotBuilt();

                    parent.Parent.headList.Add(option);
                    return parent;
                }

                TOut ISelectOptionsBuilder<TMgr, TOut>.Option() => parent;
                TOut ITreeOptionsBuilder<TMgr, TOut>.Option() => parent;
            }

            public readonly struct TailBuilder : ISelectOptionsBuilder<TMgr, TOut>, ITreeOptionsBuilder<TMgr, TOut>
            {
                private readonly TOut parent;
                public TailBuilder(TOut parent) => this.parent = parent;

                public TOut Append(object item)
                {
                    parent.AssertNotBuilt();

                    parent.Parent.tailList.Add(item);
                    return parent;
                }

                public TOut Option(ISelectOption<TMgr> option)
                {
                    parent.AssertNotBuilt();

                    parent.Parent.tailList.Add(option);
                    return parent;
                }

                public TOut Option(ITreeOption<TMgr> option)
                {
                    parent.AssertNotBuilt();

                    parent.Parent.tailList.Add(option);
                    return parent;
                }

                TOut ISelectOptionsBuilder<TMgr, TOut>.Option() => parent;
                TOut ITreeOptionsBuilder<TMgr, TOut>.Option() => parent;
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
