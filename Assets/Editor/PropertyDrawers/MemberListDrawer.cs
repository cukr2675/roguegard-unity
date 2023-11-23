using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using Roguegard.CharacterCreation;

namespace RoguegardUnity
{
    [CustomPropertyDrawer(typeof(MemberList))]
    public class MemberListDrawer : PropertyDrawer
    {
        private readonly bool showAsInlineProperties = true;

        private static readonly List<IMember> membersBuffer = new List<IMember>();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var members = property.FindPropertyRelative("_items");
            var size = members.arraySize;
            var dirty = EditorUtility.IsDirty(property.serializedObject.targetObject);
            var baseHeight = base.GetPropertyHeight(property, label);
            var y = position.y;
            if (dirty) { y += baseHeight; }
            for (int i = 0; i < size; i++)
            {
                var member = members.GetArrayElementAtIndex(i);

                if (showAsInlineProperties)
                {
                    var path = member.propertyPath;
                    while (member.NextVisible(true) && member.propertyPath.Contains(path)) // 次に移動できない or 親プロパティに出た 場合は終了
                    {
                        // 子プロパティは表示しない。
                        if (member.propertyPath.LastIndexOf('.') != path.Length) continue;

                        var height = EditorGUI.GetPropertyHeight(member, true);
                        var memberPosition = new Rect(position.x, y, position.width, height);
                        y += height + EditorGUIUtility.standardVerticalSpacing;

                        EditorGUI.PropertyField(memberPosition, member, true);
                    }
                }
                else
                {
                    var height = EditorGUI.GetPropertyHeight(member, true);
                    var memberPosition = new Rect(position.x, y, position.width, height);
                    y += height;

                    var content = new GUIContent();
                    var startIndex = "managedReference<".Length;
                    var length = member.type.Length - startIndex - ">".Length;
                    content.text = member.type.Substring(startIndex, length); // 型名から managedReference<> を取り除いて表示する。

                    EditorGUI.PropertyField(memberPosition, member, content, true);
                }
            }

            if (!dirty) return;

            position.height = baseHeight;
            if (GUI.Button(position, "Update Members"))
            {
                var path = property.propertyPath;
                var length = path.LastIndexOf('.');
                var parentPath = path.Substring(0, length);
                var parent = property.serializedObject.FindProperty(parentPath);

                var _option = parent.FindPropertyRelative("_option");
                if (_option?.propertyType == SerializedPropertyType.ObjectReference)
                {
                    Validate((IMemberableOption)_option.objectReferenceValue, members);
                }
                else if (_option?.propertyType == SerializedPropertyType.Generic)
                {
                    _option = parent.FindPropertyRelative("_option._ref");
                    Validate((IMemberableOption)_option.managedReferenceValue, members);
                }
            }
        }

        public void Validate(IMemberableOption _option, SerializedProperty _members)
        {
            if (_option == null)
            {
                _members.arraySize = 0;
                return;
            }

            membersBuffer.Clear();
            for (int i = 0; i < _option.MemberSources.Count; i++)
            {
                var source = _option.MemberSources[i];
                var member = FirstOrDefault(source);
                if (member != null) { membersBuffer.Add(member.Clone()); } // 参照越しに変更されないようにクローンを使用する。
                else { membersBuffer.Add(source.CreateMember()); } // 元のリストになければ新規作成する。
            }
            _members.arraySize = membersBuffer.Count;
            for (int i = 0; i < membersBuffer.Count; i++)
            {
                var item = _members.GetArrayElementAtIndex(i);
                item.managedReferenceValue = membersBuffer[i];
            }

            IMember FirstOrDefault(IMemberSource source)
            {
                for (int i = 0; i < _members.arraySize; i++)
                {
                    var item = (IMember)_members.GetArrayElementAtIndex(i).managedReferenceValue;
                    if (item?.Source == source) return item;
                }
                return null;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var members = property.FindPropertyRelative("_items");
            var size = members.arraySize;
            var dirty = EditorUtility.IsDirty(property.serializedObject.targetObject);
            var baseHeight = base.GetPropertyHeight(property, label);
            var sumHeight = 0f;
            if (dirty) { sumHeight += baseHeight; }
            for (int i = 0; i < size; i++)
            {
                var member = members.GetArrayElementAtIndex(i);
                if (showAsInlineProperties)
                {
                    var path = member.propertyPath;
                    while (member.NextVisible(true) && member.propertyPath.Contains(path))  // 次に移動できない or 親プロパティに出た 場合は終了
                    {
                        // 子プロパティは表示しない。
                        if (member.propertyPath.LastIndexOf('.') != path.Length) continue;

                        sumHeight += EditorGUI.GetPropertyHeight(member, true);
                        sumHeight += EditorGUIUtility.standardVerticalSpacing;
                    }
                }
                else
                {
                    sumHeight += EditorGUI.GetPropertyHeight(member, true);
                }
            }
            if (showAsInlineProperties) { sumHeight -= EditorGUIUtility.standardVerticalSpacing; }
            return sumHeight;
        }
    }
}
