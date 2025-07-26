namespace Roguegard
{
    public class OmnivoreEat : BaseEatRogueMethod, IEatActiveRogueMethod
    {
        public override string Name => "雑食";
        public override Spanning<IKeyword> Edibles => new IKeyword[] { MaterialKw.Flesh, MaterialKw.Veggy };
    }
}
