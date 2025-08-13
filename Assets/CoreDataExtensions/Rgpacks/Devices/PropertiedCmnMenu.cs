using Lysionium;
using Roguegard.CharacterCreation;
using Roguegard.Rgpacks;
using System.Collections.Generic;
using TMPro;

namespace Roguegard.Device
{
    public class PropertiedCmnMenu : RogueMenuScreen
    {
        private readonly List<object> list = new();
        private readonly VariableWidgetsMenuViewData<MMgr, MArg> view = new()
        {
        };

        private CharacterCreationOptionMenu characterCreationOptionMenu;
        private StartingItemTableMenu startingItemTableMenu;

        public override void OpenScreen(in MMgr manager, in MArg arg)
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
                            list.Add(StackViewWidget.CreateOption(
                                ("1*", pair.Key),
                                ("1*", InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                    (manager, arg) => numberCmnProperty.Value.ToString(),
                                    (manager, arg, value) => (numberCmnProperty.Value = float.Parse(value)).ToString(),
                                    TMP_InputField.ContentType.DecimalNumber))));
                        }
                        else if (pair.Value is StartingItemCmnProperty startingItemCmnProperty)
                        {
                            characterCreationOptionMenu ??= new CharacterCreationOptionMenu(RoguegardSettings.CharacterCreationDatabase);
                            startingItemCmnProperty.Value ??= new StartingItem() { Option = RoguegardSettings.CharacterCreationDatabase.StartingItemOptions[0] };
                            list.Add(SelectOption.Create<MMgr, MArg>(
                                pair.Key,
                                (manager, arg) => manager.PushMenuScreen(characterCreationOptionMenu, other: startingItemCmnProperty.Value)));
                        }
                        else if (pair.Value is StartingItemTableCmnProperty startingItemTableCmnProperty)
                        {
                            startingItemTableMenu ??= new StartingItemTableMenu();
                            list.Add(SelectOption.Create<MMgr, MArg>(
                                pair.Key,
                                (manager, arg) => manager.PushMenuScreen(startingItemTableMenu, other: startingItemTableCmnProperty)));
                        }
                    }
                }
            }

            view.Show(list, manager, arg)
                ?
                .HeadStack("アセットID", InputFieldViewWidget.CreateOption<MMgr, MArg>(
                    (manager, arg) => ((PropertiedCmnData)arg.Arg.Other).Cmn,
                    (manager, arg, value) => ((PropertiedCmnData)arg.Arg.Other).Cmn = value))

                .Build();
        }

        private class StartingItemTableMenu : RogueMenuScreen
        {
            private readonly CharacterCreationData characterCreationData = new();
            private readonly CharacterCreationOptionMenu characterCreationOptionMenu = new(RoguegardSettings.CharacterCreationDatabase);
            private readonly CharacterCreationAddMenu characterCreationAddMenu = new(RoguegardSettings.CharacterCreationDatabase);

            private readonly List<StartingItem> startingItems = new();
            private readonly ScrollMenuViewData<StartingItem, MMgr, MArg> view;

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

                    .OnClick((startingItem, manager, arg) => manager.PushMenuScreen(characterCreationOptionMenu, other: startingItem))

                    .TailOption("+ アイテムを追加", (manager, arg) => manager.PushMenuScreen(characterCreationAddMenu, other: typeof(StartingItem)))

                    .Build();
            }
        }
    }
}
