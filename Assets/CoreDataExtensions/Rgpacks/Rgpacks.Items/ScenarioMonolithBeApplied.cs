using Lysionium;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public class ScenarioMonolithBeApplied : BaseApplyRogueMethod
    {
        [SerializeField, DescribeElement] private AssetStartingItem[] _shopItems = null;

        private Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            menu ??= new(this);
            if (ScenarioMonolithInfo.Get(self) == null) { ScenarioMonolithInfo.SetTo(self); }

            RogueDevice.Primary.AddMenu(menu, user, null, new(tool: self));
            return false;
        }

        private class Menu : RogueMenuScreen
        {
            private readonly ScenarioMonolithBeApplied parent;

            private readonly MainMenuViewData<MMgr, MArg> view = new()
            {
                PrimaryCommandSubviewName = StandardSubviewTable.ScrollName,
                BackAnchorSubviewName = StandardSubviewTable.BackAnchorName,
            };

            public Menu(ScenarioMonolithBeApplied parent)
            {
                this.parent = parent;
            }

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.Show(manager, arg)
                    ?
                    .Option("ショップ", new ShopScreen() { parent = parent })
                    .Option("メインチャート設定", new SetMainChartScreen())
                    .Option("テストプレイ", Playtest)
                    .Option("アトリエから出る", Leave)
                    .Build();
            }

            private static void Playtest(MMgr manager, MArg arg)
            {
                var monolith = arg.Arg.Tool;
                var scenarioAtelier = monolith.Location;
                var rgpack = Rgpacker.Pack(scenarioAtelier);
                RogueDevice.Add(DeviceKw.StartPlaytest, rgpack);
            }

            private static void Leave(MMgr manager, MArg arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, CategoryKw.DownStairs);
                default(IActiveRogueMethodCaller).LocateSavePoint(arg.Self, null, 0f, RogueWorldSavePointInfo.Instance, true);
                var memberInfo = LobbyMemberList.GetMemberInfo(arg.Self);
                memberInfo.SavePoint = RogueWorldSavePointInfo.Instance;
                manager.Done();
            }
        }

        private class ShopScreen : RogueMenuScreen
        {
            public ScenarioMonolithBeApplied parent;

            private readonly ScrollMenuViewData<AssetStartingItem, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.Show(parent._shopItems, manager, arg)
                    ?
                    .NameFrom(item => item.Name)

                    .OnClick((item, manager, arg) =>
                    {
                        manager.AddObject(DeviceKw.AppendText, item);
                        manager.AddObject(DeviceKw.AppendText, "を手に入れた\n");
                        item.Option.CreateObj(item, arg.Self, Vector2Int.zero, RogueRandom.Primary);
                    })

                    .Build();
            }
        }

        private class SetMainChartScreen : RogueMenuScreen
        {
            private readonly DialogViewData<MMgr, MArg> view = new()
            {
                DialogSubviewName = StandardSubviewTable.WidgetsName,
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.Show("", manager, arg)
                    ?
                    .Tail(InputFieldViewWidget.CreateOption<MMgr, MArg>(
                        (manager, arg) =>
                        {
                            var monolith = arg.Arg.Tool;
                            var info = ScenarioMonolithInfo.Get(monolith);
                            return info.MainChart;
                        },
                        (manager, arg, value) =>
                        {
                            var monolith = arg.Arg.Tool;
                            var info = ScenarioMonolithInfo.Get(monolith);
                            return info.MainChart = value;
                        }))

                    .Build();
            }
        }
    }
}
