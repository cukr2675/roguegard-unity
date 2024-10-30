using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using TMPro;
using ListingMF;

namespace RoguegardUnity
{
    public class CharacterCreationOptionMenu : RogueMenuScreen
    {
        private readonly List<object> elms = new();
        private readonly ICharacterCreationDatabase database;
        private readonly CharacterCreationOptionsSelectOption selectOption;
        private readonly RemoveSelectOption removeSelectOption;

        // selectOption �Ɠ����ɏo�����郁���o�[�͕ʃC���X�^���X�ɂ���B
        private readonly CharacterCreationOptionsSelectOption singleItemMemberSelectOption;
        private readonly CharacterCreationOptionsSelectOption alphabetTypeMemberSelectOption;

        private readonly VariableWidgetsViewTemplate<RogueMenuManager, ReadOnlyMenuArg> view;

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

        public void Set(CharacterCreationDataBuilder builder)
        {
            removeSelectOption.builder = builder;
        }

        public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
        {
            if (arg.Arg.Other is RaceBuilder raceBuilder)
            {
                elms.Clear();
                elms.Add(
                    new object[]
                    {
                        "�E�Ɓ^���",
                        InputFieldViewWidget.CreateOption<RogueMenuManager, ReadOnlyMenuArg>(
                            (manager, arg) => removeSelectOption.builder.ShortName,
                            (manager, arg, value) => removeSelectOption.builder.ShortName = value)
                    });
                elms.Add(selectOption.Set(raceBuilder));
                elms.Add(
                    new object[]
                    {
                        ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(
                            (manager, arg) =>
                            {
                                if (arg.Arg.Other is RaceBuilder raceBuilder)
                                {
                                    return $"���ʁF{raceBuilder.Gender.Name}";
                                }
                                Debug.LogError("�s���Ȍ^�ł��B");
                                return "���ʁF";
                            },
                            (manager, arg) =>
                            {
                                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                                if (arg.Arg.Other is RaceBuilder raceBuilder)
                                {
                                    var nextMenu = new SelectGenderMenu() { database = database };
                                    manager.PushMenuScreen(nextMenu, arg.Self, other: arg.Arg.Other);
                                    return;
                                }
                                Debug.LogError("�s���Ȍ^�ł��B");
                            })
                    });
                elms.Add(
                    ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(
                        $"<#{ColorUtility.ToHtmlStringRGBA(raceBuilder.BodyColor)}>�J���[",
                        ColorPicker()));
                AddMemberElements(raceBuilder);
            }
            else if (arg.Arg.Other is AppearanceBuilder appearanceBuilder)
            {
                elms.Clear();
                elms.Add(selectOption.Set(appearanceBuilder));
                elms.Add(
                    ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(
                        $"<#{ColorUtility.ToHtmlStringRGBA(appearanceBuilder.Color)}>�J���[",
                        ColorPicker()));
                AddMemberElements(appearanceBuilder);
                elms.Add(removeSelectOption);
            }
            else if (arg.Arg.Other is IntrinsicBuilder intrinsicBuilder)
            {
                elms.Clear();
                elms.Add(selectOption.Set(intrinsicBuilder));
                elms.Add(
                    new object[]
                    {
                        "���O",
                        InputFieldViewWidget.CreateOption<RogueMenuManager, ReadOnlyMenuArg>(
                            (manager, arg) =>
                            {
                                if (arg.Arg.Other is IntrinsicBuilder intrinsicBuilder)
                                {
                                    return intrinsicBuilder.OptionName;
                                }
                                Debug.LogError("�s���Ȍ^�ł��B");
                                return "???";
                            },
                            (manager, arg, value) =>
                            {
                                // �󗓂̏ꍇ�͏㏑�����Ȃ��悤 null �ɂ���
                                if (string.IsNullOrWhiteSpace(value)) { value = null; }

                                if (arg.Arg.Other is IntrinsicBuilder intrinsicBuilder)
                                {
                                    return intrinsicBuilder.OptionName = value;
                                }
                                Debug.LogError("�s���Ȍ^�ł��B");
                                return null;
                            })
                    });
                AddMemberElements(intrinsicBuilder);
                elms.Add(removeSelectOption);
            }
            else if (arg.Arg.Other is StartingItemBuilder startingItemBuilder)
            {
                elms.Clear();
                elms.Add(selectOption.Set(startingItemBuilder));
                elms.Add(
                    new object[]
                    {
                        "��",
                        InputFieldViewWidget.CreateOption<RogueMenuManager, ReadOnlyMenuArg>(
                            (manager, arg) =>
                            {
                                if (arg.Arg.Other is StartingItemBuilder startingItemBuilder)
                                {
                                    var value = startingItemBuilder.Stack
                                        / RogueObj.GetMaxStack(startingItemBuilder.Option.InfoSet, StackOption.Default);
                                    return value.ToString();
                                }
                                Debug.LogError("�s���Ȍ^�ł��B");
                                return "0";
                            },
                            (manager, arg, valueString) =>
                            {
                                if (arg.Arg.Other is StartingItemBuilder startingItemBuilder && int.TryParse(valueString, out var value))
                                {
                                    startingItemBuilder.Stack = value * RogueObj.GetMaxStack(startingItemBuilder.Option.InfoSet, StackOption.Default);
                                    if (startingItemBuilder.Stack <= 0) { startingItemBuilder.Stack = 1; }
                                    return startingItemBuilder.Stack.ToString();
                                }
                                Debug.LogError("�s���Ȍ^�ł��B");
                                return null;
                            },
                            TMP_InputField.ContentType.IntegerNumber)
                    });
                AddMemberElements(startingItemBuilder);
                elms.Add(removeSelectOption);
            }

            view.ShowTemplate(elms, manager, arg)
                ?
                .Build();
        }

        private void AddMemberElements(IMemberable memberable)
        {
            for (int i = 0; i < memberable.MemberSources.Count; i++)
            {
                var member = memberable.GetMember(memberable.MemberSources[i]);
                if (member is SingleItemMember singleItemMember)
                {
                    elms.Add(singleItemMemberSelectOption.Set(singleItemMember));
                }
                else if (member is EquipMember equipMember)
                {
                    elms.Add(
                        ListMenuSelectOption.Create<RogueMenuManager, ReadOnlyMenuArg>(
                            (manager, arg) =>
                            {
                                return $"<#808080>��������F{equipMember.IsEquipped}";
                            },
                            (manager, arg) =>
                            {
                                equipMember.IsEquipped = !equipMember.IsEquipped;
                            }));
                }
                else if (member is AlphabetTypeMember alphabetTypeMember && memberable is AppearanceBuilder appearanceBuilder)
                {
                    appearanceBuilder.Option.UpdateMemberRange(alphabetTypeMember, appearanceBuilder, removeSelectOption.builder);
                    elms.Add(alphabetTypeMemberSelectOption.Set(alphabetTypeMember));
                }
                else if (member is StandardRaceMember standardRaceMember && memberable is RaceBuilder raceBuilder)
                {
                    raceBuilder.Option.UpdateMemberRange(standardRaceMember, raceBuilder.Option, removeSelectOption.builder);
                    var standardRaceOption = (IStandardRaceOption)raceBuilder.Option;
                    elms.Add(
                        new object[]
                        {
                            "�T�C�Y",
                            InputFieldViewWidget.CreateOption<RogueMenuManager, ReadOnlyMenuArg>(
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

        private ColorPickerMenuScreen<RogueMenuManager, ReadOnlyMenuArg> ColorPicker()
        {
            return new ColorPickerMenuScreen<RogueMenuManager, ReadOnlyMenuArg>(
                (manager, arg) =>
                {
                    if (arg.Arg.Other is RaceBuilder raceBuilder)
                    {
                        return raceBuilder.BodyColor;
                    }
                    else if (arg.Arg.Other is AppearanceBuilder appearanceBuilder)
                    {
                        return appearanceBuilder.Color;
                    }
                    Debug.LogError("�s���Ȍ^�ł��B");
                    return Color.white;
                },
                (manager, arg, color) =>
                {
                    if (arg.Arg.Other is RaceBuilder raceBuilder)
                    {
                        raceBuilder.BodyColor = color;
                        return;
                    }
                    else if (arg.Arg.Other is AppearanceBuilder appearanceBuilder)
                    {
                        appearanceBuilder.Color = color;
                        return;
                    }
                    Debug.LogError("�s���Ȍ^�ł��B");
                });
        }

        private class RemoveSelectOption : IListMenuSelectOption
        {
            public CharacterCreationDataBuilder builder;

            string IListMenuSelectOption.GetName(IListMenuManager manager, IListMenuArg arg) => "<#f00>�폜";

            void IListMenuSelectOption.HandleClick(IListMenuManager iManager, IListMenuArg iArg)
            {
                var manager = (RogueMenuManager)iManager;
                var arg = (ReadOnlyMenuArg)iArg;

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                if (arg.Arg.Other is IMemberable memberable)
                {
                    for (int i = 0; i < memberable.MemberSources.Count; i++)
                    {
                        var member = memberable.GetMember(memberable.MemberSources[i]);
                        if (member is SingleItemMember singleItemMember)
                        {
                            // �����ڑ������폜�����Ƃ��A���̑����i���l������
                            CharacterCreationAddMenu.ReceiveStartingItemOptionObj(singleItemMember.ItemOption, arg.Self);
                        }
                    }
                }

                if (arg.Arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    builder.Appearances.Remove(appearanceBuilder);
                }
                else if (arg.Arg.Other is IntrinsicBuilder intrinsicBuilder)
                {
                    builder.Intrinsics.Remove(intrinsicBuilder);
                }
                else if (arg.Arg.Other is StartingItemBuilder startingItemBuilder)
                {
                    // �����A�C�e�����폜�����Ƃ��A���̃A�C�e�����l������
                    CharacterCreationAddMenu.ReceiveStartingItemOptionObj(startingItemBuilder.Option, arg.Self);
                    builder.StartingItemTable.Remove(startingItemBuilder, true);
                }
                manager.HandleClickBack();
            }
        }

        private class SelectGenderMenu : RogueMenuScreen
        {
            public ICharacterCreationDatabase database;
            private RaceBuilder builder;
            private readonly List<IRogueGender> list = new();

            private readonly ScrollViewTemplate<IRogueGender, RogueMenuManager, ReadOnlyMenuArg> view = new()
            {
            };

            public override void OpenScreen(in RogueMenuManager manager, in ReadOnlyMenuArg arg)
            {
                builder = (RaceBuilder)arg.Arg.Other;
                var genders = builder.Option.Genders;
                list.Clear();
                for (int i = 0; i < genders.Count; i++)
                {
                    list.Add(genders[i]);
                }

                view.ShowTemplate(list, manager, arg)
                    ?
                    .ElementNameFrom((gender, manager, arg) =>
                    {
                        return gender.Name;
                    })

                    .OnClickElement((gender, manager, arg) =>
                    {
                        builder.Gender = gender;
                        manager.HandleClickBack();
                    })

                    .Build();
            }
        }
    }
}
