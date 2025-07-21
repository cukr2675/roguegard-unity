using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Roguegard
{
    [Objforming.Formable]
    public class SubTimelineMotionGrapherTrack : IMotionGrapherTrack
    {
        private readonly List<ISubTimelineClip> clips;

        [Objforming.CreateInstance, SuppressMessage("Style", "IDE0051")]
        private SubTimelineMotionGrapherTrack(bool _) { }

        public SubTimelineMotionGrapherTrack()
        {
            clips = new List<ISubTimelineClip>();
        }

        public void AddClip(ISubTimelineClip clip)
        {
            clips.Add(clip);
        }

        public bool TryGetValue(float time, out ISubTimelineClip value)
        {
            var index = IndexOf(time);
            if (index >= 0)
            {
                value = clips[index];
                return true;
            }
            else
            {
                value = clips[^1];
                return true;
            }
        }

        private int IndexOf(float time)
        {
            for (int i = 0; i < clips.Count; i++)
            {
                if (clips[i].StartTime == time) return i;
            }
            return -1;
        }
    }
}
