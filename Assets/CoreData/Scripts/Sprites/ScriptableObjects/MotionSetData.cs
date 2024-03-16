using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SkeletalSprite;

namespace Roguegard
{
    [CreateAssetMenu(menuName = "RoguegardData/Sprite/MotionSet")]
    public class MotionSetData : ScriptableObject, ISpriteMotionSet
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
            IKeyword keyword, int animationTime, SpriteDirection direction, ref SkeletalSpriteTransform transform, out bool endOfMotion)
        {
            if (!TryGetValue(keyword, out var value))
            {
                // キーワードと一致するモーションが存在しない場合、代わりに最初のモーションを使う。
                if (firstValue == null) { firstValue = _items[0].Value; }
                value = firstValue;
            }

            value.ApplyTo(animationTime, direction, ref transform, out endOfMotion);
        }

        [System.Serializable]
        public class Item
        {
            [SerializeField] private KeywordData _key;
            public IKeyword Key => _key;

            [SerializeField] private SpriteMotionData _value;
            public ISpriteMotion Value => _value;
        }
    }
}
