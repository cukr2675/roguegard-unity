using UnityEngine;

namespace Lysionium
{
    public interface IListMenuManager
    {
        ISelectOption<IListMenuManager, IListMenuArg> ErrorOption { get; }

        event System.Action OnUnload;

        void HideAll(bool back);

        void SetInvisibleDropdownPosition(Rect rect);

        string Localize(string text);
    }
}
