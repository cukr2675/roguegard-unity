using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Roguegard.CharacterCreation;
using Roguegard.Extensions;
using Roguegard.Device;

namespace Roguegard
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

        private class Menu : BaseScrollModelsMenu<object>
        {
            private readonly object[] models;

            public Menu(SpQuestMonolithBeApplied parent)
            {
                models = new object[]
                {
                    new ShotChoice() { parent = parent },
                    new SetMainChartChoice(),
                    new PlaytestChoice(),
                    new LeaveChoice(),
                };
            }

            protected override Spanning<object> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return models;
            }

            protected override string GetItemName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return ChoiceListPresenter.Instance.GetItemName(model, root, self, user, arg);
            }

            protected override void ActivateItem(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                ChoiceListPresenter.Instance.ActivateItem(model, root, self, user, arg);
            }
        }

        private class ShotChoice : BaseScrollModelsMenu<ScriptableStartingItem>, IModelsMenuChoice
        {
            public SpQuestMonolithBeApplied parent;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "ショップ";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.OpenMenu(this, self, null, RogueMethodArgument.Identity);
            }

            protected override Spanning<ScriptableStartingItem> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return parent._shopItems;
            }

            protected override string GetItemName(
                ScriptableStartingItem startingItem, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return startingItem.Name;
            }

            protected override void ActivateItem(
                ScriptableStartingItem startingItem, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.AddObject(DeviceKw.AppendText, startingItem);
                root.AddObject(DeviceKw.AppendText, "を手に入れた\n");
                startingItem.Option.CreateObj(startingItem, self, Vector2Int.zero, RogueRandom.Primary);
            }
        }

        private class SetMainChartChoice : IModelsMenuChoice, IModelsMenu
        {
            private readonly object[] models = new object[]
            {
                new AssetID(),
            };

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "メインチャート設定";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                var monolith = arg.Tool;
                root.OpenMenu(this, null, null, new(tool: monolith));
            }

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var monolith = arg.Tool;

                var options = root.Get(DeviceKw.MenuOptions);
                options.OpenView(ChoiceListPresenter.Instance, models, root, null, null, new(tool: monolith));
                options.SetPosition(0f);
                ExitModelsMenuChoice.OpenLeftAnchorExit(root);
            }

            private class AssetID : IModelsMenuOptionText
            {
                public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

                public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                    => "アセットID";

                public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                {
                    var monolith = arg.Tool;
                    var info = SpQuestMonolithInfo.Get(monolith);
                    return info.MainChart?.AssetID;
                }

                public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
                {
                    var monolith = arg.Tool;
                    var info = SpQuestMonolithInfo.Get(monolith);
                    info.MainChart = new RgpackReference(info.MainChart?.RgpackID, value);
                }
            }
        }

        private class PlaytestChoice : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "テストプレイ";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var monolith = arg.Tool;
                var spQuestAtelier = monolith.Location;
                var rgpackBuilder = new RgpackBuilder("Playtest");
                rgpackBuilder.AddAllAssets(spQuestAtelier);
                RogueDevice.Add(DeviceKw.StartPlaytest, rgpackBuilder);
            }
        }

        private class LeaveChoice : IModelsMenuChoice
        {
            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "アトリエから出る";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.AddObject(DeviceKw.EnqueueSE, CategoryKw.DownStairs);
                default(IActiveRogueMethodCaller).LocateSavePoint(self, null, 0f, RogueWorldSavePointInfo.Instance, true);
                var memberInfo = LobbyMemberList.GetMemberInfo(self);
                memberInfo.SavePoint = RogueWorldSavePointInfo.Instance;
                root.Done();
            }
        }
    }
}
