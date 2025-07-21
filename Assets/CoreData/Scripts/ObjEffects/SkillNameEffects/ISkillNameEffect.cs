namespace Roguegard
{
    public interface ISkillNameEffect
    {
        float Order { get; }

        void GetEffectedName(RogueNameBuilder refName, ISkill skill);
    }
}
