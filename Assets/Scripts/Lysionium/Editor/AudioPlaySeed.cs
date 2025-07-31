using UnityEngine;

namespace Lysionium.Audio.Editor
{
    public abstract class AudioPlaySeed : ScriptableObject
    {
        public abstract AudioPlayTable.Item[] CreatePlayItems(string directory, int blankSamples);

        public abstract void ClearSeedIsDirty();
    }
}
