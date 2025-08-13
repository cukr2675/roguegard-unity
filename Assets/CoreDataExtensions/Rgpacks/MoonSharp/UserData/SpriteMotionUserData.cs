using MoonSharp.Interpreter;
using OchalikeSprites;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    [SuppressMessage("Style", "IDE1006")]
    public class SpriteMotionUserData
    {
        private readonly ISpriteMotion spriteMotion;

        public SpriteMotionUserData(ISpriteMotion spriteMotion)
        {
            this.spriteMotion = spriteMotion;
        }

        public void playAt(int x, int y, RogueDirectionUserData direction)
        {
            RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateEffect(new Vector2Int(x, y), direction.Direction, spriteMotion, true));
        }

        public void playWith(RogueObjUserData obj)
        {
            RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateSpriteMotion(obj.Obj, spriteMotion, true));
        }
    }
}
