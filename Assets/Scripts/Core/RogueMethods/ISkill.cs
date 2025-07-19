namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface ISkill : IActiveRogueMethod, ISkillDescription, IRogueDescription, System.IEquatable<ISkill>
    {
    }
}
