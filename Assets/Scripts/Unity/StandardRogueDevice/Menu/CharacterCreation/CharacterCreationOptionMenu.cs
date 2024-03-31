using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using TMPro;

namespace RoguegardUnity
{
    public class CharacterCreationOptionMenu : BaseScrollModelsMenu<object>
    {
        protected override IKeyword ViewKeyword => DeviceKw.MenuOptions;

        private readonly List<object> models = new();
        private readonly ICharacterCreationDatabase database;
        private readonly CharacterCreationSelectOptionChoice selectOptionChoice;
        private readonly RemoveChoice removeChoice;

        // selectOptionChoice と同時に出現するメンバーは別インスタンスにする。
        private readonly CharacterCreationSelectOptionChoice singleItemMemberOptionChoice;
        private readonly CharacterCreationSelectOptionChoice alphabetTypeMemberOptionChoice;

        public CharacterCreationOptionMenu(ICharacterCreationDatabase database)
        {
            this.database = database;
            selectOptionChoice = new CharacterCreationSelectOptionChoice(database);
            removeChoice = new RemoveChoice();

            singleItemMemberOptionChoice = new CharacterCreationSelectOptionChoice(database);
            alphabetTypeMemberOptionChoice = new CharacterCreationSelectOptionChoice(database);
        }

        public void Set(CharacterCreationDataBuilder builder)
        {
            removeChoice.builder = builder;
        }

        protected override Spanning<object> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (arg.Other is RaceBuilder raceBuilder)
            {
                models.Clear();
                models.Add(new ShortNameOption(removeChoice.builder));
                models.Add(selectOptionChoice.Set(raceBuilder));
                models.Add(new GenderOption(database));
                models.Add(new ColorPicker());
                AddMemberModels(raceBuilder);
            }
            else if (arg.Other is AppearanceBuilder appearanceBuilder)
            {
                models.Clear();
                models.Add(selectOptionChoice.Set(appearanceBuilder));
                models.Add(new ColorPicker());
                AddMemberModels(appearanceBuilder);
                models.Add(removeChoice);
            }
            else if (arg.Other is IntrinsicBuilder intrinsicBuilder)
            {
                models.Clear();
                models.Add(selectOptionChoice.Set(intrinsicBuilder));
                models.Add(new NameOption());
                AddMemberModels(intrinsicBuilder);
                models.Add(removeChoice);
            }
            else if (arg.Other is StartingItemBuilder startingItemBuilder)
            {
                models.Clear();
                models.Add(selectOptionChoice.Set(startingItemBuilder));
                models.Add(new StackOption());
                AddMemberModels(startingItemBuilder);
                models.Add(removeChoice);
            }
            return models;
        }

        private void AddMemberModels(IMemberable memberable)
        {
            for (int i = 0; i < memberable.MemberableOption.MemberSources.Count; i++)
            {
                var member = memberable.GetMember(memberable.MemberableOption.MemberSources[i]);
                if (member is SingleItemMember singleItemMember)
                {
                    models.Add(singleItemMemberOptionChoice.Set(singleItemMember));
                }
                else if (member is EquipMember equipMember)
                {
                    models.Add(new IsEquippedChoice(equipMember));
                }
                else if (member is AlphabetTypeMember alphabetTypeMember && memberable is AppearanceBuilder appearanceBuilder)
                {
                    appearanceBuilder.Option.UpdateMemberRange(alphabetTypeMember, appearanceBuilder, removeChoice.builder);
                    models.Add(alphabetTypeMemberOptionChoice.Set(alphabetTypeMember));
                }
                else if (member is StandardRaceMember standardRaceMember && memberable is RaceBuilder raceBuilder)
                {
                    raceBuilder.Option.UpdateMemberRange(standardRaceMember, raceBuilder.Option, removeChoice.builder);
                    models.Add(new CommonSlider(standardRaceMember, (IStandardRaceOption)raceBuilder.Option));
                }
            }
        }

        protected override string GetItemName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            return ChoicesModelsMenuItemController.Instance.GetName(model, root, self, user, arg);
        }

        protected override void ItemActivate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            ChoicesModelsMenuItemController.Instance.Activate(model, root, self, user, arg);
        }

        private class ShortNameOption : IModelsMenuOptionText
        {
            private readonly CharacterCreationDataBuilder builder;
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;
            public ShortNameOption(CharacterCreationDataBuilder builder) => this.builder = builder;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg) => "職業／二つ名";
            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg) => builder.ShortName;
            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value) => builder.ShortName = value;
        }

        private class NameOption : IModelsMenuOptionText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg) => "名前";

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is IntrinsicBuilder intrinsicBuilder)
                {
                    return intrinsicBuilder.OptionName;
                }
                Debug.LogError("不正な型です。");
                return "???";
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                // 空欄の場合は上書きしないよう null にする
                if (string.IsNullOrWhiteSpace(value)) { value = null; }

                if (arg.Other is IntrinsicBuilder intrinsicBuilder)
                {
                    intrinsicBuilder.OptionName = value;
                }
                Debug.LogError("不正な型です。");
            }
        }

        private class RemoveChoice : BaseModelsMenuChoice
        {
            public override string Name => "<#f00>削除";

            public CharacterCreationDataBuilder builder;

            public override void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                if (arg.Other is IMemberable memberable)
                {
                    for (int i = 0; i < memberable.MemberableOption.MemberSources.Count; i++)
                    {
                        var member = memberable.GetMember(memberable.MemberableOption.MemberSources[i]);
                        if (member is SingleItemMember singleItemMember)
                        {
                            // 見た目装備を削除したとき、その装備品を獲得する
                            CharacterCreationAddMenu.ReceiveStartingItemOptionObj(singleItemMember.ItemOption, self);
                        }
                    }
                }

                if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    builder.Appearances.Remove(appearanceBuilder);
                }
                else if (arg.Other is IntrinsicBuilder intrinsicBuilder)
                {
                    builder.Intrinsics.Remove(intrinsicBuilder);
                }
                else if (arg.Other is StartingItemBuilder startingItemBuilder)
                {
                    // 初期アイテムを削除したとき、そのアイテムを獲得する
                    CharacterCreationAddMenu.ReceiveStartingItemOptionObj(startingItemBuilder.Option, self);
                    builder.StartingItemTable.Remove(startingItemBuilder, true);
                }
                root.Back();
            }
        }

        private class ColorPicker : IModelsMenuOptionColor
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    return $"<#{ColorUtility.ToHtmlStringRGBA(raceBuilder.BodyColor)}>カラー";
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    return $"<#{ColorUtility.ToHtmlStringRGBA(appearanceBuilder.Color)}>カラー";
                }
                Debug.LogError("不正な型です。");
                return "???";
            }

            public Color GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    return raceBuilder.BodyColor;
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    return appearanceBuilder.Color;
                }
                Debug.LogError("不正な型です。");
                return Color.white;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, Color value)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    raceBuilder.BodyColor = value;
                    return;
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    appearanceBuilder.Color = value;
                    return;
                }
                Debug.LogError("不正な型です。");
            }
        }

        private class GenderOption : IModelsMenuChoice
        {
            private readonly SelectGenderMenu nextMenu;
            public GenderOption(ICharacterCreationDatabase database) => nextMenu = new SelectGenderMenu() { database = database };

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    return $"性別：{raceBuilder.Gender.Name}";
                }
                Debug.LogError("不正な型です。");
                return "性別：";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    root.OpenMenu(nextMenu, self, null, new(other: arg.Other));
                    return;
                }
                Debug.LogError("不正な型です。");
            }
        }

        private class SelectGenderMenu : BaseScrollModelsMenu<IRogueGender>
        {
            public ICharacterCreationDatabase database;
            private RaceBuilder builder;

            protected override Spanning<IRogueGender> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                builder = (RaceBuilder)arg.Other;
                return builder.Option.Genders;
            }

            protected override string GetItemName(IRogueGender gender, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return gender.Name;
            }

            protected override void ItemActivate(IRogueGender gender, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                builder.Gender = gender;
                root.Back();
            }
        }

        private class StackOption : IModelsMenuOptionSlider
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg) => "個数";

            public float GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is StartingItemBuilder startingItemBuilder)
                {
                    return (float)startingItemBuilder.Stack / RogueObj.GetMaxStack(startingItemBuilder.Option.InfoSet, Roguegard.StackOption.Default);
                }
                Debug.LogError("不正な型です。");
                return 0;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
            {
                if (arg.Other is StartingItemBuilder startingItemBuilder)
                {
                    startingItemBuilder.Stack = (int)(value * RogueObj.GetMaxStack(startingItemBuilder.Option.InfoSet, Roguegard.StackOption.Default));
                    if (startingItemBuilder.Stack <= 0) { startingItemBuilder.Stack = 1; }
                    return;
                }
                Debug.LogError("不正な型です。");
            }
        }

        private class IsEquippedChoice : IModelsMenuChoice
        {
            private readonly EquipMember equipMember;
            public IsEquippedChoice(EquipMember equipMember) => this.equipMember = equipMember;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return $"<#808080>装備する：{equipMember.IsEquipped}";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                //equipMember.IsEquipped = !equipMember.IsEquipped;
            }
        }

        private class CommonSlider : IModelsMenuOptionSlider
        {
            private readonly StandardRaceMember standardRaceMember;
            private readonly IStandardRaceOption standardRaceOption;

            public CommonSlider(StandardRaceMember standardRaceMember, IStandardRaceOption standardRaceOption)
            {
                this.standardRaceMember = standardRaceMember;
                this.standardRaceOption = standardRaceOption;
            }

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg) => "サイズ";

            public float GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return Mathf.InverseLerp(standardRaceOption.MinSize, standardRaceOption.MaxSize, standardRaceMember.Size);
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
            {
                standardRaceMember.Size = standardRaceOption.MinSize + Mathf.RoundToInt(value * (standardRaceOption.MaxSize - standardRaceOption.MinSize));
            }
        }
    }
}
