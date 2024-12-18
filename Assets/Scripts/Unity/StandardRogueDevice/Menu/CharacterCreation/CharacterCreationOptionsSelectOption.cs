using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using ListingMF;

namespace RoguegardUnity
{
    public class CharacterCreationOptionsSelectOption : ISelectOption
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

            this.builder = builder;
            return this;
        }

        public CharacterCreationOptionsSelectOption Set(RaceBuilder builder) => SetInner(builder);
        public CharacterCreationOptionsSelectOption Set(AppearanceBuilder builder) => SetInner(builder);
        public CharacterCreationOptionsSelectOption Set(IntrinsicBuilder builder) => SetInner(builder);
        public CharacterCreationOptionsSelectOption Set(StartingItemBuilder builder) => SetInner(builder);
        public CharacterCreationOptionsSelectOption Set(SingleItemMember builder) => SetInner(builder);
        public CharacterCreationOptionsSelectOption Set(AlphabetTypeMember builder) => SetInner(builder);

        string ISelectOption.GetName(IListMenuManager manager, IListMenuArg arg)
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

        string ISelectOption.GetStyle(IListMenuManager manager, IListMenuArg arg) => null;

        void ISelectOption.HandleClick(IListMenuManager iManager, IListMenuArg iArg)
        {
            var manager = (MMgr)iManager;
            var arg = (MArg)iArg;
            manager.PushMenuScreen(nextMenu, arg.Self, other: builder);
        }

        private class SelectOptionMenu : RogueMenuScreen
        {
            private readonly List<object> elms = new();

            public ICharacterCreationDatabase database;

            private readonly ScrollViewTemplate<object, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var builder = arg.Arg.Other;

                elms.Clear();
                CharacterCreationAddMenu.AddOptionsTo(elms, arg.Self, builder, database);

                view.ShowTemplate(elms, manager, arg, builder?.GetType())
                    ?
                    .ElementNameFrom((element, manager, arg) =>
                    {
                        if (element is IRogueDescription description)
                        {
                            return description.Name;
                        }
                        else if (arg.Arg.Other is AlphabetTypeMember alphabetTypeMember)
                        {
                            return $"タイプ{alphabetTypeMember.Types[(int)element]}";
                        }
                        Debug.LogError("不正な型です。");
                        return null;
                    })

                    .OnClickElement((element, manager, arg) =>
                    {
                        if (arg.Arg.Other is RaceBuilder raceBuilder)
                        {
                            raceBuilder.Option = (IRaceOption)element;
                        }
                        else if (arg.Arg.Other is AppearanceBuilder appearanceBuilder)
                        {
                            appearanceBuilder.Option = (IAppearanceOption)element;
                        }
                        else if (arg.Arg.Other is IntrinsicBuilder intrinsicBuilder)
                        {
                            intrinsicBuilder.Option = (IIntrinsicOption)element;
                        }
                        else if (arg.Arg.Other is StartingItemBuilder startingItemBuilder)
                        {
                            CharacterCreationAddMenu.ReceiveStartingItemOptionObj(startingItemBuilder.Option, arg.Self);
                            startingItemBuilder.Option = (IStartingItemOption)element;
                            CharacterCreationAddMenu.ConsumeStartingItemOptionObj(startingItemBuilder.Option, arg.Self);
                        }
                        else if (arg.Arg.Other is SingleItemMember singleItemMember)
                        {
                            CharacterCreationAddMenu.ReceiveStartingItemOptionObj(singleItemMember.ItemOption, arg.Self);
                            singleItemMember.ItemOption = (IStartingItemOption)element;
                            CharacterCreationAddMenu.ConsumeStartingItemOptionObj(singleItemMember.ItemOption, arg.Self);
                        }
                        else if (arg.Arg.Other is AlphabetTypeMember alphabetTypeMember)
                        {
                            alphabetTypeMember.TypeIndex = (int)element;
                        }
                        manager.Back();
                    })

                    .Build();
            }
        }
    }
}
