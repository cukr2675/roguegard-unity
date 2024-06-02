using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class RgpackReference : System.IEquatable<RgpackReference>
    {
        public string ID { get; }

        [System.NonSerialized] private string _rgpackID;
        private string RgpackID => _rgpackID ?? throw new RogueException($"{ID} の Rgpack 名が読み込まれていません。");

        [System.NonSerialized] private string _assetID;
        private string AssetID => _assetID ?? throw new RogueException($"{ID} の Rgpack 名が読み込まれていません。");

        private static readonly Dictionary<string, IRgpack> loadedRgpackTable = new();

        private RgpackReference() { }

        public RgpackReference(string id)
        {
            ID = id;
        }

        public void LoadFullID(string envRgpackID)
        {
            _rgpackID = GetRgpackID(ID, envRgpackID);
            _assetID = GetAssetID(ID);
        }

        public static string GetRgpackID(string id, string envRgpackID)
        {
            if (id == null) return "";
            if (id.StartsWith('.')) return envRgpackID;
            else return id.Substring(0, id.IndexOf('.'));
        }

        public static string GetAssetID(string id)
        {
            if (id == null) return "";
            return id.Substring(id.IndexOf('.') + 1);
        }

        public T GetData<T>()
        {

            if (!TryGetRgpack(RgpackID, out var rgpack)) throw new RogueException(
                $"Rgpack ({RgpackID}) が見つかりません。");
            if (!rgpack.TryGetAsset<T>(AssetID, out var asset)) throw new RogueException(
                $"Rgpack ({RgpackID}) に ID ({AssetID}) のデータが見つかりません。");

            return asset;
        }

        public static bool TryGetRgpack(string rgpackID, out IRgpack rgpack)
        {
            return loadedRgpackTable.TryGetValue(rgpackID, out rgpack);
        }

        public static void LoadRgpack(IRgpack rgpack)
        {
            loadedRgpackTable.Add(rgpack.ID, rgpack);
        }

        public bool Equals(RgpackReference other)
        {
            return other.RgpackID == RgpackID && other.AssetID == AssetID;
        }

        public static bool Equals(RgpackReference a, RgpackReference b)
        {
            if (a == b) return true;
            if (a == null || b == null) return false;
            return a.Equals(b);
        }

        public override int GetHashCode()
        {
            return GetAssetID(ID).GetHashCode();
        }
    }
}
