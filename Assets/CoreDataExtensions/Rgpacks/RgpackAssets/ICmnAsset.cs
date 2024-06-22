using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public interface ICmnAssset
    {
        IReadOnlyDictionary<string, ICmnPropertySource> PropertySources { get; }

        object Invoke(IReadOnlyDictionary<string, ICmnProperty> properties);
    }
}
