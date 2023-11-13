using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CoreMotions : ScriptableLoader
    {
        private static CoreMotions instance;

        [SerializeField] private RotatableBoneMotionData _beThrownDrop;
        public static IBoneMotion BeThrownDrop => instance._beThrownDrop;

        [SerializeField] private RotatableBoneMotionData _beThrownFlying;
        public static IBoneMotion BeThrownFlying => instance._beThrownFlying;

        [SerializeField] private RotatableBoneMotionData _bomb;
        public static IBoneMotion Bomb => instance._bomb;

        [SerializeField] private RotatableBoneMotionData _buff;
        public static IBoneMotion Buff => instance._buff;

        [SerializeField] private RotatableBoneMotionData _confusion;
        public static IBoneMotion Confusion => instance._confusion;

        [SerializeField] private RotatableBoneMotionData _debuff;
        public static IBoneMotion Debuff => instance._debuff;

        [SerializeField] private RotatableBoneMotionData _discus;
        public static IBoneMotion Discus => instance._discus;

        [SerializeField] private RotatableBoneMotionData _eat;
        public static IBoneMotion Eat => instance._eat;

        [SerializeField] private RotatableBoneMotionData _fullTurn;
        public static IBoneMotion FullTurn => instance._fullTurn;

        [SerializeField] private RotatableBoneMotionData _heal;
        public static IBoneMotion Heal => instance._heal;

        [SerializeField] private RotatableBoneMotionData _paralysis;
        public static IBoneMotion Paralysis => instance._paralysis;

        [SerializeField] private Rotatable1To8BoneMotionData _powerSlash;
        public static IBoneMotion PowerSlash => instance._powerSlash;

        [SerializeField] private RotatableBoneMotionData _sleep;
        public static IBoneMotion Sleep => instance._sleep;

        [SerializeField] private RotatableBoneMotionData _smoke;
        public static IBoneMotion Smoke => instance._smoke;

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
