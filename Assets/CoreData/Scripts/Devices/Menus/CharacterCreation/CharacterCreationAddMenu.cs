using Lysionium;
using OchalikeSprites;
using Roguegard.CharacterCreation;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class CharacterCreationAddMenu : RogueMenuScreen
    {
        private readonly ICharacterCreationDatabase database;
        private readonly List<object> elms;
        private readonly ScrollViewTemplate<object, MMgr, MArg> view;

        private CharacterCreationData characterCreationData;

        public CharacterCreationAddMenu(ICharacterCreationDatabase database)
        {
            this.database = database;
            elms = new List<object>();

            view = new()
            {
            };
        }

        public void Set(CharacterCreationData characterCreationData)
        {
            this.characterCreationData = characterCreationData;
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            elms.Clear();
            AddOptionsTo(elms, arg.Self, (System.Type)arg.Arg.Other, database);

            view.ShowTemplate(elms, manager, arg)
                ?
                .NameFrom((element, manager, arg) => ((IRogueDescribable)element).Name)

                .OnClick((element, manager, arg) =>
                {
                    var editTargetType = (System.Type)arg.Arg.Other;
                    if (editTargetType == typeof(Appearance))
                    {
                        var appearance = characterCreationData.Appearances.Add();
                        appearance.Option = (IAppearanceOption)element;
                    }
                    else if (editTargetType == typeof(Intrinsic))
                    {
                        var intrinsic = characterCreationData.Intrinsics.Add();
                        intrinsic.Option = (IIntrinsicOption)element;
                    }
                    else if (editTargetType == typeof(StartingItem))
                    {
                        var startingItem = characterCreationData.StartingItemTable.Add().Add();
                        startingItem.Option = (IStartingItemOption)element;
                        startingItem.Stack = 1;
                        ConsumeStartingItemOptionObj(startingItem.Option, arg.Self);
                    }

                    manager.PopMenuScreen();
                })

                .Build();
        }

        public static void AddOptionsTo(List<object> elms, RogueObj player, object editTarget, ICharacterCreationDatabase database)
        {
            if (editTarget is Race)
            {
                foreach (var option in database.RaceOptions)
                {
                    elms.Add(option);
                }
            }
            else if (editTarget is Appearance appearance)
            {
                foreach (var option in database.AppearanceOptions)
                {
                    if (appearance.Option != null && option.BoneName == appearance.Option.BoneName)
                    {
                        elms.Add(option);
                    }
                }
            }
            else if (editTarget is Intrinsic)
            {
                foreach (var option in database.IntrinsicOptions)
                {
                    elms.Add(option);
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
                            !elms.Contains(option))
                        {
                            elms.Add(option);
                        }
                        if (item?.Main.BaseInfoSet is SewedEquipmentInfoSet &&
                            item.Main.RogueEffects.Effects.Length <= 1)
                        {
                            elms.Add(new ObjStartingItemOption { Obj = item.Clone() });
                            continue;
                        }
                    }
                }
                else
                {
                    foreach (var option in database.StartingItemOptions)
                    {
                        elms.Add(option);
                    }
                }
            }
            else if (editTarget is AlphabetTypeMember alphabetTypeMember)
            {
                for (int i = 0; i < alphabetTypeMember.Types.Length; i++)
                {
                    elms.Add(i);
                }
            }
        }

        public static void AddOptionsTo(List<object> elms, RogueObj player, System.Type editTargetType, ICharacterCreationDatabase database)
        {
            if (editTargetType == typeof(Race))
            {
                foreach (var option in database.AppearanceOptions)
                {
                    elms.Add(option);
                }
            }
            else if (editTargetType == typeof(Appearance))
            {
                foreach (var option in database.AppearanceOptions)
                {
                    if (option.BoneName == BoneKeyword.Free)
                    {
                        elms.Add(option);
                    }
                }
            }
            else if (editTargetType == typeof(Intrinsic))
            {
                foreach (var option in database.IntrinsicOptions)
                {
                    elms.Add(option);
                }
            }
            else if (editTargetType == typeof(StartingItem) || editTargetType == typeof(SingleItemMember))
            {
                foreach (var item in player.Space.Objs)
                {
                    if (item?.Main.BaseInfoSet is CharacterCreationInfoSet itemInfoSet &&
                        itemInfoSet.Data is IStartingItemOption option &&
                        item.Main.RogueEffects.Effects.Length <= 1 &&
                        !elms.Contains(option))
                    {
                        elms.Add(option);
                        continue;
                    }
                    if (item?.Main.BaseInfoSet is SewedEquipmentInfoSet &&
                        item.Main.RogueEffects.Effects.Length <= 1)
                    {
                        elms.Add(new ObjStartingItemOption { Obj = item.Clone() });
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
