using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.Device;

namespace Roguegard
{
    public static class TickUtility
    {
        public static RogueObjUpdaterContinueType Section0Update(RogueObj self, ref int sectionIndex, bool invokeByDevice = false)
        {
            if (!invokeByDevice)
            {
                // Device 以外から呼び出された場合
                // DeviceInfo があるとき、 Device から操作されることを想定して何もしない。
                var deviceInfo = RogueDeviceEffect.Get(self);
                if (deviceInfo != null) return RogueObjUpdaterContinueType.Break;
            }

            var stats = self.Main.Stats;
            
            var speed = SpeedCalculator.Get(self);
            if (speed.BeInhibited) return RogueObjUpdaterContinueType.Break;
            if (speed.Hungry && self.Main.GetPlayerLeaderInfo(self) == null) return RogueObjUpdaterContinueType.Break; // プレイヤーリーダー以外の空腹時にパス
            if (speed.Speed < 0 && stats.ChargedSpeed < -speed.Speed)
            {
                // Speed がマイナスのとき数回に一回行動する。
                // 行動できないときは行動できるまでのカウントを進める。
                stats.ChargedSpeed += 1;
                return RogueObjUpdaterContinueType.Break;
            }
            stats.ChargedSpeed = 0;
            sectionIndex = 1 + Mathf.Max(speed.Speed, 0);
            return RogueObjUpdaterContinueType.Continue;
        }

        public static RogueObjUpdaterContinueType SectionAfter1LateUpdate(RogueObj self, ref int sectionIndex)
        {
            if (sectionIndex <= 0) throw new System.ArgumentOutOfRangeException();

            // 再行動
            sectionIndex += self.Main.Stats.ChargedSpeed;
            self.Main.Stats.ChargedSpeed = 0;

            if (sectionIndex <= 1) return RogueObjUpdaterContinueType.Break;

            sectionIndex--;
            return RogueObjUpdaterContinueType.Continue;
        }
    }
}
