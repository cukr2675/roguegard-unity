using Lysionium;
using Roguegard.CharacterCreation;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class CharacterCreationOptionsSelectOption : ISelectOption<MMgr, MArg>
    {
        private object editTarget;

        private readonly SelectOptionMenu nextMenu;

        public CharacterCreationOptionsSelectOption(ICharacterCreationDatabase database)
        {
            nextMenu = new SelectOptionMenu() { database = database };
        }

        public CharacterCreationOptionsSelectOption Set(object editTarget)
        {
            this.editTarget = editTarget ?? throw new System.ArgumentNullException(nameof(editTarget));
            return this;
        }

        string ISelectOption<MMgr, MArg>.GetName(MMgr manager, MArg arg)
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

        string ISelectOption<MMgr, MArg>.GetStyle(MMgr manager, MArg arg) => null;

        void ISelectOption<MMgr, MArg>.Click(MMgr iManager, MArg iArg)
        {
            var manager = (MMgr)iManager;
            var arg = (MArg)iArg;
            manager.PushScreen(nextMenu, arg.Self, other: editTarget);
        }

        private class SelectOptionMenu : RogueListuiScreen
        {
            public ICharacterCreationDatabase database;

            private readonly List<object> list = new();
            private readonly ScrollMenuViewData<object, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                var editTarget = arg.Arg.Other;

                list.Clear();
                CharacterCreationAddScreen.AddOptionsTo(list, arg.Self, editTarget, database);

                view.Show(list, manager, arg, editTarget?.GetType())
                    ?
                    .NameFrom((item, manager, arg) =>
                    {
                        if (item is IRogueDescribable describable)
                        {
                            return describable.Name;
                        }
                        else if (arg.Arg.Other is AlphabetTypeMember alphabetTypeMember)
                        {
                            return $"タイプ{alphabetTypeMember.Types[(int)item]}";
                        }
                        Debug.LogError("不正な型です。");
                        return null;
                    })

                    .OnClick((item, manager, arg) =>
                    {
                        if (arg.Arg.Other is Race race)
                        {
                            race.Option = (IRaceOption)item;
                        }
                        else if (arg.Arg.Other is Appearance appearance)
                        {
                            appearance.Option = (IAppearanceOption)item;
                        }
                        else if (arg.Arg.Other is Intrinsic intrinsic)
                        {
                            intrinsic.Option = (IIntrinsicOption)item;
                        }
                        else if (arg.Arg.Other is StartingItem startingItem)
                        {
                            CharacterCreationAddScreen.ReceiveStartingItemOptionObj(startingItem.Option, arg.Self);
                            startingItem.Option = (IStartingItemOption)item;
                            CharacterCreationAddScreen.ConsumeStartingItemOptionObj(startingItem.Option, arg.Self);
                        }
                        else if (arg.Arg.Other is SingleItemMember singleItemMember)
                        {
                            CharacterCreationAddScreen.ReceiveStartingItemOptionObj(singleItemMember.ItemOption, arg.Self);
                            singleItemMember.ItemOption = (IStartingItemOption)item;
                            CharacterCreationAddScreen.ConsumeStartingItemOptionObj(singleItemMember.ItemOption, arg.Self);
                        }
                        else if (arg.Arg.Other is AlphabetTypeMember alphabetTypeMember)
                        {
                            alphabetTypeMember.TypeIndex = (int)item;
                        }
                        manager.PopScreen();
                    })

                    .Build();
            }
        }
    }
}
