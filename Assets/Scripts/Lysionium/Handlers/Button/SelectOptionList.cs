using System.Collections;
using System.Collections.Generic;

namespace Lysionium
{
    /// <inheritdoc/>
    public class SelectOptionList<TMgr> : SelectOptionList<TMgr, IListMenuArg>
        where TMgr : IListMenuManager
    { }

    public class SelectOptionList<TMgr, TArg>
        : IList<ISelectOption>, IReadOnlyList<ISelectOption>, ISelectOptionListBuilder<TMgr, TArg, SelectOptionList<TMgr, TArg>>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly List<ISelectOption> list = new();

        public ISelectOption this[int index] { get => list[index]; set => list[index] = value; }
        public int Count => list.Count;
        public bool IsReadOnly => ((ICollection<ISelectOption>)list).IsReadOnly;

        public SelectOptionList(System.Action<SelectOptionList<TMgr, TArg>> initializeAction = null)
        {
            initializeAction?.Invoke(this);
        }

        public SelectOptionList<TMgr, TArg> Option(ISelectOption option)
        {
            list.Add(option);
            return this;
        }

        public void Add(ISelectOption item) => list.Add(item);
        public void Insert(int index, ISelectOption item) => list.Insert(index, item);
        public bool Remove(ISelectOption item) => list.Remove(item);
        public void RemoveAt(int index) => list.RemoveAt(index);
        public void Clear() => list.Clear();
        public bool Contains(ISelectOption item) => list.Contains(item);
        public int IndexOf(ISelectOption item) => list.IndexOf(item);
        public void CopyTo(ISelectOption[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        public IEnumerator<ISelectOption> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }
}
