using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/ObjCommandTable")]
    public class CoreObjCommandTable : ObjCommandTable
    {
        [SerializeField] private Item[] _items = null;

        [SerializeField] private ScriptField<IObjCommand>[] _equipChoices;
        [SerializeField] private ScriptField<IObjCommand>[] _unequipChoices;

        [SerializeField] private ScriptField<IObjCommand>[] _rideChoices;
        [SerializeField] private ScriptField<IObjCommand>[] _unrideChoices;

        [SerializeField] private ScriptField<IObjCommand>[] _itemsChoices;
        [SerializeField] private ScriptField<IObjCommand>[] _groundChoices;

        [SerializeField] private ScriptField<IObjCommand> _shotChoice;
        [SerializeField] private ScriptField<IObjCommand> _throwChoice;

        private Dictionary<IKeyword, IObjCommand[]> choiceTable;

        private IKeyword[] _categories;
        public override Spanning<IKeyword> Categories => _categories ?? Initialize();

        public override IObjCommand PickUpCommand => _groundChoices[0].Ref;

        private IKeyword[] Initialize()
        {
            choiceTable = new Dictionary<IKeyword, IObjCommand[]>();
            foreach (var item in _items)
            {
                choiceTable.Add(item.Keyword, item.GetCommands());
            }

            return _categories = new IKeyword[] { CategoryKw.Equipment, CategoryKw.Vehicle }.Concat(choiceTable.Keys).ToArray();
        }

        public override void GetCommands(RogueObj self, RogueObj tool, IList<IObjCommand> commands)
        {
            if (choiceTable == null) { Initialize(); }

            commands.Clear();

            if (tool.Location == self.Location)
            {
                AddRange(_groundChoices);
            }

            var category = tool.Main.Category;
            if (category == null)
            {
                return;
            }

            var toolEquipmentInfo = tool.Main.GetEquipmentInfo(tool);
            var toolVehicleInfo = VehicleInfo.Get(tool);
            if (category == CategoryKw.Equipment && toolEquipmentInfo != null)
            {
                if (toolEquipmentInfo.EquipIndex >= 0)
                {
                    AddRange(_unequipChoices);
                }
                else
                {
                    AddRange(_equipChoices);
                }
            }
            else if (category == CategoryKw.Vehicle && toolVehicleInfo != null)
            {
                if (toolVehicleInfo.Rider != null)
                {
                    AddRange(_unrideChoices);
                }
                else
                {
                    AddRange(_rideChoices);
                }
            }
            else if (choiceTable.TryGetValue(category, out var categoryChoices))
            {
                for (int i = 0; i < categoryChoices.Length; i++)
                {
                    commands.Add(categoryChoices[i]);
                }
            }
            else
            {
                Debug.LogError("道具に対応するコマンドが見つかりません。");
            }

            var ammoInfo = EquipmentUtility.GetAmmoInfo(tool);
            if (ammoInfo != null)
            {
                EquipmentUtility.GetWeapon(self, out var weaponInfo);
                if (weaponInfo != null && EquipmentUtility.MatchAmmo(ammoInfo, weaponInfo.Throw.AmmoCategories))
                {
                    // 装備中の武器に対応した弾なら「投げる」ではなく「撃つ」にする。
                    for (int i = 0; i < commands.Count; i++)
                    {
                        if (commands[i].GetType() != _throwChoice.Ref.GetType()) continue;

                        commands.RemoveAt(i);
                        commands.Insert(i, _shotChoice.Ref);
                        break;
                    }
                }
            }

            if (tool.Location == self)
            {
                AddRange(_itemsChoices);
            }

            void AddRange(ScriptField<IObjCommand>[] array)
            {
                foreach (var item in array)
                {
                    commands.Add(item.Ref);
                }
            }
        }

        [System.Serializable]
        private class Item
        {
            [SerializeField] private KeywordData _keyword;
            public KeywordData Keyword => _keyword;

            [SerializeField] private ScriptField<IObjCommand>[] _commands;

            public IObjCommand[] GetCommands() => _commands.Select(x => x.Ref).ToArray();
        }
    }
}
