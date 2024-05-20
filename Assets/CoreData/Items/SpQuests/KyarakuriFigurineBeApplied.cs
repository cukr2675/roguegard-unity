using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
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
                KyarakuriFigurineInfo.SetTo(self);
            }

            RogueDevice.Primary.AddMenu(menu, user, null, new(targetObj: self));
            return false;
        }

        private class Menu : BaseScrollModelsMenu<object>
        {
            private static readonly object[] models = new object[]
            {
                new AssetID(),
                new SetStartingPlayerChoice(),
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

        private class AssetID : IModelsMenuOptionText
        {
            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "アセットID";

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var figurine = arg.TargetObj;
                var characterCreationInfo = KyarakuriFigurineInfo.Get(figurine);
                return characterCreationInfo.ID;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var figurine = arg.TargetObj;
                var characterCreationInfo = KyarakuriFigurineInfo.Get(figurine);
                characterCreationInfo.ID = value;
            }
        }

        private class SetStartingPlayerChoice : IModelsMenuChoice
        {
            private static readonly EditMenu nextMenu = new();

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => "キャラクリ設定";

            public void Activate(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var figurine = arg.TargetObj;
                var characterCreationInfo = KyarakuriFigurineInfo.Get(figurine);
                var builder = characterCreationInfo.Main;

                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);
                root.OpenMenu(nextMenu, self, user, new(targetObj: figurine, other: builder));
            }
        }

        private class EditMenu : IModelsMenu
        {
            private readonly object[] models = new object[] { DialogModelsMenuChoice.CreateExit(Save) };

            public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                var builder = (CharacterCreationDataBuilder)arg.Other;
                root.Get(DeviceKw.MenuCharacterCreation).OpenView(null, models, root, self, user, new(targetObj: arg.TargetObj, other: builder));
            }

            private static void Save(IModelsMenuRoot root, RogueObj player, RogueObj user, in RogueMethodArgument arg)
            {
                root.AddObject(DeviceKw.EnqueueSE, DeviceKw.Submit);

                if (arg.Other is CharacterCreationDataBuilder builder)
                {
                    // キャラクリ画面から戻ったとき、人形を更新する
                    var figurine = arg.TargetObj;
                    var characterCreationInfo = KyarakuriFigurineInfo.Get(figurine);
                    characterCreationInfo.Main = builder;
                }

                root.Back();
                root.Back();
            }
        }
    }
}
