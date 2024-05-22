using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Roguegard.Device;

namespace Roguegard
{
    public class EvtFairyBeApplied : BaseApplyRogueMethod
    {
        private static Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            menu ??= new();
            var characterCreationInfo = EvtFairyInfo.Get(self);
            if (characterCreationInfo == null)
            {
                EvtFairyInfo.SetTo(self);
            }

            RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self));
            return false;
        }

        private class Menu : BaseScrollModelsMenu<object>
        {
            private static readonly List<object> models = new();
            private static readonly AssetID assetID = new();
            private static readonly AppearanceAssetID appearanceAssetID = new AppearanceAssetID();
            private static readonly OpensMenu nextMenu = new();

            protected override IKeyword ViewKeyword => DeviceKw.MenuOptions;

            protected override Spanning<object> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var fairy = arg.TargetObj;
                var eventFairyInfo = EvtFairyInfo.Get(fairy);
                models.Clear();
                models.Add(assetID);
                models.Add(appearanceAssetID);
                for (int i = 0; i < eventFairyInfo.Opens.Count; i++)
                {
                    models.Add(eventFairyInfo.Opens[i]);
                }
                return models;
            }

            protected override string GetItemName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model is RgpackReference reference) return reference.AssetID;
                else return "+ Open を追加";
            }

            protected override void ActivateItem(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model is RgpackReference reference)
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    var fairy = arg.TargetObj;
                    var eventFairyInfo = EvtFairyInfo.Get(fairy);
                    var index = eventFairyInfo.Opens.IndexOf(reference);
                    root.OpenMenu(nextMenu, null, null, new(targetObj: fairy, count: index));

                }
                else
                {
                    root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                    var fairy = arg.TargetObj;
                    var eventFairyInfo = EvtFairyInfo.Get(fairy);
                    var newOpens = new RgpackReference[eventFairyInfo.Opens.Count + 1];
                    for (int i = 0; i < eventFairyInfo.Opens.Count; i++)
                    {
                        newOpens[i] = eventFairyInfo.Opens[i];
                    }
                    newOpens[newOpens.Length - 1] = new RgpackReference(null, null);
                    eventFairyInfo.SetOpens(newOpens);
                    root.Reopen(null, null, arg);
                }
            }
        }

        private class AssetID : IModelsMenuOptionText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "アセットID";

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var fairy = arg.TargetObj;
                var eventFairyInfo = EvtFairyInfo.Get(fairy);
                return eventFairyInfo.ID;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var fairy = arg.TargetObj;
                var eventFairyInfo = EvtFairyInfo.Get(fairy);
                eventFairyInfo.ID = value;
            }
        }

        private class AppearanceAssetID : IModelsMenuOptionText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "見た目アセットID";

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var fairy = arg.TargetObj;
                var eventFairyInfo = EvtFairyInfo.Get(fairy);
                return eventFairyInfo.Appearance.AssetID;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var fairy = arg.TargetObj;
                var eventFairyInfo = EvtFairyInfo.Get(fairy);
                eventFairyInfo.Appearance = new RgpackReference(eventFairyInfo.Appearance.RgpackID, value);
            }
        }

        private class OpensMenu : IModelsMenu
        {
            private readonly object[] models = new object[]
            {
                new AssetID(),
            };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var fairy = arg.TargetObj;
                var index = arg.Count;

                var options = root.Get(DeviceKw.MenuOptions);
                options.OpenView(ChoiceListPresenter.Instance, models, root, null, null, new(targetObj: fairy, count: index));
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
                    var fairy = arg.TargetObj;
                    var eventFairyInfo = EvtFairyInfo.Get(fairy);
                    var index = arg.Count;
                    var rgpackReference = eventFairyInfo.Opens[index];
                    return rgpackReference.AssetID;
                }

                public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
                {
                    var fairy = arg.TargetObj;
                    var eventFairyInfo = EvtFairyInfo.Get(fairy);
                    var index = arg.Count;
                    var opens = eventFairyInfo.Opens.ToArray();
                    var rgpackReference = opens[index];
                    opens[index] = new RgpackReference(rgpackReference.RgpackID, value);
                    eventFairyInfo.SetOpens(opens);
                }
            }
        }
    }
}
