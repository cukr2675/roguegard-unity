using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;

namespace Roguegard.Rgpacks
{
    public class KyarakuriFigurineBeApplied : BaseApplyRogueMethod
    {
        private static Menu menu;

        public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            menu ??= new();
            var characterCreationInfo = KyarakuriFigurineInfo.Get(self);
            if (characterCreationInfo == null)
            {
                KyarakuriFigurineInfo.SetTo(self, RoguegardSettings.CharacterCreationDatabase.LoadPreset(0));
            }

            RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self));
            return false;
        }

        private class Menu : BaseScrollListMenu<object>
        {
            private static readonly object[] elms = new object[]
            {
                new AssetID(),
                new SetStartingPlayerSelectOption(),
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

        private class AssetID : IOptionsMenuText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "アセットID";

            public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var figurine = arg.TargetObj;
                return NamingEffect.Get(figurine)?.Naming;
            }

            public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var figurine = arg.TargetObj;
                default(IActiveRogueMethodCaller).Affect(figurine, 1f, NamingEffect.Callback);
                NamingEffect.Get(figurine).Naming = value;
            }
        }

        private class SetStartingPlayerSelectOption : IListMenuSelectOption
        {
            private static readonly EditMenu nextMenu = new();

            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "キャラクリ設定";

            public void Activate(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var figurine = arg.TargetObj;
                var builder = KyarakuriFigurineInfo.Get(figurine);

                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                manager.OpenMenu(nextMenu, self, user, new(targetObj: figurine, other: builder));
            }
        }

        private class EditMenu : IListMenu
        {
            private readonly object[] elms = new object[] { DialogListMenuSelectOption.CreateExit(Save) };

            public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var builder = (CharacterCreationDataBuilder)arg.Other;
                manager.GetView(DeviceKw.MenuCharacterCreation).OpenView(null, elms, manager, self, user, new(targetObj: arg.TargetObj, other: builder));
            }

            private static void Save(IListMenuManager manager, RogueObj player, RogueObj user, in RogueMethodArgument arg)
            {
                manager.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                if (arg.Other is CharacterCreationDataBuilder builder)
                {
                    // キャラクリ画面から戻ったとき、人形を更新する
                    var figurine = arg.TargetObj;
                    KyarakuriFigurineInfo.SetTo(figurine, builder);
                }

                manager.Back();
                manager.Back();
            }
        }
    }
}
