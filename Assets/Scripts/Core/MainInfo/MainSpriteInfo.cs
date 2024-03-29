using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    public class MainSpriteInfo
    {
        private IRogueObjSprite objSprite;

        public ISpriteMotionSet MotionSet { get; private set; }

        private bool isDirty;

        // 外部との共有による AddEquipment 等の複数呼び出しを防ぐため型そのままの取得は隠す。
        public IRogueSprite Sprite => objSprite;

        public void Update(RogueObj self)
        {
            if (isDirty)
            {
                self.Main.InfoSet.GetObjSprite(self, out objSprite, out var motionSet);
                MotionSet = motionSet;
                isDirty = false;
                var boneSpriteState = self.Main.GetBoneSpriteEffectState(self);
                boneSpriteState.SetTo(self, objSprite, true);
            }
            else
            {
                var boneSpriteState = self.Main.GetBoneSpriteEffectState(self);
                boneSpriteState.SetTo(self, objSprite, false);
            }

            var spriteMotionState = self.Main.GetSpriteMotionEffectState(self);
            spriteMotionState.Update();
        }

        public void SetTo(ISkeletalSpriteRenderController renderController, SpritePose pose, SpriteDirection direction)
        {
            objSprite.SetTo(renderController, pose, direction);
        }

        public void SetDirty()
        {
            isDirty = true;
        }
    }
}
