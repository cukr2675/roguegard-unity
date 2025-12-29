using System.Collections.Generic;

namespace Lysionium
{
    public class TreeOptionViewItemHandler<TMgr, TArg> : ITreeViewItemHandler
    {
        public static TreeOptionViewItemHandler<TMgr, TArg> Instance { get; } = new();

        public string GetName(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (LuiAssert.Type<ITreeOption<TMgr, TArg>>(item, out var selectOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetName(manager, arg);

            return selectOption.GetName(tMgr, tArg);
        }

        public string GetStyle(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (LuiAssert.Type<ITreeOption<TMgr, TArg>>(item, out var selectOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return manager.ErrorOption.GetStyle(manager, arg);

            return selectOption.GetStyle(tMgr, tArg);
        }

        public IReadOnlyList<object> GetChildren(object item, IListMenuManager manager, IListMenuArg arg)
        {
            if (LuiAssert.Type<ITreeOption<TMgr, TArg>>(item, out var selectOption) ||
                LuiAssert.Type<TMgr>(manager, out var tMgr) ||
                LuiAssert.Type<TArg>(arg, out var tArg)) return System.Array.Empty<object>();

            return selectOption.GetChildren(tMgr, tArg);
        }
    }
}
