using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    public interface IListMenuManager
    {
        ISelectOption BackOption { get; }

        ISelectOption ErrorOption { get; }

        IElementsSubview GetSubview(string subviewName);

        void HideAll(bool back);

        string Localize(string text);

        T Localize<T>(T obj);

        void PushMenuScreenFromExtension(object menuScreen, IListMenuArg arg);
    }
}
