namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IRogueChart
    {
        IRogueChartSource Source { get; }

        void MoveNext();
    }
}
