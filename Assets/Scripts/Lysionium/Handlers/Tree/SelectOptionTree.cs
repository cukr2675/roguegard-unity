using System.Collections;
using System.Collections.Generic;

namespace Lysionium
{
    /// <inheritdoc/>
    public class SelectOptionTree<TMgr> : SelectOptionTree<TMgr, IListMenuArg>
        where TMgr : IListMenuManager
    { }

    public class SelectOptionTree<TMgr, TArg> :
        IReadOnlyList<object>,
        ISelectOptionListBuilder<TMgr, TArg, SelectOptionTree<TMgr, TArg>>,
        ITreeOptionListBuilder<TMgr, TArg, SelectOptionTree<TMgr, TArg>>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly List<object> list = new();

        public object this[int index] => list[index];
        public int Count => list.Count;

        public SelectOptionTree(System.Action<SelectOptionTree<TMgr, TArg>> initializeAction = null)
        {
            initializeAction?.Invoke(this);
        }

        public SelectOptionTree<TMgr, TArg> Option(ISelectOption<TMgr, TArg> option)
        {
            list.Add(option);
            return this;
        }

        public SelectOptionTree<TMgr, TArg> Option(ITreeOption<TMgr, TArg> option)
        {
            list.Add(option);
            return this;
        }

        public IEnumerator<object> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
        SelectOptionTree<TMgr, TArg> ISelectOptionListBuilder<TMgr, TArg, SelectOptionTree<TMgr, TArg>>.Option() => this;
        SelectOptionTree<TMgr, TArg> ITreeOptionListBuilder<TMgr, TArg, SelectOptionTree<TMgr, TArg>>.Option() => this;
    }
}
