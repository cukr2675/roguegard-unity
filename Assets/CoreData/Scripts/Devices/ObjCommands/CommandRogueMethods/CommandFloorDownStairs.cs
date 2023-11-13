using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

namespace Roguegard
{
    public class CommandFloorDownStairs : CommandRogueMethod
    {
        public override IKeyword Keyword => CategoryKw.DownStairs;

        public override string Name => "下りる";

        public override bool Invoke(RogueObj player, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            if (RogueDevice.Primary.Player != player) return false;
            if (CommonAssert.RequireTool(arg, out var tool)) return false;

            if (tool.Main.Category != CategoryKw.LevelDownStairs)
            {
                Debug.LogError($"{tool} のカテゴリは {CategoryKw.LevelDownStairs.Name} ではありません。");
                return false;
            }

            var info = SavePointInfo.Get(tool);
            if (!this.LocateSavePoint(player, tool, activationDepth, info)) return false;

            RogueDevice.Add(DeviceKw.AutoSave, info);
            return this.LoadSavePoint(player, activationDepth, info);
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
