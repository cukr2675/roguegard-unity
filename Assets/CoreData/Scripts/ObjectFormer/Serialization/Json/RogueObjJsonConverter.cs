using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;
using Newtonsoft.Json;
using ObjectFormer;
using ObjectFormer.Serialization.Json;

namespace Roguegard.ObjectFormer.Json
{
    public class RogueObjJsonConverter : FormerJsonConverter
    {
        public override IReadOnlyList<Type> FieldTypes { get; }

        public RogueObjJsonConverter(Former former)
            : base(former)
        {
            var fieldTypes = base.FieldTypes.ToArray();
            for (int i = 0; i < fieldTypes.Length; i++)
            {
                // Json 化で Dictionary から List に変換する
                if (fieldTypes[i] == typeof(Dictionary<Type, IRogueObjInfo>))
                {
                    fieldTypes[i] = typeof(List<IRogueObjInfo>);
                }
            }
            FieldTypes = fieldTypes;
        }

        public static RogueObjJsonConverter Create(bool force = false)
        {
            var type = typeof(RogueObj);
            var members = FormerMember.Generate(type, force);
            var former = new Former(type, members);
            return new RogueObjJsonConverter(former);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            if (JsonConverterUtility.WriteReferenceOrIDAndType(writer, value, serializer))
            {
                for (int i = 0; i < Former.Members.Count; i++)
                {
                    var member = Former.Members[i];
                    var memberValue = member.GetValue(value);
                    var memberType = member.FieldType;

                    // Json 化で除外する IRogueObjInfo をのぞいたリストを生成し、それを書き込む
                    if (member.CamelName == "infos")
                    {
                        var table = (Dictionary<Type, IRogueObjInfo>)memberValue;
                        var list = new List<IRogueObjInfo>();
                        foreach (var pair in table)
                        {
                            if (!pair.Value.IsExclusedWhenSerialize)
                            {
                                list.Add(pair.Value);
                            }
                        }
                        memberValue = list;
                        memberType = typeof(List<IRogueObjInfo>);
                    }

                    writer.WritePropertyName(member.CamelName);
                    serializer.Serialize(writer, memberValue, memberType);
                }
            }
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            if (JsonConverterUtility.ReadTryResolveReference(reader, serializer, out var referencedValue, out var id, out _)) return referencedValue;

            var value = existingValue ?? Former.CreateInstance();
            JsonConverterUtility.AddReference(id, value, serializer);
            while (true)
            {
                if (reader.TokenType == JsonToken.EndObject) break;

                var propertyName = (string)reader.Value;

                reader.Read();
                if (!Former.TryGetMemberByCamel(propertyName, out var member)) continue;

                var memberType = member.FieldType;
                if (member.CamelName == "infos") { memberType = typeof(List<IRogueObjInfo>); }
                var memberValue = serializer.Deserialize(reader, memberType);

                // Json 化で Dictionary から List にしたので戻す
                if (member.CamelName == "infos")
                {
                    var list = (List<IRogueObjInfo>)memberValue;
                    var table = new Dictionary<Type, IRogueObjInfo>();
                    foreach (var info in list)
                    {
                        table.Add(info.GetType(), info);
                    }
                    memberValue = table;
                }

                member.SetValue(value, memberValue);

                reader.Read();
            }
            return value;
        }
    }
}
