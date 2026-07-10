using System;
using System.Collections.Generic;
using System.Reflection;
using GameKit.Core;
using UnityEditor;
using UnityEngine;

namespace GameKit.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects]
    public sealed class FoldableComponentEditor : UnityEditor.Editor
    {
        private static readonly Dictionary<string, bool> FoldoutState = new();

        private string cachedTypeName;
        private List<Group> groups;

        private sealed class Group
        {
            public string Header;
            public readonly List<SerializedProperty> Properties = new();
            public readonly List<FieldInfo> Fields = new();
        }

        public override void OnInspectorGUI()
        {
            Type targetType = serializedObject.targetObject.GetType();
            FoldableInspectorAttribute attribute = targetType.GetCustomAttribute<FoldableInspectorAttribute>();
            if (attribute == null)
            {
                DrawDefaultInspector();
                return;
            }

            if (cachedTypeName != targetType.FullName || groups == null)
                BuildGroups(targetType);

            serializedObject.Update();

            DrawScriptField();

            string title = string.IsNullOrEmpty(attribute.DisplayName) ? targetType.Name : attribute.DisplayName;
            EditorGUILayout.HelpBox(title, MessageType.None);

            foreach (Group group in groups)
            {
                string key = $"{targetType.FullName}::{group.Header}";
                if (!FoldoutState.TryGetValue(key, out bool isOpen))
                    FoldoutState[key] = isOpen = true;

                EditorGUILayout.BeginVertical(GUI.skin.box);
                FoldoutState[key] = EditorGUILayout.Foldout(isOpen, group.Header, true);

                if (FoldoutState[key])
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < group.Properties.Count; i++)
                    {
                        SerializedProperty property = group.Properties[i];
                        FieldInfo field = group.Fields[i];
                        if (property == null)
                            continue;

                        string tooltip = field?.GetCustomAttribute<TooltipAttribute>()?.tooltip ?? "";
                        EditorGUILayout.PropertyField(property, new GUIContent(property.displayName, tooltip), true);
                    }
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawScriptField()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                MonoScript script = MonoScript.FromMonoBehaviour(target as MonoBehaviour);
                EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            }
        }

        private void BuildGroups(Type inspectedType)
        {
            cachedTypeName = inspectedType.FullName;
            groups = new List<Group>();
            Group currentGroup = null;

            SerializedProperty property = serializedObject.GetIterator();
            if (!property.NextVisible(true))
                return;

            do
            {
                if (property.name == "m_Script")
                    continue;

                FieldInfo field = FindFieldInfo(inspectedType, property.name);
                HeaderAttribute header = field?.GetCustomAttribute<HeaderAttribute>();

                if (header != null && !string.IsNullOrEmpty(header.header))
                {
                    currentGroup = new Group { Header = header.header };
                    groups.Add(currentGroup);
                }

                if (currentGroup == null)
                {
                    currentGroup = new Group { Header = "Settings" };
                    groups.Add(currentGroup);
                }

                if (field != null && field.GetCustomAttribute<HideInInspector>() != null)
                    continue;

                currentGroup.Properties.Add(serializedObject.FindProperty(property.propertyPath));
                currentGroup.Fields.Add(field);
            }
            while (property.NextVisible(false));
        }

        private static FieldInfo FindFieldInfo(Type type, string propertyName)
        {
            for (Type current = type; current != null; current = current.BaseType)
            {
                FieldInfo field = current.GetField(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                    return field;
            }

            return null;
        }
    }
}
