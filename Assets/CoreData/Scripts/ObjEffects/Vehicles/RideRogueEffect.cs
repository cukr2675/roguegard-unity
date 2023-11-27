using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// このクラスのインスタンスは <see cref="IVehicleInfo.TryOpen(RogueObj, RideRogueEffect)"/> 内で生成する。
    /// （生成したインスタンスを <see cref="IVehicleInfo"/> 側で使いまわすため）
    /// </summary>
    [ObjectFormer.Formable]
    public class RideRogueEffect : IRogueEffect
    {
        private RogueObj vehicle;

        [field: System.NonSerialized] public RogueObj Rider { get; private set; }

        [ObjectFormer.CreateInstance]
        private RideRogueEffect() { }

        public RideRogueEffect(RogueObj vehicle)
        {
            this.vehicle = vehicle;
        }

        public void SetRide(RogueObj rider)
        {
            Rider = rider;
            VehicleInfo.SetRiderTo(vehicle, rider);
        }

        public void RemoveClose()
        {
            Rider.Main.RogueEffects.Remove(this);
            Rider = null;
        }

        void IRogueEffect.Open(RogueObj rider)
        {
            var info = VehicleInfo.Get(vehicle);
            info.TryOpen(vehicle, rider, this);
        }

        public static RogueObj GetVehicle(RogueObj owner)
        {
            if (owner.Main.RogueEffects.TryGetEffect<RideRogueEffect>(out var effect))
            {
                return effect.vehicle;
            }
            else
            {
                return null;
            }
        }

        bool IRogueEffect.CanStack(RogueObj obj, RogueObj otherObj, IRogueEffect other) => false;

        IRogueEffect IRogueEffect.DeepOrShallowCopy(RogueObj rider, RogueObj clonedRider)
        {
            if (rider != Rider) throw new RogueException();

            var clone = new RideRogueEffect(vehicle);
            clone.Rider = clonedRider;
            return clone;
        }

        IRogueEffect IRogueEffect.ReplaceCloned(RogueObj obj, RogueObj clonedObj)
        {
            if (vehicle == obj) { vehicle = clonedObj; }
            if (Rider == obj) { Rider = clonedObj; }
            return this;
        }
    }
}
