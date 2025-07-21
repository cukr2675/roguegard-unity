namespace Roguegard
{
    [Objforming.RequireRelationalComponent]
    public interface IRogueChartSource : System.IEquatable<IRogueChartSource>
    {
        IRogueChart CreateChart();
    }
}
