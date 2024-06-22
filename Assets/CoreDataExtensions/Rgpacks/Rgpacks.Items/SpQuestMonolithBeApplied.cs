using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
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

        private class Menu : BaseScrollListMenu<object>
        {
            private readonly object[] elms;

            public Menu(SpQuestMonolithBeApplied parent)
            {
                elms = new object[]
                {
                    new ShotSelectOption() { parent = parent },
                    new SetMainChartSelectOption(),
                    new PlaytestSelectOption(),
                    new LeaveSelectOption(),
                };
            }

            protected override Spanning<object> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return elms;
            }

            protected override string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return SelectOptionPresenter.Instance.GetItemName(element, manager, self, user, arg);
            }

            protected override void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                SelectOptionPresenter.Instance.ActivateItem(element, manager, self, user, arg);
            }
        }

        private class ShotSelectOption : BaseScrollListMenu<ScriptableStartingItem>, IListMenuSelectOption
        {
            public SpQuestMonolithBeApplied parent;

            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "ショップ";

            public void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.OpenMenu(this, self, null, RogueMethodArgument.Identity);
            }

            protected override Spanning<ScriptableStartingItem> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return parent._shopItems;
            }

            protected override string GetItemName(
                ScriptableStartingItem startingItem, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return startingItem.Name;
            }

            protected override void ActivateItem(
                ScriptableStartingItem startingItem, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.AddObject(DeviceKw.AppendText, startingItem);
                manager.AddObject(DeviceKw.AppendText, "を手に入れた\n");
                startingItem.Option.CreateObj(startingItem, self, Vector2Int.zero, RogueRandom.Primary);
            }
        }

        private class SetMainChartSelectOption : IListMenuSelectOption, IListMenu
        {
            private readonly object[] elms = new object[]
            {
                new AssetID(),
            };

            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "メインチャート設定";

            public void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var monolith = arg.Tool;
                manager.OpenMenu(this, null, null, new(tool: monolith));
            }

            public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var monolith = arg.Tool;

                var options = manager.GetView(DeviceKw.MenuOptions);
                options.OpenView(SelectOptionPresenter.Instance, elms, manager, null, null, new(tool: monolith));
                options.SetPosition(0f);
                ExitListMenuSelectOption.OpenLeftAnchorExit(manager);
            }

            private class AssetID : IOptionsMenuText
            {
                public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

                public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    => "アセットID";

                public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var monolith = arg.Tool;
                    var info = SpQuestMonolithInfo.Get(monolith);
                    return info.MainChart;
                }

                public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
                {
                    var monolith = arg.Tool;
                    var info = SpQuestMonolithInfo.Get(monolith);
                    info.MainChart = value;
                }
            }
        }

        private class PlaytestSelectOption : IListMenuSelectOption
        {
            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "テストプレイ";

            public void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var monolith = arg.Tool;
                var spQuestAtelier = monolith.Location;
                var rgpack = Rgpacker.Pack(spQuestAtelier);
                RogueDevice.Add(DeviceKw.StartPlaytest, rgpack);
            }
        }

        private class LeaveSelectOption : IListMenuSelectOption
        {
            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "アトリエから出る";

            public void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.AddObject(DeviceKw.EnqueueSE, CategoryKw.DownStairs);
                default(IActiveRogueMethodCaller).LocateSavePoint(self, null, 0f, RogueWorldSavePointInfo.Instance, true);
                var memberInfo = LobbyMemberList.GetMemberInfo(self);
                memberInfo.SavePoint = RogueWorldSavePointInfo.Instance;
                manager.Done();
            }
        }
    }
}
