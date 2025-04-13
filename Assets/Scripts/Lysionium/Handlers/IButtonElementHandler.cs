using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    public interface IButtonElementHandler : IElementHandler
    {
        void HandleClick(object element, IListMenuManager manager, IListMenuArg arg);
    }
}
