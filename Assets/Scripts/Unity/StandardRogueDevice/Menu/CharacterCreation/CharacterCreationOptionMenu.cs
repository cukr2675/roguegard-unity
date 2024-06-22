using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using TMPro;

namespace RoguegardUnity
{
    public class CharacterCreationOptionMenu : BaseScrollListMenu<object>
    {
        protected override IKeyword ViewKeyword => DeviceKw.MenuOptions;

        private readonly List<object> elms = new();
        private readonly ICharacterCreationDatabase database;
        private readonly CharacterCreationOptionsSelectOption selectOption;
        private readonly RemoveSelectOption removeSelectOption;

        // selectOption �Ɠ����ɏo�����郁���o�[�͕ʃC���X�^���X�ɂ���B
        private readonly CharacterCreationOptionsSelectOption singleItemMemberSelectOption;
        private readonly CharacterCreationOptionsSelectOption alphabetTypeMemberSelectOption;

        public CharacterCreationOptionMenu(ICharacterCreationDatabase database)
        {
            this.database = database;
            selectOption = new CharacterCreationOptionsSelectOption(database);
            removeSelectOption = new RemoveSelectOption();

            singleItemMemberSelectOption = new CharacterCreationOptionsSelectOption(database);
            alphabetTypeMemberSelectOption = new CharacterCreationOptionsSelectOption(database);
        }

        public void Set(CharacterCreationDataBuilder builder)
        {
            removeSelectOption.builder = builder;
        }

        protected override Spanning<object> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (arg.Other is RaceBuilder raceBuilder)
            {
                elms.Clear();
                elms.Add(new ShortNameOption(removeSelectOption.builder));
                elms.Add(selectOption.Set(raceBuilder));
                elms.Add(new GenderOption(database));
                elms.Add(new ColorPicker());
                AddMemberElements(raceBuilder);
            }
            else if (arg.Other is AppearanceBuilder appearanceBuilder)
            {
                elms.Clear();
                elms.Add(selectOption.Set(appearanceBuilder));
                elms.Add(new ColorPicker());
                AddMemberElements(appearanceBuilder);
                elms.Add(removeSelectOption);
            }
            else if (arg.Other is IntrinsicBuilder intrinsicBuilder)
            {
                elms.Clear();
                elms.Add(selectOption.Set(intrinsicBuilder));
                elms.Add(new NameOption());
                AddMemberElements(intrinsicBuilder);
                elms.Add(removeSelectOption);
            }
            else if (arg.Other is StartingItemBuilder startingItemBuilder)
            {
                elms.Clear();
                elms.Add(selectOption.Set(startingItemBuilder));
                elms.Add(new StackOption());
                AddMemberElements(startingItemBuilder);
                elms.Add(removeSelectOption);
            }
            return elms;
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
                    elms.Add(new IsEquippedSelectOption(equipMember));
                }
                else if (member is AlphabetTypeMember alphabetTypeMember && memberable is AppearanceBuilder appearanceBuilder)
                {
                    appearanceBuilder.Option.UpdateMemberRange(alphabetTypeMember, appearanceBuilder, removeSelectOption.builder);
                    elms.Add(alphabetTypeMemberSelectOption.Set(alphabetTypeMember));
                }
                else if (member is StandardRaceMember standardRaceMember && memberable is RaceBuilder raceBuilder)
                {
                    raceBuilder.Option.UpdateMemberRange(standardRaceMember, raceBuilder.Option, removeSelectOption.builder);
                    elms.Add(new CommonSlider(standardRaceMember, (IStandardRaceOption)raceBuilder.Option));
                }
            }
        }

        protected override string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            return SelectOptionPresenter.Instance.GetItemName(element, manager, self, user, arg);
        }

        protected override void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            SelectOptionPresenter.Instance.ActivateItem(element, manager, self, user, arg);
        }

        private class ShortNameOption : IOptionsMenuText
        {
            private readonly CharacterCreationDataBuilder builder;
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;
            public ShortNameOption(CharacterCreationDataBuilder builder) => this.builder = builder;

            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg) => "�E�Ɓ^���";
            public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg) => builder.ShortName;
            public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value) => builder.ShortName = value;
        }

        private class NameOption : IOptionsMenuText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg) => "���O";

            public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is IntrinsicBuilder intrinsicBuilder)
                {
                    return intrinsicBuilder.OptionName;
                }
                Debug.LogError("�s���Ȍ^�ł��B");
                return "???";
            }

            public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                // �󗓂̏ꍇ�͏㏑�����Ȃ��悤 null �ɂ���
                if (string.IsNullOrWhiteSpace(value)) { value = null; }

                if (arg.Other is IntrinsicBuilder intrinsicBuilder)
                {
                    intrinsicBuilder.OptionName = value;
                }
                Debug.LogError("�s���Ȍ^�ł��B");
            }
        }

        private class RemoveSelectOption : BaseListMenuSelectOption
        {
            public override string Name => "<#f00>�폜";

            public CharacterCreationDataBuilder builder;

            public override void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                if (arg.Other is IMemberable memberable)
                {
                    for (int i = 0; i < memberable.MemberSources.Count; i++)
                    {
                        var member = memberable.GetMember(memberable.MemberSources[i]);
                        if (member is SingleItemMember singleItemMember)
                        {
                            // �����ڑ������폜�����Ƃ��A���̑����i���l������
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
                    // �����A�C�e�����폜�����Ƃ��A���̃A�C�e�����l������
                    CharacterCreationAddMenu.ReceiveStartingItemOptionObj(startingItemBuilder.Option, self);
                    builder.StartingItemTable.Remove(startingItemBuilder, true);
                }
                manager.Back();
            }
        }

        private class ColorPicker : IOptionsMenuColor
        {
            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    return $"<#{ColorUtility.ToHtmlStringRGBA(raceBuilder.BodyColor)}>�J���[";
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    return $"<#{ColorUtility.ToHtmlStringRGBA(appearanceBuilder.Color)}>�J���[";
                }
                Debug.LogError("�s���Ȍ^�ł��B");
                return "???";
            }

            public Color GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    return raceBuilder.BodyColor;
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    return appearanceBuilder.Color;
                }
                Debug.LogError("�s���Ȍ^�ł��B");
                return Color.white;
            }

            public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, Color value)
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
                Debug.LogError("�s���Ȍ^�ł��B");
            }
        }

        private class GenderOption : IListMenuSelectOption
        {
            private readonly SelectGenderMenu nextMenu;
            public GenderOption(ICharacterCreationDatabase database) => nextMenu = new SelectGenderMenu() { database = database };

            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    return $"���ʁF{raceBuilder.Gender.Name}";
                }
                Debug.LogError("�s���Ȍ^�ł��B");
                return "���ʁF";
            }

            public void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    manager.OpenMenu(nextMenu, self, null, new(other: arg.Other));
                    return;
                }
                Debug.LogError("�s���Ȍ^�ł��B");
            }
        }

        private class SelectGenderMenu : BaseScrollListMenu<IRogueGender>
        {
            public ICharacterCreationDatabase database;
            private RaceBuilder builder;

            protected override Spanning<IRogueGender> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                builder = (RaceBuilder)arg.Other;
                return builder.Option.Genders;
            }

            protected override string GetItemName(IRogueGender gender, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return gender.Name;
            }

            protected override void ActivateItem(IRogueGender gender, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                builder.Gender = gender;
                manager.Back();
            }
        }

        private class StackOption : IOptionsMenuSlider
        {
            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg) => "��";

            public float GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is StartingItemBuilder startingItemBuilder)
                {
                    return (float)startingItemBuilder.Stack / RogueObj.GetMaxStack(startingItemBuilder.Option.InfoSet, Roguegard.StackOption.Default);
                }
                Debug.LogError("�s���Ȍ^�ł��B");
                return 0;
            }

            public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
            {
                if (arg.Other is StartingItemBuilder startingItemBuilder)
                {
                    startingItemBuilder.Stack = (int)(value * RogueObj.GetMaxStack(startingItemBuilder.Option.InfoSet, Roguegard.StackOption.Default));
                    if (startingItemBuilder.Stack <= 0) { startingItemBuilder.Stack = 1; }
                    return;
                }
                Debug.LogError("�s���Ȍ^�ł��B");
            }
        }

        private class IsEquippedSelectOption : IListMenuSelectOption
        {
            private readonly EquipMember equipMember;
            public IsEquippedSelectOption(EquipMember equipMember) => this.equipMember = equipMember;

            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return $"<#808080>��������F{equipMember.IsEquipped}";
            }

            public void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Cancel);
                //equipMember.IsEquipped = !equipMember.IsEquipped;
            }
        }

        private class CommonSlider : IOptionsMenuSlider
        {
            private readonly StandardRaceMember standardRaceMember;
            private readonly IStandardRaceOption standardRaceOption;

            public CommonSlider(StandardRaceMember standardRaceMember, IStandardRaceOption standardRaceOption)
            {
                this.standardRaceMember = standardRaceMember;
                this.standardRaceOption = standardRaceOption;
            }

            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg) => "�T�C�Y";

            public float GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return Mathf.InverseLerp(standardRaceOption.MinSize, standardRaceOption.MaxSize, standardRaceMember.Size);
            }

            public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
            {
                standardRaceMember.Size = standardRaceOption.MinSize + Mathf.RoundToInt(value * (standardRaceOption.MaxSize - standardRaceOption.MinSize));
            }
        }
    }
}
