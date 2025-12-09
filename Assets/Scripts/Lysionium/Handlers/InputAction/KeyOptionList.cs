using System.Collections;
using System.Collections.Generic;

namespace Lysionium
{
    /// <inheritdoc/>
    public class KeyOptionList<TMgr> : KeyOptionList<TMgr, IListMenuArg>
        where TMgr : IListMenuManager
    { }

    public class KeyOptionList<TMgr, TArg>
        : IList<IKeyOption<TMgr, TArg>>, IReadOnlyList<IKeyOption<TMgr, TArg>>, IKeyOptionListBuilder<TMgr, TArg, KeyOptionList<TMgr, TArg>>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly List<IKeyOption<TMgr, TArg>> list = new();

        public IKeyOption<TMgr, TArg> this[int index] { get => list[index]; set => list[index] = value; }
        public int Count => list.Count;
        public bool IsReadOnly => ((ICollection<IKeyOption<TMgr, TArg>>)list).IsReadOnly;

        public KeyOptionList(System.Action<KeyOptionList<TMgr, TArg>> initializeAction = null)
        {
            initializeAction?.Invoke(this);
        }

        public KeyOptionList<TMgr, TArg> Option(IKeyOption<TMgr, TArg> option)
        {
            list.Add(option);
            return this;
        }

        public void Add(IKeyOption<TMgr, TArg> item) => list.Add(item);
        public void Insert(int index, IKeyOption<TMgr, TArg> item) => list.Insert(index, item);
        public bool Remove(IKeyOption<TMgr, TArg> item) => list.Remove(item);
        public void RemoveAt(int index) => list.RemoveAt(index);
        public void Clear() => list.Clear();
        public bool Contains(IKeyOption<TMgr, TArg> item) => list.Contains(item);
        public int IndexOf(IKeyOption<TMgr, TArg> item) => list.IndexOf(item);
        public void CopyTo(IKeyOption<TMgr, TArg>[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        public IEnumerator<IKeyOption<TMgr, TArg>> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
    }
}
