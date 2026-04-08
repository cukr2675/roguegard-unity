using Lysionium;
using Roguegard.CharacterCreation;
using Roguegard.Rgpacks;
using System.Collections.Generic;
using TMPro;

namespace Roguegard.Device
{
    public class PropertiedCmnMenuScreen : RogueListuiScreen
    {
        private readonly VariableWidgetsMenuViewData<MMgr> view = new()
        {
        };

        public PropertiedCmnMenuScreen()
        {
            var list = new List<object>();
            CharacterCreationOptionScreen characterCreationOptionScreen = null;
            StartingItemSelectionScreen startingItemSelectionScreen = null;

            OnOpenScreen += (manager) =>
            {
                var cmnData = (PropertiedCmnData)Arg.Arg.Other;

                list.Clear();
                if (!string.IsNullOrWhiteSpace(cmnData.Cmn))
                {
                    // コモンイベントのプロパティ一覧を取得するためにビルドする
                    var atelier = ScenarioMonolithInfo.GetAtelierByCharacter(Arg.Self);
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
                                list.Add(StackWidgetOption.Create(
                                    ("1*", pair.Key),
                                    ("1*", InputFieldWidgetOption.Create<MMgr>(
                                        _ => numberCmnProperty.Value.ToString(),
                                        value => (numberCmnProperty.Value = float.Parse(value)).ToString(),
                                        TMP_InputField.ContentType.DecimalNumber))));
                            }
                            else if (pair.Value is StartingItemCmnProperty startingItemCmnProperty)
                            {
                                characterCreationOptionScreen ??= new CharacterCreationOptionScreen(RoguegardSettings.CharacterCreationDatabase);
                                startingItemCmnProperty.Value ??= new StartingItem() { Option = RoguegardSettings.CharacterCreationDatabase.StartingItemOptions[0] };
                                list.Add(SelectOption.Create<MMgr>(
                                    pair.Key,
                                    m => m.PushScreen(characterCreationOptionScreen, other: startingItemCmnProperty.Value)));
                            }
                            else if (pair.Value is StartingItemTableCmnProperty startingItemTableCmnProperty)
                            {
                                startingItemSelectionScreen ??= new StartingItemSelectionScreen();
                                list.Add(SelectOption.Create<MMgr>(
                                    pair.Key,
                                    m => m.PushScreen(startingItemSelectionScreen, other: startingItemTableCmnProperty)));
                            }
                        }
                    }
                }

                view.Show(list, manager)
                ?
                .HeadStack("アセットID", InputFieldWidgetOption.Create<MMgr>(
                    _ => ((PropertiedCmnData)Arg.Arg.Other).Cmn,
                    value => ((PropertiedCmnData)Arg.Arg.Other).Cmn = value))

                .Build();
            };
        }

        private class StartingItemSelectionScreen : RogueListuiScreen
        {
            private readonly ScrollMenuViewData<StartingItem, MMgr> view;

            public StartingItemSelectionScreen()
            {
                var characterCreationData = new CharacterCreationData();
                var characterCreationOptionScreen = new CharacterCreationOptionScreen(RoguegardSettings.CharacterCreationDatabase);
                var characterCreationAddScreen = new CharacterCreationAddScreen(RoguegardSettings.CharacterCreationDatabase);
                view = new()
                {
                    BackAnchorList = new(
                        _ => _
                        .Option(":Back", (manager) =>
                        {
                            var startingItemTableCmnProperty = (StartingItemTableCmnProperty)Arg.Arg.Other;
                            startingItemTableCmnProperty.Value.Clear();
                            startingItemTableCmnProperty.Value.AddClones(characterCreationData.StartingItemTable);
                            manager.PopScreen();
                        }, "Cancel"))
                };

                var startingItems = new List<StartingItem>();

                OnOpenScreen += (manager) =>
                {
                    var startingItemTableCmnProperty = (StartingItemTableCmnProperty)Arg.Arg.Other;
                    var startingItemTable = startingItemTableCmnProperty.Value;
                    characterCreationData.StartingItemTable.Clear();
                    characterCreationData.StartingItemTable.AddClones(startingItemTable);
                    startingItems.Clear();
                    for (int i = 0; i < startingItemTable.Count; i++)
                    {
                        startingItems.Add(startingItemTable[i][0]);
                    }

                    view.Show(startingItems, manager)
                    ?
                    .NameFrom(startingItem => startingItem.Name)

                    .OnClick((startingItem, m) => m.PushScreen(characterCreationOptionScreen, other: startingItem))

                    .Tail.Option("+ アイテムを追加", m => m.PushScreen(characterCreationAddScreen, other: typeof(StartingItem)))

                    .Build();
                };
            }
        }
    }
}
