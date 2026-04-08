using System.Collections;
using System.Collections.Generic;

namespace Lysionium
{
    // KeyOptionList は BackSelectOption がない（マネージャーに依存することが少ない）ので、型引数を省略可能にする
    /// <inheritdoc/>
    public class KeyOptionList : KeyOptionList<IListuiManager>
    {
        public KeyOptionList(System.Action<KeyOptionList<IListuiManager>> initializeAction = null)
            : base(initializeAction)
        {
        }
    }

    public class KeyOptionList<TMgr>
        : IReadOnlyList<IKeyOption<TMgr>>, IKeyOptionsBuilder<TMgr, KeyOptionList<TMgr>>
        where TMgr : IListuiManager
    {
        private readonly List<IKeyOption<TMgr>> list = new();

        public IKeyOption<TMgr> this[int index] => list[index];
        public int Count => list.Count;

        public KeyOptionList(System.Action<KeyOptionList<TMgr>> initializeAction = null)
        {
            initializeAction?.Invoke(this);
        }

        public KeyOptionList<TMgr> Option(IKeyOption<TMgr> option)
        {
            list.Add(option);
            return this;
        }

        public void Clear() => list.Clear();
        public IEnumerator<IKeyOption<TMgr>> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
        KeyOptionList<TMgr> IKeyOptionsBuilder<TMgr, KeyOptionList<TMgr>>.Option() => this;
    }
}
