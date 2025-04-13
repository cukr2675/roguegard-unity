using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    public interface IElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
    {
        TBuilder NameFrom(GetElementName<TElm, TMgr, TArg> nameFrom);
        TBuilder StyleFrom(GetElementStyle<TElm, TMgr, TArg> styleFrom);
    }
}
