using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Enemy_Spawning_Script.Horde))]
public class HordePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
    {
        SerializedProperty name = prop.FindPropertyRelative("name");
        SerializedProperty waves = prop.FindPropertyRelative("waves");


        EditorGUILayout.PropertyField(name, new GUIContent("Horde Name"));
        EditorGUI.indentLevel += 1;
        name.isExpanded = EditorGUILayout.Foldout(name.isExpanded, "Horde Composition", true);
        //bool dropdown = EditorGUILayout.Foldout(false, "Horde Makeup", true);
        if (name.isExpanded)
        {
            EditorGUI.indentLevel += 1;
            ShowArrayProperty(waves);
            EditorGUI.indentLevel -= 1;
        }
        EditorGUI.indentLevel += 1;
    }

    public void ShowArrayProperty(SerializedProperty list)
    {
        EditorGUILayout.PropertyField(list.FindPropertyRelative("Array.size"), new GUIContent("Number of Waves"));

        EditorGUI.indentLevel += 1;
        for (int i = 0; i < list.arraySize; i++)
        {
            EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i), new GUIContent("Wave " + (i + 1).ToString()));
        }
        EditorGUI.indentLevel -= 1;
    }
}
