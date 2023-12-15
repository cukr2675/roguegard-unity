using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace RoguegardUnity
{
    public class CharacterCreationOptionMenu : IModelsMenu
    {
        private readonly List<object> models = new List<object>();
        private readonly ICharacterCreationDatabase database;
        private readonly RemoveChoice removeChoice;

        public CharacterCreationOptionMenu(CharacterCreationDataBuilder builder, ICharacterCreationDatabase database)
        {
            removeChoice = new RemoveChoice() { builder = builder };
            this.database = database;
        }

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            if (arg.Other is RaceBuilder raceBuilder)
            {
                models.Clear();
                models.Add(new SelectOptionChoice(raceBuilder, database));
                models.Add(new GenderOption(database));
                models.Add(new RSlider());
                models.Add(new GSlider());
                models.Add(new BSlider());
                models.Add(new ASlider());
                AddMemberModels(raceBuilder);
            }
            else if (arg.Other is AppearanceBuilder appearanceBuilder)
            {
                models.Clear();
                models.Add(new SelectOptionChoice(appearanceBuilder, database));
                models.Add(new RSlider());
                models.Add(new GSlider());
                models.Add(new BSlider());
                models.Add(new ASlider());
                AddMemberModels(appearanceBuilder);
                models.Add(removeChoice);
            }
            else if (arg.Other is IntrinsicBuilder intrinsicBuilder)
            {
                models.Clear();
                models.Add(new SelectOptionChoice(intrinsicBuilder, database));
                AddMemberModels(intrinsicBuilder);
                models.Add(removeChoice);
            }
            else if (arg.Other is StartingItemBuilder startingItemBuilder)
            {
                models.Clear();
                models.Add(new SelectOptionChoice(startingItemBuilder, database));
                models.Add(new StackOption());
                AddMemberModels(startingItemBuilder);
                models.Add(removeChoice);
            }

            var options = (OptionsMenuView)root.Get(DeviceKw.MenuOptions);
            options.OpenView(ChoicesModelsMenuItemController.Instance, models, root, self, null, new(other: arg.Other));
            options.ShowExitButton(ExitModelsMenuChoice.Instance);
        }

        private void AddMemberModels(IMemberable memberable)
        {
            for (int i = 0; i < memberable.MemberableOption.MemberSources.Count; i++)
            {
                var member = memberable.GetMember(memberable.MemberableOption.MemberSources[i]);
                if (member is SingleItemMember singleItemMember)
                {
                    models.Add(new SelectOptionChoice(singleItemMember, database));
                }
                else if (member is EquipMember equipMember)
                {
                    models.Add(new IsEquippedChoice(equipMember));
                }
                else if (member is AlphabetTypeMember alphabetTypeMember && memberable is AppearanceBuilder appearanceBuilder)
                {
                    appearanceBuilder.Option.UpdateMemberRange(alphabetTypeMember, appearanceBuilder, removeChoice.builder);
                    models.Add(new SelectOptionChoice(alphabetTypeMember, database));
                }
                else if (member is StandardRaceMember standardRaceMember && memberable is RaceBuilder raceBuilder)
                {
                    raceBuilder.Option.UpdateMemberRange(standardRaceMember, raceBuilder.Option, removeChoice.builder);
                    models.Add(new CommonSlider(standardRaceMember, (IStandardRaceOption)raceBuilder.Option));
                }
            }
        }

        private class SelectOptionChoice : IModelsMenuChoice
        {
            private readonly object builder;
            private readonly SelectOptionMenu nextMenu;

            public SelectOptionChoice(object builder, ICharacterCreationDatabase database)
            {
                this.builder = builder;
                nextMenu = new SelectOptionMenu() { database = database };
            }

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
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

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.OpenMenu(nextMenu, self, null, new(other: builder), arg);
            }
        }

        private class SelectOptionMenu : IModelsMenu, IModelsMenuItemController
        {
            private readonly List<object> models = new List<object>();

            public ICharacterCreationDatabase database;

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                models.Clear();
                CharacterCreationAddMenu.AddOptionsTo(models, self, arg.Other, database);
                var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(this, models, root, self, null, new(other: arg.Other));
                scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
            }

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model is IRogueDescription description)
                {
                    return description.Name;
                }
                else if (arg.Other is AlphabetTypeMember alphabetTypeMember)
                {
                    return $"タイプ{alphabetTypeMember.Types[(int)model]}";
                }
                Debug.LogError("不正な型です。");
                return null;
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                if (arg.Other is RaceBuilder raceBuilder)
                {
                    raceBuilder.Option = (IRaceOption)model;
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    appearanceBuilder.Option = (IAppearanceOption)model;
                }
                else if (arg.Other is IntrinsicBuilder intrinsicBuilder)
                {
                    intrinsicBuilder.Option = (IIntrinsicOption)model;
                }
                else if (arg.Other is StartingItemBuilder startingItemBuilder)
                {
                    CharacterCreationAddMenu.ReceiveStartingItemOptionObj(startingItemBuilder.Option, self);
                    startingItemBuilder.Option = (IStartingItemOption)model;
                    CharacterCreationAddMenu.ConsumeStartingItemOptionObj(startingItemBuilder.Option, self);
                }
                else if (arg.Other is SingleItemMember singleItemMember)
                {
                    CharacterCreationAddMenu.ReceiveStartingItemOptionObj(singleItemMember.ItemOption, self);
                    singleItemMember.ItemOption = (IStartingItemOption)model;
                    CharacterCreationAddMenu.ConsumeStartingItemOptionObj(singleItemMember.ItemOption, self);
                }
                else if (arg.Other is AlphabetTypeMember alphabetTypeMember)
                {
                    alphabetTypeMember.TypeIndex = (int)model;
                }
                root.Back();
            }
        }

        private class RemoveChoice : IModelsMenuChoice
        {
            public CharacterCreationDataBuilder builder;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "<#f00>削除";
            }

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
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

        private class RSlider : IModelsMenuOptionSlider
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "R";
            }

            public float GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    return raceBuilder.BodyColor.r / 255f;
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    return appearanceBuilder.Color.r / 255f;
                }
                Debug.LogError("不正な型です。");
                return 0;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    var color = raceBuilder.BodyColor;
                    color.r = (byte)(value * 255f);
                    raceBuilder.BodyColor = color;
                    return;
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    var color = appearanceBuilder.Color;
                    color.r = (byte)(value * 255f);
                    appearanceBuilder.Color = color;
                    return;
                }
                Debug.LogError("不正な型です。");
            }
        }

        private class GSlider : IModelsMenuOptionSlider
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "G";
            }

            public float GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    return raceBuilder.BodyColor.g / 255f;
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    return appearanceBuilder.Color.g / 255f;
                }
                Debug.LogError("不正な型です。");
                return 0;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    var color = raceBuilder.BodyColor;
                    color.g = (byte)(value * 255f);
                    raceBuilder.BodyColor = color;
                    return;
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    var color = appearanceBuilder.Color;
                    color.g = (byte)(value * 255f);
                    appearanceBuilder.Color = color;
                    return;
                }
                Debug.LogError("不正な型です。");
            }
        }

        private class BSlider : IModelsMenuOptionSlider
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "B";
            }

            public float GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    return raceBuilder.BodyColor.b / 255f;
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    return appearanceBuilder.Color.b / 255f;
                }
                Debug.LogError("不正な型です。");
                return 0;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    var color = raceBuilder.BodyColor;
                    color.b = (byte)(value * 255f);
                    raceBuilder.BodyColor = color;
                    return;
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    var color = appearanceBuilder.Color;
                    color.b = (byte)(value * 255f);
                    appearanceBuilder.Color = color;
                    return;
                }
                Debug.LogError("不正な型です。");
            }
        }

        private class ASlider : IModelsMenuOptionSlider
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "不透明度";
            }

            public float GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    return raceBuilder.BodyColor.a / 255f;
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    return appearanceBuilder.Color.a / 255f;
                }
                Debug.LogError("不正な型です。");
                return 0;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, float value)
            {
                if (arg.Other is RaceBuilder raceBuilder)
                {
                    var color = raceBuilder.BodyColor;
                    color.a = (byte)(value * 255f);
                    raceBuilder.BodyColor = color;
                    return;
                }
                else if (arg.Other is AppearanceBuilder appearanceBuilder)
                {
                    var color = appearanceBuilder.Color;
                    color.a = (byte)(value * 255f);
                    appearanceBuilder.Color = color;
                    return;
                }
                Debug.LogError("不正な型です。");
            }
        }

        private class GenderOption : IModelsMenuChoice
        {
            private readonly SelectGenderMenu nextMenu;

            public GenderOption(ICharacterCreationDatabase database)
            {
                nextMenu = new SelectGenderMenu() { database = database };
            }

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
                    root.OpenMenu(nextMenu, self, null, new(other: arg.Other), arg);
                    return;
                }
                Debug.LogError("不正な型です。");
            }
        }

        private class SelectGenderMenu : IModelsMenu, IModelsMenuItemController
        {
            private readonly List<object> models = new List<object>();

            public ICharacterCreationDatabase database;
            private RaceBuilder builder;

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                builder = (RaceBuilder)arg.Other;
                var scroll = (IScrollModelsMenuView)root.Get(DeviceKw.MenuScroll);
                scroll.OpenView(this, builder.Option.Genders, root, self, null, new(other: arg.Other));
                scroll.ShowExitButton(ExitModelsMenuChoice.Instance);
            }

            public string GetName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ((IRogueGender)model).Name;
            }

            public void Activate(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                builder.Gender = (IRogueGender)model;
                root.Back();
            }
        }

        private class StackOption : IModelsMenuOptionSlider
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "個数";
            }

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
                }
                Debug.LogError("不正な型です。");
            }
        }

        private class IsEquippedChoice : IModelsMenuChoice
        {
            private readonly EquipMember equipMember;

            public IsEquippedChoice(EquipMember equipMember)
            {
                this.equipMember = equipMember;
            }

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

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return "サイズ";
            }

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
