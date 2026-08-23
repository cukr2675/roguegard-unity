using Lysionium;
using Roguegard.CharacterCreation;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Roguegard.Device
{
    public class CharacterCreationOptionScreen : RogueListuiScreen
    {
        private readonly List<object> list = new();
        private readonly ICharacterCreationDatabase database;
        private readonly CharacterCreationOptionsSelectOption selectOption;
        private readonly RemoveSelectOption removeSelectOption;

        // selectOption と同時に出現するメンバーは別インスタンスにする。
        private readonly CharacterCreationOptionsSelectOption singleItemMemberSelectOption;
        private readonly CharacterCreationOptionsSelectOption alphabetTypeMemberSelectOption;

        private readonly VariableWidgetsMenuViewData<MMgr> raceView = new();
        private readonly VariableWidgetsMenuViewData<MMgr> appearanceView = new();
        private readonly VariableWidgetsMenuViewData<MMgr> intrinsicView = new();
        private readonly VariableWidgetsMenuViewData<MMgr> startingItemView = new();

        public CharacterCreationOptionScreen(ICharacterCreationDatabase database)
        {
            this.database = database;
            selectOption = new CharacterCreationOptionsSelectOption(database);
            removeSelectOption = new RemoveSelectOption();

            singleItemMemberSelectOption = new CharacterCreationOptionsSelectOption(database);
            alphabetTypeMemberSelectOption = new CharacterCreationOptionsSelectOption(database);

            OnOpenScreen += (manager) =>
            {
                selectOption.Set(Arg.Self, Arg.Arg.Other);
                list.Clear();
                AddMemberElementsTo(list, (IReadOnlyMemberable)Arg.Arg.Other);

                if (Arg.Arg.Other is Race race)
                {
                    raceView.Show(list, manager)
                    ?
                    .HeadStack("職業／二つ名", InputFieldWidgetOption.Create<MMgr>(
                        _ => removeSelectOption.characterCreationData.ShortName,
                        value => removeSelectOption.characterCreationData.ShortName = value))

                    .Head.Option(selectOption)

                    .Head.Option(
                        (manager) =>
                        {
                            var race = (Race)Arg.Arg.Other;
                            return $"性別：{race.Gender.Name}";
                        },
                        (manager) =>
                        {
                            var race = (Race)Arg.Arg.Other;
                            var nextScreen = new GenderSelectionScreen { database = database };
                            manager.PushScreen(nextScreen, Arg.Self, other: Arg.Arg.Other);
                        })

                    .Head.Option(
                        (manager) =>
                        {
                            var race = (Race)Arg.Arg.Other;
                            return $"<#{ColorUtility.ToHtmlStringRGBA(race.BodyColor)}>カラー";
                        },
                        ColorPicker())

                    .Build();
                }
                else if (Arg.Arg.Other is Appearance appearance)
                {
                    appearanceView.Show(list, manager)
                    ?
                    .Head.Option(selectOption)

                    .Head.Option(
                        (manager) =>
                        {
                            var appearance = (Appearance)Arg.Arg.Other;
                            return $"<#{ColorUtility.ToHtmlStringRGBA(appearance.Color)}>カラー";
                        },
                        ColorPicker())

                    .Tail.Option(removeSelectOption)

                    .Build();
                }
                else if (Arg.Arg.Other is Intrinsic intrinsic)
                {
                    intrinsicView.Show(list, manager)
                    ?
                    .Head.Option(selectOption)

                    .HeadStack("名前", InputFieldWidgetOption.Create<MMgr>(
                        _ =>
                        {
                            var intrinsic = (Intrinsic)Arg.Arg.Other;
                            return intrinsic.CustomName;
                        },
                        value =>
                        {
                            // 空欄の場合は上書きしないよう null にする
                            if (string.IsNullOrWhiteSpace(value)) { value = null; }

                            var intrinsic = (Intrinsic)Arg.Arg.Other;
                            return intrinsic.CustomName = value;
                        }))

                    .Tail.Option(removeSelectOption)

                    .Build();
                }
                else if (Arg.Arg.Other is StartingItem startingItem)
                {
                    startingItemView.Show(list, manager)
                    ?
                    .Head.Option(selectOption)

                    .HeadStack("個数", InputFieldWidgetOption.Create<MMgr>(
                        _ =>
                        {
                            var startingItem = (StartingItem)Arg.Arg.Other;
                            var value = startingItem.Stack / RogueObj.GetMaxStack(startingItem.Option.InfoSet, StackOption.Default);
                            return value.ToString();
                        },
                        valueString =>
                        {
                            if (int.TryParse(valueString, out var value))
                            {
                                var startingItem = (StartingItem)Arg.Arg.Other;
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
            };
        }

        public void Set(RogueObj self, object other, CharacterCreationData characterCreationData)
        {
            removeSelectOption.self = self;
            removeSelectOption.other = other;
            removeSelectOption.characterCreationData = characterCreationData;
        }

        private void AddMemberElementsTo(List<object> list, IReadOnlyMemberable memberable)
        {
            foreach (var memberSource in memberable.MemberSources)
            {
                var member = memberable.GetMember(memberSource);
                if (member is SingleItemMember singleItemMember)
                {
                    list.Add(singleItemMemberSelectOption.Set(Arg.Self, singleItemMember));
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
                    list.Add(alphabetTypeMemberSelectOption.Set(Arg.Self, alphabetTypeMember));
                }
                else if (member is StandardRaceMember standardRaceMember && memberable is Race race)
                {
                    race.Option.UpdateMemberRange(standardRaceMember, race.Option, removeSelectOption.characterCreationData);
                    var standardRaceOption = (IStandardRaceOption)race.Option;
                    list.Add(StackWidgetOption.Create(
                        ("1*", "サイズ"),
                        ("1*", InputFieldWidgetOption.Create<MMgr>(
                            _ => standardRaceMember.Size.ToString(),
                            valueString =>
                            {
                                if (!int.TryParse(valueString, out var value)) return valueString;

                                standardRaceMember.Size = Mathf.Clamp(value, standardRaceOption.MinSize, standardRaceOption.MaxSize);
                                return standardRaceMember.Size.ToString();
                            },
                            TMP_InputField.ContentType.IntegerNumber))));
                }
            }
        }

        private ColorPickerScreen<MMgr> ColorPicker()
        {
            return new ColorPickerScreen<MMgr>(
                (manager) =>
                {
                    if (Arg.Arg.Other is Race race)
                    {
                        return race.BodyColor;
                    }
                    else if (Arg.Arg.Other is Appearance appearance)
                    {
                        return appearance.Color;
                    }
                    Debug.LogError("不正な型です。");
                    return Color.white;
                },
                (color, manager) =>
                {
                    if (Arg.Arg.Other is Race race)
                    {
                        race.BodyColor = color;
                        return;
                    }
                    else if (Arg.Arg.Other is Appearance appearance)
                    {
                        appearance.Color = color;
                        return;
                    }
                    Debug.LogError("不正な型です。");
                });
        }

        private class RemoveSelectOption : ISelectOption<MMgr>
        {
            public RogueObj self;
            public object other;
            public CharacterCreationData characterCreationData;

            private static readonly IReadOnlyList<string> clickOnlyEventGestureNames
                = new List<string> { "Click" }.AsReadOnly();

            string ISelectOption<MMgr>.GetName(MMgr manager) => "<#f00>削除";

            string ISelectOption<MMgr>.GetStyle(MMgr manager) => null;

            IReadOnlyList<string> ISelectOption<MMgr>.GetCandidateEventGestureNames(MMgr manager)
                => clickOnlyEventGestureNames;

            void ISelectOption<MMgr>.EventGestureConfirmed(MMgr manager, string eventGestureName)
            {
                if (other is IReadOnlyMemberable memberable)
                {
                    foreach (var memberSource in memberable.MemberSources)
                    {
                        var member = memberable.GetMember(memberSource);
                        if (member is SingleItemMember singleItemMember)
                        {
                            // 見た目装備を削除したとき、その装備品を獲得する
                            CharacterCreationAddScreen.ReceiveStartingItemOptionObj(singleItemMember.ItemOption, self);
                        }
                    }
                }

                if (other is Appearance appearance)
                {
                    characterCreationData.Appearances.Remove(appearance);
                }
                else if (other is Intrinsic intrinsic)
                {
                    characterCreationData.Intrinsics.Remove(intrinsic);
                }
                else if (other is StartingItem startingItem)
                {
                    // 初期アイテムを削除したとき、そのアイテムを獲得する
                    CharacterCreationAddScreen.ReceiveStartingItemOptionObj(startingItem.Option, self);
                    characterCreationData.StartingItemTable.Remove(startingItem, true);
                }
                manager.PopScreen();
            }
        }

        private class GenderSelectionScreen : RogueListuiScreen
        {
            public ICharacterCreationDatabase database;

            private readonly ScrollMenuViewData<IRogueGender, MMgr> view = new()
            {
            };

            public GenderSelectionScreen()
            {
                Race race;

                OnOpenScreen += (manager) =>
                {
                    race = (Race)Arg.Arg.Other;

                    view.Show(race.Option.Genders, manager)
                    ?
                    .NameFrom(gender => gender.Name)

                    .OnClick((gender, manager) =>
                    {
                        race.Gender = gender;
                        manager.PopScreen();
                    })

                    .Build();
                };
            }
        }
    }
}
