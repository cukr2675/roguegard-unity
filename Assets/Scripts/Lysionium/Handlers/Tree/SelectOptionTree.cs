using System.Collections;
using System.Collections.Generic;

namespace Lysionium
{
    public class SelectOptionTree<TMgr> :
        IReadOnlyList<object>,
        ISelectOptionsBuilder<TMgr, SelectOptionTree<TMgr>>,
        ITreeOptionsBuilder<TMgr, SelectOptionTree<TMgr>>
        where TMgr : IListuiManager
    {
        private readonly List<object> list = new();

        public object this[int index] => list[index];
        public int Count => list.Count;

        public SelectOptionTree(System.Action<SelectOptionTree<TMgr>> initializeAction = null)
        {
            initializeAction?.Invoke(this);
        }

        public SelectOptionTree<TMgr> Option(ISelectOption<TMgr> option)
        {
            list.Add(option);
            return this;
        }

        public SelectOptionTree<TMgr> Option(ITreeOption<TMgr> option)
        {
            list.Add(option);
            return this;
        }

        public void Clear() => list.Clear();
        public IEnumerator<object> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
        SelectOptionTree<TMgr> ISelectOptionsBuilder<TMgr, SelectOptionTree<TMgr>>.Option() => this;
        SelectOptionTree<TMgr> ITreeOptionsBuilder<TMgr, SelectOptionTree<TMgr>>.Option() => this;
    }
}
