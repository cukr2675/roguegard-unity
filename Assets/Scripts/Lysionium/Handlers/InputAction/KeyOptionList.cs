using System.Collections;
using System.Collections.Generic;

namespace Lysionium
{
    /// <inheritdoc/>
    public class KeyOptionList<TMgr> : KeyOptionList<TMgr, IListMenuArg>
        where TMgr : IListMenuManager
    { }

    public class KeyOptionList<TMgr, TArg>
        : IReadOnlyList<IKeyOption<TMgr, TArg>>, IKeyOptionListBuilder<TMgr, TArg, KeyOptionList<TMgr, TArg>>
        where TMgr : IListMenuManager
        where TArg : IListMenuArg
    {
        private readonly List<IKeyOption<TMgr, TArg>> list = new();

        public IKeyOption<TMgr, TArg> this[int index] => list[index];
        public int Count => list.Count;

        public KeyOptionList(System.Action<KeyOptionList<TMgr, TArg>> initializeAction = null)
        {
            initializeAction?.Invoke(this);
        }

        public KeyOptionList<TMgr, TArg> Option(IKeyOption<TMgr, TArg> option)
        {
            list.Add(option);
            return this;
        }

        public IEnumerator<IKeyOption<TMgr, TArg>> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
        KeyOptionList<TMgr, TArg> IKeyOptionListBuilder<TMgr, TArg, KeyOptionList<TMgr, TArg>>.Option() => this;
    }
}
