using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public interface IVehicleInfo
    {
        RogueObj Rider { get; }

        IApplyRogueMethod BeRidden { get; }

        // IEquipmentInfo.BeUnequipped と違ってメソッド内に空間移動を含むため IApplyRogueMethod にする。
        IApplyRogueMethod BeUnridden { get; }

        bool TryOpen(RogueObj vehicle, RogueObj rider, RideRogueEffect rideEffect = null);

        void RemoveClose(RogueObj vehicle);
    }
}
