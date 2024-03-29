using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    /// <summary>
    /// <see cref="RogueObj"/> の外見のスプライト。
    /// </summary>
    public interface IRogueObjSprite : IRogueSprite
    {
        // これらのメソッドは RogueObj の MotionSet や RogueEffect ありきで呼び出すことを想定するため、
        // MainSpriteInfo 内でのみ使用する。

        void SetBoneSpriteEffects(RogueObj self, Spanning<IBoneSpriteEffect> effects);

        void SetTo(ISkeletalSpriteRenderController renderController, SpritePose pose, SpriteDirection direction);
    }
}
