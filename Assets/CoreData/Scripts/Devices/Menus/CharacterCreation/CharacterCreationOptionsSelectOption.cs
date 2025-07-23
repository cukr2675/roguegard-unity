using Lysionium;
using Roguegard.CharacterCreation;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class CharacterCreationOptionsSelectOption : ISelectOption
    {
        private object editTarget;

        private readonly SelectOptionMenu nextMenu;

        public CharacterCreationOptionsSelectOption(ICharacterCreationDatabase database)
        {
            nextMenu = new SelectOptionMenu() { database = database };
        }

        private CharacterCreationOptionsSelectOption SetInner(object editTarget)
        {
            this.editTarget = editTarget ?? throw new System.ArgumentNullException(nameof(editTarget));
            return this;
        }

        public CharacterCreationOptionsSelectOption Set(Race race) => SetInner(race);
        public CharacterCreationOptionsSelectOption Set(Appearance appearance) => SetInner(appearance);
        public CharacterCreationOptionsSelectOption Set(Intrinsic intrinsic) => SetInner(intrinsic);
        public CharacterCreationOptionsSelectOption Set(StartingItem startingItem) => SetInner(startingItem);
        public CharacterCreationOptionsSelectOption Set(SingleItemMember singleItemMember) => SetInner(singleItemMember);
        public CharacterCreationOptionsSelectOption Set(AlphabetTypeMember alphabetTypeMember) => SetInner(alphabetTypeMember);

        string ISelectOption.GetName(IListMenuManager manager, IListMenuArg arg)
        {
            if (editTarget is Race race)
            {
                return race.Option.Name;
            }
            else if (editTarget is Appearance appearance)
            {
                return appearance.Option.Name;
            }
            else if (editTarget is Intrinsic intrinsic)
            {
                return intrinsic.Option.Name;
            }
            else if (editTarget is StartingItem startingItem)
            {
                return startingItem.Option.Name;
            }
            else if (editTarget is SingleItemMember singleItemMember)
            {
                return singleItemMember.ItemOption?.Name;
            }
            else if (editTarget is AlphabetTypeMember alphabetTypeMember)
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
            manager.PushMenuScreen(nextMenu, arg.Self, other: editTarget);
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
                var editTarget = arg.Arg.Other;

                elms.Clear();
                CharacterCreationAddMenu.AddOptionsTo(elms, arg.Self, editTarget, database);

                view.ShowTemplate(elms, manager, arg, editTarget?.GetType())
                    ?
                    .NameFrom((element, manager, arg) =>
                    {
                        if (element is IRogueDescribable describable)
                        {
                            return describable.Name;
                        }
                        else if (arg.Arg.Other is AlphabetTypeMember alphabetTypeMember)
                        {
                            return $"タイプ{alphabetTypeMember.Types[(int)element]}";
                        }
                        Debug.LogError("不正な型です。");
                        return null;
                    })

                    .OnClick((element, manager, arg) =>
                    {
                        if (arg.Arg.Other is Race race)
                        {
                            race.Option = (IRaceOption)element;
                        }
                        else if (arg.Arg.Other is Appearance appearance)
                        {
                            appearance.Option = (IAppearanceOption)element;
                        }
                        else if (arg.Arg.Other is Intrinsic intrinsic)
                        {
                            intrinsic.Option = (IIntrinsicOption)element;
                        }
                        else if (arg.Arg.Other is StartingItem startingItem)
                        {
                            CharacterCreationAddMenu.ReceiveStartingItemOptionObj(startingItem.Option, arg.Self);
                            startingItem.Option = (IStartingItemOption)element;
                            CharacterCreationAddMenu.ConsumeStartingItemOptionObj(startingItem.Option, arg.Self);
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
                        manager.PopMenuScreen();
                    })

                    .Build();
            }
        }
    }
}
