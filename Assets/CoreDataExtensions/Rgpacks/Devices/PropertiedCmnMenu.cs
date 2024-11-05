using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using ListingMF;
using Roguegard.Rgpacks;

namespace Roguegard.Device
{
    public class PropertiedCmnMenu : RogueMenuScreen
    {
        private readonly List<object> elms = new();

        private readonly VariableWidgetsViewTemplate<MMgr, MArg> view = new()
        {
        };

        public override void OpenScreen(in MMgr manager, in MArg arg)
        {
            var cmnData = (PropertiedCmnData)arg.Arg.Other;

            elms.Clear();
            if (!string.IsNullOrWhiteSpace(cmnData.Cmn))
            {
                // コモンイベントのプロパティ一覧を取得するためにビルドする
                var atelier = SpQuestMonolithInfo.GetAtelierByCharacter(arg.Self);
                var rgpackDirectory = Rgpacker.Pack(atelier);
                var rgpack = new Rgpack("Playtest", rgpackDirectory, Rgpacker.DefaultEvaluator);
                RgpackReference.LoadRgpack(rgpack);

                var properties = cmnData.GetProperties(rgpack.ID);
                foreach (var pair in properties)
                {
                    if (pair.Value is NumberCmnProperty numberCmnProperty)
                    {
                        elms.Add(
                            new object[]
                            {
                                pair.Key,
                                InputFieldViewWidget.CreateOption<MMgr, MArg>(
                                    (manager, arg) => numberCmnProperty.Value.ToString(),
                                    (manager, arg, value) =>(numberCmnProperty.Value = float.Parse(value)).ToString(),
                                    TMP_InputField.ContentType.DecimalNumber)
                            });
                    }
                }
            }

            view.ShowTemplate(elms, manager, arg)
                ?
                .InsertNext(
                    new object[]
                    {
                        "アセットID",
                        InputFieldViewWidget.CreateOption<MMgr, MArg>(
                            (manager, arg) => ((PropertiedCmnData)arg.Arg.Other).Cmn,
                            (manager, arg, value) => ((PropertiedCmnData)arg.Arg.Other).Cmn = value)
                    })

                .Build();
        }
    }
}
