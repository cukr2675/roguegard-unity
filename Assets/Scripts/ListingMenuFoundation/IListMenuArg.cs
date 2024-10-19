using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public interface IListMenuArg
    {
        void CopyTo(ref IListMenuArg dest);
    }
}
