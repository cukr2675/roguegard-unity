using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class SubTimelineMotionGrapherTrack : IMotionGrapherTrack
    {
        private readonly List<ISubTimelineClip> clips;

        public SubTimelineMotionGrapherTrack()
        {
            clips = new List<ISubTimelineClip>();
        }

        [Objforming.CreateInstance] private SubTimelineMotionGrapherTrack(bool dummy) { }

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
                value = clips[clips.Count - 1];
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
