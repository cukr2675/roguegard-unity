using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public interface IButtonElementHandler : IElementHandler
    {
        void HandleClick(object element, IListMenuManager manager, IListMenuArg arg);
    }
}
