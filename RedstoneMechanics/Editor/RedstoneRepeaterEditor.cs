using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RedstoneRepeater))]
public class RedstoneRepeaterEditor : RedstoneBaseEditor
{
    private SerializedProperty _spriteOn;
    private SerializedProperty _spriteOff;
    private SerializedProperty _tickPosLocked;
    private SerializedProperty _iconRepeaterLocked;


    private RedstoneRepeater _script;

    protected override void OnEnable()
    {
        //repeator
        _spriteOn = serializedObject.FindProperty("_spriteOn");
        _spriteOff = serializedObject.FindProperty("_spriteOff");
        _tickPosLocked = serializedObject.FindProperty("_tickPosLocked");
        _iconRepeaterLocked = serializedObject.FindProperty("_iconRepeaterLocked");

        //base
        base.OnEnable();

        _script = target as RedstoneRepeater;
    }

    public override void OnInspectorGUI()
    {
        //base
        base.OnInspectorGUI();
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(_spriteOn);
        EditorGUILayout.PropertyField(_spriteOff);
        EditorGUILayout.PropertyField(_tickPosLocked);
        EditorGUILayout.PropertyField(_iconRepeaterLocked);

        _script.isLock = EditorGUILayout.Toggle("Is Lock", _script.isLock);


        _script.tickLevel = EditorGUILayout.IntSlider("Tick Level", _script.tickLevel, RedstoneRepeater.MIN_TICK_LEVEL, RedstoneRepeater.MAX_TICK_LEVEL);

        serializedObject.ApplyModifiedProperties();
    }

}
