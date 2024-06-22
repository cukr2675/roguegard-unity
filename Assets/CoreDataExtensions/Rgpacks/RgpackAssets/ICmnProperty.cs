using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.RequireRelationalComponent]
    public interface ICmnProperty
    {
        ICmnPropertySource Source { get; }

        ICmnProperty Clone();
    }
}
