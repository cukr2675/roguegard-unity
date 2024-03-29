using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Roguegard
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class ElementDescriptionAttribute : PropertyAttribute
    {
        public string DescriptionVariableName { get; }

        public ElementDescriptionAttribute(string descriptionVariableName)
        {
            DescriptionVariableName = descriptionVariableName;
        }
    }
}
