//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//using TMPro;
//using Roguegard.Rgpacks;

//namespace Roguegard.Device
//{
//    public class PropertiedCmnMenu : IListMenu
//    {
//        private readonly List<object> elms = new();
//        private static readonly AssetID assetID = new();

//        public void OpenMenu(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
//        {
//            var cmnData = (PropertiedCmnData)arg.Other;

//            elms.Clear();
//            elms.Add(assetID);
//            if (!string.IsNullOrWhiteSpace(cmnData.Cmn))
//            {
//                // コモンイベントのプロパティ一覧を取得するためにビルドする
//                var atelier = SpQuestMonolithInfo.GetAtelierByCharacter(self);
//                var rgpackDirectory = Rgpacker.Pack(atelier);
//                var rgpack = new Rgpack("Playtest", rgpackDirectory, Rgpacker.DefaultEvaluator);
//                RgpackReference.LoadRgpack(rgpack);

//                var properties = cmnData.GetProperties(rgpack.ID);
//                foreach (var pair in properties)
//                {
//                    if (pair.Value is NumberCmnProperty numberCmnProperty)
//                    {
//                        elms.Add(new NumberProperty(pair.Key, numberCmnProperty));
//                    }
//                }
//            }

//            var options = manager.GetView(DeviceKw.MenuOptions);
//            options.OpenView(SelectOptionPresenter.Instance, elms, manager, self, user, arg);
//            options.SetPosition(0f);
//            ExitListMenuSelectOption.OpenLeftAnchorExit(manager);
//        }

//        private class AssetID : IOptionsMenuText
//        {
//            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.Standard;

//            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
//                => "アセットID";

//            public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
//            {
//                var cmnData = (PropertiedCmnData)arg.Other;
//                return cmnData.Cmn;
//            }

//            public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
//            {
//                var cmnData = (PropertiedCmnData)arg.Other;
//                cmnData.Cmn = value;
//            }
//        }

//        private class NumberProperty : IOptionsMenuText
//        {
//            private readonly string name;
//            private readonly NumberCmnProperty cmnProperty;

//            public TMP_InputField.ContentType ContentType => TMP_InputField.ContentType.DecimalNumber;

//            public NumberProperty(string name, NumberCmnProperty cmnProperty)
//            {
//                this.name = name;
//                this.cmnProperty = cmnProperty;
//            }

//            public string GetName(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
//                => name;

//            public string GetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg)
//            {
//                return cmnProperty.Value.ToString();
//            }

//            public void SetValue(IListMenuManager manager, RogueObj self, RogueObj user, in RogueMethodArgument arg, string value)
//            {
//                cmnProperty.Value = float.Parse(value);
//            }
//        }
//    }
//}
