using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Lysionium;
using Roguegard.Rgpacks;
using Roguegard.CharacterCreation;

namespace Roguegard.Device
{
    public class PropertiedCmnMenu : RogueMenuScreen
    {
        private readonly List<object> elms = new();

        private CharacterCreationOptionMenu characterCreationOptionMenu;
        private StartingItemTableMenu startingItemTableMenu;

        private readonly VariableWidgetsViewTemplate<MMgr, MArg> view = new()
        {
        };

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            var cmnData = (PropertiedCmnData)arg.Arg.Other;

            elms.Clear();
            if (!string.IsNullOrWhiteSpace(cmnData.Cmn))
            {
                // コモンイベントのプロパティ一覧を取得するためにビルドする
                var atelier = ScenarioMonolithInfo.GetAtelierByCharacter(arg.Self);
                var rgpackDirectory = Rgpacker.Pack(atelier);
                var rgpack = new Rgpack("Playtest", rgpackDirectory, Rgpacker.DefaultEvaluator);
                RgpackReference.LoadRgpack(rgpack);

                var properties = cmnData.GetProperties(rgpack.Id);
                if (properties != null)
                {
                    foreach (var pair in properties)
                    {
                        if (pair.Value is NumberCmnProperty numberCmnProperty)
                        {
                            elms.Add(
                                new object[]
                                {
                                pair.Key,
                                InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                    (manager, arg) => numberCmnProperty.Value.ToString(),
                                    (manager, arg, value) => (numberCmnProperty.Value = float.Parse(value)).ToString(),
                                    TMP_InputField.ContentType.DecimalNumber)
                                });
                        }
                        else if (pair.Value is StartingItemCmnProperty startingItemCmnProperty)
                        {
                            characterCreationOptionMenu ??= new CharacterCreationOptionMenu(RoguegardSettings.CharacterCreationDatabase);
                            startingItemCmnProperty.Value ??= new StartingItem() { Option = RoguegardSettings.CharacterCreationDatabase.StartingItemOptions[0] };
                            elms.Add(
                                SelectOption.Create<MMgr, MArg>(
                                    pair.Key,
                                    (manager, arg) => manager.PushMenuScreen(characterCreationOptionMenu, other: startingItemCmnProperty.Value)));
                        }
                        else if (pair.Value is StartingItemTableCmnProperty startingItemTableCmnProperty)
                        {
                            startingItemTableMenu ??= new StartingItemTableMenu();
                            elms.Add(
                                SelectOption.Create<MMgr, MArg>(
                                    pair.Key,
                                    (manager, arg) => manager.PushMenuScreen(startingItemTableMenu, other: startingItemTableCmnProperty)));
                        }
                    }
                }
            }

            view.ShowTemplate(elms, manager, arg)
                ?
                .HeadStack("アセットID", InputFieldViewWidget.CreateOption<MMgr, MArg>(
                    (manager, arg) => ((PropertiedCmnData)arg.Arg.Other).Cmn,
                    (manager, arg, value) => ((PropertiedCmnData)arg.Arg.Other).Cmn = value))

                .Build();
        }

        private class StartingItemTableMenu : RogueMenuScreen
        {
            private readonly List<object> elms = new();
            private readonly CharacterCreationData characterCreationData = new();
            private readonly CharacterCreationAddMenu characterCreationAddMenu = new(RoguegardSettings.CharacterCreationDatabase);
            private readonly CharacterCreationOptionMenu characterCreationOptionMenu = new(RoguegardSettings.CharacterCreationDatabase);

            private readonly ScrollViewTemplate<object, MMgr, MArg> view;

            public StartingItemTableMenu()
            {
                view = new()
                {
                    BackAnchorList = new List<ISelectOption>()
                    {
                        SelectOption.Create<MMgr, MArg>(":Back", (manager, arg) =>
                        {
                            var startingItemTableCmnProperty = (StartingItemTableCmnProperty)arg.Arg.Other;
                            startingItemTableCmnProperty.Value.Clear();
                            startingItemTableCmnProperty.Value.AddClones(characterCreationData.StartingItemTable);
                            manager.PopMenuScreen();
                        }, "Cancel")
                    }
                };
            }

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var startingItemTableCmnProperty = (StartingItemTableCmnProperty)arg.Arg.Other;
                var table = startingItemTableCmnProperty.Value;
                characterCreationData.StartingItemTable.Clear();
                characterCreationData.StartingItemTable.AddClones(table);
                elms.Clear();
                for (int i = 0; i < table.Count; i++)
                {
                    elms.Add(table[i][0]);
                }

                view.ShowTemplate(elms, manager, arg)
                    ?
                    .Tail(SelectOption.Create<MMgr, MArg>(
                        "+ アイテムを追加",
                        (manager, arg) => manager.PushMenuScreen(characterCreationAddMenu, other: typeof(StartingItem))))

                    .NameFrom((element, manager, arg) =>
                    {
                        if (element is StartingItem startingItem) return startingItem.Name;
                        else throw new System.InvalidOperationException();
                    })

                    .OnClick((element, manager, arg) =>
                    {
                        if (element is StartingItem startingItem) { manager.PushMenuScreen(characterCreationOptionMenu, other: startingItem); }
                        else throw new System.InvalidOperationException();
                    })

                    .Build();
            }
        }
    }
}
