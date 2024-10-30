using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;
using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;

using ListingMF;

namespace RoguegardUnity
{
    public class CharacterCreationAddMenu : RogueMenuScreen
    {
        private readonly ICharacterCreationDatabase database;
        private readonly List<object> elms;
        private readonly ScrollViewTemplate<object, RogueMenuManager, ReadOnlyMenuArg> view;

        private CharacterCreationDataBuilder builder;

        public CharacterCreationAddMenu(ICharacterCreationDatabase database)
        {
            this.database = database;
            elms = new List<object>();

            view = new()
            {
            };
        }

        public void Set(CharacterCreationDataBuilder builder)
        {
            this.builder = builder;
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            elms.Clear();
            AddOptionsTo(elms, arg.Self, (System.Type)arg.Arg.Other, database);

            view.ShowTemplate(elms, manager, arg)
                ?
                .ElementNameFrom((element, manager, arg) => ((IRogueDescription)element).Name)

                .OnClickElement((element, manager, arg) =>
                {
                    var builderType = (System.Type)arg.Arg.Other;
                    if (builderType == typeof(AppearanceBuilder))
                    {
                        var appearanceBuilder = builder.Appearances.Add();
                        appearanceBuilder.Option = (IAppearanceOption)element;
                    }
                    else if (builderType == typeof(IntrinsicBuilder))
                    {
                        var intrinsicBuilder = builder.Intrinsics.Add();
                        intrinsicBuilder.Option = (IIntrinsicOption)element;
                    }
                    else if (builderType == typeof(StartingItemBuilder))
                    {
                        var startingItemBuilder = builder.StartingItemTable.Add().Add();
                        startingItemBuilder.Option = (IStartingItemOption)element;
                        startingItemBuilder.Stack = 1;
                        ConsumeStartingItemOptionObj(startingItemBuilder.Option, arg.Self);
                    }

                    manager.HandleClickBack();
                })

                .Build();
        }

        public static void AddOptionsTo(List<object> elms, RogueObj player, object builder, ICharacterCreationDatabase database)
        {
            if (builder is RaceBuilder)
            {
                for (int i = 0; i < database.RaceOptions.Count; i++)
                {
                    var option = database.RaceOptions[i];
                    elms.Add(option);
                }
            }
            else if (builder is AppearanceBuilder appearanceBuilder)
            {
                for (int i = 0; i < database.AppearanceOptions.Count; i++)
                {
                    var option = database.AppearanceOptions[i];
                    if (appearanceBuilder.Option != null && option.BoneName == appearanceBuilder.Option.BoneName)
                    {
                        elms.Add(option);
                    }
                }
            }
            else if (builder is IntrinsicBuilder)
            {
                for (int i = 0; i < database.IntrinsicOptions.Count; i++)
                {
                    var option = database.IntrinsicOptions[i];
                    elms.Add(option);
                }
            }
            else if (builder is StartingItemBuilder || builder is SingleItemMember)
            {
                var playerItems = player.Space.Objs;
                for (int i = 0; i < playerItems.Count; i++)
                {
                    var item = playerItems[i];
                    if (item?.Main.BaseInfoSet is CharacterCreationInfoSet itemInfoSet &&
                        itemInfoSet.Data is IStartingItemOption option &&
                        item.Main.RogueEffects.Effects.Count <= 1 &&
                        !elms.Contains(option))
                    {
                        elms.Add(option);
                    }
                    if (item?.Main.BaseInfoSet is SewedEquipmentInfoSet &&
                        item.Main.RogueEffects.Effects.Count <= 1)
                    {
                        var objOption = new ObjStartingItemOption();
                        objOption.Obj = item.Clone();
                        elms.Add(objOption);
                        continue;
                    }
                }
            }
            else if (builder is AlphabetTypeMember alphabetTypeMember)
            {
                for (int i = 0; i < alphabetTypeMember.Types.Count; i++)
                {
                    elms.Add(i);
                }
            }
        }

        public static void AddOptionsTo(List<object> elms, RogueObj player, System.Type builderType, ICharacterCreationDatabase database)
        {
            if (builderType == typeof(RaceBuilder))
            {
                for (int i = 0; i < database.AppearanceOptions.Count; i++)
                {
                    var option = database.AppearanceOptions[i];
                    elms.Add(option);
                }
            }
            else if (builderType == typeof(AppearanceBuilder))
            {
                for (int i = 0; i < database.AppearanceOptions.Count; i++)
                {
                    var option = database.AppearanceOptions[i];
                    if (option.BoneName == BoneKeyword.Other)
                    {
                        elms.Add(option);
                    }
                }
            }
            else if (builderType == typeof(IntrinsicBuilder))
            {
                for (int i = 0; i < database.IntrinsicOptions.Count; i++)
                {
                    var option = database.IntrinsicOptions[i];
                    elms.Add(option);
                }
            }
            else if (builderType == typeof(StartingItemBuilder) || builderType == typeof(SingleItemMember))
            {
                var playerItems = player.Space.Objs;
                for (int i = 0; i < playerItems.Count; i++)
                {
                    var item = playerItems[i];
                    if (item?.Main.BaseInfoSet is CharacterCreationInfoSet itemInfoSet &&
                        itemInfoSet.Data is IStartingItemOption option &&
                        item.Main.RogueEffects.Effects.Count <= 1 &&
                        !elms.Contains(option))
                    {
                        elms.Add(option);
                        continue;
                    }
                    if (item?.Main.BaseInfoSet is SewedEquipmentInfoSet &&
                        item.Main.RogueEffects.Effects.Count <= 1)
                    {
                        var objOption = new ObjStartingItemOption();
                        objOption.Obj = item.Clone();
                        elms.Add(objOption);
                        continue;
                    }
                }
            }
        }

        public static void ConsumeStartingItemOptionObj(IStartingItemOption startingItemOption, RogueObj player)
        {
            if (startingItemOption == null) throw new System.ArgumentNullException(nameof(startingItemOption));

            var playerItems = player.Space.Objs;
            for (int i = 0; i < playerItems.Count; i++)
            {
                var item = playerItems[i];
                if (item?.Main.BaseInfoSet is CharacterCreationInfoSet itemInfoSet &&
                    itemInfoSet.Data is IStartingItemOption option &&
                    item.Main.RogueEffects.Effects.Count <= 1 &&
                    option == startingItemOption)
                {
                    item.TrySetStack(item.Stack - 1);
                    return;
                }
            }

            // 見つからなかったら何もしない
        }

        public static void ReceiveStartingItemOptionObj(IStartingItemOption startingItemOption, RogueObj player)
        {
            if (startingItemOption == null) return;

            var playerItems = player.Space.Objs;
            for (int i = 0; i < playerItems.Count; i++)
            {
                var item = playerItems[i];
                if (item?.Main.BaseInfoSet is CharacterCreationInfoSet itemInfoSet &&
                    itemInfoSet.Data is IStartingItemOption option &&
                    item.Main.RogueEffects.Effects.Count <= 1 &&
                    option == startingItemOption &&
                    item.Stack < item.GetMaxStack(StackOption.Default))
                {
                    item.TrySetStack(item.Stack + 1);
                    return;
                }
            }

            // 見つからないかスタックできなかったら新規オブジェクトを生成して獲得
            if (startingItemOption is ScriptableCharacterCreationData data)
            {
                data.CreateObj(player, Vector2Int.zero, RogueRandom.Primary);
            }
        }
    }
}
