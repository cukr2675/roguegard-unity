using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using ListingMF;
using Roguegard.CharacterCreation;
using Roguegard.Extensions;
using Roguegard.Device;

namespace Roguegard.Rgpacks
{
    public class SpQuestMonolithBeApplied : BaseApplyRogueMethod
    {
        [SerializeField, ElementDescription("_option")] private ScriptableStartingItem[] _shopItems = null;

        private Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            menu ??= new(this);
            if (SpQuestMonolithInfo.Get(self) == null) { SpQuestMonolithInfo.SetTo(self); }

            RogueDevice.Primary.AddMenu(menu, user, null, new(tool: self));
            return false;
        }

        private class Menu : RogueMenuScreen
        {
            private readonly SpQuestMonolithBeApplied parent;

            private readonly MainMenuViewTemplate<MMgr, MArg> view = new()
            {
                PrimaryCommandSubViewName = StandardSubViewTable.ScrollName,
            };

            public Menu(SpQuestMonolithBeApplied parent)
            {
                this.parent = parent;
            }

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate(manager, arg)
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
                var spQuestAtelier = monolith.Location;
                var rgpack = Rgpacker.Pack(spQuestAtelier);
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
            public SpQuestMonolithBeApplied parent;

            private readonly ScrollViewTemplate<ScriptableStartingItem, MMgr, MArg> view = new()
            {
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                var list = parent._shopItems;

                view.ShowTemplate(list, manager, arg)
                    ?
                    .ElementNameFrom((item, manager, arg) =>
                    {
                        return item.Name;
                    })

                    .OnClickElement((item, manager, arg) =>
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
            private readonly DialogViewTemplate<MMgr, MArg> view = new()
            {
                DialogSubViewName = StandardSubViewTable.WidgetsName,
            };

            public override void OpenScreen(in MMgr manager, in MArg arg)
            {
                view.ShowTemplate("", manager, arg)
                    ?.Append(InputFieldViewWidget.CreateOption<MMgr, MArg>(
                        (manager, arg) =>
                        {
                            var monolith = arg.Arg.Tool;
                            var info = SpQuestMonolithInfo.Get(monolith);
                            return info.MainChart;
                        },
                        (manager, arg, value) =>
                        {
                            var monolith = arg.Arg.Tool;
                            var info = SpQuestMonolithInfo.Get(monolith);
                            return info.MainChart = value;
                        }))
                    .Build();
            }
        }
    }
}
