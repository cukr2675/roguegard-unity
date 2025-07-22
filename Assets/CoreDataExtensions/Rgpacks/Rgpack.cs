using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OchalikeSprites;
using Roguegard.CharacterCreation;

namespace Roguegard.Rgpacks
{
    public class Rgpack
    {
        public string Id { get; }

        private readonly Dictionary<string, object> table;

        public Rgpack(string id, IReadOnlyDictionary<string, object> directory, IScriptEvaluator evaluator)
        {
            Id = id;
            table = new Dictionary<string, object>();
            Add(directory, evaluator, "");
        }

        private void Add(IReadOnlyDictionary<string, object> directory, IScriptEvaluator evaluator, string directoryName)
        {
            foreach (var pair in directory)
            {
                var assetId = directoryName + pair.Key;
                var fullId = Id + "." + assetId;

                // キーにドット (.) が存在するとき、文字の長さが 1 である、または終端以外にドットが存在する場合は警告を表示して無視する
                if (pair.Key.Contains('.') && (pair.Key.Length == 1 || pair.Key.IndexOf('.') != pair.Key.Length - 1))
                {
                    Debug.LogWarning($"不正な名前 ({fullId}) が存在します。");
                    continue;
                }

                // サブディレクトリ
                if (pair.Key.EndsWith("."))
                {
                    if (pair.Value is IReadOnlyDictionary<string, object> subDirectory)
                    {
                        Add(subDirectory, evaluator, assetId);
                    }
                    continue;
                }

                // 文字列はスクリプトとして評価する
                if (pair.Value is string code)
                {
                    var evaluatedPairs = evaluator.Evaluate(code, Id);
                    foreach (var evaluatedPair in evaluatedPairs)
                    {
                        table.Add(evaluatedPair.Key, evaluatedPair.Value);
                    }
                    continue;
                }

                // その他はアセットに変換
                if (pair.Value is CharacterCreationDataBuilder characterCreationDataBuilder)
                {
                    table.Add(assetId, new CharacterCreationPresetAsset(characterCreationDataBuilder));
                }
                if (pair.Value is RaceOptionalCreationDataAsset raceOptionalCreationData)
                {
                    table.Add(assetId, raceOptionalCreationData);
                }
                else if (pair.Value is KyarakuriClayInfo kyarakuriClayInfo)
                {
                    table.Add(assetId, new KyarakuriClayAsset(kyarakuriClayInfo, Id, fullId));
                }
                else if (pair.Value is MysteryDioramaInfo mysteryDioramaInfo)
                {
                    table.Add(assetId, new MysteryDioramaAsset(mysteryDioramaInfo, Id, fullId));
                }
                else if (pair.Value is MapDioramaFloorInfo mapDioramaFloorInfo)
                {
                    table.Add(assetId, new MapDioramaFloorAsset(mapDioramaFloorInfo, fullId));
                }
                else if (pair.Value is EffectStickerInfo effectStickerInfo)
                {
                    table.Add(assetId, new EffectStickerAsset(effectStickerInfo, Id, fullId));
                }
                else if (pair.Value is EvtFairyInfo evtFairyInfo)
                {
                    table.Add(assetId, new EvtFairyAsset(evtFairyInfo, Id, fullId));
                }
                else if (pair.Value is ChartPadInfo chartPadInfo)
                {
                    table.Add(assetId, new ChartPadAsset(chartPadInfo, Id, fullId));
                }
                else if (pair.Value is SewedEquipmentData sewedEquipmentData)
                {
                    table.Add(assetId, sewedEquipmentData);
                }
                else if (pair.Value is ISpriteMotion spriteMotion)
                {
                    table.Add(assetId, spriteMotion);
                }
                else if (pair.Value is ScenarioMonolithInfo monolithInfo)
                {
                    table.Add(assetId, new ScenarioMonolithAsset(monolithInfo, Id));
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
