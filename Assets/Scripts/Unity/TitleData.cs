using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Audio;
using Roguegard;
using Roguegard.CharacterCreation;

namespace RoguegardUnity
{
    [CreateAssetMenu(menuName = "RoguegardData/TitleData")]
    public class TitleData : ScriptableObject
    {
        [SerializeField] private Sprite _progressCircle = null;
        [SerializeField] private RoguegardSettingsData _settings = null;
        [SerializeField] private Sprite _titleLogo = null;
        [SerializeField] private TitleMenu _menuPrefab = null;
        [SerializeField] private MenuController _menuControllerPrefab = null;
        [Space]
        [SerializeField] private PresetCreationData[] _presets = null;
        [Space]
        [SerializeField] private RogueSpriteRendererPool _spriteRendererPoolPrefab = null;
        [SerializeField] private RogueTilemapRenderer _tilemapRendererPrefab = null;
        [SerializeField] private TouchController _touchControllerPrefab = null;
        [SerializeField] private SoundTable _soundTable = null;
        [SerializeField] private AudioMixer _audioMixer = null;
        [SerializeField] private AudioSource _seAudioSourcePrefab = null;
        [SerializeField] private AudioSource _bgmAudioSourcePrefab = null;

        public Sprite ProgressCircle => _progressCircle;
        public RoguegardSettingsData Settings => _settings;

        public void InstantiateTitleMenu()
        {
            var characterCreationDatabase = new CharacterCreationDatabase();
            foreach (var preset in _presets)
            {
                characterCreationDatabase.AddPreset(preset);
            }

            var assetTable = RoguegardSettings.GetAssetTable("Core");
            foreach (var asset in assetTable.Values)
            {
                if (asset is IAppearanceOption appearanceOption)
                {
                    characterCreationDatabase.AddAppearanceOption(appearanceOption);
                }
                else if (asset is IIntrinsicOption intrinsicOption)
                {
                    characterCreationDatabase.AddIntrinsicOption(intrinsicOption);
                }
            }

            var menu = Instantiate(_menuPrefab);
            var spriteRendererPool = Instantiate(_spriteRendererPoolPrefab);
            menu.Initialize(
                spriteRendererPool, _menuControllerPrefab, _tilemapRendererPrefab, _touchControllerPrefab,
                _soundTable, _audioMixer, _seAudioSourcePrefab, _bgmAudioSourcePrefab, characterCreationDatabase, _titleLogo);
        }
    }
}
