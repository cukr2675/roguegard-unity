using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    public delegate string GetElementName<TElm, TMgr, TArg>(TElm element, TMgr manager, TArg arg);

    public delegate string GetElementName<TMgr, TArg>(TMgr manager, TArg arg);

    public delegate string GetElementStyle<TElm, TMgr, TArg>(TElm element, TMgr manager, TArg arg);

    public delegate string GetElementStyle<TMgr, TArg>(TMgr manager, TArg arg);

    public delegate void HandleClickElement<TElm, TMgr, TArg>(TElm element, TMgr manager, TArg arg);

    public delegate void HandleClickElement<TMgr, TArg>(TMgr manager, TArg arg);

    public delegate void HandleEndAnimation(IListMenuManager manager, IListMenuArg arg);
}
