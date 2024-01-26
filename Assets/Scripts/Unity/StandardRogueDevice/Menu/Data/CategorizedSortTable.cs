using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;

namespace RoguegardUnity
{
    public class CategorizedSortTable
    {
        private readonly Dictionary<IKeyword, RogueObjList[]> categorizedBufferTable;

        private static readonly OtherKeyword other = new OtherKeyword();
        private static readonly Comparer comparer = new Comparer();
        private static readonly RogueObjList mainBuffer = new RogueObjList();

        public CategorizedSortTable(Spanning<IKeyword> categories)
        {
            categorizedBufferTable = new Dictionary<IKeyword, RogueObjList[]>();
            for (int i = 0; i < categories.Count; i++)
            {
                categorizedBufferTable.Add(categories[i], new[] { new RogueObjList(), new RogueObjList(), new RogueObjList(), new RogueObjList() });
            }
            categorizedBufferTable.Add(other, new[] { new RogueObjList(), new RogueObjList(), new RogueObjList(), new RogueObjList() });
        }

        public void Sort(RogueObj location)
        {
            // �e�[�u����������
            foreach (var pair in categorizedBufferTable)
            {
                foreach (var listItem in pair.Value)
                {
                    listItem.Clear();
                }
            }

            // �A�C�e������בւ���
            var storageObjs = ChestInfo.GetStorage(location);
            var objs = storageObjs ?? location.Space.Objs;
            for (int i = 0; i < objs.Count; i++)
            {
                var obj = objs[i];
                if (obj == null) continue;

                // �I�u�W�F�N�g�̃J�e�S�����Ƃ̃��X�g�ɐU�蕪����
                if (obj.Main.InfoSet.Category == null || !categorizedBufferTable.TryGetValue(obj.Main.InfoSet.Category, out var categorizedBuffers))
                {
                    categorizedBuffers = categorizedBufferTable[other];
                }
                var weight = WeightCalculator.Get(obj);
                var equipmentInfo = obj.Main.GetEquipmentInfo(obj);
                var vehicleInfo = VehicleInfo.Get(obj);
                var equipped = equipmentInfo?.EquipIndex >= 0 || vehicleInfo?.Rider != null;

                // �d���[���̃A�C�e������ɕ��ׂ�
                if (weight.TotalWeight <= 0f)
                {
                    // ���̒��ł������ς݂̃A�C�e�����ł���ɕ��ׂ�
                    if (equipped) { categorizedBuffers[0].Add(obj); }
                    else { categorizedBuffers[1].Add(obj); }
                }
                else
                {
                    // �����ς݂̃A�C�e������ɕ��ׂ�
                    if (equipped) { categorizedBuffers[2].Add(obj); }
                    else { categorizedBuffers[3].Add(obj); }
                }
            }

            // �o�b�t�@�����Ƃɕ��בւ���
            mainBuffer.Clear();
            foreach (var pair in categorizedBufferTable)
            {
                foreach (var listItem in pair.Value)
                {
                    // �����N���X�̃A�C�e���͖��O���ŕ��ׂ�
                    listItem.Sort(comparer);

                    for (int i = 0; i < listItem.Count; i++)
                    {
                        mainBuffer.Add(listItem[i]);
                    }
                }
            }
            if (storageObjs != null) { storageObjs.Sort(mainBuffer); }
            else { location.Space.Sort(mainBuffer); }
        }

        private class OtherKeyword : IKeyword
        {
            public string Name => null;
            public Sprite Icon => null;
            public Color Color => default;
            public string Caption => null;
            public IRogueDetails Details => null;
        }

        private class Comparer : IComparer<RogueObj>
        {
            public int Compare(RogueObj x, RogueObj y)
            {
                if (x == null || y == null) return 0;

                return string.CompareOrdinal(x.Main.InfoSet.Name, y.Main.InfoSet.Name);
            }
        }
    }
}
