using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;
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

        public StartingItemCmnPropertyUserData(StartingItemCmnProperty cmnProperty, string envRgpackID)
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
