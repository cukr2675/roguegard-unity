using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    public readonly struct BoneMotionKeyword
    {
        public string Name { get; }

        public BoneMotionKeyword(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return Name.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(BoneMotionKeyword left, BoneMotionKeyword right)
        {
            return left.Name == right.Name;
        }

        public static bool operator !=(BoneMotionKeyword left, BoneMotionKeyword right)
        {
            return left.Name != right.Name;
        }
    }
}
