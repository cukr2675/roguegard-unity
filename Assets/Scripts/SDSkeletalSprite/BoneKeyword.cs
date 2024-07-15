using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SDSSprite
{
    public readonly struct BoneKeyword
    {
        public string Name { get; }

        public static BoneKeyword Body { get; } = new BoneKeyword("Body");
        public static BoneKeyword UpperBody { get; } = new BoneKeyword("UpperBody");
        public static BoneKeyword LeftArm { get; } = new BoneKeyword("LeftArm");
        public static BoneKeyword LeftHand { get; } = new BoneKeyword("LeftHand");
        public static BoneKeyword RightArm { get; } = new BoneKeyword("RightArm");
        public static BoneKeyword RightHand { get; } = new BoneKeyword("RightHand");
        public static BoneKeyword LeftLeg { get; } = new BoneKeyword("LeftLeg");
        public static BoneKeyword LeftFoot { get; } = new BoneKeyword("LeftFoot");
        public static BoneKeyword RightLeg { get; } = new BoneKeyword("RightLeg");
        public static BoneKeyword RightFoot { get; } = new BoneKeyword("RightFoot");
        public static BoneKeyword Head { get; } = new BoneKeyword("Head");
        public static BoneKeyword Hair { get; } = new BoneKeyword("Hair");
        public static BoneKeyword LeftEar { get; } = new BoneKeyword("LeftEar");
        public static BoneKeyword RightEar { get; } = new BoneKeyword("RightEar");
        public static BoneKeyword LeftEye { get; } = new BoneKeyword("LeftEye");
        public static BoneKeyword RightEye { get; } = new BoneKeyword("RightEye");
        public static BoneKeyword LeftWing { get; } = new BoneKeyword("LeftWing");
        public static BoneKeyword RightWing { get; } = new BoneKeyword("RightWing");
        public static BoneKeyword Tail { get; } = new BoneKeyword("Tail");
        public static BoneKeyword Effect { get; } = new BoneKeyword("Effect");
        public static BoneKeyword Other { get; } = new BoneKeyword(null);

        public BoneKeyword(string name)
        {
            Name = name;
        }

        public override bool Equals(object obj)
        {
            return obj is BoneKeyword boneKeyword && Name == boneKeyword.Name;
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
