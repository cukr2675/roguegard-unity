using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class CharacterCreationOptionsSelectOption : IListMenuSelectOption
    {
        private object builder;

        private readonly SelectOptionMenu nextMenu;

        public CharacterCreationOptionsSelectOption(ICharacterCreationDatabase database)
        {
            nextMenu = new SelectOptionMenu() { database = database };
        }

        private CharacterCreationOptionsSelectOption SetInner(object builder)
        {
            if (builder == null) throw new System.ArgumentNullException(nameof(builder));
            if (builder.GetType() != this.builder?.GetType())
            {
                nextMenu.ViewPosition.Reset();
            }

            this.builder = builder;
            return this;
        }

        public CharacterCreationOptionsSelectOption Set(RaceBuilder builder) => SetInner(builder);
        public CharacterCreationOptionsSelectOption Set(AppearanceBuilder builder) => SetInner(builder);
        public CharacterCreationOptionsSelectOption Set(IntrinsicBuilder builder) => SetInner(builder);
        public CharacterCreationOptionsSelectOption Set(StartingItemBuilder builder) => SetInner(builder);
        public CharacterCreationOptionsSelectOption Set(SingleItemMember builder) => SetInner(builder);
        public CharacterCreationOptionsSelectOption Set(AlphabetTypeMember builder) => SetInner(builder);

        public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
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

        public void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
            manager.OpenMenu(nextMenu, self, null, new(other: builder));
        }

        private class SelectOptionMenu : BaseScrollListMenu<object>
        {
            private readonly List<object> elms = new();

            public ICharacterCreationDatabase database;

            protected override Spanning<object> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                elms.Clear();
                CharacterCreationAddMenu.AddOptionsTo(elms, self, arg.Other, database);
                return elms;
            }

            protected override string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (element is IRogueDescription description)
                {
                    return description.Name;
                }
                else if (arg.Other is AlphabetTypeMember alphabetTypeMember)
                {
                    return $"タイプ{alphabetTypeMember.Types[(int)element]}";
                }
                Debug.LogError("不正な型です。");
                return null;
            }

            protected override void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    raceBuilder.Option = (IRaceOption)element;
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    appearanceBuilder.Option = (IAppearanceOption)element;
                }
                else if (arg.Other is IntrinsicBuilder intrinsicBuilder)
                {
                    intrinsicBuilder.Option = (IIntrinsicOption)element;
                }
                else if (arg.Other is StartingItemBuilder startingItemBuilder)
                {
                    CharacterCreationAddMenu.ReceiveStartingItemOptionObj(startingItemBuilder.Option, self);
                    startingItemBuilder.Option = (IStartingItemOption)element;
                    CharacterCreationAddMenu.ConsumeStartingItemOptionObj(startingItemBuilder.Option, self);
                }
                else if (arg.Other is SingleItemMember singleItemMember)
                {
                    CharacterCreationAddMenu.ReceiveStartingItemOptionObj(singleItemMember.ItemOption, self);
                    singleItemMember.ItemOption = (IStartingItemOption)element;
                    CharacterCreationAddMenu.ConsumeStartingItemOptionObj(singleItemMember.ItemOption, self);
                }
                else if (arg.Other is AlphabetTypeMember alphabetTypeMember)
                {
                    alphabetTypeMember.TypeIndex = (int)element;
                }
                manager.Back();
            }
        }
    }
}
