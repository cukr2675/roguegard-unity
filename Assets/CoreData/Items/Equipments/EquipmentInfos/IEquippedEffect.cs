using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public interface IEquippedEffect
    {
        /// <summary>
        /// 乗り物（騎手が親オブジェクトとは限らない）にも対応するため、 <paramref name="owner"/> の位置は不定
        /// </summary>
        void AddEffect(RogueObj equipment, RogueObj owner);

        /// <summary>
        /// 乗り物（騎手が親オブジェクトとは限らない）にも対応するため、 <paramref name="owner"/> の位置は不定
        /// </summary>
        void RemoveEffect(RogueObj equipment, RogueObj owner);
    }
}
