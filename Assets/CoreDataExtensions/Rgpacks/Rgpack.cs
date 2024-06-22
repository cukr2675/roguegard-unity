using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard.Rgpacks
{
    public class Rgpack
    {
        public string ID { get; }

        private readonly Dictionary<string, object> table;

        public Rgpack(string id, IReadOnlyDictionary<string, object> directory, IScriptEvaluator evaluator)
        {
            ID = id;
            table = new Dictionary<string, object>();
            Add(directory, evaluator, "");
        }

        private void Add(IReadOnlyDictionary<string, object> directory, IScriptEvaluator evaluator, string directoryName)
        {
            foreach (var pair in directory)
            {
                var assetID = directoryName + pair.Key;
                var fullID = ID + "." + assetID;

                // キーにドット (.) が存在するとき、文字の長さが 1 である、または終端以外にドットが存在する場合は警告を表示して無視する
                if (pair.Key.Contains('.') && (pair.Key.Length == 1 || pair.Key.IndexOf('.') != pair.Key.Length - 1))
                {
                    Debug.LogWarning($"不正な名前 ({fullID}) が存在します。");
                    continue;
                }

                // サブディレクトリ
                if (pair.Key.EndsWith("."))
                {
                    if (pair.Value is IReadOnlyDictionary<string, object> subDirectory)
                    {
                        Add(subDirectory, evaluator, assetID);
                    }
                    continue;
                }

                // 文字列はスクリプトとして評価する
                if (pair.Value is string code)
                {
                    var evaluatedPairs = evaluator.Evaluate(code);
                    foreach (var evaluatedPair in evaluatedPairs)
                    {
                        table.Add(evaluatedPair.Key, evaluatedPair.Value);
                    }
                    continue;
                }
                
                // その他はアセットに変換
                if (pair.Value is CharacterCreationDataBuilder characterCreationDataBuilder)
                {
                    table.Add(assetID, new CharacterCreationPresetAsset(characterCreationDataBuilder));
                }
                else if (pair.Value is KyarakuriClayInfo kyarakuriClayInfo)
                {
                    table.Add(assetID, new KyarakuriClayAsset(kyarakuriClayInfo, ID, fullID));
                }
                else if (pair.Value is MysteryDioramaInfo mysteryDioramaInfo)
                {
                    table.Add(assetID, new MysteryDioramaAsset(mysteryDioramaInfo, ID, fullID));
                }
                else if (pair.Value is MapDioramaFloorInfo mapDioramaFloorInfo)
                {
                    table.Add(assetID, new MapDioramaFloorAsset(mapDioramaFloorInfo, fullID));
                }
                else if (pair.Value is EvtFairyInfo evtFairyInfo)
                {
                    table.Add(assetID, new EvtFairyAsset(evtFairyInfo, ID, fullID));
                }
                else if (pair.Value is ChartPadInfo chartPadInfo)
                {
                    table.Add(assetID, new ChartPadAsset(chartPadInfo, ID, fullID));
                }
                else if (pair.Value is SpQuestMonolithInfo monolithInfo)
                {
                    table.Add(assetID, new SpQuestMonolithAsset(monolithInfo, ID));
                }
            }
        }

        public bool TryGetAsset<T>(string id, out T asset)
        {
            if (table.TryGetValue(id, out var assetObj) && assetObj is T assetT)
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

        public IEnumerable<T> GetSubAssets<T>(string id)
        {
            foreach (var pair in table)
            {
                if (!pair.Key.StartsWith(id)) continue;
                if (pair.Key == id) continue;
                if (pair.Key.IndexOf('.', id.Length + 1) >= 0) continue;
                if (!(pair.Value is T t)) continue;

                yield return t;
            }
        }
    }
}
