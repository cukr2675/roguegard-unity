using System.Collections;
using System.Collections.Generic;

namespace Lysionium
{
    // KeyOptionList は BackSelectOption がない（マネージャーに依存することが少ない）ので、型引数を省略可能にする
    /// <inheritdoc/>
    public class KeyOptionList : KeyOptionList<IListuiManager, IListuiArg>
    {
        public KeyOptionList(System.Action<KeyOptionList<IListuiManager, IListuiArg>> initializeAction = null)
            : base(initializeAction)
        {
        }
    }

    /// <inheritdoc/>
    public class KeyOptionList<TMgr> : KeyOptionList<TMgr, IListuiArg>
        where TMgr : IListuiManager
    {
        public KeyOptionList(System.Action<KeyOptionList<TMgr, IListuiArg>> initializeAction = null)
            : base(initializeAction)
        {
        }
    }

    public class KeyOptionList<TMgr, TArg>
        : IReadOnlyList<IKeyOption<TMgr, TArg>>, IKeyOptionListBuilder<TMgr, TArg, KeyOptionList<TMgr, TArg>>
        where TMgr : IListuiManager
        where TArg : IListuiArg
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

        public void Clear() => list.Clear();
        public IEnumerator<IKeyOption<TMgr, TArg>> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
        KeyOptionList<TMgr, TArg> IKeyOptionListBuilder<TMgr, TArg, KeyOptionList<TMgr, TArg>>.Option() => this;
    }
}
