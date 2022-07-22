using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RedstoneTorch))]
public class RedstoneTorchEditor : RedstoneBaseEditor
{
    private RedstoneTorch _script;

    protected override void OnEnable()
    {
        //base
        base.OnEnable();

        _script = target as RedstoneTorch;
    }

    public override void OnInspectorGUI()
    {
        //base
        base.OnInspectorGUI();
        //serializedObject.Update();
        
        //serializedObject.ApplyModifiedProperties();
    }

}
