using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class RgpackReferenceTimelineClip : ISubTimelineClip
    {
        public string Id { get; set; }
        public float StartTime { get; set; }

        public bool TryGet<T>(out T value)
        {
            //var envRgpackID = executionContext.OwnerScript.DoString("return __rgpack").String;
            var envRgpackID = "Playtest";
            var rgpackID = RgpackReference.GetRgpackID(Id, envRgpackID);
            var assetID = RgpackReference.GetAssetID(Id);

            if (!RgpackReference.TryGetRgpack(rgpackID, out var rgpack))
            {
                //Debug.LogError($"Rgpack ({rgpackID}) が見つかりません。");
                value = default;
                return false;
            }
            if (!rgpack.TryGetAsset<T>(assetID, out var spriteMotion))
            {
                //Debug.LogError($"Rgpack ({rgpackID}) に ID ({assetID}) のデータが見つかりません。");
                value = default;
                return false;
            }

            value = spriteMotion;
            return true;
        }
    }
}
