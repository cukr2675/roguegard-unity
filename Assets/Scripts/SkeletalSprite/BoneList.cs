using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace SkeletalSprite
{
    public class BoneList : IList<NodeBone>, IReadOnlyList<IReadOnlyNodeBone>
    {
        private readonly List<NodeBone> bones = new List<NodeBone>();

        public NodeBone this[int index]
        {
            get => bones[index];
            set => bones[index] = value;
        }
        IReadOnlyNodeBone IReadOnlyList<IReadOnlyNodeBone>.this[int index] => bones[index];

        public int Count => bones.Count;

        bool ICollection<NodeBone>.IsReadOnly => false;

        public void Add(NodeBone bone) => bones.Add(bone);
        public void Clear() => bones.Clear();
        public bool Contains(NodeBone bone) => bones.Contains(bone);
        public void CopyTo(NodeBone[] bones, int arrayIndex) => this.bones.CopyTo(bones, arrayIndex);
        public int IndexOf(NodeBone bone) => bones.IndexOf(bone);
        public void Insert(int index, NodeBone bone) => bones.Insert(index, bone);
        public bool Remove(NodeBone bone) => bones.Remove(bone);
        public void RemoveAt(int index) => bones.RemoveAt(index);
        public IEnumerator<NodeBone> GetEnumerator() => bones.GetEnumerator();
        IEnumerator<IReadOnlyNodeBone> IEnumerable<IReadOnlyNodeBone>.GetEnumerator() => bones.Cast<IReadOnlyNodeBone>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => bones.GetEnumerator();
    }
}
