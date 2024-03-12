using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkeletalSprite
{
    public readonly struct BoneKeyword
    {
        public string Name { get; }

        public static BoneKeyword Body { get; } = new BoneKeyword("Body");

        public BoneKeyword(string name)
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

        public static bool operator ==(BoneKeyword left, BoneKeyword right)
        {
            return left.Name == right.Name;
        }

        public static bool operator !=(BoneKeyword left, BoneKeyword right)
        {
            return left.Name != right.Name;
        }
    }
}
