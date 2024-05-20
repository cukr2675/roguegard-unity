using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
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
                    return "+ äKëwÇí«â¡";
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
                    SpaceUtility.TryLocate(self, dioramaFloorObj, Vector2Int.one);
                    root.Done();
                }
                else if (model == null)
                {
                    var diorama = arg.TargetObj;
                    _newFloor.Option.CreateObj(_newFloor, diorama, Vector2Int.zero, RogueRandom.Primary);
                    root.Reopen(null, null, arg);
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
                => "ÉAÉZÉbÉgID";

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var diorama = arg.TargetObj;
                var mysteryDioramaInfo = MysteryDioramaInfo.Get(diorama);
                return mysteryDioramaInfo.ID;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var diorama = arg.TargetObj;
                var mysteryDioramaInfo = MysteryDioramaInfo.Get(diorama);
                mysteryDioramaInfo.ID = value;
            }
        }
    }
}
