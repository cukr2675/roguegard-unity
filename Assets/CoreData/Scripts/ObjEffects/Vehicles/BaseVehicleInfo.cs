using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public abstract class BaseVehicleInfo : IVehicleInfo
    {
        /// <summary>
        /// 生成したインスタンスを使いまわす
        /// </summary>
        [System.NonSerialized] private RideRogueEffect rideEffect;

        public RogueObj Rider => rideEffect?.Rider;

        public abstract IApplyRogueMethod BeRidden { get; }
        public abstract IApplyRogueMethod BeUnridden { get; }

        bool IVehicleInfo.TryOpen(RogueObj vehicle, RogueObj rider, RideRogueEffect rideEffect)
        {
            var vehicleInfo = VehicleInfo.Get(vehicle);
            if (vehicleInfo != this)
            {
                Debug.LogError("変化した装備品を装備しようとしました。");
                return false;
            }

            var addEffect = false;
            if (rideEffect == null)
            {
                addEffect = true;
                if (this.rideEffect == null) { this.rideEffect = new RideRogueEffect(vehicle); }
                rideEffect = this.rideEffect;
            }
            else if (this.rideEffect != null && this.rideEffect.Rider != null)
            {
                // すでに装備されていたら失敗させる。
                return false;
            }
            rideEffect.SetRide(rider);
            this.rideEffect = rideEffect;

            if (addEffect) { rider.Main.RogueEffects.AddOpen(rider, rideEffect); }
            AddEffect(vehicle);
            return true;
        }

        protected virtual void AddEffect(RogueObj vehicle)
        {
            RogueEffectUtility.AddFromRogueEffect(Rider, this);
        }

        void IVehicleInfo.Close(RogueObj vehicle)
        {
            RemoveEffect(vehicle);
            rideEffect.Close();
            VehicleInfo.SetRiderTo(vehicle, null);
        }

        protected virtual void RemoveEffect(RogueObj vehicle)
        {
            RogueEffectUtility.Remove(Rider, this);
        }
    }
}
