using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    public class CoreMotions : ScriptableLoader
    {
        private static CoreMotions instance;

        [SerializeField] private RotatableSpriteMotionData _beThrownDrop;
        public static ISpriteMotion BeThrownDrop => instance._beThrownDrop;

        [SerializeField] private RotatableSpriteMotionData _beThrownFlying;
        public static ISpriteMotion BeThrownFlying => instance._beThrownFlying;

        [SerializeField] private RotatableSpriteMotionData _bomb;
        public static ISpriteMotion Bomb => instance._bomb;

        [SerializeField] private RotatableSpriteMotionData _buff;
        public static ISpriteMotion Buff => instance._buff;

        [SerializeField] private RotatableSpriteMotionData _confusion;
        public static ISpriteMotion Confusion => instance._confusion;

        [SerializeField] private RotatableSpriteMotionData _debuff;
        public static ISpriteMotion Debuff => instance._debuff;

        [SerializeField] private RotatableSpriteMotionData _discus;
        public static ISpriteMotion Discus => instance._discus;

        [SerializeField] private RotatableSpriteMotionData _eat;
        public static ISpriteMotion Eat => instance._eat;

        [SerializeField] private RotatableSpriteMotionData _fullTurn;
        public static ISpriteMotion FullTurn => instance._fullTurn;

        [SerializeField] private RotatableSpriteMotionData _heal;
        public static ISpriteMotion Heal => instance._heal;

        [SerializeField] private RotatableSpriteMotionData _paralysis;
        public static ISpriteMotion Paralysis => instance._paralysis;

        [SerializeField] private Rotatable1To8SpriteMotionData _powerSlash;
        public static ISpriteMotion PowerSlash => instance._powerSlash;

        [SerializeField] private RotatableSpriteMotionData _sleep;
        public static ISpriteMotion Sleep => instance._sleep;

        [SerializeField] private RotatableSpriteMotionData _smoke;
        public static ISpriteMotion Smoke => instance._smoke;

        public override IEnumerator LoadAsync()
        {
            instance = this;
            yield break;
        }

        public override void TestLoad()
        {
#if UNITY_EDITOR
            instance = this;
#else
            throw new RogueException("This method is Editor Only.");
#endif
        }
    }
}
