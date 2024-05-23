using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [Objforming.Formable]
    public class RgpackReference : System.IEquatable<RgpackReference>
    {
        public string RgpackID { get; }
        public string AssetID { get; }

        private static readonly Dictionary<string, IRgpack> loadedRgpackTable = new();

        private RgpackReference() { }

        public RgpackReference(string rgpackID, string assetID)
        {
            RgpackID = rgpackID;
            AssetID = assetID;
        }

        public T GetData<T>()
        {
            if (!TryGetRgpack(RgpackID, out var rgpack)) throw new RogueException($"Rgpack ({RgpackID}) が見つかりません。");
            if (!rgpack.TryGetAsset<T>(AssetID, out var asset)) throw new RogueException($"Rgpack ({RgpackID}) に ID ({AssetID}) のデータが見つかりません。");

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
            return AssetID?.GetHashCode() ?? 0;
        }
    }
}
