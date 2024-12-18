using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;
using SDSSprite;
using Roguegard;

namespace RoguegardUnity
{
    public class RogueCharacterSpriteRenderer : MonoBehaviour
    {
        [SerializeField] private SortingGroup _sortingGroup = null;
        [SerializeField] private SpriteRenderer _shadowRenderer = null;

        private RogueObjSpriteRenderer mainRenderer;
        private RogueObjSpriteRenderer statusEffectIconRenderer;

        private const int headIconCount = 60;

        private static ColoredRogueSprite effectSprite;

        public void Initialize(RogueSpriteRendererPool pool)
        {
            gameObject.SetActive(true);

            var mainRendererSiblingIndex = _shadowRenderer.transform.GetSiblingIndex();

            mainRenderer = pool.GetRogueSpriteRenderer(transform);
            mainRenderer.transform.SetSiblingIndex(mainRendererSiblingIndex + 1);

            statusEffectIconRenderer = pool.GetRogueSpriteRenderer(transform);
            statusEffectIconRenderer.transform.SetSiblingIndex(mainRendererSiblingIndex + 2);
        }

        public void SetSprite(
            RogueObj obj, ISpriteMotion spriteMotion, int spriteMotionAnimationTime, int motionEffectAnimationTime, RogueDirection direction,
            out bool endOfMotion)
        {
            // 下のキャラを手前に表示させる。
            _sortingGroup.sortingOrder = -obj.Position.y;
            if (obj.HasCollider) _sortingGroup.sortingOrder += obj.Location?.Space.Tilemap?.Height ?? 0;

            endOfMotion = SetTo(obj, spriteMotion, spriteMotionAnimationTime, direction, motionEffectAnimationTime);

            // 影の設定
            _shadowRenderer.enabled = obj.HasCollider;

            // ステータスエフェクトの頭上アイコンの設定
            var statusEffectState = obj.Main.GetStatusEffectState(obj);
            var statusEffects = statusEffectState.StatusEffectsWithHeadIcon;
            if (statusEffects.Count >= 1)
            {
                var headIconIndex = motionEffectAnimationTime % headIconCount % statusEffects.Count;
                var iconSpriteMotion = statusEffects[headIconIndex].HeadIcon;
                SetTo(statusEffectIconRenderer, iconSpriteMotion, motionEffectAnimationTime, RogueDirection.Down);
            }
            else
            {
                // 頭上アイコンを消す
                statusEffectIconRenderer.AdjustBones(0);
            }
        }

        private bool SetTo(RogueObj obj, ISpriteMotion spriteMotion, int spriteMotionAnimationTime, RogueDirection direction, int motionEffectAnimationTime)
        {
            var motionSet = obj.Main.Sprite.MotionSet;
            var transform = SkeletalSpriteTransform.Identity;
            IKeyword keyword = null;
            bool endOfMotion;
            if (spriteMotion is IRogueSpriteMotion rogueSpriteMotion)
            {
                rogueSpriteMotion.ApplyTo(motionSet, spriteMotionAnimationTime, direction, ref transform, out endOfMotion);
                keyword = rogueSpriteMotion.Keyword;
            }
            else
            {
                spriteMotion.ApplyTo(spriteMotionAnimationTime, direction, ref transform, out endOfMotion);
            }

            var motionEffectState = obj.Main.GetSpriteMotionEffectState(obj);
            motionEffectState.ApplyTo(motionSet, keyword, motionEffectAnimationTime, direction, ref transform);

            var x = Mathf.Round(transform.Position.x * RoguegardSettings.PixelsPerUnit) / RoguegardSettings.PixelsPerUnit;
            var y = Mathf.Round(transform.Position.y * RoguegardSettings.PixelsPerUnit) / RoguegardSettings.PixelsPerUnit;
            transform.Position = new Vector3(x, y);

            if (transform.PoseSource == null) { transform.PoseSource = DefaultSpriteMotionPoseSource.Instance; }
            var pose = transform.PoseSource.GetSpritePose(transform.Direction);
            obj.Main.Sprite.SetTo(mainRenderer, pose, direction);
            var rendererTransform = mainRenderer.transform;
            rendererTransform.localPosition = transform.Position;
            rendererTransform.localRotation = transform.Rotation;
            rendererTransform.localScale = transform.Scale;

            //IRogueSprite statusEffectIcon = null;
            //if (statusEffectIcon != null)
            //{
            //    statusEffectIcon.GetTileSprite(0, out var statusEffectSprite, out var statusEffectColor);
            //    statusEffectIconRenderer.sprite = statusEffectSprite;
            //    statusEffectIconRenderer.color = statusEffectColor;
            //}

            return endOfMotion;
        }

        public void SetEffectSprite(
            Vector3 position, ISpriteMotion spriteMotion, int motionEffectAnimationTime, RogueDirection direction,
            out bool endOfMotion)
        {
            // 下のキャラを手前に表示させる。
            _sortingGroup.sortingOrder = -Mathf.FloorToInt(position.y) + 1000;

            // 影の設定
            _shadowRenderer.enabled = false;

            endOfMotion = SetTo(mainRenderer, spriteMotion, motionEffectAnimationTime, direction);
        }

        private static bool SetTo(RogueObjSpriteRenderer renderer, ISpriteMotion spriteMotion, int motionEffectAnimationTime, RogueDirection direction)
        {
            effectSprite ??= ColoredRogueSprite.Create(null, RoguegardSettings.BoneSpriteBaseColor);

            var transform = SkeletalSpriteTransform.Identity;
            spriteMotion.ApplyTo(motionEffectAnimationTime, direction, ref transform, out var endOfMotion);

            var x = Mathf.Round(transform.Position.x * RoguegardSettings.PixelsPerUnit) / RoguegardSettings.PixelsPerUnit;
            var y = Mathf.Round(transform.Position.y * RoguegardSettings.PixelsPerUnit) / RoguegardSettings.PixelsPerUnit;
            transform.Position = new Vector3(x, y);

            if (transform.PoseSource == null) { transform.PoseSource = DefaultSpriteMotionPoseSource.Instance; }
            var pose = transform.PoseSource.GetSpritePose(transform.Direction);
            effectSprite.SetTo(renderer, pose, direction);
            var rendererTransform = renderer.transform;
            rendererTransform.localPosition = transform.Position;
            rendererTransform.localRotation = transform.Rotation;
            rendererTransform.localScale = transform.Scale;

            return endOfMotion;
        }

        public void Destroy()
        {
            mainRenderer.BePooledParent();
            mainRenderer = null;
            statusEffectIconRenderer.BePooledParent();
            statusEffectIconRenderer = null;
            gameObject.SetActive(false);
        }
    }
}
