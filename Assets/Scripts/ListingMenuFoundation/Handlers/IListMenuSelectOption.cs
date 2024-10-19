using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    /// <summary>
    /// <see cref="SelectOptionHandler"/> のモデルとして扱うインターフェース。
    /// </summary>
    public interface IListMenuSelectOption
    {
        string GetName(IListMenuManager manager, IListMenuArg arg);

        void HandleClick(IListMenuManager manager, IListMenuArg arg);
    }
}
