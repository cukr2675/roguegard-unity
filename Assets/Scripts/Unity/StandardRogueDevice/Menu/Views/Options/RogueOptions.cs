using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;

namespace RoguegardUnity
{
    [ObjectFormer.Formable]
    public class RogueOptions
    {
        public float MasterVolume { get; private set; }
        public int WindowFrameIndex { get; private set; }
        private Color32 _windowFrameColor;
        public Color WindowFrameColor => _windowFrameColor;

        [System.NonSerialized] private MenuController menuController;
        [System.NonSerialized] private AudioMixer audioMixer;

        public void Initialize(MenuController menuController, AudioMixer audioMixer)
        {
            this.menuController = menuController;
            this.audioMixer = audioMixer;
        }

        public void Set(RogueOptions options)
        {
            SetMasterVolume(options.MasterVolume);
            SetWindowFrame(options.WindowFrameIndex, options.WindowFrameColor);
        }

        /// <summary>
        /// 設定を初期化する。このインスタンスの値を初期化するのみで、 Unity コンポーネントには何も反映されない。
        /// </summary>
        public void ClearWithoutSet()
        {
            MasterVolume = .5f;
            WindowFrameIndex = 0;
            _windowFrameColor = ColorPreset.GetColor(0);
        }

        public void SetMasterVolume(float value)
        {
            MasterVolume = value;
            var volume = Mathf.Clamp(Mathf.Log10(value) * 20f, -80f, 0f);
            audioMixer.SetFloat("Master", volume);
        }

        public void SetWindowFrame(int index, Color color)
        {
            WindowFrameIndex = index;
            _windowFrameColor = color;
            WindowFrameList.GetWindowFrame(index, out var spriteA, out var spriteB);
            menuController.SetWindowFrame(spriteA, spriteB, color);
        }
    }
}
