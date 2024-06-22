using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard.Rgpacks
{
    public class MysteryDioramaBeApplied : BaseApplyRogueMethod
    {
        [SerializeField] private ScriptableStartingItem _newFloor = null;

        private Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            menu ??= new() { _newFloor = _newFloor };
            var mysteryDioramaInfo = MysteryDioramaInfo.Get(self);
            if (mysteryDioramaInfo == null)
            {
                MysteryDioramaInfo.SetTo(self);
            }

            RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self));
            return false;
        }

        private class Menu : BaseScrollModelsMenu<object>
        {
            public ScriptableStartingItem _newFloor;

            private static readonly List<object> models = new();
            private static readonly AssetID assetID = new();
            private static readonly FloorMenu nextMenu = new();

            protected override IKeyword ViewKeyword => DeviceKw.MenuOptions;

            protected override Spanning<object> GetModels(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var diorama = arg.TargetObj;
                var dioramaFloorObjs = diorama.Space.Objs;
                models.Clear();
                models.Add(assetID);
                for (int i = 0; i < dioramaFloorObjs.Count; i++)
                {
                    var dioramaFloorObj = dioramaFloorObjs[i];
                    if (dioramaFloorObj == null) continue;

                    models.Add(dioramaFloorObj);
                }
                models.Add(null);
                return models;
            }

            protected override string GetItemName(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model is RogueObj dioramaFloorObj)
                {
                    return dioramaFloorObj.GetName();
                }
                else if (model == null)
                {
                    return "+ ŠK‘w‚ð’Ç‰Á";
                }
                else
                {
                    return ChoiceListPresenter.Instance.GetItemName(model, root, self, user, arg);
                }
            }

            protected override void ActivateItem(object model, IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (model is RogueObj dioramaFloorObj)
                {
                    root.OpenMenu(nextMenu, self, null, new(targetObj: dioramaFloorObj));
                }
                else if (model == null)
                {
                    var diorama = arg.TargetObj;
                    _newFloor.Option.CreateObj(_newFloor, diorama, Vector2Int.zero, RogueRandom.Primary);
                    root.Reopen(self, user, arg);
                }
                else
                {
                    ChoiceListPresenter.Instance.ActivateItem(model, root, self, user, arg);
                }
            }
        }

        private class AssetID : IModelsMenuOptionText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "ƒAƒZƒbƒgID";

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var diorama = arg.TargetObj;
                return NamingEffect.Get(diorama)?.Naming;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var diorama = arg.TargetObj;
                default(IActiveRogueMethodCaller).Affect(diorama, 1f, NamingEffect.Callback);
                NamingEffect.Get(diorama).Naming = value;
            }
        }

        private class FloorMenu : BaseScrollModelsMenu<object>
        {
            public ScriptableStartingItem _newFloor;

            private static readonly object[] models = new object[]
            {
                new AssetID(),
                new EnterChoice()
            };

            protected override IKeyword ViewKeyword => DeviceKw.MenuOptions;

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

        private class EnterChoice : IModelsMenuChoice
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "“ü‚é";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var dioramaFloor = arg.TargetObj;
                SpaceUtility.TryLocate(self, dioramaFloor, Vector2Int.one);
                root.Done();
            }
        }
    }
}
