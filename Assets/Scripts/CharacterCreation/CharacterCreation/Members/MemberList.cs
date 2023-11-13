using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// インスペクターにリスト要素をインライン展開するクラス。
    /// リストに対して PropertyDrawer は無効なので専用のクラスが必要。
    /// </summary>
    [System.Serializable]
    public class MemberList //: IReadOnlyList<IMember>
    {
        [SerializeReference] private List<IMember> _items = new List<IMember>();

        public IMember this[int index] => _items[index];

        public int Count => _items.Count;

        public void Add(IMember member)
        {
            _items.Add(member);
        }

        public void Clear()
        {
            _items.Clear();
        }

        private IEnumerator<IMember> GetEnumerator() => _items.GetEnumerator();
        public static implicit operator Spanning<IMember>(MemberList list) => list._items;
    }
}
