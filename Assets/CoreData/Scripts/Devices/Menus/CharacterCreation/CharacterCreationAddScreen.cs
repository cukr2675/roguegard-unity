using Lysionium;
using OchalikeSprites;
using Roguegard.CharacterCreation;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class CharacterCreationAddScreen : RogueListuiScreen
    {
        private readonly List<object> list = new();
        private readonly ScrollMenuViewData<object, MMgr, MArg> view = new()
        {
        };

        private readonly ICharacterCreationDatabase database;

        private CharacterCreationData characterCreationData;

        public CharacterCreationAddScreen(ICharacterCreationDatabase database)
        {
            this.database = database;
        }

        public void Set(CharacterCreationData characterCreationData)
        {
            this.characterCreationData = characterCreationData;
        }

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            list.Clear();
            AddOptionsTo(list, arg.Self, (System.Type)arg.Arg.Other, database);

            view.Show(list, manager, arg)
                ?
                .NameFrom((item, manager, arg) => ((IRogueDescribable)item).Name)

                .OnClick((item, manager, arg) =>
                {
                    var editTargetType = (System.Type)arg.Arg.Other;
                    if (editTargetType == typeof(Appearance))
                    {
                        var appearance = characterCreationData.Appearances.Add();
                        appearance.Option = (IAppearanceOption)item;
                    }
                    else if (editTargetType == typeof(Intrinsic))
                    {
                        var intrinsic = characterCreationData.Intrinsics.Add();
                        intrinsic.Option = (IIntrinsicOption)item;
                    }
                    else if (editTargetType == typeof(StartingItem))
                    {
                        var startingItem = characterCreationData.StartingItemTable.Add().Add();
                        startingItem.Option = (IStartingItemOption)item;
                        startingItem.Stack = 1;
                        ConsumeStartingItemOptionObj(startingItem.Option, arg.Self);
                    }

                    manager.PopScreen();
                })

                .Build();
        }

        public static void AddOptionsTo(List<object> list, RogueObj player, object editTarget, ICharacterCreationDatabase database)
        {
            if (editTarget is Race)
            {
                foreach (var option in database.RaceOptions)
                {
                    list.Add(option);
                }
            }
            else if (editTarget is Appearance appearance)
            {
                foreach (var option in database.AppearanceOptions)
                {
                    if (appearance.Option != null && option.BoneName == appearance.Option.BoneName)
                    {
                        list.Add(option);
                    }
                }
            }
            else if (editTarget is Intrinsic)
            {
                foreach (var option in database.IntrinsicOptions)
                {
                    list.Add(option);
                }
            }
            else if (editTarget is StartingItem || editTarget is SingleItemMember)
            {
                if (player != null)
                {
                    foreach (var item in player.Space.Objs)
                    {
                        if (item?.Main.BaseInfoSet is CharacterCreationInfoSet itemInfoSet &&
                            itemInfoSet.Data is IStartingItemOption option &&
                            item.Main.RogueEffects.Effects.Length <= 1 &&
                            !list.Contains(option))
                        {
                            list.Add(option);
                        }
                        if (item?.Main.BaseInfoSet is SewedEquipmentInfoSet &&
                            item.Main.RogueEffects.Effects.Length <= 1)
                        {
                            list.Add(new ObjStartingItemOption { Obj = item.Clone() });
                            continue;
                        }
                    }
                }
                else
                {
                    foreach (var option in database.StartingItemOptions)
                    {
                        list.Add(option);
                    }
                }
            }
            else if (editTarget is AlphabetTypeMember alphabetTypeMember)
            {
                for (int i = 0; i < alphabetTypeMember.Types.Length; i++)
                {
                    list.Add(i);
                }
            }
        }

        public static void AddOptionsTo(List<object> list, RogueObj player, System.Type editTargetType, ICharacterCreationDatabase database)
        {
            if (editTargetType == typeof(Race))
            {
                foreach (var option in database.AppearanceOptions)
                {
                    list.Add(option);
                }
            }
            else if (editTargetType == typeof(Appearance))
            {
                foreach (var option in database.AppearanceOptions)
                {
                    if (option.BoneName == BoneKeyword.Free)
                    {
                        list.Add(option);
                    }
                }
            }
            else if (editTargetType == typeof(Intrinsic))
            {
                foreach (var option in database.IntrinsicOptions)
                {
                    list.Add(option);
                }
            }
            else if (editTargetType == typeof(StartingItem) || editTargetType == typeof(SingleItemMember))
            {
                foreach (var item in player.Space.Objs)
                {
                    if (item?.Main.BaseInfoSet is CharacterCreationInfoSet itemInfoSet &&
                        itemInfoSet.Data is IStartingItemOption option &&
                        item.Main.RogueEffects.Effects.Length <= 1 &&
                        !list.Contains(option))
                    {
                        list.Add(option);
                        continue;
                    }
                    if (item?.Main.BaseInfoSet is SewedEquipmentInfoSet &&
                        item.Main.RogueEffects.Effects.Length <= 1)
                    {
                        list.Add(new ObjStartingItemOption { Obj = item.Clone() });
                        continue;
                    }
                }
            }
        }

        public static void ConsumeStartingItemOptionObj(IStartingItemOption startingItemOption, RogueObj player)
        {
            if (startingItemOption == null) throw new System.ArgumentNullException(nameof(startingItemOption));

            if (player != null)
            {
                foreach (var item in player.Space.Objs)
                {
                    if (item?.Main.BaseInfoSet is CharacterCreationInfoSet itemInfoSet &&
                        itemInfoSet.Data is IStartingItemOption option &&
                        item.Main.RogueEffects.Effects.Length <= 1 &&
                        option == startingItemOption)
                    {
                        item.TrySetStack(item.Stack - 1);
                        return;
                    }
                }

                // 見つからなかったら何もしない
            }
        }

        public static void ReceiveStartingItemOptionObj(IStartingItemOption startingItemOption, RogueObj player)
        {
            if (startingItemOption == null) return;

            if (player != null)
            {
                foreach (var item in player.Space.Objs)
                {
                    if (item?.Main.BaseInfoSet is CharacterCreationInfoSet itemInfoSet &&
                        itemInfoSet.Data is IStartingItemOption option &&
                        item.Main.RogueEffects.Effects.Length <= 1 &&
                        option == startingItemOption &&
                        item.Stack < item.GetMaxStack(StackOption.Default))
                    {
                        item.TrySetStack(item.Stack + 1);
                        return;
                    }
                }

                // 見つからないかスタックできなかったら新規オブジェクトを生成して獲得
                if (startingItemOption is CharacterCreationDataAsset data)
                {
                    data.CreateObj(player, Vector2Int.zero, RogueRandom.Primary);
                }
            }
        }
    }
}
