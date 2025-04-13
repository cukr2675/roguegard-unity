using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium
{
    public interface IListMenuArg
    {
        void CopyTo(ref IListMenuArg dest);
    }
}
