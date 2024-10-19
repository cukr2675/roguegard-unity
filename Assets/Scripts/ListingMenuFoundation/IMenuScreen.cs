using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public interface IMenuScreen
    {
        void OpenScreen(IListMenuManager manager, IListMenuArg arg);
    }
}
