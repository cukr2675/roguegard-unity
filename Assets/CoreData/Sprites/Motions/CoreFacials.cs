using System.Collections;
using UnityEngine;

namespace Roguegard
{
    public class CoreFacials : ScriptableLoader
    {
        private static CoreFacials instance;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _coldLook;
        public static OchalikeSprites.ISpriteMotion ColdLook => instance._coldLook;

        [SerializeField] private OchalikeSprites.RotatableSpriteMotionAsset _smile;
        public static OchalikeSprites.ISpriteMotion Smile => instance._smile;

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
