using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.Text;
using UnityEditor;

namespace ListingMF.Editor
{
    [CreateAssetMenu(menuName = "ListingMenuFoundation/CharacterFileGenerator")]
    public class CharacterFileGenerator : ScriptableObject
    {
        [Header("ASCII")]
        [SerializeField] private bool _includeASCII = true;

        [Header("Shift_JIS")]
        [Tooltip("01区～02区：全角記号")]
        [SerializeField] private bool _include2ByteSpecialCharacter = true;
        [Tooltip("03区：全角英数")]
        [SerializeField] private bool _include2ByteDigitsAndRoman = true;
        [Tooltip("04区：ひらがな")]
        [SerializeField] private bool _includeHiragana = true;
        [Tooltip("05区：カタカナ")]
        [SerializeField] private bool _includeKatakana = true;
        [Tooltip("06区：ギリシャ文字")]
        [SerializeField] private bool _includeGreek = true;
        [Tooltip("07区：キリル文字")]
        [SerializeField] private bool _includeCyrillic = true;
        [Tooltip("08区：罫線")]
        [SerializeField] private bool _includeBoxDrawing = true;
        [Tooltip("13区：NEC特殊文字")]
        [SerializeField] private bool _includeNECSpecialCharacter = true;
        [Tooltip("16区～47区：JIS第１水準漢字")]
        [SerializeField] private bool _incluldeJISLevel1Kanji = true;
        [Tooltip("48区～84区：JIS第２水準漢字")]
        [SerializeField] private bool _incluldeJISLevel2Kanji = true;

        [Header("Manual")]
        [SerializeField, TextArea(3, 6)] private string _otherCharacters = "♥";

        private void Generate()
        {
            var thisPath = AssetDatabase.GetAssetPath(this);
            var thisDirectory = Path.GetDirectoryName(thisPath);
            var targetPath = $@"{thisDirectory}\{name}.txt";
            if (targetPath == thisPath) throw new System.InvalidOperationException($"{nameof(Generate)} でアセット自体を上書きしようとしました。");

            using (var writer = new StreamWriter(targetPath))
            {
                // ASCII
                if (_includeASCII) { for (byte i = 0x20; i <= 0x7e; i++) writer.Write((char)i); }

                // Shift_JIS
                if (_include2ByteSpecialCharacter) { WriteShiftJIS(writer, 1, 2); }
                if (_include2ByteDigitsAndRoman) { WriteShiftJIS(writer, 3); }
                if (_includeHiragana) { WriteShiftJIS(writer, 4); }
                if (_includeKatakana) { WriteShiftJIS(writer, 5); }
                if (_includeGreek) { WriteShiftJIS(writer, 6); }
                if (_includeCyrillic) { WriteShiftJIS(writer, 7); }
                if (_includeBoxDrawing) { WriteShiftJIS(writer, 8); }
                if (_includeNECSpecialCharacter) { WriteShiftJIS(writer, 13); }
                if (_incluldeJISLevel1Kanji) { for (int i = 16; i <= 47; i++) WriteShiftJIS(writer, i); }
                if (_incluldeJISLevel2Kanji) { for (int i = 48; i <= 84; i++) WriteShiftJIS(writer, i); }

                // Others
                writer.Write(_otherCharacters);
            }
            AssetDatabase.ImportAsset(targetPath);
            AssetDatabase.Refresh();
        }

        private static void WriteShiftJIS(StreamWriter writer, params int[] rowNumbers)
        {
            foreach (var rowNumber in rowNumbers)
            {
                WriteShiftJIS(writer, rowNumber);
            }
        }

        private static void WriteShiftJIS(StreamWriter writer, int rowNumber)
        {
            var encoding = Encoding.GetEncoding("Shift_JIS");
            var bytes = new byte[2];
            if (rowNumber < 63)
            {
                bytes[0] = (byte)(0x81 + (rowNumber - 1) / 2);
            }
            else // 0xa0 ~ 0xdf は含まない
            {
                bytes[0] = (byte)(0xe0 + (rowNumber - 63) / 2);
            }

            byte start1, end1;
            if (rowNumber % 2 == 1)
            {
                // 奇数区は 0x40 ~ 0x9e
                start1 = 0x40;
                end1 = 0x9e;
            }
            else
            {
                // 偶数区は 0x9f ~ 0xfc
                start1 = 0x9f;
                end1 = 0xfc;
            }

            for (byte i = start1; i <= end1; i++)
            {
                if (i == 0x7f) continue; // 0x7f は制御コード

                bytes[1] = i;
                var str = encoding.GetString(bytes);
                if (str.Length != 1 || str == "?") continue; // 印字範囲外は除外

                writer.Write(str);
            }
        }

        [CustomEditor(typeof(CharacterFileGenerator))]
        [CanEditMultipleObjects]
        private class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (GUILayout.Button("Generate"))
                {
                    foreach (var target in targets)
                    {
                        ((CharacterFileGenerator)target).Generate();
                    }
                }
            }
        }
    }
}
