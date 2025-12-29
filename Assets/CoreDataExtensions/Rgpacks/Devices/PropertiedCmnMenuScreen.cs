using Lysionium;
using Roguegard.CharacterCreation;
using Roguegard.Rgpacks;
using System.Collections.Generic;
using TMPro;

namespace Roguegard.Device
{
    public class PropertiedCmnMenuScreen : RogueListuiScreen
    {
        private readonly List<object> list = new();
        private readonly VariableWidgetsMenuViewData<MMgr, MArg> view = new()
        {
        };

        private CharacterCreationOptionScreen characterCreationOptionScreen;
        private StartingItemSelectionScreen startingItemSelectionScreen;

        public override void OpenScreen(MMgr manager, MArg arg)
        {
            var cmnData = (PropertiedCmnData)arg.Arg.Other;

            list.Clear();
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
                            list.Add(StackWidgetOption.Create(
                                ("1*", pair.Key),
                                ("1*", InputFieldWidgetOption.Create<MMgr, MArg>(
                                    (manager, arg) => numberCmnProperty.Value.ToString(),
                                    (manager, arg, value) => (numberCmnProperty.Value = float.Parse(value)).ToString(),
                                    TMP_InputField.ContentType.DecimalNumber))));
                        }
                        else if (pair.Value is StartingItemCmnProperty startingItemCmnProperty)
                        {
                            characterCreationOptionScreen ??= new CharacterCreationOptionScreen(RoguegardSettings.CharacterCreationDatabase);
                            startingItemCmnProperty.Value ??= new StartingItem() { Option = RoguegardSettings.CharacterCreationDatabase.StartingItemOptions[0] };
                            list.Add(SelectOption.Create<MMgr, MArg>(
                                pair.Key,
                                (manager, arg) => manager.PushScreen(characterCreationOptionScreen, other: startingItemCmnProperty.Value)));
                        }
                        else if (pair.Value is StartingItemTableCmnProperty startingItemTableCmnProperty)
                        {
                            startingItemSelectionScreen ??= new StartingItemSelectionScreen();
                            list.Add(SelectOption.Create<MMgr, MArg>(
                                pair.Key,
                                (manager, arg) => manager.PushScreen(startingItemSelectionScreen, other: startingItemTableCmnProperty)));
                        }
                    }
                }
            }

            view.Show(list, manager, arg)
                ?
                .HeadStack("アセットID", InputFieldWidgetOption.Create<MMgr, MArg>(
                    (manager, arg) => ((PropertiedCmnData)arg.Arg.Other).Cmn,
                    (manager, arg, value) => ((PropertiedCmnData)arg.Arg.Other).Cmn = value))

                .Build();
        }

        private class StartingItemSelectionScreen : RogueListuiScreen
        {
            private readonly CharacterCreationData characterCreationData = new();
            private readonly CharacterCreationOptionScreen characterCreationOptionScreen = new(RoguegardSettings.CharacterCreationDatabase);
            private readonly CharacterCreationAddScreen characterCreationAddScreen = new(RoguegardSettings.CharacterCreationDatabase);

            private readonly List<StartingItem> startingItems = new();
            private readonly ScrollMenuViewData<StartingItem, MMgr, MArg> view;

            public StartingItemSelectionScreen()
            {
                view = new()
                {
                    BackAnchorList = new(
                        _ => _
                        .Option(":Back", (manager, arg) =>
                        {
                            var startingItemTableCmnProperty = (StartingItemTableCmnProperty)arg.Arg.Other;
                            startingItemTableCmnProperty.Value.Clear();
                            startingItemTableCmnProperty.Value.AddClones(characterCreationData.StartingItemTable);
                            manager.PopScreen();
                        }, "Cancel"))
                };
            }

            public override void OpenScreen(MMgr manager, MArg arg)
            {
                var startingItemTableCmnProperty = (StartingItemTableCmnProperty)arg.Arg.Other;
                var startingItemTable = startingItemTableCmnProperty.Value;
                characterCreationData.StartingItemTable.Clear();
                characterCreationData.StartingItemTable.AddClones(startingItemTable);
                startingItems.Clear();
                for (int i = 0; i < startingItemTable.Count; i++)
                {
                    startingItems.Add(startingItemTable[i][0]);
                }

                view.Show(startingItems, manager, arg)
                    ?
                    .NameFrom(startingItem => startingItem.Name)

                    .OnClick((startingItem, manager, arg) => manager.PushScreen(characterCreationOptionScreen, other: startingItem))

                    .Tail.Option("+ アイテムを追加", (manager, arg) => manager.PushScreen(characterCreationAddScreen, other: typeof(StartingItem)))

                    .Build();
            }
        }
    }
}
