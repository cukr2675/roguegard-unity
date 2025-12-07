using System.Collections;
using System.Collections.Generic;

namespace Lysionium
{
    public class KeyOptionList<TMgr, TArg>
        : IList<IKeyOption>, IReadOnlyList<IKeyOption>, IKeyOptionListBuilder<TMgr, TArg, KeyOptionList<TMgr, TArg>>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly List<IKeyOption> list = new();

        public IKeyOption this[int index] { get => list[index]; set => list[index] = value; }
        public int Count => list.Count;
        public bool IsReadOnly => ((ICollection<IKeyOption>)list).IsReadOnly;

        public KeyOptionList(System.Action<KeyOptionList<TMgr, TArg>> initializeAction = null)
        {
            initializeAction?.Invoke(this);
        }

        public KeyOptionList<TMgr, TArg> Option(IKeyOption option)
        {
            list.Add(option);
            return this;
        }

        public void Add(IKeyOption item) => list.Add(item);
        public void Insert(int index, IKeyOption item) => list.Insert(index, item);
        public bool Remove(IKeyOption item) => list.Remove(item);
        public void RemoveAt(int index) => list.RemoveAt(index);
        public void Clear() => list.Clear();
        public bool Contains(IKeyOption item) => list.Contains(item);
        public int IndexOf(IKeyOption item) => list.IndexOf(item);
        public void CopyTo(IKeyOption[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        public IEnumerator<IKeyOption> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }
}
