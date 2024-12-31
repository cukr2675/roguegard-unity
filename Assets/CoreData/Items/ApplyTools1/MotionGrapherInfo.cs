using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RuntimeDotter;

namespace Roguegard
{
    [Objforming.Formable]
    public class MotionGrapherInfo : ISubTimelineClip
    {
        private List<IMotionGrapherTrack> _tracks;
        public Spanning<IMotionGrapherTrack> Tracks => _tracks;

        public int LoopCount { get; set; }

        public float PlaybackSpeed { get; set; }

        public Color32 MainColor { get; set; }

        private ShiftableColor[] _palette;
        public Spanning<ShiftableColor> Palette => _palette;

        float ISubTimelineClip.StartTime => 0f;

        private MotionGrapherInfo() { }

        public void AddTrack(IMotionGrapherTrack track)
        {
            _tracks.Add(track);
        }

        public void InsertTrack(int index, IMotionGrapherTrack track)
        {
            _tracks.Insert(index, track);
        }

        public void RemoveTrackAt(int index)
        {
            _tracks.RemoveAt(index);
        }

        public void SetPalette(int index, ShiftableColor color)
        {
            _palette[index] = color;
        }

        public static MotionGrapherInfo Get(RogueObj obj)
        {
            if (obj.TryGet<Info>(out var info))
            {
                return info.info;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 上書き不可
        /// </summary>
        public static void SetTo(RogueObj obj)
        {
            if (!obj.TryGet<Info>(out var info))
            {
                info = new Info();
                obj.SetInfo(info);
            }

            // 上書き不可
            if (info.info != null) throw new RogueException();

            info.info = new MotionGrapherInfo();
            info.info._tracks = new List<IMotionGrapherTrack>();
            info.info.LoopCount = 0; // 無限ループ
            info.info.PlaybackSpeed = 1f;
            info.info.MainColor = Color.white;
            info.info._palette = RoguegardSettings.DefaultPalette.ToArray();
        }

        bool ISubTimelineClip.TryGet<T>(out T value)
        {
            if (this is T tValue)
            {
                value = tValue;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        [Objforming.Formable]
        private class Info : IRogueObjInfo
        {
            public MotionGrapherInfo info;

            public bool IsExclusedWhenSerialize => false;

            public bool CanStack(IRogueObjInfo coming) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceObj(RogueObj obj, RogueObj clonedObj) => this;
        }
    }
}
