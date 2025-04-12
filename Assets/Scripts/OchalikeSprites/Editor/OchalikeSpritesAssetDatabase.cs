using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace OchalikeSprites.Editor
{
    public static class OchalikeSpritesAssetDatabase
    {
        public static Sprite GetSprite(string name, string[] searchInFolders)
        {
            return (Sprite)AssetDatabase.FindAssets($"{name} t:{nameof(Sprite)}", searchInFolders)
                .Distinct() // AssetDatabase.FindAssets は Sprite ではなく Texture2D を取得するため重複する。 Distinct で重複をなくす。
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .SelectMany(path => AssetDatabase.LoadAllAssetRepresentationsAtPath(path))
                .First(sprite => sprite.name.Contains(name)); // 見つからなければ例外を投げる
        }

        private static IEnumerable<Sprite> GetSprites(string format, IEnumerable<object> args, string[] searchInFolders)
        {
            var searchName = string.Format(format, args.Append("").ToArray());
            var regexName = string.Format(format, args.Append("(_(NF|NR|BF|BR)+)?").ToArray()) + '$';
            var sprites = AssetDatabase.FindAssets($"{searchName} t:sprite", searchInFolders)
                .Distinct() // AssetDatabase.FindAssets は Sprite ではなく Texture2D を取得するため重複する。 Distinct で重複をなくす。
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .SelectMany(path => AssetDatabase.LoadAllAssetRepresentationsAtPath(path))
                .Where(sprite => Regex.IsMatch(sprite.name, regexName))
                .OfType<Sprite>();
            return sprites;
        }

        public static ColorRangedBoneSprite CreateColorRangedBoneSpriteOrNull(bool colorRanged, string format, string[] searchInFolders, params object[] args)
        {
            if (colorRanged)
            {
                var lightSprites = GetSprites(format, args.Append(0), searchInFolders);
                var darkSprites = GetSprites(format, args.Append(1), searchInFolders);
                if (lightSprites.Any() && darkSprites.Any())
                {
                    return new ColorRangedBoneSprite(CreateBoneSprite(lightSprites), CreateBoneSprite(darkSprites));
                }
            }
            else
            {
                var sprites = GetSprites(format, args, searchInFolders);
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
    }
}
