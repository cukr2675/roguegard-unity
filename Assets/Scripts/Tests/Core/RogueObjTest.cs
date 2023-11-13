using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Roguegard
{
    public class RogueObjTest
    {
        [Test]
        public void Clone()
        {
            var obj = new RogueObj();
            obj.Main = new MainRogueObjInfo();
            obj.Main.SetBaseInfoSet(obj, TestMainInfoSet.Instance);
            var clone = obj.Clone();

            
        }
    }
}
