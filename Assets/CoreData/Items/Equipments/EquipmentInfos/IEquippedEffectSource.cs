namespace Roguegard.CharacterCreation
{
    public interface IEquippedEffectSource
    {
        IEquippedEffect CreateOrReuse(RogueObj equipment, IEquippedEffect effect);
    }
}
