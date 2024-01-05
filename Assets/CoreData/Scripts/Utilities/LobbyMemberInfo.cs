using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Roguegard.CharacterCreation;

namespace Roguegard
{
    [ObjectFormer.Formable]
    public class LobbyMemberInfo
    {
        public CharacterCreationDataBuilder CharacterCreationData { get; set; }

        public ISavePointInfo SavePoint { get; set; }

        public RogueObj Seat { get; private set; }

        public void SetSeat(RogueObj self, RogueObj seat, RogueBehaviourNodeList behaviourNode)
        {
            if (seat == null) throw new System.ArgumentNullException(nameof(seat));
            if (behaviourNode == null) throw new System.ArgumentNullException(nameof(behaviourNode));

            Seat = seat;
            RogueBehaviourNodeEffect.SetBehaviourNode(self, behaviourNode);
        }

        public void ResetSeat(RogueObj self)
        {
            Seat = null;
            RogueBehaviourNodeEffect.RemoveBehaviourNode(self);
        }
    }
}
