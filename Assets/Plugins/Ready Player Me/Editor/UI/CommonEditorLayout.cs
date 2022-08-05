using System;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe
{
    public class CommonEditorLayout : IDisposable
    {
        public CommonEditorLayout()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical("Box", GUILayout.Width(512));
        }

        public void Dispose()
        {
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}
