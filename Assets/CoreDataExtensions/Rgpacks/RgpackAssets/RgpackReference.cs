using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    public static class RgpackReference
    {
        private static readonly Dictionary<string, Rgpack> loadedRgpackTable = new();

        public static string GetRgpackID(string id, string envRgpackID)
        {
            if (id == null)
            {
                return "";
            }
            else if (id.StartsWith('.'))
            {
                if (string.IsNullOrWhiteSpace(envRgpackID)) throw new RogueException(
                    $"ドットで始まるID ({id}) の読み込み時、環境 RgpackID が指定されませんでした。");
                return envRgpackID;
            }
            else
            {
                return id.Substring(0, id.IndexOf('.'));
            }
        }

        public static string GetAssetID(string id)
        {
            if (id == null) return "";
            return id.Substring(id.IndexOf('.') + 1);
        }

        public static bool TryGetRgpack(string rgpackID, out Rgpack rgpack)
        {
            return loadedRgpackTable.TryGetValue(rgpackID, out rgpack);
        }

        public static IEnumerable<T> GetSubAssets<T>(string id, string envRgpackID)
        {
            var rgpackID = GetRgpackID(id, envRgpackID);
            if (!TryGetRgpack(rgpackID, out var rgpack)) throw new RogueException(
                 $"Rgpack ({rgpackID}) が見つかりません。");

            var assetID = GetAssetID(id);
            return rgpack.GetSubAssets<T>(assetID);
        }

        public static void LoadRgpack(Rgpack rgpack)
        {
            loadedRgpackTable[rgpack.ID] = rgpack;
        }
    }

    public abstract class RgpackReference<T>
    {
        public string FullID { get; }

        [System.NonSerialized] private string _rgpackID;
        public string RgpackID => _rgpackID ??= FullID is null ? "" : FullID.Substring(0, FullID.IndexOf('.'));

        [System.NonSerialized] private string _assetID;
        public string AssetID => _assetID ??= FullID is null ? "" : FullID.Substring(FullID.IndexOf('.') + 1);

        [System.NonSerialized] private T _asset;
        protected T Asset => _asset ??= GetAsset();

        protected RgpackReference() { }

        protected RgpackReference(string id, string envRgpackID)
        {
            if (string.IsNullOrWhiteSpace(id)) return;

            if (id.StartsWith("."))
            {
                FullID = envRgpackID + id;
            }
            else
            {
                FullID = id;
            }
        }

        private T GetAsset()
        {
            if (!RgpackReference.TryGetRgpack(RgpackID, out var rgpack)) throw new RogueException(
                $"Rgpack ({RgpackID}) が見つかりません。");
            if (!rgpack.TryGetAsset<T>(AssetID, out var asset)) throw new RogueException(
                $"Rgpack ({RgpackID}) に ID ({AssetID}) のデータが見つかりません。");

            return asset;
        }
    }
}
