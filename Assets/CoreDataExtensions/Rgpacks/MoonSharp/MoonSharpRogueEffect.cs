using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MoonSharp.Interpreter;

namespace Roguegard.Rgpacks.MoonSharp
{
    [Objforming.Formable]
    internal class MoonSharpRogueEffect : IRogueEffect, IRogueObjUpdater, IRogueMethodPassiveAspect
    {
        private readonly MoonSharpScriptingType type;
        private readonly MoonSharpTableSerial serial;

        [System.NonSerialized] private Table scriptingInstance;
        [System.NonSerialized] private float updateOrder;
        [System.NonSerialized] private Closure updateMethod;
        [System.NonSerialized] private float passiveAspectOrder;
        [System.NonSerialized] private Closure passiveAspectMethod;

        float IRogueObjUpdater.Order => updateOrder;
        float IRogueMethodPassiveAspect.Order => passiveAspectOrder;

        private MoonSharpRogueEffect() { }

        public MoonSharpRogueEffect(MoonSharpScriptingType type)
        {
            this.type = type;
            serial = new MoonSharpTableSerial();
        }

        public void Open(RogueObj self)
        {
            if (scriptingInstance == null)
            {
                var type = this.type.GetTable();
                var typeNew = type.Get("new").Function;
                scriptingInstance = typeNew.Call(type).Table;

                var updateOrderItem = scriptingInstance.MetaTable.Get("update_order");
                if (updateOrderItem.IsNotNil()) { updateOrder = (float)updateOrderItem.Number; }
                else { updateOrder = 100f; }
                updateMethod = scriptingInstance.MetaTable.Get("update").Function;

                passiveAspectOrder = (float)scriptingInstance.MetaTable.Get("passive_aspect_order").Number;
                passiveAspectMethod = scriptingInstance.MetaTable.Get("passive_aspect").Function;
            }

            if (updateMethod != null)
            {
                var updaterState = self.Main.GetRogueObjUpdaterState(self);
                updaterState.AddFromRogueEffect(self, this);
            }
            if (passiveAspectMethod != null)
            {
                var aspectState = self.Main.GetRogueMethodAspectState(self);
                aspectState.AddPassiveFromRogueEffect(self, this);
            }
        }

        RogueObjUpdaterContinueType IRogueObjUpdater.UpdateObj(RogueObj self, float activationDepth, ref int sectionIndex)
        {
            return updateMethod.Call().Boolean ? RogueObjUpdaterContinueType.Continue : RogueObjUpdaterContinueType.Break;
        }

        bool IRogueMethodPassiveAspect.PassiveInvoke(
            IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
            RogueMethodAspectState.PassiveNext next)
        {
            return passiveAspectMethod.Call().Boolean;
        }

        public bool CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other)
        {
            return other is MoonSharpRogueEffect effect && effect.type.Equals(type) && effect.serial.CanStack(serial);
        }

        public IRogueEffect DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf)
        {
            var clone = new MoonSharpRogueEffect(type);
            serial.CopyTo(clone.serial);
            serial.ReplaceCloned(self, clonedSelf);
            return clone;
        }

        public IRogueEffect ReplaceCloned(RogueObj obj, RogueObj clonedObj)
        {
            serial.ReplaceCloned(obj, clonedObj);
            return this;
        }
    }
}
