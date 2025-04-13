using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lysionium.Audio.Editor
{
    public abstract class AudioPlaySeed : ScriptableObject
    {
        public abstract AudioPlayTable.Item[] CreatePlayItems(string directory, int blankSamples);
    }
}
