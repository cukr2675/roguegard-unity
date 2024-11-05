using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ListingMF;
using RuntimeDotter;
using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    public static class RoguegardSettings
    {
        public static IRogueObjGenerator WorldGenerator { get; set; }

        public static IMainInfoSet MoneyInfoSet { get; set; }

        public static Vector2Int MaxTilemapSize { get; set; }

        public static float DefaultVisibleRadius { get; set; }

        /// <summary>
        /// ローグガルドで標準の PixelsPerUnit
        /// </summary>
        public static int PixelsPerUnit { get; set; }

        public static Color BoneSpriteBaseColor { get; set; }

        /// <summary>
        /// customShift シェーダーの明度補正値
        /// </summary>
        public static float LightRatio { get; set; }

        private const int paletteLength = 16;
        private static ShiftableColor[] _defaultPalette;
        public static Spanning<ShiftableColor> DefaultPalette
        {
            get => _defaultPalette;
            set => _defaultPalette = (value.Count == paletteLength ? value.ToArray() : throw new RogueException());
        }

        public static Color White => Color.white * LightRatio;

        public static Color Gray => Color.gray * LightRatio;

        private static IKeyword[] _keywordsNotEnqueueMessageRule;
        public static Spanning<IKeyword> KeywordsNotEnqueueMessageRule
        {
            get => _keywordsNotEnqueueMessageRule;
            set => _keywordsNotEnqueueMessageRule = value.ToArray();
        }

        public static string DefaultSaveFileName { get; set; }

        public static DefaultRaceOption DefaultRaceOption { get; set; }

        public static ObjCommandTable ObjCommandTable { get; set; }

        public static ICharacterCreationDatabase CharacterCreationDatabase { get; set; }

        public static DungeonQuestGenerator DungeonQuestGenerator { get; set; }

        public static IJsonSerializationSetting JsonSerialization { get; set; }

        private static readonly List<ISelectOption> _dungeonSelectOption = new List<ISelectOption>();

        public static Spanning<ISelectOption> DungeonSelectOption => _dungeonSelectOption;

        private static readonly Dictionary<string, Dictionary<string, object>> _assetTables = new Dictionary<string, Dictionary<string, object>>();

        public static void AddDungeonSelectOption(ISelectOption dungeonSelectOption)
        {
            _dungeonSelectOption.Add(dungeonSelectOption);
        }

        public static void ClearDungeonSelectOptions()
        {
            _dungeonSelectOption.Clear();
        }

        public static void AddAssetTable(IReadOnlyDictionary<string, object> table)
        {
            foreach (var pair in table)
            {
                var spaceLength = pair.Key.LastIndexOf('.');
                var space = pair.Key.Substring(0, spaceLength);
                if (!_assetTables.TryGetValue(space, out var assetTable))
                {
                    assetTable = new Dictionary<string, object>();
                    _assetTables.Add(space, assetTable);
                }

                if (assetTable.ContainsKey(pair.Key))
                {
                    throw new RogueException($"{pair.Key} が重複しています。（{pair.Value}, already exists {assetTable[pair.Key]}）");
                }

                assetTable.Add(pair.Key, pair.Value);
            }
        }

        public static void ClearAssetTable()
        {
            _assetTables.Clear();
        }

        public static IReadOnlyDictionary<string, object> GetAssetTable(string space)
        {
            return _assetTables[space];
        }
    }
}
