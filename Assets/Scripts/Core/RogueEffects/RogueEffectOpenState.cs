namespace Roguegard
{
    public enum RogueEffectOpenState
    {
        NotStarted,
        OpeningInfoSet, // EquipmentState は InfoSet 依存なため OpeningInfoSet を先にする。
        OpeningEffects,
        Finished
    }
}
