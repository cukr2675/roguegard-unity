using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public interface IListMenuManager
    {
        string BackName { get; }

        IElementsSubView GetSubView(string subViewName);

        void HideAll(bool back);

        void PushMenuScreen(IMenuScreen menuScreen, IListMenuArg arg);

        void HandleClickBack();

        void HandleClickError();
    }
}
