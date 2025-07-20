using UnityEngine;

namespace OchalikeSprites
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class VisibleByAttribute : PropertyAttribute
    {
        public string BooleanName { get; }
        public string FixedName { get; }

        public VisibleByAttribute(string booleanName, string fixedName = null)
        {
            BooleanName = booleanName;
            FixedName = fixedName;
        }
    }
}
