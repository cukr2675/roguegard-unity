using System.Collections;
using UnityEngine;

namespace Roguegard
{
    public class CoreMotions : ScriptableLoader
    {
        private static CoreMotions instance;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _beThrownDrop;
        public static OchalikeSprites.ISpriteMotion BeThrownDrop => instance._beThrownDrop;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _beThrownFlying;
        public static OchalikeSprites.ISpriteMotion BeThrownFlying => instance._beThrownFlying;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _bomb;
        public static OchalikeSprites.ISpriteMotion Bomb => instance._bomb;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _buff;
        public static OchalikeSprites.ISpriteMotion Buff => instance._buff;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _confusion;
        public static OchalikeSprites.ISpriteMotion Confusion => instance._confusion;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _debuff;
        public static OchalikeSprites.ISpriteMotion Debuff => instance._debuff;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _discus;
        public static OchalikeSprites.ISpriteMotion Discus => instance._discus;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _eat;
        public static OchalikeSprites.ISpriteMotion Eat => instance._eat;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _fullTurn;
        public static OchalikeSprites.ISpriteMotion FullTurn => instance._fullTurn;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _heal;
        public static OchalikeSprites.ISpriteMotion Heal => instance._heal;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _paralysis;
        public static OchalikeSprites.ISpriteMotion Paralysis => instance._paralysis;

        [SerializeField] private OchalikeSprites.Rotatable1To8SpriteMotionAsset _powerSlash;
        public static OchalikeSprites.ISpriteMotion PowerSlash => instance._powerSlash;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _sleep;
        public static OchalikeSprites.ISpriteMotion Sleep => instance._sleep;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _smoke;
        public static OchalikeSprites.ISpriteMotion Smoke => instance._smoke;

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
            throw new System.InvalidOperationException("This method is Editor Only.");
#endif
        }
    }
}
