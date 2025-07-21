using OchalikeSprites;
using RuntimeDotter;
using UnityEngine;

namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IPaintBoneSprite
    {
        void AddTo(OchalikeMorph ochalikeMorph, Color mainColor, Spanning<ShiftableColor> palette);

        Sprite GetIcon(Spanning<ShiftableColor> palette);

        IPaintBoneSprite Clone();
    }
}
