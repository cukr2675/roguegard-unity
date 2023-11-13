using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
using Roguegard.Device;

namespace Roguegard
{
    public static class RoguegardSettings
    {
        public static IRogueObjGenerator InitialPlayerCharacterGenerator { get; set; }

        public static Vector2Int MaxTilemapSize { get; set; }

        public static float DefaultVisibleRadius { get; set; }

        /// <summary>
        /// ローグガルドで標準の PixelPerUnit
        /// </summary>
        public static int PixelPerUnit { get; set; }

        public static Color BoneSpriteBaseColor { get; set; }

        /// <summary>
        /// customShift シェーダーの明度補正値
        /// </summary>
        public static float LightRatio { get; set; }

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

        private static readonly List<IModelsMenuChoice> _dungeonChoices = new List<IModelsMenuChoice>();

        public static Spanning<IModelsMenuChoice> DungeonChoices => _dungeonChoices;

        private static readonly Dictionary<string, Dictionary<string, object>> _assetTables = new Dictionary<string, Dictionary<string, object>>();

        public static void AddDungeonChoice(IModelsMenuChoice dungeonChoice)
        {
            _dungeonChoices.Add(dungeonChoice);
        }

        public static void ClearDungeonChoices()
        {
            _dungeonChoices.Clear();
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
