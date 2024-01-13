using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public class CommonBeDefeated : ReferableScript, IAffectRogueMethod
    {
        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            // すでに倒れているオブジェクトを倒すことはできない。
            if (self.Stack == 0) return false;

            var lootLocation = self.Location;
            var lootPosition = self.Position;

            if (LobbyMemberList.GetMemberInfo(self) != null && self.Main.GetPlayerLeaderInfo(self) != null)
            {
                // プレイヤーパーティのリーダーが倒れたときゲームオーバー処理
                if (MainCharacterWorkUtility.VisibleAt(self.Location, self.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, "は倒れてしまった！\n");
                    RogueDevice.Add(DeviceKw.EnqueueSE, DeviceKw.GameOver);
                    RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateBoneMotion(self, CoreMotions.FullTurn, false));
                    RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateBoneMotion(self, KeywordBoneMotion.Wait, true));
                    RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateBoneMotion(self, CoreMotions.FullTurn, false));
                    RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateBoneMotion(self, KeywordBoneMotion.BeDefeated, true));
                    RogueDevice.Add(DeviceKw.EnqueueWaitSeconds, 1f);
                }
                RogueDevice.Add(DeviceKw.GameOver, self);
                return true;
            }
            else if (MainCharacterWorkUtility.VisibleAt(self.Location, self.Position))
            {
                RogueDevice.Add(DeviceKw.AppendText, self);
                RogueDevice.Add(DeviceKw.AppendText, "は倒れた！\n");
                RogueDevice.AddWork(DeviceKw.EnqueueWork, RogueCharacterWork.CreateBoneMotion(self, KeywordBoneMotion.BeDefeated, false));
            }

            // 倒れたキャラクターを消す。
            self.TrySetStack(0, user);

            // アイテムドロップ
            WeightedRogueObjGeneratorUtility.CreateObjs(self.Main.InfoSet.LootTable, lootLocation, lootPosition, RogueRandom.Primary);

            return true;
        }
    }
}
