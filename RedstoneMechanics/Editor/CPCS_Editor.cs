using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CPCS_Editor : MonoBehaviour
{
    public static void ReadDictionaryInEditor<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
    {
        Type[] arguments = dictionary.GetType().GetGenericArguments();
        Type keyType = arguments[0];
        Type valueType = arguments[1];

        GUILayout.Space(3f);
        GUIStyle style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
        EditorGUILayout.LabelField($"Dictionary<{keyType},{valueType}>", style, GUILayout.ExpandWidth(true));
        EditorGUILayout.LabelField("Count = " + dictionary.Count);

        for (int i = 0; i < dictionary.Count; i++)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{dictionary.Keys.ElementAt(i)} => {dictionary.Values.ElementAt(i)}");
            GUILayout.EndHorizontal();
        }
    }
}
