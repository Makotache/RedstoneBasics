using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RedstoneBase))]
public class RedstoneBaseEditor : Editor
{
    private SerializedProperty _isSpecial;
    private SerializedProperty _isLikeASource;
    private SerializedProperty _canBeConfigurate;
    private SerializedProperty _canBeTurn;
    private SerializedProperty canBeReplace;
    private SerializedProperty _forceConnectToIt;

    private SerializedProperty _connectorCount;

    private SerializedProperty tileSpriteOn;
    private SerializedProperty tileSpriteOff;

    private SerializedProperty childSpriteObj;

    private RedstoneBase _scriptRedstoneBase;

   

    protected virtual void OnEnable()
    {
        _isSpecial = serializedObject.FindProperty("_isSpecial");
        _isLikeASource = serializedObject.FindProperty("_isLikeASource");
        _canBeConfigurate = serializedObject.FindProperty("_canBeConfigurate");
        _canBeTurn = serializedObject.FindProperty("_canBeTurn");
        canBeReplace = serializedObject.FindProperty("canBeReplace");
        _forceConnectToIt = serializedObject.FindProperty("_forceConnectToIt");

        _connectorCount = serializedObject.FindProperty("_connectorCount");

        tileSpriteOn = serializedObject.FindProperty("tileSpriteOn");
        tileSpriteOff = serializedObject.FindProperty("tileSpriteOff");

        childSpriteObj = serializedObject.FindProperty("childSpriteObj");

        _scriptRedstoneBase = target as RedstoneBase;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        /*EditorGUILayout.PropertyField(_isLikeASource);
        EditorGUILayout.PropertyField(_canBeConfigurate);
        EditorGUILayout.PropertyField(_canBeTurn);
        EditorGUILayout.PropertyField(_canBeReplace);*/


        _isSpecial.boolValue = EditorGUILayout.Toggle("Is special", _isSpecial.boolValue);
        _isLikeASource.boolValue = EditorGUILayout.Toggle("Is like a source", _isLikeASource.boolValue);
        _canBeConfigurate.boolValue = EditorGUILayout.Toggle("Can be configurate", _canBeConfigurate.boolValue);
        _canBeTurn.boolValue = EditorGUILayout.Toggle("Can be turn", _canBeTurn.boolValue);
        canBeReplace.boolValue = EditorGUILayout.Toggle("Can be replace", canBeReplace.boolValue);
        _forceConnectToIt.boolValue = EditorGUILayout.Toggle("Force connect to it", _forceConnectToIt.boolValue);

        EditorGUILayout.PropertyField(_connectorCount);

        _scriptRedstoneBase.strengthSignal = EditorGUILayout.IntSlider("Strength Signal", _scriptRedstoneBase.strengthSignal, RedstoneRepeater.MIN_STRENGTH_SIGNAL, RedstoneRepeater.MAX_STRENGTH_SIGNAL);


        EditorGUILayout.PropertyField(tileSpriteOn);
        EditorGUILayout.PropertyField(tileSpriteOff);
        EditorGUILayout.PropertyField(childSpriteObj);

        CPCS_Editor.ReadDictionaryInEditor(_scriptRedstoneBase.inputOutputConnector_dict);
 
        serializedObject.ApplyModifiedProperties();
    }

}
