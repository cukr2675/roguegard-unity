using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;
using Roguegard;
using Roguegard.CharacterCreation;

namespace RoguegardUnity.Editor
{
#if UNITY_EDITOR
    [ExecuteAlways]
    [RequireComponent(typeof(RogueCharacterSpriteRenderer))]
    internal class RogueObjSpritePreview : MonoBehaviour
    {
        [SerializeField] private RoguegardSettingsData _settings = null;

        [SerializeField] private int _time = 0;
        [SerializeField] private int _angle = 6;
        [SerializeField] private ScriptableStartingItem _objItem = null;
        [SerializeField] private SpriteMotionData _motion = null;

        private RogueSpriteRendererPool pool;
        private RogueCharacterSpriteRenderer rogueSpriteRenderer;

        private ScriptableCharacterCreationData createdItemOption;
        private RogueObj obj;

        private void Start()
        {
            if (!Application.isPlaying)
            {
                _settings.TestLoad();
                RogueRandom.Primary = new RogueRandom(0);
                MessageWorkListener.ClearListeners();
                StaticID.Next();
            }

            pool = GetComponentInChildren<RogueSpriteRendererPool>();
            rogueSpriteRenderer = GetComponent<RogueCharacterSpriteRenderer>();

            var renderers = transform.GetComponentsInChildren<RogueObjSpriteRenderer>();
            foreach (var renderer in renderers)
            {
                DestroyImmediate(renderer.gameObject);
            }
            rogueSpriteRenderer.Initialize(pool);
        }

        private void Update()
        {
            if (rogueSpriteRenderer == null) return;
            if (_objItem.Option == createdItemOption && obj == null) return;

            if (_objItem.Option != createdItemOption)
            {
                createdItemOption = _objItem.Option;
                obj = _objItem.Option.CreateObj(_objItem, null, Vector2Int.zero, new RogueRandom(0));
                obj.Main.Sprite.Update(obj);
            }

            rogueSpriteRenderer.SetSprite(obj, _motion, _time, _time, new RogueDirection(_angle), out _);
        }
    }
#endif
}
