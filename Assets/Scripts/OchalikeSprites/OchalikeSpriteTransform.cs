using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OchalikeSprites
{
    public ref struct OchalikeSpriteTransform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public IDirectionalSpritePoseSource PoseSource { get; set; }
        public SpriteDirection Direction { get; set; }

        /// <summary>
        /// 効果音などのモーションに付随する要素をポーズと同等に取得・制御するためのプロパティ。
        /// より自由度の高い制御がいる場合はタイムラインを使用すべき（ただし、コマ単位の同期には注意が必要）
        /// </summary>
        public string Play { get; set; }

        public static OchalikeSpriteTransform Identity => new OchalikeSpriteTransform(false);

        private OchalikeSpriteTransform(bool flag)
        {
            Position = Vector3.zero;
            Rotation = Quaternion.identity;
            Scale = Vector3.one;
            PoseSource = null;
            Direction = SpriteDirection.Down;
            Play = null;
        }
    }
}
