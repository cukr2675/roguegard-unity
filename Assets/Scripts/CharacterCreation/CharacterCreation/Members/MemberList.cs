using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    /// <summary>
    /// インスペクターにリスト要素をインライン展開するクラス。
    /// リストに対して PropertyDrawer は無効なので専用のクラスが必要。
    /// </summary>
    [System.Serializable]
    public class MemberList
    {
        [SerializeReference] private List<IMember> _items = new();

        public IMember this[int index] => _items[index];

        public int Count => _items.Count;

        public Spanning<IMember> Span => Spanning.Get(_items);

        public void Add(IMember member)
        {
            _items.Add(member);
        }

        public void Clear()
        {
            _items.Clear();
        }
    }
}
