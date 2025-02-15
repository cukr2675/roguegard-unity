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
            //var envRgpackId = executionContext.OwnerScript.DoString("return __rgpack").String;
            var envRgpackId = "Playtest";
            var rgpackId = RgpackReference.GetRgpackId(Id, envRgpackId);
            var assetId = RgpackReference.GetAssetId(Id);

            if (!RgpackReference.TryGetRgpack(rgpackId, out var rgpack))
            {
                //Debug.LogError($"Rgpack ({rgpackId}) が見つかりません。");
                value = default;
                return false;
            }
            if (!rgpack.TryGetAsset<T>(assetId, out var spriteMotion))
            {
                //Debug.LogError($"Rgpack ({rgpackId}) に ID ({assetId}) のデータが見つかりません。");
                value = default;
                return false;
            }

            value = spriteMotion;
            return true;
        }
    }
}
