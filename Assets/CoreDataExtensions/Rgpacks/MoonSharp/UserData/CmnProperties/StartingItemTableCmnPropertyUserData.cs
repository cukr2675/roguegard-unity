using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using MoonSharp.Interpreter;

namespace Roguegard.Rgpacks.MoonSharp
{
    [MoonSharpUserData]
    public class StartingItemTableCmnPropertyUserData
    {
        private readonly StartingItemTableCmnProperty cmnProperty;

        private static readonly RogueObjList generatedObjs = new();

        public StartingItemTableCmnPropertyUserData()
        {
            cmnProperty = StartingItemTableCmnProperty.Default;
        }

        public StartingItemTableCmnPropertyUserData(StartingItemTableCmnProperty cmnProperty, string envRgpackID)
        {
            this.cmnProperty = cmnProperty;
        }

        public RogueObjUserData[] CreateObj(RogueObjUserData location, int x, int y)
        {
            WeightedRogueObjGeneratorUtility.CreateObjs(cmnProperty.Value, location.Obj, new(x, y), RogueRandom.Primary, generatedObjs: generatedObjs);

            var result = new RogueObjUserData[generatedObjs.Count];
            for (int i = 0; i < generatedObjs.Count; i++)
            {
                result[i] = new RogueObjUserData(generatedObjs[i]);
            }
            return result;
        }
    }
}
