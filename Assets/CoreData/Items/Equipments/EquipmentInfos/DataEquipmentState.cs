using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class DataEquipmentState : IEquipmentState
    {
        private readonly EquipmentStateData data;

        private readonly Dictionary<IKeyword, RogueObj[]> table;

        public Spanning<IKeyword> Parts => data.Parts;

        private DataEquipmentState(EquipmentStateData data)
        {
            this.data = data;
            table = new Dictionary<IKeyword, RogueObj[]>();
            foreach (var dataItem in data)
            {
                var array = new RogueObj[dataItem.Value];
                table.Add(dataItem.Key, array);
            }
        }

        public static DataEquipmentState CreateOrReuse(RogueObj self, EquipmentStateData data)
        {
            var equipmentState = self.Main.GetEquipmentState(self);
            if (equipmentState is DataEquipmentState dataState && dataState.data == data) return dataState;
            else return new DataEquipmentState(data);
        }

        public int GetLength(IKeyword keyword)
        {
            if (table.TryGetValue(keyword, out var array))
            {
                return array.Length;
            }
            else
            {
                return -1;
            }
        }

        public RogueObj GetEquipment(IKeyword keyword, int index)
        {
            if (table.TryGetValue(keyword, out var array))
            {
                return array[index];
            }
            else
            {
                throw new System.ArgumentException();
            }
        }

        public void SetEquipment(IKeyword keyword, int index, RogueObj equipment)
        {
            var array = table[keyword];
            if (array[index] != null) throw new RogueException();

            array[index] = equipment;
        }

        public void RemoveEquipment(IKeyword keyword, int index)
        {
            var array = table[keyword];
            if (array[index] == null) throw new RogueException();

            array[index] = null;
        }
    }
}
