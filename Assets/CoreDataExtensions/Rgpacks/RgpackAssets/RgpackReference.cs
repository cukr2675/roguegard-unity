using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public static class RgpackReference
    {
        private static readonly Dictionary<string, Rgpack> loadedRgpackTable = new();

        public static string GetRgpackId(string id, string envRgpackId)
        {
            if (id == null)
            {
                return "";
            }
            else if (id.StartsWith('.'))
            {
                if (string.IsNullOrWhiteSpace(envRgpackId)) throw new RogueException(
                    $"ドットで始まるID ({id}) の読み込み時、環境 RgpackId が指定されませんでした。");
                return envRgpackId;
            }
            else
            {
                return id.Substring(0, id.IndexOf('.'));
            }
        }

        public static string GetAssetId(string id)
        {
            if (id == null) return "";
            return id.Substring(id.IndexOf('.') + 1);
        }

        public static bool TryGetRgpack(string rgpackId, out Rgpack rgpack)
        {
            return loadedRgpackTable.TryGetValue(rgpackId, out rgpack);
        }

        public static IEnumerable<T> GetSubAssets<T>(string id, string envRgpackId)
        {
            var rgpackId = GetRgpackId(id, envRgpackId);
            if (!TryGetRgpack(rgpackId, out var rgpack)) throw new RogueException(
                 $"Rgpack ({rgpackId}) が見つかりません。");

            var assetId = GetAssetId(id);
            return rgpack.GetSubAssets<T>(assetId);
        }

        public static void LoadRgpack(Rgpack rgpack)
        {
            loadedRgpackTable[rgpack.Id] = rgpack;
        }
    }

    public abstract class RgpackReference<T>
    {
        public string FullId { get; }

        [System.NonSerialized] private string _rgpackId;
        public string RgpackId => _rgpackId ??= FullId is null ? "" : FullId.Substring(0, FullId.IndexOf('.'));

        [System.NonSerialized] private string _assetId;
        public string AssetId => _assetId ??= FullId is null ? "" : FullId.Substring(FullId.IndexOf('.') + 1);

        [System.NonSerialized] private T _asset;
        protected T Asset => _asset ??= GetAsset();

        public bool AssetExists
        {
            get
            {
                if (!RgpackReference.TryGetRgpack(RgpackId, out var rgpack)) return false;
                if (!rgpack.TryGetAsset<T>(AssetId, out var asset)) return false;
                return true;
            }
        }

        protected RgpackReference() { }

        protected RgpackReference(string id, string envRgpackId)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            if (id.StartsWith("."))
            {
                FullId = envRgpackId + id;
            }
            else
            {
                FullId = id;
            }
        }

        private T GetAsset()
        {
            if (!RgpackReference.TryGetRgpack(RgpackId, out var rgpack)) throw new RogueException(
                $"Rgpack ({RgpackId}) が見つかりません。");
            if (!rgpack.TryGetAsset<T>(AssetId, out var asset)) throw new RogueException(
                $"Rgpack ({RgpackId}) に ID ({AssetId}) のデータが見つかりません。");

            return asset;
        }
    }
}
