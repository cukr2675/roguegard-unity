using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public interface IButtonElementHandlerBuilder<TElm, TMgr, TArg, TBuilder> : IElementHandlerBuilder<TElm, TMgr, TArg, TBuilder>
    {
        TBuilder OnClick(HandleClickElement<TElm, TMgr, TArg> onClick);
    }
}
