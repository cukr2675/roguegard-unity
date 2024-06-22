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

        [SerializeField] private ScriptField<IObjCommand>[] _equipOptions;
        [SerializeField] private ScriptField<IObjCommand>[] _unequipOptions;

        [SerializeField] private ScriptField<IObjCommand>[] _rideOptions;
        [SerializeField] private ScriptField<IObjCommand>[] _unrideOptions;

        [SerializeField] private ScriptField<IObjCommand>[] _itemsOptions;
        [SerializeField] private ScriptField<IObjCommand>[] _groundOptions;

        [SerializeField] private ScriptField<IObjCommand> _shotOption;
        [SerializeField] private ScriptField<IObjCommand> _throwOption;

        private Dictionary<IKeyword, IObjCommand[]> optionTable;

        private IKeyword[] _categories;
        public override Spanning<IKeyword> Categories => _categories ?? Initialize();

        public override IObjCommand PickUpCommand => _groundOptions[0].Ref;

        private IKeyword[] Initialize()
        {
            optionTable = new Dictionary<IKeyword, IObjCommand[]>();
            foreach (var item in _items)
            {
                optionTable.Add(item.Keyword, item.GetCommands());
            }

            return _categories = new IKeyword[] { CategoryKw.Equipment, CategoryKw.Vehicle }.Concat(optionTable.Keys).ToArray();
        }

        public override void GetCommands(RogueObj self, RogueObj tool, IList<IObjCommand> commands)
        {
            if (optionTable == null) { Initialize(); }

            commands.Clear();

            if (tool.Location == self.Location)
            {
                AddRange(_groundOptions);
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
                    AddRange(_unequipOptions);
                }
                else
                {
                    AddRange(_equipOptions);
                }
            }
            else if (category == CategoryKw.Vehicle && toolVehicleInfo != null)
            {
                if (toolVehicleInfo.Rider != null)
                {
                    AddRange(_unrideOptions);
                }
                else
                {
                    AddRange(_rideOptions);
                }
            }
            else if (optionTable.TryGetValue(category, out var categoryOptions))
            {
                for (int i = 0; i < categoryOptions.Length; i++)
                {
                    commands.Add(categoryOptions[i]);
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
                        if (commands[i].GetType() != _throwOption.Ref.GetType()) continue;

                        commands.RemoveAt(i);
                        commands.Insert(i, _shotOption.Ref);
                        break;
                    }
                }
            }

            if (tool.Location == self)
            {
                AddRange(_itemsOptions);
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
