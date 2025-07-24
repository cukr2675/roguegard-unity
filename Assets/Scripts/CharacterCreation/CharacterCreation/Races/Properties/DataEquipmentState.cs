using System.Collections.Generic;

namespace Roguegard.CharacterCreation
{
    public class DataEquipmentState : IEquipmentState
    {
        private readonly EquipmentStateAsset data;

        private readonly Dictionary<IKeyword, RogueObj[]> table;

        public Spanning<IKeyword> Slots => data.Slots;

        private DataEquipmentState(EquipmentStateAsset data)
        {
            this.data = data;
            table = new Dictionary<IKeyword, RogueObj[]>();
            foreach (var dataItem in data)
            {
                var array = new RogueObj[dataItem.Value];
                table.Add(dataItem.Key, array);
            }
        }

        public static DataEquipmentState CreateOrReuse(RogueObj self, EquipmentStateAsset data)
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
            if (array[index] != null) throw new System.InvalidOperationException();

            array[index] = equipment;
        }

        public void RemoveEquipment(IKeyword keyword, int index)
        {
            var array = table[keyword];
            if (array[index] == null) throw new System.InvalidOperationException();

            array[index] = null;
        }
    }
}
