using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class StairPickupAbility : ToolAbilityIntrinsicOptionScript
    {
        protected override float GetCost(
            IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, IReadOnlyStartingItem tool, out bool costIsUnknown)
        {
            var cost = tool.Option.GetCost(tool, out costIsUnknown);
            cost *= 200f;
            return cost;
        }

        protected override ISortedIntrinsic CreateSortedIntrinsic(
            IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv, IReadOnlyStartingItem tool)
        {
            return new Intrinsic(lv, tool);
        }

        private class Intrinsic : AbilitySortedIntrinsic, IRogueMethodPassiveAspect
        {
            private readonly IReadOnlyStartingItem tool;

            float IRogueMethodPassiveAspect.Order => 0f;

            public Intrinsic(int lv, IReadOnlyStartingItem tool)
                : base(lv)
            {
                this.tool = tool;
            }

            bool IRogueMethodPassiveAspect.PassiveInvoke(
                IKeyword keyword, IRogueMethod method, RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg,
                RogueMethodAspectState.PassiveNext next)
            {
                var generate = keyword == MainInfoKw.Locate && self.Location.Space.Tilemap == null;

                var result = next.Invoke(keyword, method, self, user, activationDepth, arg);
                if (!result) return false;

                if (generate && self.Location.Space.Tilemap != null && DungeonInfo.TryGet(self.Location, out _))
                {
                    // 階層移動に成功したとき足元にアイテムを生成する。
                    tool.Option.CreateObj(tool, self.Location, self.Position, RogueRandom.Primary);

                    if (RogueDevice.Primary.Player.Main.Stats.Party.Members.Contains(self))
                    {
                        RogueDevice.Add(DeviceKw.AppendText, "足元に");
                        RogueDevice.Add(DeviceKw.AppendText, tool);
                        RogueDevice.Add(DeviceKw.AppendText, "が転がり込んできた\n");
                    }
                }
                return true;
            }
        }
    }
}
