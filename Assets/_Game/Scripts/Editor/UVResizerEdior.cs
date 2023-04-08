using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UVResizer))]
public class UVResizerEdior : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        UVResizer myScript = (UVResizer)target;
        if (GUILayout.Button("Resize UV's"))
        {
            myScript.setBounds();
        }
    }
}
