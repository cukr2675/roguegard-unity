using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ListingMF
{
    public class ElementToStringHandler : IElementHandler
    {
        public static ElementToStringHandler Instance { get; } = new();

        public string GetName(object element, IListMenuManager manager, IListMenuArg arg)
        {
            return element?.ToString();
        }
    }
}
