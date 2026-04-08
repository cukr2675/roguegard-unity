using System.Collections;
using System.Collections.Generic;

namespace Lysionium
{
    public class SelectOptionList<TMgr>
        : IReadOnlyList<ISelectOption<TMgr>>, ISelectOptionsBuilder<TMgr, SelectOptionList<TMgr>>
        where TMgr : IListuiManager
    {
        private readonly List<ISelectOption<TMgr>> list = new();

        public ISelectOption<TMgr> this[int index] => list[index];
        public int Count => list.Count;

        public SelectOptionList(System.Action<SelectOptionList<TMgr>> initializeAction = null)
        {
            initializeAction?.Invoke(this);
        }

        public SelectOptionList<TMgr> Option(ISelectOption<TMgr> option)
        {
            list.Add(option);
            return this;
        }

        public void Clear() => list.Clear();
        public IEnumerator<ISelectOption<TMgr>> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
        SelectOptionList<TMgr> ISelectOptionsBuilder<TMgr, SelectOptionList<TMgr>>.Option() => this;
    }
}
