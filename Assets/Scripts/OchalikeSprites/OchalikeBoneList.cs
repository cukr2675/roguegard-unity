using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OchalikeSprites
{
    public class OchalikeBoneList : IList<OchalikeBone>, IReadOnlyList<IReadOnlyOchalikeBone>
    {
        private readonly List<OchalikeBone> bones = new();

        public OchalikeBone this[int index]
        {
            get => bones[index];
            set => bones[index] = value;
        }
        IReadOnlyOchalikeBone IReadOnlyList<IReadOnlyOchalikeBone>.this[int index] => bones[index];

        public int Count => bones.Count;

        bool ICollection<OchalikeBone>.IsReadOnly => false;

        public void Add(OchalikeBone bone) => bones.Add(bone);
        public void Clear() => bones.Clear();
        public bool Contains(OchalikeBone bone) => bones.Contains(bone);
        public void CopyTo(OchalikeBone[] bones, int arrayIndex) => this.bones.CopyTo(bones, arrayIndex);
        public int IndexOf(OchalikeBone bone) => bones.IndexOf(bone);
        public void Insert(int index, OchalikeBone bone) => bones.Insert(index, bone);
        public bool Remove(OchalikeBone bone) => bones.Remove(bone);
        public void RemoveAt(int index) => bones.RemoveAt(index);
        public IEnumerator<OchalikeBone> GetEnumerator() => bones.GetEnumerator();
        IEnumerator<IReadOnlyOchalikeBone> IEnumerable<IReadOnlyOchalikeBone>.GetEnumerator() => bones.Cast<IReadOnlyOchalikeBone>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => bones.GetEnumerator();
    }
}
