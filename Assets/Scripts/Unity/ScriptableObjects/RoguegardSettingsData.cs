using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Reflection;
using Roguegard;
using Roguegard.CharacterCreation;
using System.IO;

namespace RoguegardUnity
{
    [CreateAssetMenu(menuName = "RoguegardData/Settings/RoguegardSettings")]
    public class RoguegardSettingsData : ScriptableObject
    {
#if UNITY_EDITOR
        [SerializeField] private bool _autoUpdateSelfOnEnable = false;
        [SerializeField] private bool _autoUpdateAssetTablesOnEnabled = false;
#endif

        [Header("Initial Assets")]
        [SerializeField] private ItemCreationData _world = null;
        [SerializeField] private LobbyCreationData _lobby = null;
        [Space]
        [SerializeField] private PresetCreationData[] _presets = null;

        [SerializeField] private ItemCreationData _money = null;

        [SerializeField] private Vector2Int _maxTilemapSize = new Vector2Int(y: 64, x: 96);
        // y: 64 透明マップのチップを4x4としたとき、縦幅を256に収めるサイズ
        // x: 96 縦幅に対して16:9に収まるサイズ

        [SerializeField] private float _defaultVisibleRadius = 2f;

        [SerializeField] private Color[] _defaultPalette = null;

        [Tooltip("メッセージを横線で分割しない行動を指定する")]
        [SerializeField] private KeywordData[] _keywordsNotEnqueueMessageRule = null;

        [Header("Sprites")]
        [Tooltip("ローグガルドで標準の PixelsPerUnit 。ドットキャラの解像度と素材の流用しやすさを考慮して 32 にする")]
        [SerializeField] private int _pixelsPerUnit = 32;
        [SerializeField] private Color _boneSpriteBaseColor = Color.white;
        [Tooltip("customShift シェーダーの明度補正値")]
        [SerializeField] private float _lightRatio = 248f / 255f;

        [Header("Options")]
        [SerializeField] private string _defaultSaveFileName = "Data.gard";

        [Header("Global Assets")]
        [SerializeField] private ScriptField<ILevelInfoInitializer> _levelInfoInitializer = null;
        [SerializeField] private EquipKeywordData _equipPartOfInnerwear = null;
        [SerializeField] private DefaultRaceOption _defaultRaceOption = null;
        [SerializeField] private ObjCommandTable _objCommandTable = null;
        [SerializeField] private DungeonQuestGenerator _dungeonQuestGenerator = null;
        [SerializeField] private RogueAssetTable[] _assetTables = null;

        [Header("Scriptable Loaders")]
        [SerializeField] private List<ScriptableLoader> _loaders = null;

        private void Init()
        {
            RoguegardSettings.WorldGenerator = new WorldGenerator() { parent = this };

            RoguegardSettings.MaxTilemapSize = _maxTilemapSize;
            RoguegardSettings.DefaultVisibleRadius = _defaultVisibleRadius;
            RoguegardSettings.DefaultPalette = _defaultPalette.Select(x => RoguePaintColor.FromColor(x)).ToArray();
            RoguegardSettings.KeywordsNotEnqueueMessageRule = _keywordsNotEnqueueMessageRule;

            RoguegardSettings.PixelsPerUnit = _pixelsPerUnit;
            RoguegardSettings.BoneSpriteBaseColor = _boneSpriteBaseColor;
            RoguegardSettings.LightRatio = _lightRatio;

            RoguegardSettings.DefaultSaveFileName = _defaultSaveFileName;

            RoguegardCharacterCreationSettings.LevelInfoInitializer = _levelInfoInitializer.Ref;
            RoguegardCharacterCreationSettings.EquipPartOfInnerwear = _equipPartOfInnerwear;
            RoguegardSettings.DefaultRaceOption = _defaultRaceOption;
            RoguegardSettings.ObjCommandTable = _objCommandTable;
            RoguegardSettings.DungeonQuestGenerator = _dungeonQuestGenerator;
            RoguegardSettings.MoneyInfoSet = _money.PrimaryInfoSet;
            RoguegardSettings.JsonSerialization = new JsonSerializationSetting();

            RoguegardSettings.ClearDungeonChoices();
            RoguegardSettings.ClearAssetTable();
            foreach (var assetTable in _assetTables)
            {
                RoguegardSettings.AddAssetTable(assetTable);

                foreach (var pair in assetTable)
                {
                    if (pair.Value is DungeonCreationData dungeonData)
                    {
                        var dungeonChoice = dungeonData.CreateDungeonChoice();
                        RoguegardSettings.AddDungeonChoice(dungeonChoice);
                    }
                }
            }

            {
                var characterCreationDatabase = new CharacterCreationDatabase();
                foreach (var preset in _presets)
                {
                    characterCreationDatabase.AddPreset(preset.ToBuilder());
                }

                var assetTable = RoguegardSettings.GetAssetTable("Core");
                foreach (var asset in assetTable.Values)
                {
                    if (asset is IRaceOption raceOption)
                    {
                        characterCreationDatabase.AddRaceOptions(raceOption);
                    }
                    if (asset is IAppearanceOption appearanceOption)
                    {
                        characterCreationDatabase.AddAppearanceOption(appearanceOption);
                    }
                    else if (asset is IIntrinsicOption intrinsicOption && intrinsicOption is not QuestEffectIntrinsicOption)
                    {
                        characterCreationDatabase.AddIntrinsicOption(intrinsicOption);
                    }
                    else if (asset is IStartingItemOption startingItemOption)
                    {
                        characterCreationDatabase.AddStartingItemOption(startingItemOption);
                    }
                }
                RoguegardSettings.CharacterCreationDatabase = characterCreationDatabase;
            }
        }

        public IEnumerator[] LoadAsync()
        {
            Init();

            return _loaders.Select(x => x.LoadAsync()).ToArray();
        }

        public void TestLoad()
        {
#if UNITY_EDITOR
            Init();

            foreach (var loader in _loaders)
            {
                loader.TestLoad();
            }
#else
            throw new RogueException($"{nameof(TestLoad)} はエディタ専用メソッドです。");
#endif
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            // isPlaying では初回実行時しか動作しない
            // isUpdating ではコンパイル時にも動作してしまう
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;

            if (_autoUpdateSelfOnEnable)
            {
                Update();
            }

            if (_autoUpdateAssetTablesOnEnabled)
            {
                foreach (var assetTable in _assetTables)
                {
                    assetTable.Update();
                }
            }
        }

        [ContextMenu("Update")]
        private void Update()
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var assemblies = callingAssembly.GetReferencedAssemblies().Select(x => Assembly.Load(x)).Append(callingAssembly).ToList();
            var loaderGUIDs = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(ScriptableLoader).Name}");
            _loaders = new List<ScriptableLoader>();
            foreach (var loaderGUID in loaderGUIDs)
            {
                var loaderPath = UnityEditor.AssetDatabase.GUIDToAssetPath(loaderGUID);
                var assemblyName = Roguegard.Editor.RoguegardAssetDatabase.GetAssemblyName(loaderPath);
                if (assemblyName == null) continue;

                Debug.Log($"{loaderPath} : {assemblyName}");
                var assembly = Assembly.Load(assemblyName);
                if (!assemblies.Contains(assembly)) continue;

                // 関連アセンブリの Loader を自動設定
                var loader = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableLoader>(loaderPath);
                _loaders.Add(loader);
            }
            //var thisPath = AssetDatabase.GetAssetPath(this);
            //EditorUtility.SetDirty(this);
            //AssetDatabase.ImportAsset(thisPath);
        }
#endif

        private class WorldGenerator : IRogueObjGenerator
        {
            public RoguegardSettingsData parent;

            public MainInfoSet InfoSet => ((IRogueObjGenerator)parent._world).InfoSet;
            public int Lv => ((IRogueObjGenerator)parent._world).Lv;
            public int Stack => ((IRogueObjGenerator)parent._world).Stack;
            public Spanning<IWeightedRogueObjGeneratorList> StartingItemTable => ((IRogueObjGenerator)parent._world).StartingItemTable;

            public RogueObj CreateObj(RogueObj location, Vector2Int position, IRogueRandom random, StackOption stackOption = StackOption.Default)
            {
                // ワールドを生成
                var world = parent._world.CreateObj(null, Vector2Int.zero, random);
                var lobby = parent._lobby.CreateObj(world, Vector2Int.zero, random);
                RogueWorldInfo.SetTo(world, lobby);
                return world;
            }
        }

        private class JsonSerializationSetting : IJsonSerializationSetting
        {
            public void Serialize<T>(Stream stream, T instance)
            {
                var config = StandardRogueDeviceSave.GetJsonSerializationConfig();
                config.Serialize(stream, instance);
            }

            public T Deserialize<T>(Stream stream)
            {
                var config = StandardRogueDeviceSave.GetJsonSerializationConfig();
                return config.Deserialize<T>(stream);
            }
        }
    }
}
