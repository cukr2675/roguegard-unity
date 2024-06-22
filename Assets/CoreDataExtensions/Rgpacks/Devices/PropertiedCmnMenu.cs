using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Roguegard.Rgpacks;

namespace Roguegard.Device
{
    public class PropertiedCmnMenu : IModelsMenu
    {
        private readonly List<object> models = new();
        private static readonly AssetID assetID = new();

        public void OpenMenu(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
        {
            var cmnData = (PropertiedCmnData)arg.Other;

            models.Clear();
            models.Add(assetID);
            if (!string.IsNullOrWhiteSpace(cmnData.Cmn))
            {
                // コモンイベントのプロパティ一覧を取得するためにビルドする
                var atelier = SpQuestMonolithInfo.GetAtelierByCharacter(self);
                var rgpackDirectory = Rgpacker.Pack(atelier);
                var rgpack = new Rgpack("Playtest", rgpackDirectory, Rgpacker.DefaultEvaluator);
                RgpackReference.LoadRgpack(rgpack);

                var properties = cmnData.GetProperties(rgpack.ID);
                foreach (var pair in properties)
                {
                    if (pair.Value is NumberCmnProperty numberCmnProperty)
                    {
                        models.Add(new NumberProperty(pair.Key, numberCmnProperty));
                    }
                }
            }

            var options = root.Get(DeviceKw.MenuOptions);
            options.OpenView(ChoiceListPresenter.Instance, models, root, self, user, arg);
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
                var cmnData = (PropertiedCmnData)arg.Other;
                return cmnData.Cmn;
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                var cmnData = (PropertiedCmnData)arg.Other;
                cmnData.Cmn = value;
            }
        }

        private class NumberProperty : IModelsMenuOptionText
        {
            private readonly string name;
            private readonly NumberCmnProperty cmnProperty;

            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.DecimalNumber;

            public NumberProperty(string name, NumberCmnProperty cmnProperty)
            {
                this.name = name;
                this.cmnProperty = cmnProperty;
            }

            public string GetName(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
                => name;

            public string GetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg)
            {
                return cmnProperty.Value.ToString();
            }

            public void SetValue(IModelsMenuRoot root, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
            {
                cmnProperty.Value = float.Parse(value);
            }
        }
    }
}
