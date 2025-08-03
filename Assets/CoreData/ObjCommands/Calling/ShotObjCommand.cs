namespace Roguegard
{
    public class ShotObjCommand : CallingObjCommand
    {
        public override string Name => "謦?▽";

        public override bool Execute(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
        {
            EquipmentUtility.GetWeapon(self, out var weaponInfo);
            if (weaponInfo != null)
            {
                var shotResult = RogueMethodAspectState.Invoke(MainInfoKw.Throw, weaponInfo.Throw, self, user, activationDepth, arg);
                if (shotResult) return true;
            }

            var keyword = MainInfoKw.Throw;
            var throwMethod = self.Main.InfoSet.Throw;
            EnqueueMessageRule(self, keyword);
            return RogueMethodAspectState.Invoke(keyword, throwMethod, self, user, activationDepth, arg);
        }

        public override ISkillDescribable GetSkillDescribable(RogueObj self, RogueObj tool)
        {
            EquipmentUtility.GetWeapon(self, out var weaponInfo);
            if (weaponInfo != null) return weaponInfo.Throw;

            return self.Main.InfoSet.Throw;
        }
    }
}
