using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace Roguegard
{
    public interface IJsonSerializationSetting
    {
        void Serialize<T>(Stream stream, T instance);

        T Deserialize<T>(Stream stream);
    }
}
