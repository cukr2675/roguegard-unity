using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IRgpack
    {
        string ID { get; }

        bool TryGetAsset<T>(string id, out T asset);
    }
}
