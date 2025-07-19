using OchalikeSprites;

namespace Roguegard
{
    public interface IStatusEffect : IRogueDescription
    {
        IKeyword EffectCategory { get; }

        /// <summary>
        /// このステータスエフェクトの発生源
        /// </summary>
        RogueObj Effecter { get; }

        ISpriteMotion HeadIcon { get; }

        float Order { get; }

        void GetEffectedName(RogueNameBuilder refName, RogueObj self);
    }
}
