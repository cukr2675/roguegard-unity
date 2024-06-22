using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public interface ICmnProperty
    {
        ICmnPropertySource Source { get; }

        ICmnProperty Clone();
    }
}
