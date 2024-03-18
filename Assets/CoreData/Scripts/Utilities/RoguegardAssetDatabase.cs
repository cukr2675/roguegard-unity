#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Text.RegularExpressions;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using SkeletalSprite;

namespace Roguegard.Editor
{
    public static class RoguegardAssetDatabase
    {
        public static Sprite GetSprite(string name)
        {
            return (Sprite)AssetDatabase.FindAssets($"{name} t:{nameof(Sprite)}")
                .Distinct() // AssetDatabase.FindAssets は Sprite ではなく Texture2D を取得するため重複する。 Distinct で重複をなくす。
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .SelectMany(path => AssetDatabase.LoadAllAssetRepresentationsAtPath(path))
                .First(sprite => sprite.name == name); // 見つからなければ例外を投げる
        }

        private static IEnumerable<Sprite> GetSprites(string format, IEnumerable<object> args)
        {
            var searchName = string.Format(format, args.Append("").ToArray());
            var regexName = '^' + string.Format(format, args.Append("(_(NF|NR|BF|BR)+)?").ToArray()) + '$';
            var sprites = AssetDatabase.FindAssets($"{searchName} t:sprite")
                .Distinct() // AssetDatabase.FindAssets は Sprite ではなく Texture2D を取得するため重複する。 Distinct で重複をなくす。
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .SelectMany(path => AssetDatabase.LoadAllAssetRepresentationsAtPath(path))
                .Where(sprite => Regex.IsMatch(sprite.name, regexName))
                .OfType<Sprite>();
            return sprites;
        }

        public static ColorRangedBoneSprite CreateColorRangedBoneSpriteOrNull(bool colorRanged, string format, params object[] args)
        {
            if (colorRanged)
            {
                var lightSprites = GetSprites(format, args.Append(0));
                var darkSprites = GetSprites(format, args.Append(1));
                if (lightSprites.Any() && darkSprites.Any())
                {
                    return new ColorRangedBoneSprite(CreateBoneSprite(lightSprites), CreateBoneSprite(darkSprites));
                }
            }
            else
            {
                var sprites = GetSprites(format, args);
                if (sprites.Any())
                {
                    return new ColorRangedBoneSprite(CreateBoneSprite(sprites));
                }
            }
            return null;
        }

        private static BoneSprite CreateBoneSprite(IEnumerable<Sprite> sprites)
        {
            Sprite normalFront = null;
            Sprite normalRear = null;
            Sprite backFront = null;
            Sprite backRear = null;
            foreach (var sprite in sprites)
            {
                var spriteSuffixOffset = sprite.name.LastIndexOf('_');
                if (spriteSuffixOffset == -1)
                {
                    // _NF や _BR などの接尾辞がない場合は _NFBF として扱う。
                    normalFront = sprite;
                    backFront = sprite;
                    continue;
                }

                var spriteSuffix = sprite.name.Substring(spriteSuffixOffset + 1);
                if (spriteSuffix.Contains("NF")) { normalFront = sprite; }
                if (spriteSuffix.Contains("NR")) { normalRear = sprite; }
                if (spriteSuffix.Contains("BF")) { backFront = sprite; }
                if (spriteSuffix.Contains("BR")) { backRear = sprite; }
            }
            return new BoneSprite(normalFront, normalRear, backFront, backRear);
        }

        public static BoneKeywordData GetBoneKeyword(string name)
        {
            return AssetDatabase.FindAssets($"{name} t:{nameof(BoneKeywordData)}")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<BoneKeywordData>(path))
                .First(); // 見つからなければ例外を投げる
        }

        /// <summary>
        /// <paramref name="filter"/> に適合するファイルのうち、指定のパスと同一または親のフォルダにあるファイルのパスを取得する。
        /// （指定のスクリプトに適応されている Assembly Definition Asset の取得を想定）
        /// 同一フォルダに適合ファイルが複数存在する場合、例外を投げる。
        /// </summary>
        public static string GetNearestPath(string path, string filter)
        {
            var directory = Path.GetDirectoryName(path);
            var allAssetGUIDs = AssetDatabase.FindAssets(filter);
            string nearestAssetPath = null;
            foreach (var assetGUID in allAssetGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetGUID);
                var assetDirectory = Path.GetDirectoryName(assetPath);

                // path と同一または親のフォルダにあるアセットを取得する。
                if (!directory.StartsWith(assetDirectory)) continue;

                if (nearestAssetPath == null || // 初期化
                    assetDirectory.Length > Path.GetDirectoryName(nearestAssetPath).Length) // 最も深いフォルダにあるアセットを取得する。
                {
                    nearestAssetPath = assetPath;
                }
                else if (assetDirectory.Length == Path.GetDirectoryName(nearestAssetPath).Length)
                {
                    // 同一フォルダに適合ファイルが複数存在する場合、例外を投げる。
                    throw new RogueException($"同一フォルダ内に {filter} に適合するファイルが複数存在します。");
                }
            }
            return nearestAssetPath;
        }

        /// <summary>
        /// Assembly Definition Asset のテキスト内からアセンブリ名を取り出す
        /// </summary>
        private static string GetAssemblyNameOfAssemblyDefinitionAsset(AssemblyDefinitionAsset assemblyDefinitionAsset)
        {
            var head = "\"name\": \"";
            var tail = "\"";
            var match = Regex.Match(assemblyDefinitionAsset.text, $@"{head}[a-z_A-Z]\w*(\.[a-z_A-Z]\w*)*{tail}");
            var assemblyName = match.Value.Substring(head.Length, match.Length - head.Length - tail.Length);
            return assemblyName;
        }

        /// <summary>
        /// 指定の <see cref="Object"/> の所属する <see cref="AssemblyDefinitionAsset"/> を取得する。
        /// <see cref="AssemblyDefinitionReferenceAsset"/> は未対応
        /// </summary>
        public static string GetAssemblyName(string assetPath)
        {
            var asmdefPath = GetNearestPath(assetPath, "t:assemblydefinitionasset");
            if (asmdefPath == null) return null;

            // Assembly Definition Asset のテキスト内からアセンブリ名を取り出す。
            var asmdef = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmdefPath);
            return GetAssemblyNameOfAssemblyDefinitionAsset(asmdef);
        }
    }
}
#endif
