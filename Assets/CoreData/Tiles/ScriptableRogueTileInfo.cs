using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    public  abstract class ScriptableRogueTileInfo : ScriptableRogueTile, IRogueTileInfo
    {
        public sealed override IRogueTileInfo Info => this;

        protected abstract string DescriptionName { get; }
        string IRogueDescription.Name => DescriptionName;
        public virtual Sprite Icon => Sprite;
        public abstract Color Color { get; }
        public abstract string Caption { get; }
        public abstract object Details { get; }

        public abstract Sprite Sprite { get; }
        public abstract IKeyword Category { get; }

        public abstract RogueTileLayer Layer { get; }
        public abstract bool HasCollider { get; }
        public abstract bool HasSightCollider { get; }

        public abstract IAffectRogueMethod Hit { get; }
        public abstract IAffectRogueMethod BeDefeated { get; }
        public abstract IApplyRogueMethod BeApplied { get; }
    }
}
