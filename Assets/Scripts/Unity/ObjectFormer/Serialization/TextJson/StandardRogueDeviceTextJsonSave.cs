using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ObjectFormer;
using ObjectFormer.Serialization.TextJson;
using Roguegard;
using Roguegard.Device;
using Roguegard.RogueObjectFormer.TextJson;

namespace RoguegardUnity
{
    internal class StandardRogueDeviceTextJsonSave : IRogueDeviceSave<StandardRogueDevice>
    {
        public string TypeName => "StandardRogueDevice";

        public StandardRogueDevice NewGame()
        {
            var random = new RogueRandom();

            // キャラクターを生成
            var player = RoguegardSettings.WorldGenerator.CreateObj(null, Vector2Int.zero, random);
            var world = RogueWorld.GetWorld(player);

            // デバイスを設定
            var data = new StandardRogueDeviceData();
            data.Player = player;
            data.TargetObj = player;
            data.World = world;
            data.CurrentRandom = random;
            var device = new StandardRogueDevice(data);
            RogueDeviceEffect.SetTo(player);
            return device;
        }

        public void SaveGame(Stream stream, string name, StandardRogueDevice device)
        {
            var save = new SaveClass();
            save.Data = device;

            var config = GetJsonSerializationConfig();
            config.Serialize(stream, save);
        }

        public StandardRogueDevice LoadGame(Stream stream)
        {
            using (var streamReader = new StreamReader(stream))
            {
                using var reader = new JsonTextReader(streamReader);

                var jObj = JObject.Load(reader);

                var typeName = jObj["type"].ToString();
                if (typeName != TypeName) throw new RogueException($"type ({typeName}) が {TypeName} と一致しません。");
            }
            stream.Position = 0;

            var config = GetJsonSerializationConfig();
            return config.Deserialize<SaveClass>(stream).Data;
        }

        public static JsonSerializationModule[] GetJsonSerializationModules()
        {
            var assemblies = new[]
            {
                Assembly.Load("UnityEngine.CoreModule"),
                Assembly.Load("Roguegard"),
                Assembly.Load("Roguegard.CharacterCreation"),
                Assembly.Load("Roguegard.Device"),
                Assembly.Load("Roguegard.CoreData")
            };
            var converters = new RelationalComponentListBuilder<RelationalJsonConverter>();
            converters.Add(FormerJsonConverter.Create(typeof(Vector2Int), true));
            converters.Add(FormerJsonConverter.Create(typeof(RectInt), true));
            converters.Add(FormerJsonConverter.Create(typeof(Color), true));
            converters.Add(RogueObjJsonConverter.Create());
            converters.Add(FormerJsonConverter.Create(typeof(StandardRogueDevice)));
            converters.AddAuto(assemblies, instanceType =>
            {
                if (instanceType.IsArray)
                {
                    var fieldElementType = instanceType.GetElementType();
                    return new RelationOnlyComponent(instanceType, fieldElementType);
                }
                if (instanceType.IsGenericType)
                {
                    var instanceTypeDefinition = instanceType.GetGenericTypeDefinition();
                    if (instanceTypeDefinition == typeof(List<>) || instanceTypeDefinition == typeof(Dictionary<,>))
                    {
                        return new RelationOnlyComponent(instanceType, instanceType.GenericTypeArguments);
                    }
                }
                if (instanceType.IsDefined(typeof(IgnoreRequireRelationalComponentAttribute)) ||
                    instanceType.IsDefined(typeof(ReferableAttribute)))
                {
                    return new RelationOnlyComponent(instanceType);
                }
                if (instanceType.IsDefined(typeof(FormableAttribute)))
                {
                    return FormerJsonConverter.Create(instanceType);
                }
                return null;
            });
            converters.Add(new RoguegardAssetTableJsonConverter("Core"));

            var name = "Core";
            var version = new System.Version(1, 0, 0);
            var module = new JsonSerializationModule(name, version, converters);

            return new[] { module };
        }

        public static JsonSerializationConfig GetJsonSerializationConfig()
        {
            var modules = GetJsonSerializationModules();
            var moduleTable = new DependencyModuleTable<JsonSerializationModule>();
            var config = new JsonSerializationConfig(modules, moduleTable);
            return config;
        }

        [Formable]
        private class SaveClass
        {
            public StandardRogueDevice Data;
        }
    }
}
