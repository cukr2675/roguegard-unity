using Lysionium;
using Roguegard.CharacterCreation;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Roguegard.Device
{
    public class CharacterCreationOptionMenu : RogueMenuScreen
    {
        private readonly List<object> list = new();
        private readonly ICharacterCreationDatabase database;
        private readonly CharacterCreationOptionsSelectOption selectOption;
        private readonly RemoveSelectOption removeSelectOption;

        // selectOption と同時に出現するメンバーは別インスタンスにする。
        private readonly CharacterCreationOptionsSelectOption singleItemMemberSelectOption;
        private readonly CharacterCreationOptionsSelectOption alphabetTypeMemberSelectOption;

        private readonly VariableWidgetsMenuViewData<MMgr, MArg> raceView = new();
        private readonly VariableWidgetsMenuViewData<MMgr, MArg> appearanceView = new();
        private readonly VariableWidgetsMenuViewData<MMgr, MArg> intrinsicView = new();
        private readonly VariableWidgetsMenuViewData<MMgr, MArg> startingItemView = new();

        public CharacterCreationOptionMenu(ICharacterCreationDatabase database)
        {
            this.database = database;
            selectOption = new CharacterCreationOptionsSelectOption(database);
            removeSelectOption = new RemoveSelectOption();

            singleItemMemberSelectOption = new CharacterCreationOptionsSelectOption(database);
            alphabetTypeMemberSelectOption = new CharacterCreationOptionsSelectOption(database);
        }

        public void Set(CharacterCreationData characterCreationData)
        {
            removeSelectOption.characterCreationData = characterCreationData;
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            selectOption.Set(arg.Arg.Other);
            list.Clear();
            AddMemberElementsTo(list, (IReadOnlyMemberable)arg.Arg.Other);

            if (arg.Arg.Other is Race race)
            {
                raceView.Show(list, manager, arg)
                    ?
                    .HeadStack("職業／二つ名", InputFieldWidgetOption.Create<MMgr, MArg>(
                        (manager, arg) => removeSelectOption.characterCreationData.ShortName,
                        (manager, arg, value) => removeSelectOption.characterCreationData.ShortName = value))

                    .Head.Option(selectOption)

                    .Head.Option(
                        (manager, arg) =>
                        {
                            var race = (Race)arg.Arg.Other;
                            return $"性別：{race.Gender.Name}";
                        },
                        (manager, arg) =>
                        {
                            var race = (Race)arg.Arg.Other;
                            var nextMenu = new SelectGenderMenu { database = database };
                            manager.PushMenuScreen(nextMenu, arg.Self, other: arg.Arg.Other);
                        })

                    .Head.Option(
                        (manager, arg) =>
                        {
                            var race = (Race)arg.Arg.Other;
                            return $"<#{ColorUtility.ToHtmlStringRGBA(race.BodyColor)}>カラー";
                        },
                        ColorPicker())

                    .Build();
            }
            else if (arg.Arg.Other is Appearance appearance)
            {
                appearanceView.Show(list, manager, arg)
                    ?
                    .Head.Option(selectOption)

                    .Head.Option(
                        (manager, arg) =>
                        {
                            var appearance = (Appearance)arg.Arg.Other;
                            return $"<#{ColorUtility.ToHtmlStringRGBA(appearance.Color)}>カラー";
                        },
                        ColorPicker())

                    .Tail.Option(removeSelectOption)

                    .Build();
            }
            else if (arg.Arg.Other is Intrinsic intrinsic)
            {
                intrinsicView.Show(list, manager, arg)
                    ?
                    .Head.Option(selectOption)

                    .HeadStack("名前", InputFieldWidgetOption.Create<MMgr, MArg>(
                        (manager, arg) =>
                        {
                            var intrinsic = (Intrinsic)arg.Arg.Other;
                            return intrinsic.CustomName;
                        },
                        (manager, arg, value) =>
                        {
                            // 空欄の場合は上書きしないよう null にする
                            if (string.IsNullOrWhiteSpace(value)) { value = null; }

                            var intrinsic = (Intrinsic)arg.Arg.Other;
                            return intrinsic.CustomName = value;
                        }))

                    .Tail.Option(removeSelectOption)

                    .Build();
            }
            else if (arg.Arg.Other is StartingItem startingItem)
            {
                startingItemView.Show(list, manager, arg)
                    ?
                    .Head.Option(selectOption)

                    .HeadStack("個数", InputFieldWidgetOption.Create<MMgr, MArg>(
                        (manager, arg) =>
                        {
                            var startingItem = (StartingItem)arg.Arg.Other;
                            var value = startingItem.Stack / RogueObj.GetMaxStack(startingItem.Option.InfoSet, StackOption.Default);
                            return value.ToString();
                        },
                        (manager, arg, valueString) =>
                        {
                            if (int.TryParse(valueString, out var value))
                            {
                                var startingItem = (StartingItem)arg.Arg.Other;
                                startingItem.Stack = value * RogueObj.GetMaxStack(startingItem.Option.InfoSet, StackOption.Default);
                                if (startingItem.Stack <= 0) { startingItem.Stack = 1; }
                                return startingItem.Stack.ToString();
                            }
                            Debug.LogError("不正な型です。");
                            return valueString;
                        },
                        TMP_InputField.ContentType.IntegerNumber))

                    .Tail.Option(removeSelectOption)

                    .Build();
            }
            else
            {
                throw new System.InvalidOperationException();
            }
        }

        private void AddMemberElementsTo(List<object> list, IReadOnlyMemberable memberable)
        {
            foreach (var memberSource in memberable.MemberSources)
            {
                var member = memberable.GetMember(memberSource);
                if (member is SingleItemMember singleItemMember)
                {
                    list.Add(singleItemMemberSelectOption.Set(singleItemMember));
                }
                else if (member is EquipMember equipMember)
                {
                    list.Add(SelectOption.Create<MMgr, MArg>(
                        (manager, arg) =>
                        {
                            return $"<#808080>装備する：{equipMember.IsEquipped}";
                        },
                        (manager, arg) =>
                        {
                            equipMember.IsEquipped = !equipMember.IsEquipped;
                        }));
                }
                else if (member is AlphabetTypeMember alphabetTypeMember && memberable is Appearance appearance)
                {
                    appearance.Option.UpdateMemberRange(alphabetTypeMember, appearance, removeSelectOption.characterCreationData);
                    list.Add(alphabetTypeMemberSelectOption.Set(alphabetTypeMember));
                }
                else if (member is StandardRaceMember standardRaceMember && memberable is Race race)
                {
                    race.Option.UpdateMemberRange(standardRaceMember, race.Option, removeSelectOption.characterCreationData);
                    var standardRaceOption = (IStandardRaceOption)race.Option;
                    list.Add(StackWidgetOption.Create(
                        ("1*", "サイズ"),
                        ("1*", InputFieldWidgetOption.Create<MMgr, MArg>(
                            (manager, arg) => standardRaceMember.Size.ToString(),
                            (manager, arg, valueString) =>
                            {
                                if (!int.TryParse(valueString, out var value)) return valueString;

                                standardRaceMember.Size = Mathf.Clamp(value, standardRaceOption.MinSize, standardRaceOption.MaxSize);
                                return standardRaceMember.Size.ToString();
                            },
                            TMP_InputField.ContentType.IntegerNumber))));
                }
            }
        }

        private ColorPickerMenuScreen<MMgr, MArg> ColorPicker()
        {
            return new ColorPickerMenuScreen<MMgr, MArg>(
                (manager, arg) =>
                {
                    if (arg.Arg.Other is Race race)
                    {
                        return race.BodyColor;
                    }
                    else if (arg.Arg.Other is Appearance appearance)
                    {
                        return appearance.Color;
                    }
                    Debug.LogError("不正な型です。");
                    return Color.white;
                },
                (manager, arg, color) =>
                {
                    if (arg.Arg.Other is Race race)
                    {
                        race.BodyColor = color;
                        return;
                    }
                    else if (arg.Arg.Other is Appearance appearance)
                    {
                        appearance.Color = color;
                        return;
                    }
                    Debug.LogError("不正な型です。");
                });
        }

        private class RemoveSelectOption : ISelectOption
        {
            public CharacterCreationData characterCreationData;

            string ISelectOption.GetName(IListMenuManager manager, IListMenuArg arg) => "<#f00>削除";

            string ISelectOption.GetStyle(IListMenuManager manager, IListMenuArg arg) => null;

            void ISelectOption.Click(IListMenuManager iManager, IListMenuArg iArg)
            {
                var manager = (MMgr)iManager;
                var arg = (MArg)iArg;

                if (arg.Arg.Other is IReadOnlyMemberable memberable)
                {
                    foreach (var memberSource in memberable.MemberSources)
                    {
                        var member = memberable.GetMember(memberSource);
                        if (member is SingleItemMember singleItemMember)
                        {
                            // 見た目装備を削除したとき、その装備品を獲得する
                            CharacterCreationAddMenu.ReceiveStartingItemOptionObj(singleItemMember.ItemOption, arg.Self);
                        }
                    }
                }

                if (arg.Arg.Other is Appearance appearance)
                {
                    characterCreationData.Appearances.Remove(appearance);
                }
                else if (arg.Arg.Other is Intrinsic intrinsic)
                {
                    characterCreationData.Intrinsics.Remove(intrinsic);
                }
                else if (arg.Arg.Other is StartingItem startingItem)
                {
                    // 初期アイテムを削除したとき、そのアイテムを獲得する
                    CharacterCreationAddMenu.ReceiveStartingItemOptionObj(startingItem.Option, arg.Self);
                    characterCreationData.StartingItemTable.Remove(startingItem, true);
                }
                manager.PopMenuScreen();
            }
        }

        private class SelectGenderMenu : RogueMenuScreen
        {
            public ICharacterCreationDatabase database;

            private readonly ScrollMenuViewData<IRogueGender, MMgr, MArg> view = new()
            {
            };

            private Race race;

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                race = (Race)arg.Arg.Other;

                view.Show(race.Option.Genders, manager, arg)
                    ?
                    .NameFrom(gender => gender.Name)

                    .OnClick((gender, manager, arg) =>
                    {
                        race.Gender = gender;
                        manager.PopMenuScreen();
                    })

                    .Build();
            }
        }
    }
}
