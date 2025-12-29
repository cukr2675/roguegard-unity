using System.Collections;
using System.Collections.Generic;

namespace Lysionium
{
    /// <inheritdoc/>
    public class SelectOptionList<TMgr> : SelectOptionList<TMgr, IListMenuArg>
        where TMgr : IListMenuManager
    { }

    public class SelectOptionList<TMgr, TArg>
        : IReadOnlyList<ISelectOption<TMgr, TArg>>, ISelectOptionListBuilder<TMgr, TArg, SelectOptionList<TMgr, TArg>>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly List<ISelectOption<TMgr, TArg>> list = new();

        public ISelectOption<TMgr, TArg> this[int index] => list[index];
        public int Count => list.Count;

        public SelectOptionList(System.Action<SelectOptionList<TMgr, TArg>> initializeAction = null)
        {
            initializeAction?.Invoke(this);
        }

        public SelectOptionList<TMgr, TArg> Option(ISelectOption<TMgr, TArg> option)
        {
            list.Add(option);
            return this;
        }

        public void Clear() => list.Clear();
        public IEnumerator<ISelectOption<TMgr, TArg>> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
        SelectOptionList<TMgr, TArg> ISelectOptionListBuilder<TMgr, TArg, SelectOptionList<TMgr, TArg>>.Option() => this;
    }
}
