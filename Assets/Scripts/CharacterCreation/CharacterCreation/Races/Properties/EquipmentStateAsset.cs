using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    [CreateAssetMenu(menuName = "Roguegard/Character Creation/Race Property/Equipment State")]
    public class EquipmentStateAsset : ScriptableObject, IEnumerable<KeyValuePair<IKeyword, int>>
    {
        [SerializeField] private List<Item> _items = null;

        [System.NonSerialized] private IKeyword[] slotsList;

        public Spanning<IKeyword> Slots => slotsList ??= _items.Select(x => x.Slot).ToArray();

        public IEnumerator<KeyValuePair<IKeyword, int>> GetEnumerator()
        {
            foreach (var item in _items)
            {
                yield return new KeyValuePair<IKeyword, int>(item.Slot, item.Length);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [System.Serializable]
        private class Item
        {
            [SerializeField] private KeywordAsset _slot;
            public IKeyword Slot => _slot;

            [SerializeField] private int _length;
            public int Length => _length;
        }
    }
}
