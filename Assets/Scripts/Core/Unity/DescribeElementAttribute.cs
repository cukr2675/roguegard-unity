using UnityEngine;

namespace Roguegard
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class DescribeElementAttribute : PropertyAttribute
    {
        public string DescribeVariableName { get; }

        public DescribeElementAttribute(string describeVariableName = "_option")
        {
            DescribeVariableName = describeVariableName;
        }
    }
}
