using NUnit.Framework;

namespace Roguegard
{
    public class RogueObjTest
    {
        [Test]
        public void Clone()
        {
            var obj = new RogueObj();
            obj.Main.SetBaseInfoSet(obj, TestMainInfoSet.Instance);
            var clone = obj.Clone();

            
        }
    }
}
