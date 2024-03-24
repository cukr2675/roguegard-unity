using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard.CharacterCreation
{
    public class ItemSearchSkill : MPSkillIntrinsicOptionScript
    {
        public override ISortedIntrinsic CreateSortedIntrinsic(
            ScriptIntrinsicOption parent, IReadOnlyIntrinsic intrinsic, ICharacterCreationData characterCreationData, int lv)
        {
            return new SortedIntrinsic(parent, lv);
        }

        [Objforming.Formable]
        private class SortedIntrinsic : MPSkillSortedIntrinsic<SortedIntrinsic>
        {
            public override IRogueMethodTarget Target => null;
            public override IRogueMethodRange Range => UserRogueMethodRange.Instance;
            public override int RequiredMP => 5;

            private SortedIntrinsic() : base(null, 0) { }

            public SortedIntrinsic(ScriptIntrinsicOption parent, int lv) : base(parent, lv) { }

            protected override bool Activate(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (self.Main.Stats.Party == null) return false;

                var leader = self.Main.Stats.Party.Members[0];
                if (!ViewInfo.TryGet(leader, out var viewInfo)) return false;

                // 階層全体のアイテムを視界に追加
                var locationObjs = self.Location.Space.Objs;
                var any = false;
                for (int i = 0; i < locationObjs.Count; i++)
                {
                    var obj = locationObjs[i];
                    if (obj == null || obj.HasCollider || obj.AsTile) continue;

                    viewInfo.AddVisibleObj(obj, true); // オブジェクトだけだと見づらいのでタイルも表示する
                    any = true;
                }
                if (any)
                {
                    RogueDevice.Add(DeviceKw.AppendText, ":ItemSearchMsgSucceed::1");
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, "\n");
                }
                else
                {
                    RogueDevice.Add(DeviceKw.AppendText, ":ItemSearchMsgFailed::1");
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, "\n");
                }
                return true;
            }

            public override int GetATK(RogueObj self, out bool additionalEffect)
            {
                additionalEffect = true;
                return 0;
            }
        }
    }
}
