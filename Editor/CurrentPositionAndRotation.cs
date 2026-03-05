using UnityEditor;
using UnityEngine;

namespace Sobia.Utils
{
    public class SceneCamDebug : EditorWindow
    {
        [MenuItem("Window/Position & Rotation")]
        public static void ShowWindow()
        {
            SceneCamDebug window = GetWindow<SceneCamDebug>("Position & Rotation");
            Texture2D icon = EditorGUIUtility.IconContent("d_Camera Icon").image as Texture2D;
            window.titleContent = new GUIContent("Position & Rotation", icon);
        }

        private void OnGUI()
        {
            if (SceneView.lastActiveSceneView != null)
            {
                var cam = SceneView.lastActiveSceneView.camera;
                Vector3 pos = cam.transform.position;
                Vector3 rot = cam.transform.eulerAngles;

                // Position Row
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Vector3Field("Position", pos);
                if (GUILayout.Button("Copy", GUILayout.Width(50)))
                {
                    EditorGUIUtility.systemCopyBuffer = $"new Vector3({pos.x}f, {pos.y}f, {pos.z}f);";
                    Debug.Log($"Copied Position: {pos}");
                }
                EditorGUILayout.EndHorizontal();

                // Rotation Row
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Vector3Field("Rotation", rot);
                if (GUILayout.Button("Copy", GUILayout.Width(50)))
                {
                    EditorGUIUtility.systemCopyBuffer = $"new Vector3({rot.x}f, {rot.y}f, {rot.z}f);";
                    Debug.Log($"Copied Rotation: {rot}");
                }
                EditorGUILayout.EndHorizontal();

                Repaint();
            }
            else
            {
                EditorGUILayout.LabelField("No active Scene View found.");
            }
        }
    }
}