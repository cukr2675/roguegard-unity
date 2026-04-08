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

        private ScenarioMonolithScreen scenarioMonolithScreen;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            scenarioMonolithScreen ??= new(this);
            if (ScenarioMonolithInfo.Get(self) == null) { ScenarioMonolithInfo.SetTo(self); }

            RogueDevice.Primary.AddScreen(scenarioMonolithScreen, user, null, new(tool: self));
            return false;
        }

        private class ScenarioMonolithScreen : RogueListuiScreen
        {
            private readonly MainMenuViewData<MMgr> view = new()
            {
                PrimaryCommandSubviewSelector = m => m.Scroll,
                BackAnchorSubviewSelector = m => m.BackAnchor,
            };

            public ScenarioMonolithScreen(ScenarioMonolithBeApplied parent)
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(manager)
                    ?
                    .Option("ショップ", new ShopScreen(parent), () => Arg)
                    .Option("メインチャート設定", new SetMainChartScreen(), () => Arg)
                    .Option("テストプレイ", Playtest)
                    .Option("アトリエから出る", Leave)
                    .Build();
                };
            }

            private void Playtest(MMgr manager)
            {
                var monolith = Arg.Arg.Tool;
                var scenarioAtelier = monolith.Location;
                var rgpack = Rgpacker.Pack(scenarioAtelier);
                RogueDevice.Add(DeviceKw.StartPlaytest, rgpack);
            }

            private void Leave(MMgr manager)
            {
                manager.AddObject(DeviceKw.EnqueueSE, CategoryKw.DownStairs);
                default(IActiveRogueMethodCaller).LocateSavePoint(Arg.Self, null, 0f, RogueWorldSavePointInfo.Instance, true);
                var memberInfo = LobbyMemberList.GetMemberInfo(Arg.Self);
                memberInfo.SavePoint = RogueWorldSavePointInfo.Instance;
                manager.Done();
            }
        }

        private class ShopScreen : RogueListuiScreen
        {
            private readonly ScrollMenuViewData<AssetStartingItem, MMgr> view = new()
            {
            };

            public ShopScreen(ScenarioMonolithBeApplied parent)
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show(parent._shopItems, manager)
                    ?
                    .NameFrom(item => item.Name)

                    .OnClick((item, manager) =>
                    {
                        manager.AddObject(DeviceKw.AppendText, item);
                        manager.AddObject(DeviceKw.AppendText, "を手に入れた\n");
                        item.Option.CreateObj(item, Arg.Self, Vector2Int.zero, RogueRandom.Primary);
                    })

                    .Build();
                };
            }
        }

        private class SetMainChartScreen : RogueListuiScreen
        {
            private readonly DialogViewData<MMgr> view = new()
            {
                DialogSubviewSelector = m => m.Widgets,
            };

            public SetMainChartScreen()
            {
                OnOpenScreen += (manager) =>
                {
                    view.Show("", manager)
                    ?
                    .Tail.Append(InputFieldWidgetOption.Create<MMgr>(
                        _ =>
                        {
                            var monolith = Arg.Arg.Tool;
                            var info = ScenarioMonolithInfo.Get(monolith);
                            return info.MainChart;
                        },
                        value =>
                        {
                            var monolith = Arg.Arg.Tool;
                            var info = ScenarioMonolithInfo.Get(monolith);
                            return info.MainChart = value;
                        }))

                    .Build();
                };
            }
        }
    }
}
