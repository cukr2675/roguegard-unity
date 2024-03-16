using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    /// <summary>
    /// <see cref="RogueObj"/> の外見のスプライト。
    /// </summary>
    public abstract class RogueObjSprite : RogueSprite
    {
        // これらのメソッドは RogueObj の MotionSet や RogueEffect ありきで呼び出すことを想定するため、
        // MainSpriteInfo 内でのみ使用する。

        public abstract void SetBoneSpriteEffects(RogueObj self, Spanning<IBoneSpriteEffect> effects);

        public abstract void SetTo(ISkeletalSpriteRenderController renderController, SpritePose pose, SpriteDirection direction);
    }
}
