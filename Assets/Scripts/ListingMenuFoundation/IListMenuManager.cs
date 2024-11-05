using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public interface IListMenuManager
    {
        ISelectOption BackOption { get; }

        ISelectOption ErrorOption { get; }

        IElementsSubView GetSubView(string subViewName);

        void HideAll(bool back);

        string Localize(string text);

        T Localize<T>(T obj);

        void PushMenuScreenFromExtension(object menuScreen, IListMenuArg arg);
    }
}
