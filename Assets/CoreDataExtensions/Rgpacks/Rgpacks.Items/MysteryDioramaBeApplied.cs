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

        private class Menu : BaseScrollListMenu<object>
        {
            public ScriptableStartingItem _newFloor;

            private static readonly List<object> elms = new();
            private static readonly AssetID assetID = new();
            private static readonly FloorMenu nextMenu = new();

            protected override IKeyword ViewKeyword => DeviceKw.MenuOptions;

            protected override Spanning<object> GetList(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var diorama = arg.TargetObj;
                var dioramaFloorObjs = diorama.Space.Objs;
                elms.Clear();
                elms.Add(assetID);
                for (int i = 0; i < dioramaFloorObjs.Count; i++)
                {
                    var dioramaFloorObj = dioramaFloorObjs[i];
                    if (dioramaFloorObj == null) continue;

                    elms.Add(dioramaFloorObj);
                }
                elms.Add(null);
                return elms;
            }

            protected override string GetItemName(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (element is RogueObj dioramaFloorObj)
                {
                    return dioramaFloorObj.GetName();
                }
                else if (element == null)
                {
                    return "+ ŠK‘w‚ð’Ç‰Á";
                }
                else
                {
                    return SelectOptionPresenter.Instance.GetItemName(element, manager, self, user, arg);
                }
            }

            protected override void ActivateItem(object element, IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                if (element is RogueObj dioramaFloorObj)
                {
                    manager.OpenMenu(nextMenu, self, null, new(targetObj: dioramaFloorObj));
                }
                else if (element == null)
                {
                    var diorama = arg.TargetObj;
                    _newFloor.Option.CreateObj(_newFloor, diorama, Vector2Int.zero, RogueRandom.Primary);
                    manager.Reopen(self, user, arg);
                }
                else
                {
                    SelectOptionPresenter.Instance.ActivateItem(element, manager, self, user, arg);
                }
            }
        }

        private class AssetID : IOptionsMenuText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "ƒAƒZƒbƒgID";

            public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var diorama = arg.TargetObj;
                return NamingEffect.Get(diorama)?.Naming;
            }

            public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var diorama = arg.TargetObj;
                default(IActiveRogueMethodCaller).Affect(diorama, 1f, NamingEffect.Callback);
                NamingEffect.Get(diorama).Naming = value;
            }
        }

        private class FloorMenu : BaseScrollListMenu<object>
        {
            public ScriptableStartingItem _newFloor;

            private static readonly object[] elms = new object[]
            {
                new AssetID(),
                new EnterSelectOption()
            };

            protected override IKeyword ViewKeyword => DeviceKw.MenuOptions;

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

        private class EnterSelectOption : IListMenuSelectOption
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "“ü‚é";

            public void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var dioramaFloor = arg.TargetObj;
                SpaceUtility.TryLocate(self, dioramaFloor, Vector2Int.one);
                manager.Done();
            }
        }
    }
}
