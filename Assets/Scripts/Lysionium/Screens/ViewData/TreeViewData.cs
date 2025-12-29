using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    public abstract class TreeViewData<TItem, TMgr, TArg> : ViewData<TMgr, TArg>
        where TItem : class
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly List<object> headList = new();
        protected List<TItem> OriginalList { get; } = new();
        private readonly List<object> tailList = new();
        protected IReadOnlyList<object> List { get; }

        // フィルタは headList や tailList には効かないほうが実用的
        private System.Func<TItem, TMgr, TArg, bool> filter;

        // 並べ替えや Map メソッドは実装しない（ビルダーの責務が増大して可読性が落ちるため）

        protected TreeViewData()
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
            where TViewData : TreeViewData<TItem, TMgr, TArg>
            where TOut : BaseListBuilder<TViewData, TOut>
        {
            protected BaseListBuilder(TViewData parent, TMgr manager, TArg arg)
                : base(parent, manager, arg)
            {
            }

            // 命名メモ: Prepend/Append (Linq風) ではなく Head/Tail
            // そもそも Linq ではないのとぱっと見の見分けやすさ重視

            public HeadBuilder Head => new((TOut)this);
            public TailBuilder Tail => new((TOut)this);

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
                for (int i = Parent.OriginalList.Count - 1; i >= 0; i--)
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

            public readonly struct HeadBuilder : ISelectOptionListBuilder<TMgr, TArg, TOut>, ITreeOptionListBuilder<TMgr, TArg, TOut>
            {
                private readonly TOut parent;
                public HeadBuilder(TOut parent) => this.parent = parent;

                public TOut Append(object item)
                {
                    parent.AssertNotBuilt();

                    parent.Parent.headList.Add(item);
                    return parent;
                }

                public TOut Option(ISelectOption<TMgr, TArg> option)
                {
                    parent.AssertNotBuilt();

                    parent.Parent.headList.Add(option);
                    return parent;
                }

                public TOut Option(ITreeOption<TMgr, TArg> option)
                {
                    parent.AssertNotBuilt();

                    parent.Parent.headList.Add(option);
                    return parent;
                }

                TOut ISelectOptionListBuilder<TMgr, TArg, TOut>.Option() => parent;
                TOut ITreeOptionListBuilder<TMgr, TArg, TOut>.Option() => parent;
            }

            public readonly struct TailBuilder : ISelectOptionListBuilder<TMgr, TArg, TOut>, ITreeOptionListBuilder<TMgr, TArg, TOut>
            {
                private readonly TOut parent;
                public TailBuilder(TOut parent) => this.parent = parent;

                public TOut Append(object item)
                {
                    parent.AssertNotBuilt();

                    parent.Parent.tailList.Add(item);
                    return parent;
                }

                public TOut Option(ISelectOption<TMgr, TArg> option)
                {
                    parent.AssertNotBuilt();

                    parent.Parent.tailList.Add(option);
                    return parent;
                }

                public TOut Option(ITreeOption<TMgr, TArg> option)
                {
                    parent.AssertNotBuilt();

                    parent.Parent.tailList.Add(option);
                    return parent;
                }

                TOut ISelectOptionListBuilder<TMgr, TArg, TOut>.Option() => parent;
                TOut ITreeOptionListBuilder<TMgr, TArg, TOut>.Option() => parent;
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
