using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.Rgpacks
{
    [Objforming.Formable]
    public class PropertiedCmnData
    {
        public string Cmn { get; set; }

        private Dictionary<string, ICmnProperty> properties;

        private static readonly List<string> notUsedKeys = new();

        public PropertiedCmnData()
        {
            properties = new Dictionary<string, ICmnProperty>();
        }

        public PropertiedCmnData(PropertiedCmnData data)
        {
            Cmn = data.Cmn;
            properties = new Dictionary<string, ICmnProperty>(data.properties);
        }

        [Objforming.CreateInstance]
        private PropertiedCmnData(bool dummy) { }

        public IReadOnlyDictionary<string, ICmnProperty> GetProperties(string envRgpackID)
        {
            if (string.IsNullOrWhiteSpace(Cmn))
            {
                properties ??= new Dictionary<string, ICmnProperty>();
                return properties;
            }

            var reference = new CmnReference(Cmn, envRgpackID);
            var cmn = reference.Asset;

            properties ??= new Dictionary<string, ICmnProperty>();
            notUsedKeys.Clear();
            notUsedKeys.AddRange(properties.Keys);

            // 不足しているプロパティを追加 + キーが一致するが型が一致しないプロパティを変更
            foreach (var pair in cmn.PropertySources)
            {
                if (!properties.TryGetValue(pair.Key, out var property) ||
                    property.Source != pair.Value)
                {
                    properties[pair.Key] = pair.Value.CreateProperty();
                }
                notUsedKeys.Remove(pair.Key);
            }

            // 未使用のプロパティを削除
            foreach (var notUsedKey in notUsedKeys)
            {
                properties.Remove(notUsedKey);
            }

            return properties;
        }
    }
}
