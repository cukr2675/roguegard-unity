using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using Roguegard.Extensions;

namespace Roguegard.CharacterCreation
{
    public class VehicleOpen : ReferableScript, IOpenEffect
    {
        [SerializeField] private ScriptField<IApplyRogueMethod> _beRidden;
        [SerializeField] private ScriptField<IApplyRogueMethod> _beUnridden;
        [SerializeField] private ScriptField<IEquippedEffectSource>[] _equippedEffectSources;

        private VehicleOpen() { }

        public IRaceOption Open(
            RogueObj self, MainInfoSetType infoSetType, bool polymorph2Base, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            var vehicleInfo = new VehicleInfo(this, self);
            Roguegard.VehicleInfo.SetTo(self, vehicleInfo);
            return raceOption;
        }

        public void Close(
            RogueObj self, MainInfoSetType infoSetType, bool base2Polymorph, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            Roguegard.VehicleInfo.RemoveFrom(self);
        }

        public IRaceOption Reopen(
            RogueObj self, MainInfoSetType infoSetType, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
            return raceOption;
        }

        public void InitializeObj(RogueObj self, IRaceOption raceOption, ICharacterCreationData characterCreationData)
        {
        }

        private class VehicleInfo : BaseVehicleInfo
        {
            private readonly VehicleOpen data;
            private readonly IEquippedEffect[] effects;

            public override IApplyRogueMethod BeRidden => data._beRidden.Ref ?? baseBeRidden;
            public override IApplyRogueMethod BeUnridden => data._beUnridden.Ref ?? baseBeUnridden;

            private static readonly IApplyRogueMethod baseBeRidden = new BeRiddenRogueMethod();
            private static readonly IApplyRogueMethod baseBeUnridden = new BeUnriddenRogueMethod();

            public VehicleInfo(VehicleOpen data, RogueObj self)
            {
                this.data = data;
                effects = data._equippedEffectSources.Select(x => x.Ref.CreateOrReuse(self, null)).ToArray();
            }

            protected override void AddEffect(RogueObj vehicle)
            {
                foreach (var effect in effects)
                {
                    effect.AddEffect(vehicle, Rider);
                }
            }

            protected override void RemoveEffect(RogueObj vehicle)
            {
                foreach (var effect in effects)
                {
                    effect.RemoveEffect(vehicle, Rider);
                }
            }
        }

        private class BeRiddenRogueMethod : BaseApplyRogueMethod
        {
            public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                if (self.Location != user)
                {
                    // 足元にあったら拾わせる。
                    if (self.Location != user.Location || self.Position != user.Position ||
                        !this.Locate(self, user, user, activationDepth))
                    {
                        // 拾えなかったら失敗させる。
                        if (RogueDevice.Primary.Player == user)
                        {
                            RogueDevice.Add(DeviceKw.AppendText, "足が届かない");
                        }
                        return false;
                    }
                }

                // スタックしていたら一つだけ分割する。
                if (self.Stack >= 2)
                {
                    if (!SpaceUtility.TryDividedLocate(self, 1, out self)) return false;
                }

                var info = Roguegard.VehicleInfo.Get(self);
                if (info.Rider != null)
                {
                    // 既に誰かに装備されている場合は失敗させる。
                    return false;
                }

                if (!info.TryOpen(self, user)) return false;

                if (RogueDevice.Primary.VisibleAt(user.Location, user.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, user);
                    RogueDevice.Add(DeviceKw.AppendText, "は");
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, "に乗った！\n");
                }
                return true;
            }
        }

        private class BeUnriddenRogueMethod : BaseApplyRogueMethod
        {
            public override bool Invoke(RogueObj self, RogueObj user, float activationDepth, in RogueMethodArgument arg)
            {
                var info = Roguegard.VehicleInfo.Get(self);
                info.Close(self);

                var owner = self.Location;
                if (RogueDevice.Primary.VisibleAt(owner.Location, owner.Position))
                {
                    RogueDevice.Add(DeviceKw.AppendText, owner);
                    RogueDevice.Add(DeviceKw.AppendText, "は");
                    RogueDevice.Add(DeviceKw.AppendText, self);
                    RogueDevice.Add(DeviceKw.AppendText, "から降りた\n");
                }
                return true;
            }
        }
    }
}
