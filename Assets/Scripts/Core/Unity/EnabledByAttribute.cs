using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class EnabledByAttribute : PropertyAttribute
    {
        public string BooleanName { get; }

        public EnabledByAttribute(string booleanName)
        {
            BooleanName = booleanName;
        }
    }
}
