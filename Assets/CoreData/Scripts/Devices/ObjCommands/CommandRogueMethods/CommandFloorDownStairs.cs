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
            if (CommonAssert.RequireTool(arg, out var tool)) return false;
            if (tool.Main.Category != CategoryKw.LevelDownStairs)
            {
                Debug.LogError($"{tool} のカテゴリは {CategoryKw.LevelDownStairs.Name} ではありません。");
                return false;
            }
            if (!LobbyMembers.Contains(player))
            {
                // ダンジョン空間オブジェクトが永続的に残ることを避けるため、 LobbyMembers によって追跡できないキャラによる階層移動は禁止する。
                Debug.LogError($"{player} は {nameof(LobbyMembers)} を持ちません。");
                return false;
            }
            var info = SavePointInfo.Get(tool);

            if (!this.LocateSavePoint(player, tool, activationDepth, info)) return false;

            LobbyMembers.SetSavePoint(player, info);

            // プレイヤーのときだけオートセーブする
            if (RogueDevice.Primary.Player == player) { RogueDevice.Add(DeviceKw.AutoSave, 0); }

            return true;
        }

        public override ISkillDescription GetSkillDescription(RogueObj self, RogueObj tool)
        {
            return null;
        }
    }
}
