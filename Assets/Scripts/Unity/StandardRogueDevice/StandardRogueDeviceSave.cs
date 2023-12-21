using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ObjectFormer;
using ObjectFormer.Serialization.Json;
using Roguegard;
using Roguegard.CharacterCreation;
using Roguegard.Device;
using Roguegard.Extensions;
using Roguegard.RogueObjectFormer.Json;

namespace RoguegardUnity
{
    public class StandardRogueDeviceSave : IRogueDeviceSave<StandardRogueDevice>
    {
        public string TypeName => "StandardRogueDevice.Json";

        public static string RootDirectory => "./Save";

        public static Spanning<string> Extensions => _extensions;
        private static readonly string[] _extensions = new[] { ".gard", ".zip" };

        private static readonly NewGamePointInfo newGamePointInfo = new NewGamePointInfo();

        private readonly CharacterCreationDataBuilder characterCreationDataBuilder;

        public StandardRogueDeviceSave()
        {
        }

        public StandardRogueDeviceSave(CharacterCreationDataBuilder characterCreationDataBuilder)
        {
            this.characterCreationDataBuilder = new CharacterCreationDataBuilder(characterCreationDataBuilder);
        }

        public static void GetFiles(System.Action<IEnumerable<string>> callback)
        {
            GetFiles(RootDirectory, callback);
        }

        private static void GetFiles(string path, System.Action<IEnumerable<string>> callback)
        {
            RogueFile.GetFiles(path, files =>
            {
                callback(files.Where(x => x.ToLower().EndsWith(_extensions[0]) || x.ToLower().EndsWith(_extensions[1])));
            });
        }

        public static void GetNewNumberingPath(string name, System.Action<string> callback)
        {
            var directory = Path.GetDirectoryName(name);
            if (directory == ".") { directory = ""; }
            var saveName = Path.GetFileNameWithoutExtension(name);
            var extension = Path.GetExtension(name);

            GetFiles(Path.Combine(RootDirectory, directory), files =>
            {
                var numbers = files.Where(x => Path.GetFileName(x).StartsWith(saveName)).Select(x => GetFirstNumber(x)).ToArray();
                var newNumber = 1;
                if (numbers.Length >= 1) { newNumber = numbers.Max() + 1; }
                if (!Extensions.Contains(extension)) { extension += _extensions[0]; }
                callback(Path.Combine(RootDirectory, directory, saveName + newNumber + extension));
            });
        }

        private static int GetFirstNumber(string x)
        {
            var matches = Regex.Matches(x, @"\d+");
            if (matches.Count >= 1) return int.Parse(matches[0].Value);
            else return 0;
        }

        public StandardRogueDevice NewGame()
        {
            var random = new RogueRandom();

            // キャラクターを生成
            var world = RoguegardSettings.WorldGenerator.CreateObj(null, Vector2Int.zero, random);
            var player = characterCreationDataBuilder.CreateObj(world, Vector2Int.zero, random);
            LobbyMembers.Add(player);

            // デバイスを設定
            var options = new RogueOptions();
            options.ClearWithoutSet();
            var data = new StandardRogueDeviceData();
            data.Player = player;
            data.World = world;
            data.Options = options;
            data.CurrentRandom = random;
            data.SavePointInfo = newGamePointInfo;
            var device = new StandardRogueDevice(data);
            RogueDeviceEffect.SetTo(player);
            return device;
        }

        public void SaveGame(Stream stream, string name, StandardRogueDeviceData data)
        {
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create, true);
            var entry = archive.CreateEntry($"{name}.json");
            using var streamWriter = new StreamWriter(entry.Open());
            using var writer = new JsonTextWriter(streamWriter);

            var config = GetJsonSerializationConfig();
            writer.WriteStartObject();

            writer.WritePropertyName("type");
            writer.WriteValue(TypeName);

            writer.WritePropertyName("version");
            writer.WriteValue(Application.version);

            writer.WritePropertyName("data");
            config.Serialize(writer, data);

            writer.WriteEndObject();
        }

        public StandardRogueDeviceData LoadGameData(Stream stream)
        {
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read, true);
            var entry = archive.Entries.FirstOrDefault(x => x.Name.EndsWith(".json"));
            if (entry == null) throw new RogueException("読み込んだファイル内で .json ファイルが見つかりません。");
            using var streamReader = new StreamReader(entry.Open());
            using var reader = new JsonTextReader(streamReader);

            var config = GetJsonSerializationConfig();
            var jObj = JObject.Load(reader);

            var typeName = jObj["type"].ToString();
            if (typeName != TypeName) throw new RogueException($"type ({typeName}) が {TypeName} と一致しません。");

            using var dataReader = jObj["data"].CreateReader();
            var data = config.Deserialize<StandardRogueDeviceData>(dataReader);
            return data;
        }

        public StandardRogueDevice LoadGame(Stream stream)
        {
            var data = LoadGameData(stream);
            return new StandardRogueDevice(data);
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
            converters.Add(FormerJsonConverter.Create(typeof(Color32), true));
            converters.Add(RogueObjJsonConverter.Create());
            converters.Add(FormerJsonConverter.Create(typeof(StandardRogueDeviceData)));
            converters.Add(FormerJsonConverter.Create(typeof(RogueOptions)));
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
            var version = new System.Version(0, 1, 0);
            var module = new JsonSerializationModule(name, version, converters);

            //var settings = new JsonSerializerSettings()
            //{
            //    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            //    TypeNameHandling = TypeNameHandling.Auto,
            //    Converters = new JsonConverter[]
            //    {
            //        NonIDNonTypeFormerJsonConverter.Create(typeof(Former)),
            //        NonIDNonTypeFormerJsonConverter.Create(typeof(FormerMember)),
            //    }
            //};
            //var json = JsonConvert.SerializeObject(module, Formatting.Indented, settings);
            //module = JsonConvert.DeserializeObject<JsonSerializationModule>(json, settings);

            return new[] { module };
        }

        public static JsonSerializationConfig GetJsonSerializationConfig()
        {
            var modules = GetJsonSerializationModules();
            var moduleTable = new DependencyModuleTable<JsonSerializationModule>();
            var config = new JsonSerializationConfig(modules, moduleTable);
            return config;
        }

        private class NewGamePointInfo : ISavePointInfo
        {
            public IApplyRogueMethod BeforeSave => throw new System.NotSupportedException();
            public IApplyRogueMethod AfterLoad { get; } = new AfterLoad();
        }

        private class AfterLoad : BaseApplyRogueMethod
        {
            public override bool Invoke(RogueObj self, RogueObj player, float activationDepth, in RogueMethodArgument arg)
            {
                if (activationDepth != 0f) throw new RogueException();
                if (!default(IActiveRogueMethodCaller).LocateSavePoint(player, null, 1f, RogueWorld.SavePointInfo, true)) throw new RogueException();
                if (!default(IActiveRogueMethodCaller).LoadSavePoint(player, 1f, RogueWorld.SavePointInfo)) throw new RogueException();
                return true;
            }
        }
    }
}
