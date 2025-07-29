using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    /// <summary>
    /// <see cref="SelectOptionViewItemHandler"/> のモデルとして扱うインターフェース。
    /// </summary>
    public interface ISelectOption
    {
        string GetName(IListMenuManager manager, IListMenuArg arg);

        string GetStyle(IListMenuManager manager, IListMenuArg arg);

        void HandleClick(IListMenuManager manager, IListMenuArg arg);
    }
}
