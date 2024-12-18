using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;

namespace RoguegardUnity
{
    [CreateAssetMenu(menuName = "RoguegardData/TitleData")]
    public class TitleData : ScriptableObject
    {
        [SerializeField] private Sprite _progressCircle = null;
        [SerializeField] private RoguegardSettingsData _settings = null;
        [SerializeField] private TitleMenu _menuPrefab = null;

        [Header("Standard Rogue Device Components")]
        [SerializeField] private RogueSpriteRendererPool _spriteRendererPoolPrefab = null;
        [SerializeField] private RogueTilemapRenderer _tilemapRendererPrefab = null;
        [SerializeField] private TouchController _touchControllerPrefab = null;
        [SerializeField] private SoundTable _soundTable = null;
        [SerializeField] private AudioMixer _audioMixer = null;
        [SerializeField] private AudioSource _seAudioSourcePrefab = null;
        [SerializeField] private AudioSource _bgmAudioSourcePrefab = null;
        [SerializeField] private StandardRogueDeviceInspector _runtimeInspectorPrefab = null;

        public Sprite ProgressCircle => _progressCircle;
        public RoguegardSettingsData Settings => _settings;

        public void ShowTitleMenu()
        {
            var titleOption = new RogueOptions();
            titleOption.Initialize(null, _audioMixer);
            titleOption.ClearWithoutSet();
            titleOption.Set(titleOption);

            var spriteRendererPool = Instantiate(_spriteRendererPoolPrefab);
            DontDestroyOnLoad(spriteRendererPool.gameObject);

            var menu = Instantiate(_menuPrefab);
            menu.Show(
                spriteRendererPool, _tilemapRendererPrefab, _touchControllerPrefab,
                _soundTable, _audioMixer, _seAudioSourcePrefab, _bgmAudioSourcePrefab, _runtimeInspectorPrefab);
        }
    }
}
