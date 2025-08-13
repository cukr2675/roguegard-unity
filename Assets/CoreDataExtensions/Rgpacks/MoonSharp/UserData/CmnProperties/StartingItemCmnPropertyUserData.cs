using MoonSharp.Interpreter;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    public class StartingItemCmnPropertyUserData
    {
        private readonly StartingItemCmnProperty cmnProperty;

        public StartingItemCmnPropertyUserData()
        {
            cmnProperty = StartingItemCmnProperty.Default;
        }

        public StartingItemCmnPropertyUserData(StartingItemCmnProperty cmnProperty)
        {
            this.cmnProperty = cmnProperty;
        }

        public RogueObjUserData CreateObj(RogueObjUserData location, int x, int y)
        {
            var obj = cmnProperty.Value.Option.CreateObj(cmnProperty.Value, location.Obj, new(x, y), RogueRandom.Primary);
            return new RogueObjUserData(obj);
        }
    }
}
