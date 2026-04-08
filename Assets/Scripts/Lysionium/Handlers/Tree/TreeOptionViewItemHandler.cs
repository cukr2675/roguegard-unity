using System.Collections.Generic;

namespace Lysionium
{
    public class TreeOptionViewItemHandler<TMgr> : ITreeViewItemHandler
    {
        public static TreeOptionViewItemHandler<TMgr> Instance { get; } = new();

        public string GetName(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<ITreeOption<TMgr>>(item, out var selectOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetName(manager);

            return selectOption.GetName(tMgr);
        }

        public string GetStyle(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<ITreeOption<TMgr>>(item, out var selectOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return manager.ErrorOption.GetStyle(manager);

            return selectOption.GetStyle(tMgr);
        }

        public IReadOnlyList<object> GetChildren(object item, IListuiManager manager)
        {
            if (LuiAssert.Type<ITreeOption<TMgr>>(item, out var selectOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr)) return System.Array.Empty<object>();

            return selectOption.GetChildren(tMgr);
        }
    }
}
