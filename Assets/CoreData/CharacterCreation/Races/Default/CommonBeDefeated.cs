using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Extensions;

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
                if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
                {
                    using var handler = h;
                    handler.AppendText(self);
                    handler.AppendText("は倒れてしまった！\n");
                    handler.EnqueueSE(DeviceKw.GameOver);
                    handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, CoreMotions.FullTurn, false));
                    handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, KeywordSpriteMotion.Wait, true));
                    handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, CoreMotions.FullTurn, false));
                    handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, KeywordSpriteMotion.BeDefeated, true));
                    handler.Handle(DeviceKw.EnqueueWaitSeconds, 1f);
                }
                if (self == RogueDevice.Primary.Subject)
                {
                    RogueDevice.Add(DeviceKw.GameOver, self);
                }
                else
                {
                    const float beDefeatedLocateActivationDepth = 99f;
                    default(IActiveRogueMethodCaller).LocateSavePoint(self, null, beDefeatedLocateActivationDepth, RogueWorldSavePointInfo.Instance, true);

                    var memberInfo = LobbyMemberList.GetMemberInfo(self);
                    memberInfo.SavePoint = RogueWorldSavePointInfo.Instance;
                }
                return true;
            }
            else if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
            {
                using var handler = h;
                handler.AppendText(self);
                handler.AppendText("は倒れた！\n");
                handler.EnqueueWork(RogueCharacterWork.CreateSpriteMotion(self, KeywordSpriteMotion.BeDefeated, false));
            }

            // 倒れたキャラクターを消す。
            self.TrySetStack(0, user);

            // アイテムドロップ
            WeightedRogueObjGeneratorUtility.CreateObjs(self.Main.InfoSet.LootTable, lootLocation, lootPosition, RogueRandom.Primary);

            return true;
        }
    }
}
