using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// <see cref="IVehicleInfo"/> を公開する <see cref="IRogueObjInfo"/>
    /// </summary>
    public static class VehicleInfo
    {
        public static IVehicleInfo Get(RogueObj vehicle)
        {
            vehicle.Main.TryOpenRogueEffects(vehicle);

            if (vehicle.TryGet<Info>(out var info))
            {
                // 誰かに騎乗されていたら騎手のエフェクトを準備させる
                // ここで準備させないと IVehicleInfo.Rider が null になってしまう
                info.rider?.Main.TryOpenRogueEffects(info.rider);

                return info.info;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 上書き不可
        /// </summary>
        public static void SetTo(RogueObj vehicle, IVehicleInfo vehicleInfo)
        {
            if (vehicleInfo == null) throw new System.ArgumentNullException(nameof(vehicleInfo));

            if (!vehicle.TryGet<Info>(out var info))
            {
                info = new Info();
                vehicle.SetInfo(info);
            }

            // 上書き不可
            if (info.info != null) throw new RogueException();

            info.info = vehicleInfo;
        }

        public static void RemoveFrom(RogueObj vehicle)
        {
            if (vehicle.TryGet<Info>(out var rideableInfo))
            {
                rideableInfo.info = null;
            }
        }

        public static void SetRiderTo(RogueObj vehicle, RogueObj rider)
        {
            if (!vehicle.TryGet<Info>(out var info))
            {
                info = new Info();
                vehicle.SetInfo(info);
            }

            info.rider = rider;
        }

        [ObjectFormer.Formable]
        private class Info : IRogueObjInfo
        {
            [System.NonSerialized] public IVehicleInfo info;

            public RogueObj rider;

            public bool IsExclusedWhenSerialize => rider == null;

            public bool CanStack(IRogueObjInfo other) => false;
            public IRogueObjInfo DeepOrShallowCopy(RogueObj self, RogueObj clonedSelf) => null;
            public IRogueObjInfo ReplaceCloned(RogueObj obj, RogueObj clonedObj) => null;
        }
    }
}
