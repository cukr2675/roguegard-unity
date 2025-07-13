using UnityEditor;
using UnityEngine;

namespace OchalikeSprites.Editor
{
    [CustomPropertyDrawer(typeof(BoneSprite), true)]
    public class BoneSpriteDrawer : PropertyDrawer
    {
        private const float indentWidth = 15;
        private const float fieldsSpacing = 4;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var beforeIndentLevel = EditorGUI.indentLevel;
            var beforeLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUI.indentLevel = 0;
            EditorGUIUtility.labelWidth = 24f;

            var y = position.y + EditorGUIUtility.standardVerticalSpacing;
            var leftMemberX = position.x + beforeIndentLevel * indentWidth;
            var memberWidth = (position.x + position.width - leftMemberX - fieldsSpacing) / 2f;
            var rightMemberX = leftMemberX + memberWidth - EditorGUI.indentLevel * indentWidth + fieldsSpacing;

            {
                var leftMember = property.FindPropertyRelative("_normalFront");
                var rightMember = property.FindPropertyRelative("_backFront");
                
                var height = EditorGUI.GetPropertyHeight(leftMember);
                var leftMemberPosition = new Rect(leftMemberX, y, memberWidth, height);
                var rightMemberPosition = new Rect(rightMemberX, y, memberWidth, height);

                EditorGUI.PropertyField(leftMemberPosition, leftMember, new GUIContent("NF"));
                EditorGUI.PropertyField(rightMemberPosition, rightMember, new GUIContent("BF"));
                y += height + EditorGUIUtility.standardVerticalSpacing;
            }
            {
                var leftMember = property.FindPropertyRelative("_normalRear");
                var rightMember = property.FindPropertyRelative("_backRear");

                var height = EditorGUI.GetPropertyHeight(leftMember);
                var leftMemberPosition = new Rect(leftMemberX, y, memberWidth, height);
                var rightMemberPosition = new Rect(rightMemberX, y, memberWidth, height);

                EditorGUI.PropertyField(leftMemberPosition, leftMember, new GUIContent("NR"));
                EditorGUI.PropertyField(rightMemberPosition, rightMember, new GUIContent("BR"));
                y += height + EditorGUIUtility.standardVerticalSpacing;
            }

            EditorGUI.indentLevel = beforeIndentLevel;
            EditorGUIUtility.labelWidth = beforeLabelWidth;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = 0f;
            {
                var member = property.FindPropertyRelative("_normalFront");
                height += EditorGUI.GetPropertyHeight(member) + EditorGUIUtility.standardVerticalSpacing;
            }
            {
                var member = property.FindPropertyRelative("_normalRear");
                height += EditorGUI.GetPropertyHeight(member) + EditorGUIUtility.standardVerticalSpacing;
            }
            return height;
        }
    }
}
