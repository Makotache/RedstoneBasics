using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RedstoneWire))]
public class RedstoneWireEditor : RedstoneBaseEditor
{
    private SerializedProperty _spriteDustOn;
    private SerializedProperty _spriteDustOff;

    
    private SerializedProperty _spriteSplitOn;
    private SerializedProperty _spriteSplitOff;

    
    private SerializedProperty childSpriteObjDust;
    
    private SerializedProperty childSpriteObjNorth;
    private SerializedProperty childSpriteObjEast;
    private SerializedProperty childSpriteObjSouth;
    private SerializedProperty childSpriteObjWest;

    
    private SerializedProperty _showStrengthSignal;

    private SerializedProperty _currentRedstoneWireType;


    private RedstoneWire _script;

    protected override void OnEnable()
    {
        //wire
        _spriteDustOn = serializedObject.FindProperty("_spriteDustOn");
        _spriteDustOff = serializedObject.FindProperty("_spriteDustOff");


        _spriteSplitOn = serializedObject.FindProperty("_spriteSplitOn");
        _spriteSplitOff = serializedObject.FindProperty("_spriteSplitOff");


        childSpriteObjDust = serializedObject.FindProperty("childSpriteObjDust");

        childSpriteObjNorth = serializedObject.FindProperty("childSpriteObjNorth");
        childSpriteObjEast = serializedObject.FindProperty("childSpriteObjEast");
        childSpriteObjSouth = serializedObject.FindProperty("childSpriteObjSouth");
        childSpriteObjWest = serializedObject.FindProperty("childSpriteObjWest");


        _showStrengthSignal = serializedObject.FindProperty("_showStrengthSignal");

        _currentRedstoneWireType = serializedObject.FindProperty("_currentRedstoneWireType");

        //base
        base.OnEnable();

        _script = target as RedstoneWire;
    }

    public override void OnInspectorGUI()
    {
        //base
        base.OnInspectorGUI();
        serializedObject.Update();
        
        //wire
        EditorGUILayout.PropertyField(_spriteDustOn);
        EditorGUILayout.PropertyField(_spriteDustOff);


        EditorGUILayout.PropertyField(_spriteSplitOn);
        EditorGUILayout.PropertyField(_spriteSplitOff);


        EditorGUILayout.PropertyField(childSpriteObjDust);

        EditorGUILayout.PropertyField(childSpriteObjNorth);
        EditorGUILayout.PropertyField(childSpriteObjEast);
        EditorGUILayout.PropertyField(childSpriteObjSouth);
        EditorGUILayout.PropertyField(childSpriteObjWest);


        EditorGUILayout.PropertyField(_showStrengthSignal);

        EditorGUILayout.PropertyField(_currentRedstoneWireType);

        serializedObject.ApplyModifiedProperties();
    }

}
