using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SDSSprite;

namespace Roguegard
{
    public readonly struct RogueCharacterWork
    {
        public RogueObj Obj { get; }
        public Vector2Int Position { get; }
        public float WalkSpeed { get; }
        public RogueDirection Direction { get; }
        public ISpriteMotion SpriteMotion { get; }
        public PopSignType PopSign { get; }
        public int PopupValue { get; }
        public Color PopupColor { get; }
        public bool PopCritical { get; }
        public bool Continues { get; }

        public static RogueCharacterWork Identity { get; } = default;

        public RogueCharacterWork(
            RogueObj obj, Vector2Int position, float walkSpeed, RogueDirection direction, ISpriteMotion spriteMotion,
            PopSignType popSign, int popupValue, Color popupColor, bool popCritical, bool continues)
        {
            Obj = obj;
            Position = position;
            WalkSpeed = walkSpeed;
            Direction = direction;
            SpriteMotion = spriteMotion;
            PopSign = popSign;
            PopupValue = popupValue;
            PopupColor = popupColor;
            PopCritical = popCritical;
            Continues = continues;
        }

        /// <summary>
        /// 一連のモーションを同期再生することを知らせる <see cref="RogueCharacterWork"/> を生成する。
        /// </summary>
        public static RogueCharacterWork CreateSync(RogueObj obj)
        {
            return new RogueCharacterWork(obj, default, 0f, obj.Main.Stats.Direction, null, PopSignType.Clear, 0, default, false, false);
        }

        /// <summary>
        /// 一連のモーションを同期再生することを知らせる <see cref="RogueCharacterWork"/> を生成する。
        /// 移動を含む <see cref="IRogueMethod"/> の開始時に必ず使用する。
        /// </summary>
        public static RogueCharacterWork CreateSyncPositioning(RogueObj obj)
        {
            return new RogueCharacterWork(obj, obj.Position, Mathf.Infinity, obj.Main.Stats.Direction, null, PopSignType.Clear, 0, default, false, false);
        }

        public static RogueCharacterWork CreateWalk(
            RogueObj obj, Vector2Int position, RogueDirection direction, ISpriteMotion spriteMotion, bool continues)
        {
            var walkSpeed = -1f / 4f;
            return new RogueCharacterWork(obj, position, walkSpeed, direction, spriteMotion, PopSignType.Clear, 0, default, false, continues);
        }

        public static RogueCharacterWork CreateWalk(
            RogueObj obj, Vector2Int position, float walkSpeed, RogueDirection direction, ISpriteMotion spriteMotion, bool continues)
        {
            return new RogueCharacterWork(obj, position, walkSpeed, direction, spriteMotion, PopSignType.Clear, 0, default, false, continues);
        }

        public static RogueCharacterWork CreateSpriteMotion(RogueObj obj, ISpriteMotion spriteMotion, bool continues)
        {
            return new RogueCharacterWork(obj, obj.Position, 0f, obj.Main.Stats.Direction, spriteMotion, PopSignType.Clear, 0, default, false, continues);
        }

        public static RogueCharacterWork CreatePopupNumber(RogueObj obj, PopSignType sign, int value, bool critical, bool continues)
        {
            return new RogueCharacterWork(obj, obj.Position, 0f, obj.Main.Stats.Direction, null, sign, value, Color.white, critical, continues);
        }

        public static RogueCharacterWork CreateEffect(Vector2Int position, ISpriteMotion spriteMotion, bool continues)
        {
            return new RogueCharacterWork(null, position, Mathf.Infinity, RogueDirection.Down, spriteMotion, PopSignType.Clear, 0, default, false, continues);
        }

        public static RogueCharacterWork CreateEffect(Vector2Int position, RogueDirection direction, ISpriteMotion spriteMotion, bool continues)
        {
            return new RogueCharacterWork(null, position, Mathf.Infinity, direction, spriteMotion, PopSignType.Clear, 0, default, false, continues);
        }

        public enum PopSignType
        {
            Clear,
            None,
            Plus,
            Minus
        }
    }
}
