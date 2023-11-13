using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// <see cref="Name"/> で指定したボーンを前か後ろに向ける。
    /// </summary>
    public struct BoneBack : System.IEquatable<BoneBack>
    {
        public IKeyword Name { get; }

        public Type LocalBack { get; }

        public BoneBack(IKeyword name, Type localBack)
        {
            Name = name;
            LocalBack = localBack;
        }

        public bool Equals(BoneBack other)
        {
            return Name == other.Name && LocalBack == other.LocalBack;
        }

        public enum Type
        {
            /// <summary>
            /// <see cref="BonePose.Back"/> と同値にする。
            /// </summary>
            ForPose,

            /// <summary>
            /// <see cref="BonePose.Back"/> の逆の値にする。
            /// </summary>
            InversePose,

            /// <summary>
            /// 常に前に向ける。
            /// </summary>
            ForcedNormal,

            /// <summary>
            /// 常に後ろに向ける。
            /// </summary>
            ForcedBack
        }
    }
}
