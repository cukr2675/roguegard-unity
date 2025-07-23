using Lysionium;
using Roguegard.CharacterCreation;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Roguegard.Device
{
    public class CharacterCreationOptionMenu : RogueMenuScreen
    {
        private readonly List<object> elms = new();
        private readonly ICharacterCreationDatabase database;
        private readonly CharacterCreationOptionsSelectOption selectOption;
        private readonly RemoveSelectOption removeSelectOption;

        // selectOption と同時に出現するメンバーは別インスタンスにする。
        private readonly CharacterCreationOptionsSelectOption singleItemMemberSelectOption;
        private readonly CharacterCreationOptionsSelectOption alphabetTypeMemberSelectOption;

        private readonly VariableWidgetsViewTemplate<MMgr, MArg> view;

        public CharacterCreationOptionMenu(ICharacterCreationDatabase database)
        {
            this.database = database;
            selectOption = new CharacterCreationOptionsSelectOption(database);
            removeSelectOption = new RemoveSelectOption();

            singleItemMemberSelectOption = new CharacterCreationOptionsSelectOption(database);
            alphabetTypeMemberSelectOption = new CharacterCreationOptionsSelectOption(database);

            view = new()
            {
            };
        }

        public void Set(CharacterCreationData characterCreationData)
        {
            removeSelectOption.characterCreationData = characterCreationData;
        }

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            if (arg.Arg.Other is Race race)
            {
                elms.Clear();
                elms.Add(
                    new object[]
                    {
                        "職業／二つ名",
                        InputFieldViewWidget.CreateOption<MMgr, MArg>(
                            (manager, arg) => removeSelectOption.characterCreationData.ShortName,
                            (manager, arg, value) => removeSelectOption.characterCreationData.ShortName = value)
                    });
                elms.Add(selectOption.Set(race));
                elms.Add(
                    new object[]
                    {
                        SelectOption.Create<MMgr, MArg>(
                            (manager, arg) =>
                            {
                                if (arg.Arg.Other is Race race)
                                {
                                    return $"性別：{race.Gender.Name}";
                                }
                                Debug.LogError("不正な型です。");
                                return "性別：";
                            },
                            (manager, arg) =>
                            {
                                if (arg.Arg.Other is Race race)
                                {
                                    var nextMenu = new SelectGenderMenu() { database = database };
                                    manager.PushMenuScreen(nextMenu, arg.Self, other: arg.Arg.Other);
                                    return;
                                }
                                Debug.LogError("不正な型です。");
                            })
                    });
                elms.Add(
                    SelectOption.Create<MMgr, MArg>(
                        $"<#{ColorUtility.ToHtmlStringRGBA(race.BodyColor)}>カラー",
                        ColorPicker()));
                AddMemberElements(race);
            }
            else if (arg.Arg.Other is Appearance appearance)
            {
                elms.Clear();
                elms.Add(selectOption.Set(appearance));
                elms.Add(
                    SelectOption.Create<MMgr, MArg>(
                        $"<#{ColorUtility.ToHtmlStringRGBA(appearance.Color)}>カラー",
                        ColorPicker()));
                AddMemberElements(appearance);
                elms.Add(removeSelectOption);
            }
            else if (arg.Arg.Other is Intrinsic intrinsic)
            {
                elms.Clear();
                elms.Add(selectOption.Set(intrinsic));
                elms.Add(
                    new object[]
                    {
                        "名前",
                        InputFieldViewWidget.CreateOption<MMgr, MArg>(
                            (manager, arg) =>
                            {
                                if (arg.Arg.Other is Intrinsic intrinsic)
                                {
                                    return intrinsic.OptionName;
                                }
                                Debug.LogError("不正な型です。");
                                return "???";
                            },
                            (manager, arg, value) =>
                            {
                                // 空欄の場合は上書きしないよう null にする
                                if (string.IsNullOrWhiteSpace(value)) { value = null; }

                                if (arg.Arg.Other is Intrinsic intrinsic)
                                {
                                    return intrinsic.OptionName = value;
                                }
                                Debug.LogError("不正な型です。");
                                return null;
                            })
                    });
                AddMemberElements(intrinsic);
                elms.Add(removeSelectOption);
            }
            else if (arg.Arg.Other is StartingItem startingItem)
            {
                Debug.Log("a");
                elms.Clear();
                elms.Add(selectOption.Set(startingItem));
                elms.Add(
                    new object[]
                    {
                        "個数",
                        InputFieldViewWidget.CreateOption<MMgr, MArg>(
                            (manager, arg) =>
                            {
                                if (arg.Arg.Other is StartingItem startingItem)
                                {
                                    var value = startingItem.Stack
                                        / RogueObj.GetMaxStack(startingItem.Option.InfoSet, StackOption.Default);
                                    return value.ToString();
                                }
                                Debug.LogError("不正な型です。");
                                return "0";
                            },
                            (manager, arg, valueString) =>
                            {
                                if (arg.Arg.Other is StartingItem startingItem && int.TryParse(valueString, out var value))
                                {
                                    startingItem.Stack = value * RogueObj.GetMaxStack(startingItem.Option.InfoSet, StackOption.Default);
                                    if (startingItem.Stack <= 0) { startingItem.Stack = 1; }
                                    return startingItem.Stack.ToString();
                                }
                                Debug.LogError("不正な型です。");
                                return null;
                            },
                            TMP_InputField.ContentType.IntegerNumber)
                    });
                AddMemberElements(startingItem);
                elms.Add(removeSelectOption);
            }
            Debug.Log(arg.Arg.Other);

            view.ShowTemplate(elms, manager, arg)
                ?
                .Build();
        }

        private void AddMemberElements(IReadOnlyMemberable memberable)
        {
            foreach (var memberSource in memberable.MemberSources)
            {
                var member = memberable.GetMember(memberSource);
                if (member is SingleItemMember singleItemMember)
                {
                    elms.Add(singleItemMemberSelectOption.Set(singleItemMember));
                }
                else if (member is EquipMember equipMember)
                {
                    elms.Add(
                        SelectOption.Create<MMgr, MArg>(
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
                    elms.Add(alphabetTypeMemberSelectOption.Set(alphabetTypeMember));
                }
                else if (member is StandardRaceMember standardRaceMember && memberable is Race race)
                {
                    race.Option.UpdateMemberRange(standardRaceMember, race.Option, removeSelectOption.characterCreationData);
                    var standardRaceOption = (IStandardRaceOption)race.Option;
                    elms.Add(
                        new object[]
                        {
                            "サイズ",
                            InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                (manager, arg) => standardRaceMember.Size.ToString(),
                                (manager, arg, valueString) =>
                                {
                                    if (!int.TryParse(valueString, out var value)) return valueString;

                                    standardRaceMember.Size = Mathf.Clamp(value, standardRaceOption.MinSize, standardRaceOption.MaxSize);
                                    return standardRaceMember.Size.ToString();
                                },
                                TMP_InputField.ContentType.IntegerNumber)
                        });
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

            void ISelectOption.HandleClick(IListMenuManager iManager, IListMenuArg iArg)
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
            private Race race;
            private readonly List<IRogueGender> list = new();

            private readonly ScrollViewTemplate<IRogueGender, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                race = (Race)arg.Arg.Other;
                list.Clear();
                foreach (var gender in race.Option.Genders)
                {
                    list.Add(gender);
                }

                view.ShowTemplate(list, manager, arg)
                    ?
                    .NameFrom((gender, manager, arg) =>
                    {
                        return gender.Name;
                    })

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
