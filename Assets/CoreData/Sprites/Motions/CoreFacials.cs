using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CoreFacials : ScriptableLoader
    {
        private static CoreFacials instance;

        [SerializeField] private SDSSprite.RotatableSpriteMotionData _coldLook;
        public static SDSSprite.ISpriteMotion ColdLook => instance._coldLook;

        [SerializeField] private SDSSprite.RotatableSpriteMotionData _smile;
        public static SDSSprite.ISpriteMotion Smile => instance._smile;

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
