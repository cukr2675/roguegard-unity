using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class CharacterCreationSelectOptionChoice : IModelsMenuChoice
    {
        private object builder;

        private readonly SelectOptionMenu nextMenu;

        public CharacterCreationSelectOptionChoice(ICharacterCreationDatabase database)
        {
            nextMenu = new SelectOptionMenu() { database = database };
        }

        private CharacterCreationSelectOptionChoice SetInner(object builder)
        {
            if (builder == null) throw new System.ArgumentNullException(nameof(builder));
            if (builder.GetType() != this.builder?.GetType())
            {
                nextMenu.ViewPosition.Reset();
            }

            this.builder = builder;
            return this;
        }

        public CharacterCreationSelectOptionChoice Set(RaceBuilder builder) => SetInner(builder);
        public CharacterCreationSelectOptionChoice Set(AppearanceBuilder builder) => SetInner(builder);
        public CharacterCreationSelectOptionChoice Set(IntrinsicBuilder builder) => SetInner(builder);
        public CharacterCreationSelectOptionChoice Set(StartingItemBuilder builder) => SetInner(builder);
        public CharacterCreationSelectOptionChoice Set(SingleItemMember builder) => SetInner(builder);
        public CharacterCreationSelectOptionChoice Set(AlphabetTypeMember builder) => SetInner(builder);

        public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (builder is RaceBuilder raceBuilder)
            {
                return raceBuilder.Option.Name;
            }
            else if (builder is AppearanceBuilder appearanceBuilder)
            {
                return appearanceBuilder.Option.Name;
            }
            else if (builder is IntrinsicBuilder intrinsicBuilder)
            {
                return intrinsicBuilder.Option.Name;
            }
            else if (builder is StartingItemBuilder startingItemBuilder)
            {
                return startingItemBuilder.Option.Name;
            }
            else if (builder is SingleItemMember singleItemMember)
            {
                return singleItemMember.ItemOption?.Name;
            }
            else if (builder is AlphabetTypeMember alphabetTypeMember)
            {
                return $"タイプ{alphabetTypeMember.Type}";
            }
            Debug.LogError("不正な型です。");
            return null;
        }

        public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            root.OpenMenu(nextMenu, self, null, new(other: builder));
        }

        private class SelectOptionMenu : BaseScrollModelsMenu<object>
        {
            private readonly List<object> models = new();

            public ICharacterCreationDatabase database;

            protected override Spanning<object> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                models.Clear();
                CharacterCreationAddMenu.AddOptionsTo(models, self, arg.Other, database);
                return models;
            }

            protected override string GetItemName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model is IRogueDescription description)
                {
                    return description.Name;
                }
                else if (arg.Other is AlphabetTypeMember alphabetTypeMember)
                {
                    return $"タイプ{alphabetTypeMember.Types[(int)model]}";
                }
                Debug.LogError("不正な型です。");
                return null;
            }

            protected override void ItemActivate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    raceBuilder.Option = (IRaceOption)model;
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    appearanceBuilder.Option = (IAppearanceOption)model;
                }
                else if (arg.Other is IntrinsicBuilder intrinsicBuilder)
                {
                    intrinsicBuilder.Option = (IIntrinsicOption)model;
                }
                else if (arg.Other is StartingItemBuilder startingItemBuilder)
                {
                    CharacterCreationAddMenu.ReceiveStartingItemOptionObj(startingItemBuilder.Option, self);
                    startingItemBuilder.Option = (IStartingItemOption)model;
                    CharacterCreationAddMenu.ConsumeStartingItemOptionObj(startingItemBuilder.Option, self);
                }
                else if (arg.Other is SingleItemMember singleItemMember)
                {
                    CharacterCreationAddMenu.ReceiveStartingItemOptionObj(singleItemMember.ItemOption, self);
                    singleItemMember.ItemOption = (IStartingItemOption)model;
                    CharacterCreationAddMenu.ConsumeStartingItemOptionObj(singleItemMember.ItemOption, self);
                }
                else if (arg.Other is AlphabetTypeMember alphabetTypeMember)
                {
                    alphabetTypeMember.TypeIndex = (int)model;
                }
                root.Back();
            }
        }
    }
}
