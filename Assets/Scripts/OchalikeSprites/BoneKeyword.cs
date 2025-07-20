namespace OchalikeSprites
{
    public readonly struct BoneKeyword : System.IEquatable<BoneKeyword>
    {
        public string Name { get; }

        public static BoneKeyword Body { get; } = new BoneKeyword("Body");
        public static BoneKeyword Chest { get; } = new BoneKeyword("Chest");
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
        public static BoneKeyword LongHair { get; } = new BoneKeyword("LongHair");
        public static BoneKeyword LeftEar { get; } = new BoneKeyword("LeftEar");
        public static BoneKeyword RightEar { get; } = new BoneKeyword("RightEar");
        public static BoneKeyword LeftEye { get; } = new BoneKeyword("LeftEye");
        public static BoneKeyword RightEye { get; } = new BoneKeyword("RightEye");
        public static BoneKeyword Mouth { get; } = new BoneKeyword("Mouth");
        public static BoneKeyword Wings { get; } = new BoneKeyword("Wings");
        public static BoneKeyword Tail { get; } = new BoneKeyword("Tail");
        public static BoneKeyword BodyEffect { get; } = new BoneKeyword("BodyEffect");
        public static BoneKeyword HeadEffect { get; } = new BoneKeyword("HeadEffect");
        public static BoneKeyword Free { get; } = new BoneKeyword(string.Empty);

        public BoneKeyword(string name)
        {
            Name = name;
        }

        public bool Equals(BoneKeyword other)
        {
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return obj is BoneKeyword other && Name == other.Name;
        }

        public override int GetHashCode()
        {
            return Name?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return $"Bone_{Name}";
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
