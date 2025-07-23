namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IRogueTileInfo : IRogueTile, IRogueDescribable
    {
        IKeyword Category { get; }

        RogueTileLayer Layer { get; }
        bool HasCollider { get; } // タイル化した RogueObj の HasCollider と共通のため HasTileCollider ではない
        bool HasSightCollider { get; }

        IAffectRogueMethod Hit { get; }
        IAffectRogueMethod BeDefeated { get; }
        IApplyRogueMethod BeSteppedOnAsTile { get; }
    }
}
