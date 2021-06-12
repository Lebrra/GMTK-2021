using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RobotAttack))]
public class AttackEditor : Editor
{
    RobotAttack myScript;

    public void OnSceneGUI()
    {
        myScript = this.target as RobotAttack;
        if (!myScript.shootPosition) myScript.shootPosition = myScript.transform;

        Handles.color = Color.blue;
        Handles.DrawWireDisc(myScript.transform.position, myScript.shootPosition.up, myScript.range);

        Handles.color = Color.green;
        Handles.DrawWireDisc(myScript.shootPosition.position, myScript.shootPosition.up, myScript.projectileRange);
    }
}
