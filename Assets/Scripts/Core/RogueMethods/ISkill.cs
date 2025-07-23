namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface ISkill : IActiveRogueMethod, ISkillDescribable, IRogueDescribable, System.IEquatable<ISkill>
    {
    }
}
