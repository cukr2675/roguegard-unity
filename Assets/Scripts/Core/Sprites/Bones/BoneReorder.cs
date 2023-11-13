using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    /// <summary>
    /// <see cref="Name"/> で指定したボーンを手前か奥に再配置する。
    /// </summary>
    public struct BoneReorder : System.IEquatable<BoneReorder>
    {
        public IKeyword Name { get; }

        public Type Reorder { get; }

        public BoneReorder(IKeyword name, Type reorder)
        {
            Name = name;
            Reorder = reorder;
        }

        public bool Equals(BoneReorder other)
        {
            return Name == other.Name && Reorder == other.Reorder;
        }

        public enum Type
        {
            Front,
            Rear
        }
    }
}
