using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore;

namespace Lysionium.Views
{
    // 命名メモ: TMP_SpriteAsset 風に ...Asset で終わる名前

    [CreateAssetMenu(menuName = "Lysionium/Keybind/Keybind Glyph Asset")]
    public class KeybindGlyphAsset : ScriptableObject
    {
        [Tooltip("パッキング先の TMP_SpriteAsset")]
        [SerializeField] private TMP_SpriteAsset _spriteAsset;
        public TMP_SpriteAsset SpriteAsset
        {
            get => _spriteAsset;
            set => _spriteAsset = value;
        }

        [Tooltip("パッキング先のテクスチャ解像度。グリフが入りきらない場合はこの値を上げてください。")]
        [SerializeField] private Vector2Int _spriteSheetSize = new(1024, 1024);

        [Tooltip("パッキング後のグリフ1つあたりの解像度")]
        [SerializeField] private Vector2Int _packedGlyphSize = new(64, 64);

        [Tooltip("modifier の後ろにつけるスプライト（ctrl + C のようにする）")]
        [SerializeField] private Sprite _modifierSeparatorSprite;
        [SerializeField] private int _modifierSeparatorWidth = 32;

        [SerializeField] private KeybindGlyphSource[] _keybindGlyphSources;

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            // 作成直後のみ実行
            var selfPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            var spriteSheet = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(selfPath).FirstOrDefault(a => a is Texture2D) as Texture2D;
            if (spriteSheet != null)
            {
                // エディタでの保存結果が変わらないように初期化する
                spriteSheet.Reinitialize(1, 1, TextureFormat.RGBA32, false);
                if (_spriteAsset != null)
                {
                    _spriteAsset.spriteGlyphTable.Clear();
                    _spriteAsset.spriteCharacterTable.Clear();
                    _spriteAsset.spriteCharacterLookupTable.Clear();
                }
                return;
            }

            // テクスチャをサブアセットに追加
            // キーバインドグリフの SpriteAsset にこのテクスチャを使用することで TextMeshPro の内部処理のエラーを避けやすくなる
            spriteSheet = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            UnityEditor.AssetDatabase.AddObjectToAsset(spriteSheet, this);

            // SpriteAsset が未設定の場合は生成する
            if (_spriteAsset == null)
            {
                // SpriteAsset を保存するフォルダを取得（デフォルトは "./Resources/Sprite Assets"）
                var selfDirectory = Path.GetDirectoryName(selfPath);
                var spriteAssetDirectory = $"{selfDirectory}/Resources/{TMP_Settings.defaultSpriteAssetPath}"[..^1]; // 終端のスラッシュを除く

                // SpriteAsset とマテリアルを保存
                // 同一の SpriteAsset に対して複数種類（白塗り、白抜き等）の KeybindGlyphAsset を使えるように SpriteAsset はサブアセットにしない
                _spriteAsset = CreateInstance<TMP_SpriteAsset>();
                _spriteAsset.name = name;
                _spriteAsset.spriteSheet = spriteSheet;
                _spriteAsset.material = new Material(Shader.Find("TextMeshPro/Sprite")) { mainTexture = spriteSheet };
                CreateFolderRecursive(spriteAssetDirectory);
                UnityEditor.AssetDatabase.CreateAsset(_spriteAsset, $"{spriteAssetDirectory}/{_spriteAsset.name}.asset");
                UnityEditor.AssetDatabase.AddObjectToAsset(_spriteAsset.material, _spriteAsset);

                static void CreateFolderRecursive(string path)
                {
                    if (Directory.Exists(path)) return; // フォルダが存在したら探索終了

                    // フォルダが存在しなかったら親フォルダを探索
                    var parentPath = Path.GetDirectoryName(path);
                    CreateFolderRecursive(parentPath);

                    // 探索終了後、フォルダを生成→子フォルダを生成...のようにフォルダ再帰生成
                    UnityEditor.AssetDatabase.CreateFolder(parentPath, Path.GetFileName(path));
                }
            }
            OnValidate();
#endif
        }

        protected virtual void OnDestroy()
        {
            _spriteAsset.spriteGlyphTable.Clear();
            _spriteAsset.spriteCharacterTable.Clear();
            _spriteAsset.spriteCharacterLookupTable.Clear();
        }

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            // サブアセットのテクスチャ名を更新
            var selfPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            var spriteSheet = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(selfPath).FirstOrDefault(a => a is Texture2D) as Texture2D;
            if (spriteSheet != null)
            {
                spriteSheet.name = _spriteAsset ? _spriteAsset.name : name;
            }

            // スプライトアセットのマテリアル名を更新
            if (_spriteAsset != null && _spriteAsset.material != null)
            {
                _spriteAsset.material.name = $"{_spriteAsset.name} Material";
            }
#endif
        }

        /// <summary>
        /// 指定の <see cref="KeybindStyleSheet"/> でキーバインドグリフを再生成する
        /// </summary>
        public void UpdateGlyph(KeybindStyleSheet keybindStyleSheet)
        {
            // キーバインド別のスプライトを取得する
            var spriteSets = GetSpriteSets(keybindStyleSheet, _keybindGlyphSources, _modifierSeparatorSprite);

            // スプライトアセットのテクスチャサイズを更新
            ResizeSpriteSheet(_spriteAsset, _spriteSheetSize);

            // 指定のスプライトアセットにグリフを書き込む
            WriteTo(_spriteAsset, _packedGlyphSize, spriteSets);

            // 影響があると思われるテキストオブジェクトを更新する
            var texts = keybindStyleSheet.GetComponentsInChildren<TMP_Text>();
            foreach (var text in texts)
            {
                text.SetAllDirty();
            }
        }

        private static List<(List<Sprite> spriteSet, string style)> GetSpriteSets(
            KeybindStyleSheet keybindStyleSheet, IEnumerable<KeybindGlyphSource> keybindGlyphSources, Sprite modifierSeparatorSprite)
        {
            var spriteSets = new List<(List<Sprite> spriteSet, string style)>();
            var addedBindingNames = new HashSet<string>();
            for (int i = 0; i < keybindStyleSheet.Bindings.Count; i++)
            {
                var style = keybindStyleSheet.Bindings[i].Style;
                var action = keybindStyleSheet.Bindings[i].Action;
                var sprites = new List<Sprite>();
                addedBindingNames.Clear();
                foreach (var binding in action.bindings)
                {
                    if (!addedBindingNames.Add(binding.name)) continue; // WASDと矢印キーの両方が設定されている場合、先に設定されている方だけ表示する

                    var inputControl = action.controls.FirstOrDefault(c => InputControlPath.Matches(binding.effectivePath, c));
                    if (inputControl == null) continue;

                    foreach (var keybindGlyphSource in keybindGlyphSources)
                    {
                        if (!keybindGlyphSource.TryGetValue(inputControl, out var sprite)) continue;

                        sprites.Add(sprite);

                        // modifier は後ろに「+」をつける（ctrl + C のようにする）
                        if (modifierSeparatorSprite && binding.name.StartsWith("modifier"))
                        {
                            sprites.Add(modifierSeparatorSprite);
                        }
                        break;
                    }
                }
                spriteSets.Add((sprites, style));
            }

            // スプライトパッキングの効率化のため、スプライト数の降順で並べ替え
            // スプライトが多いものから順にパッキングする
            spriteSets.Sort((a, b) => b.spriteSet.Count.CompareTo(a.spriteSet.Count));

            return spriteSets;
        }

        private static void ResizeSpriteSheet(TMP_SpriteAsset spriteAsset, Vector2Int spriteSheetSize)
        {
            var spriteSheet = (Texture2D)spriteAsset.spriteSheet;
            if (spriteSheet.width == spriteSheetSize.x && spriteSheet.height == spriteSheetSize.y) return;

            spriteSheet.Reinitialize(spriteSheetSize.x, spriteSheetSize.y, TextureFormat.RGBA32, false);
        }

        private void WriteTo(
            TMP_SpriteAsset spriteAsset, Vector2Int packedGlyphSize, List<(List<Sprite> spriteSet, string style)> spriteSets)
        {
            // スプライトパッキング先のテクスチャを取得
            var spriteSheet = (Texture2D)spriteAsset.spriteSheet;
            var spriteSheetSize = new Vector2Int(spriteSheet.width, spriteSheet.height);

            // スプライトアセットを生成
            spriteAsset.spriteGlyphTable.Clear();
            spriteAsset.spriteCharacterTable.Clear();
            using (var packer = new RuntimeSpritePacker(spriteSheetSize, packedGlyphSize, _modifierSeparatorSprite, _modifierSeparatorWidth))
            {
                for (int i = 0; i < spriteSets.Count; i++)
                {
                    // スプライトパッキング
                    var rect = packer.PackSpriteSet(spriteSets[i].spriteSet);
                    rect.y = spriteSheetSize.y - (rect.y + packedGlyphSize.y); // スクリーン座標からテクスチャ座標に変換

                    // SpriteGlyph を生成
                    const float offsetYCoef = 0.75f; // 0だとスプライトの中心がベースラインに乗るため上にずらす
                    const float scale = 1.5f; // 1だと小さく見えるため大きくする
                    var glyphMetrics = new GlyphMetrics(rect.width, rect.height, 0f, rect.height * offsetYCoef, rect.width);
                    var glyphRect = new GlyphRect(rect);
                    var spriteGlyph = new TMP_SpriteGlyph((uint)i, glyphMetrics, glyphRect, scale, i);
                    spriteAsset.spriteGlyphTable.Add(spriteGlyph);

                    // SpriteCharacter を生成
                    var spriteCharacter = new TMP_SpriteCharacter(0, spriteGlyph)
                    {
                        name = spriteSets[i].style // スプライト名はスタイル名と等しい
                    };
                    spriteAsset.spriteCharacterTable.Add(spriteCharacter);
                }
                packer.WriteTo(spriteSheet);
            }
            spriteSheet.Apply(spriteSheet);
            spriteAsset.UpdateLookupTables();
        }
    }
}
