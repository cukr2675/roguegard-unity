using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    public class BomberTortoiseHit : ReferableScript, IAffectRogueMethod
    {
        [SerializeField] private ScriptableCharacterCreationData _bomberShell = null;

        private BomberTortoiseHit() { }

        public bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            var useValue = AttackUtility.GetUseValue(arg.RefValue);
            if (useValue)
            {
                var damage = !arg.RefValue.SubValues.Is(StdKw.Heal);
                if (damage && !arg.RefValue.SubValues.Is(MainInfoKw.BeDefeated))
                {
                    // 変化している場合は除外
                    if (self.Main.InfoSetState == MainInfoSetType.Base)
                    {
                        // ダメージの判定が出ていれば、爆弾コウラになる。
                        PolymorphStatusEffect.AffectTo(self, _bomberShell.PrimaryInfoSet, 3);
                        if (MessageWorkListener.TryOpenHandler(self.Location, self.Position, out var h))
                        {
                            using var handler = h;
                            handler.AppendText(self).AppendText("は甲羅にこもった！\n");
                        }
                        return true;
                    }
                }
            }
            var result = CommonHit.Instance.Invoke(self, user, activationDepth, arg);
            return result;
        }
    }
}
