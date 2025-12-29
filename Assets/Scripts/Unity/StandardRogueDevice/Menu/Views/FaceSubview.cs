using Lysionium;
using Lysionium.Views;
using OchalikeSprites;
using Roguegard;
using Roguegard.Device;
using System.Collections.Generic;
using UnityEngine;

namespace RoguegardUnity
{
    public class FaceSubview : Subview
    {
        [SerializeField] private RectTransform _content = null;

        private MenuRogueObjSpriteRenderer spriteRenderer;

        public void Initialize(RogueSpriteRendererPool rendererPool)
        {
            spriteRenderer = rendererPool.GetMenuRogueSpriteRenderer(_content);
            var spriteRendererTransform = spriteRenderer.GetComponent<RectTransform>();
            spriteRendererTransform.anchorMin = spriteRendererTransform.anchorMax = new Vector2(.5f, 0f);
            spriteRendererTransform.sizeDelta = Vector2.zero;
            spriteRendererTransform.localPosition = Vector3.zero;
            spriteRendererTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -128f, 0f);
            spriteRendererTransform.localScale = Vector3.one * 8f;
        }

        public override void SetListHandler(
            IReadOnlyList<object> list, IViewItemHandler handler, IListuiManager manager, IListuiArg arg,
            ref ISubviewStateProvider stateProvider)
        {
            var mArg = (MArg)arg;
            var obj = mArg.Arg.TargetObj;
            var facial = (ISpriteMotion)mArg.Arg.Other;
            obj.Main.Sprite.Update(obj);
            var spriteTransform = OchalikeSpriteTransform.Identity;
            if (facial != null)
            {
                facial.ApplyTo(0, RogueDirection.Right, ref spriteTransform, out _);
            }
            else
            {
                KeywordSpriteMotion.Wait.ApplyTo(obj.Main.Sprite.MotionSet, 0, RogueDirection.Right, ref spriteTransform, out _);
            }
            obj.Main.Sprite.SetTo(spriteRenderer, spriteTransform.PoseSource.GetSpritePose(spriteTransform.Direction), spriteTransform.Direction);
        }
    }
}
