using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public static class Rgpacker
    {
        public static IScriptEvaluator DefaultEvaluator { get; set; }

        /// <summary>
        /// 指定の <see cref="RogueObj"/> 以下のすべてのアセットを Rgpack にパッキングする
        /// </summary>
        public static Dictionary<string, object> Pack(RogueObj rootObj)
        {
            var directory = new Dictionary<string, object>();
            AddAllAssetsTo(directory, rootObj);
            return directory;
        }

        private static void AddAllAssetsTo(Dictionary<string, object> directory, RogueObj obj)
        {
            var useSubDirectory = false;
            var assetID = NamingEffect.Get(obj)?.Naming;
            if (assetID != null)
            {
                var characterCreationDataBuilder = KyarakuriFigurineInfo.Get(obj);
                if (characterCreationDataBuilder != null) { directory.Add(assetID, characterCreationDataBuilder); }

                var kyarakuriClayInfo = KyarakuriClayInfo.Get(obj);
                if (kyarakuriClayInfo != null) { directory.Add(assetID, kyarakuriClayInfo); }

                var effectStickerInfo = EffectStickerInfo.Get(obj);
                if (effectStickerInfo != null) { directory.Add(assetID, effectStickerInfo); }

                var mysteryDioramaInfo = MysteryDioramaInfo.Get(obj);
                if (mysteryDioramaInfo != null)
                {
                    directory.Add(assetID, mysteryDioramaInfo);
                    useSubDirectory = true;
                }

                var dioramaFloorInfo = DioramaFloorInfo.Get(obj);
                if (dioramaFloorInfo is MapDioramaFloorInfo)
                {
                    directory.Add(assetID, new MapDioramaFloorInfo(obj));
                    useSubDirectory = true;
                }

                var evtFairyInfo = EvtFairyInfo.Get(obj);
                if (evtFairyInfo != null)
                {
                    evtFairyInfo.Pages[0].Position = obj.Position;
                    directory.Add(assetID, evtFairyInfo);
                }

                var chartPadInfo = ChartPadInfo.Get(obj);
                if (chartPadInfo != null) { directory.Add(assetID, chartPadInfo); }

                var sewedInfoSet = obj.Main.InfoSet as SewedEquipmentInfoSet;
                if (sewedInfoSet != null)
                {
                    directory.Add(assetID, sewedInfoSet.GetDataClone());
                }

                var choreographerInfo = MotionGrapherInfo.Get(obj);
                if (choreographerInfo != null)
                {
                    directory.Add(assetID, new MotionGrapherSpriteMotion(choreographerInfo));
                }

                var text = NotepadInfo.GetText(obj);
                if (text != null) { directory.Add(assetID, text); }
            }
            else
            {
                // 名前が設定されていない場合は UUID を設定
                var text = NotepadInfo.GetText(obj);
                if (text != null) { directory.Add($"__script: {System.Guid.NewGuid()}", text); }
            }

            var monolithInfo = SpQuestMonolithInfo.Get(obj);
            if (monolithInfo != null) { directory.Add("__main", monolithInfo); }

            if (useSubDirectory)
            {
                var subDirectory = new Dictionary<string, object>();
                var spaceObjs = obj.Space.Objs;
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    var spaceObj = spaceObjs[i];
                    if (spaceObj == null) continue;

                    AddAllAssetsTo(subDirectory, spaceObj);
                }
                directory.Add($"{assetID}.", subDirectory);
            }
            else
            {
                var spaceObjs = obj.Space.Objs;
                for (int i = 0; i < spaceObjs.Count; i++)
                {
                    var spaceObj = spaceObjs[i];
                    if (spaceObj == null) continue;

                    AddAllAssetsTo(directory, spaceObj);
                }
            }
        }
    }
}
