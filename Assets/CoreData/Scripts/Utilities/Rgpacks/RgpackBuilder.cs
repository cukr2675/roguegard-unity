using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class RgpackBuilder : IRgpack
    {
        public string ID { get; }

        private readonly Dictionary<string, object> assetTable = new();

        public object this[string id]
        {
            get => assetTable[id];
            set => assetTable[id] = value;
        }

        public RgpackBuilder(string id)
        {
            ID = id;
        }

        public bool TryGetAsset<T>(string id, out T asset)
        {
            if (assetTable.TryGetValue(id, out var assetObj) && assetObj is T assetT)
            {
                asset = assetT;
                return true;
            }
            else
            {
                asset = default;
                return false;
            }
        }

        /// <summary>
        /// 指定の <see cref="RogueObj"/> 以下のすべてのアセットを追加する
        /// </summary>
        public void AddAllAssets(RogueObj obj)
        {
            var characterCreationInfo = KyarakuriFigurineInfo.Get(obj);
            if (characterCreationInfo != null)
            {
                assetTable.Add(characterCreationInfo.ID, characterCreationInfo);
            }

            var mysteryDioramaInfo = MysteryDioramaInfo.Get(obj);
            if (mysteryDioramaInfo != null)
            {
                mysteryDioramaInfo.AddAssets(obj, ID);
                assetTable.Add(mysteryDioramaInfo.ID, mysteryDioramaInfo);
            }

            var rogueChart = RogueChartInfo.GetChart(obj);
            if (rogueChart != null)
            {
                rogueChart.SetRgpackID(ID);
                assetTable.Add(rogueChart.ID, rogueChart);
            }

            var monolithInfo = SpQuestMonolithInfo.Get(obj);
            if (monolithInfo != null)
            {
                monolithInfo.SetRgpackID(ID);
                assetTable.Add("Main", monolithInfo);
            }

            var notepadInfo = NotepadInfo.GetText(obj);
            if (notepadInfo != null)
            {
                RoguegardSettings.ScriptingEvaluator.Evaluate(notepadInfo, this);
            }

            var spaceObjs = obj.Space.Objs;
            for (int i = 0; i < spaceObjs.Count; i++)
            {
                var spaceObj = spaceObjs[i];
                if (spaceObj == null) continue;

                AddAllAssets(spaceObj);
            }
        }

        public void AddAsset(string id, object asset)
        {
            assetTable.Add(id, asset);
        }
    }
}
