using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard
{
    [Objforming.Formable]
    public class SpriteMotionGrapherTrack : IMotionGrapherTrack
    {
        private List<Bone> _bones;
        public Spanning<Bone> Bones => _bones;

        public SpriteMotionGrapherTrack()
        {
            _bones = new List<Bone>();
        }

        [Objforming.CreateInstance] private SpriteMotionGrapherTrack(bool dummy) { }

        public void AddBone(string boneName)
        {
            var bone = new Bone();
            bone.BoneName = boneName;
            _bones.Add(bone);
        }

        public void RemoveBoneAt(int index)
        {
            _bones.RemoveAt(index);
        }

        public float[] SelectKeyTimes()
        {
            var times = new HashSet<float>();
            times.Add(0f);
            for (int i = 0; i < _bones.Count; i++)
            {
                var bone = _bones[i];
                AddRange(bone.Position.XKeys.ToKeyTimes());
                AddRange(bone.Position.YKeys.ToKeyTimes());
                AddRange(bone.Position.ZKeys.ToKeyTimes());
                AddRange(bone.Rotation.XKeys.ToKeyTimes());
                AddRange(bone.Rotation.YKeys.ToKeyTimes());
                AddRange(bone.Rotation.ZKeys.ToKeyTimes());
                AddRange(bone.Sprite.ToKeyTimes());
            }
            return times.OrderBy(x => x).ToArray();

            void AddRange(IEnumerable<float> keyTimes)
            {
                foreach (var keyTime in keyTimes)
                {
                    times.Add(keyTime);
                }
            }
        }

        [Objforming.Formable]
        public class Bone
        {
            public string BoneName { get; set; }
            public Vector3KeyFrameList Position { get; }
            public Vector3KeyFrameList Rotation { get; }
            public PaintKeyFrameList Sprite { get; }
            public FloatKeyFrameList Reorder { get; } = new FloatKeyFrameList();

            public Bone()
            {
                Position = new Vector3KeyFrameList();
                Rotation = new Vector3KeyFrameList();
                Sprite = new PaintKeyFrameList();
                Reorder = new FloatKeyFrameList();
            }

            [Objforming.CreateInstance] private Bone(bool dummy) { }
        }
    }
}
