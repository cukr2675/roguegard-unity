using Lysionium;
using Roguegard.CharacterCreation;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Device
{
    public class CharacterCreationOptionsSelectOption : ISelectOption<MMgr>
    {
        private RogueObj self;
        private object editTarget;

        private readonly SelectOptionMenu nextMenu;

        public CharacterCreationOptionsSelectOption(ICharacterCreationDatabase database)
        {
            nextMenu = new SelectOptionMenu() { database = database };
        }

        public CharacterCreationOptionsSelectOption Set(RogueObj self, object editTarget)
        {
            this.self = self;
            this.editTarget = editTarget ?? throw new System.ArgumentNullException(nameof(editTarget));
            return this;
        }

        string ISelectOption<MMgr>.GetName(MMgr manager)
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

        string ISelectOption<MMgr>.GetStyle(MMgr manager) => null;

        void ISelectOption<MMgr>.Click(MMgr manager, string clickName)
        {
            manager.PushScreen(nextMenu, self, other: editTarget);
        }

        private class SelectOptionMenu : RogueListuiScreen
        {
            public ICharacterCreationDatabase database;

            private readonly List<object> list = new();
            private readonly ScrollMenuViewData<object, MMgr> view = new()
            {
            };

            public SelectOptionMenu()
            {
                OnOpenScreen += (manager) =>
                {
                    var editTarget = Arg.Arg.Other;

                    list.Clear();
                    CharacterCreationAddScreen.AddOptionsTo(list, Arg.Self, editTarget, database);

                    view.Show(list, manager, editTarget?.GetType())
                    ?
                    .NameFrom((item, manager) =>
                    {
                        if (item is IRogueDescribable describable)
                        {
                            return describable.Name;
                        }
                        else if (Arg.Arg.Other is AlphabetTypeMember alphabetTypeMember)
                        {
                            return $"タイプ{alphabetTypeMember.Types[(int)item]}";
                        }
                        Debug.LogError("不正な型です。");
                        return null;
                    })

                    .OnClick((item, manager) =>
                    {
                        if (Arg.Arg.Other is Race race)
                        {
                            race.Option = (IRaceOption)item;
                        }
                        else if (Arg.Arg.Other is Appearance appearance)
                        {
                            appearance.Option = (IAppearanceOption)item;
                        }
                        else if (Arg.Arg.Other is Intrinsic intrinsic)
                        {
                            intrinsic.Option = (IIntrinsicOption)item;
                        }
                        else if (Arg.Arg.Other is StartingItem startingItem)
                        {
                            CharacterCreationAddScreen.ReceiveStartingItemOptionObj(startingItem.Option, Arg.Self);
                            startingItem.Option = (IStartingItemOption)item;
                            CharacterCreationAddScreen.ConsumeStartingItemOptionObj(startingItem.Option, Arg.Self);
                        }
                        else if (Arg.Arg.Other is SingleItemMember singleItemMember)
                        {
                            CharacterCreationAddScreen.ReceiveStartingItemOptionObj(singleItemMember.ItemOption, Arg.Self);
                            singleItemMember.ItemOption = (IStartingItemOption)item;
                            CharacterCreationAddScreen.ConsumeStartingItemOptionObj(singleItemMember.ItemOption, Arg.Self);
                        }
                        else if (Arg.Arg.Other is AlphabetTypeMember alphabetTypeMember)
                        {
                            alphabetTypeMember.TypeIndex = (int)item;
                        }
                        manager.PopScreen();
                    })

                    .Build();
                };
            }
        }
    }
}
