using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;
using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class CharacterCreationAddMenu : BaseScrollModelsMenu<object>
    {
        private readonly ICharacterCreationDatabase database;
        private readonly List<object> models;

        private CharacterCreationDataBuilder builder;

        public CharacterCreationAddMenu(ICharacterCreationDatabase database)
        {
            this.database = database;
            models = new List<object>();
        }

        public void Set(CharacterCreationDataBuilder builder)
        {
            this.builder = builder;
        }

        protected override Spanning<object> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            models.Clear();
            AddOptionsTo(models, self, (System.Type)arg.Other, database);
            return models;
        }

        protected override string GetItemName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            return ((IRogueDescription)model).Name;
        }

        protected override void ItemActivate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var builderType = (System.Type)arg.Other;
            if (builderType == typeof(AppearanceBuilder))
            {
                var appearanceBuilder = builder.Appearances.Add();
                appearanceBuilder.Option = (IAppearanceOption)model;
            }
            else if (builderType == typeof(IntrinsicBuilder))
            {
                var intrinsicBuilder = builder.Intrinsics.Add();
                intrinsicBuilder.Option = (IIntrinsicOption)model;
            }
            else if (builderType == typeof(StartingItemBuilder))
            {
                var startingItemBuilder = builder.StartingItemTable.Add().Add();
                startingItemBuilder.Option = (IStartingItemOption)model;
                startingItemBuilder.Stack = 1;
                ConsumeStartingItemOptionObj(startingItemBuilder.Option, self);
            }

            root.Back();
        }

        public static void AddOptionsTo(List<object> models, RogueObj player, object builder, ICharacterCreationDatabase database)
        {
            if (builder is RaceBuilder)
            {
                for (int i = 0; i < database.RaceOptions.Count; i++)
                {
                    var option = database.RaceOptions[i];
                    models.Add(option);
                }
            }
            else if (builder is AppearanceBuilder appearanceBuilder)
            {
                for (int i = 0; i < database.AppearanceOptions.Count; i++)
                {
                    var option = database.AppearanceOptions[i];
                    if (appearanceBuilder.Option != null && option.BoneName == appearanceBuilder.Option.BoneName)
                    {
                        models.Add(option);
                    }
                }
            }
            else if (builder is IntrinsicBuilder)
            {
                for (int i = 0; i < database.IntrinsicOptions.Count; i++)
                {
                    var option = database.IntrinsicOptions[i];
                    models.Add(option);
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
                        !models.Contains(option))
                    {
                        models.Add(option);
                    }
                    if (item?.Main.BaseInfoSet is SewedEquipmentInfoSet &&
                        item.Main.RogueEffects.Effects.Count <= 1)
                    {
                        var objOption = new ObjStartingItemOption();
                        objOption.Obj = item.Clone();
                        models.Add(objOption);
                        continue;
                    }
                }
            }
            else if (builder is AlphabetTypeMember alphabetTypeMember)
            {
                for (int i = 0; i < alphabetTypeMember.Types.Count; i++)
                {
                    models.Add(i);
                }
            }
        }

        public static void AddOptionsTo(List<object> models, RogueObj player, System.Type builderType, ICharacterCreationDatabase database)
        {
            if (builderType == typeof(RaceBuilder))
            {
                for (int i = 0; i < database.AppearanceOptions.Count; i++)
                {
                    var option = database.AppearanceOptions[i];
                    models.Add(option);
                }
            }
            else if (builderType == typeof(AppearanceBuilder))
            {
                for (int i = 0; i < database.AppearanceOptions.Count; i++)
                {
                    var option = database.AppearanceOptions[i];
                    if (option.BoneName == BoneKeyword.Other)
                    {
                        models.Add(option);
                    }
                }
            }
            else if (builderType == typeof(IntrinsicBuilder))
            {
                for (int i = 0; i < database.IntrinsicOptions.Count; i++)
                {
                    var option = database.IntrinsicOptions[i];
                    models.Add(option);
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
                        !models.Contains(option))
                    {
                        models.Add(option);
                        continue;
                    }
                    if (item?.Main.BaseInfoSet is SewedEquipmentInfoSet &&
                        item.Main.RogueEffects.Effects.Count <= 1)
                    {
                        var objOption = new ObjStartingItemOption();
                        objOption.Obj = item.Clone();
                        models.Add(objOption);
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
