using OchalikeSprites;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "Roguegard/Sprite/Motion Set")]
    public class SpriteMotionSetAsset : ScriptableObject, ISpriteMotionSet
    {
        [SerializeField] private List<Item> _items = null;

        [System.NonSerialized] private ISpriteMotion firstValue;

        private bool TryGetValue(IKeyword key, out ISpriteMotion value)
        {
            foreach (var item in _items)
            {
                if (item.Key == key)
                {
                    value = item.Value;
                    return true;
                }
            }
            value = null;
            return false;
        }

        void ISpriteMotionSet.GetPose(
            IKeyword keyword, int animationTime, SpriteDirection direction, ref OchalikeSpriteTransform transform, out bool endOfMotion)
        {
            if (!TryGetValue(keyword, out var value))
            {
                // キーワードと一致するモーションが存在しない場合、代わりに最初のモーションを使う。
                firstValue ??= _items[0].Value;
                value = firstValue;
            }

            value.ApplyTo(animationTime, direction, ref transform, out endOfMotion);
        }

        [System.Serializable]
        public class Item
        {
            [SerializeField] private KeywordAsset _key;
            public IKeyword Key => _key;

            [SerializeField] private SpriteMotionAsset _value;
            public ISpriteMotion Value => _value;
        }
    }
}
