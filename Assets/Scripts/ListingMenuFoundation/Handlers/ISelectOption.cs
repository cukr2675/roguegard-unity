using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

namespace ListingMF
{
    /// <summary>
    /// <see cref="SelectOptionHandler"/> のモデルとして扱うインターフェース。
    /// </summary>
    public interface ISelectOption
    {
        string GetName(IListMenuManager manager, IListMenuArg arg);

        string GetStyle(IListMenuManager manager, IListMenuArg arg);

        void HandleClick(IListMenuManager manager, IListMenuArg arg);
    }
}
