using UnityEngine;

namespace Lysionium.Audio.Editor
{
    public abstract class EvtfxAudioSeed : ScriptableObject
    {
        public abstract EvtfxAudioTable.Item[] CreateEvtfxAudioItems(string directory, int blankSamples);

        public abstract void ClearSeedIsDirty();
    }
}
