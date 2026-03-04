using UnityEngine;
using UnityEditor;
using System.IO;

namespace Sobia.Utils
{
    public class CreateDashTexture
    {
        [MenuItem("Tools/Create Dash Texture")]
        public static void CreateTexture()
        {
            // Create a 2x1 texture
            Texture2D tex = new Texture2D(2, 1, TextureFormat.RGBA32, false);

            // Set pixels: [0] is White, [1] is Transparent
            tex.SetPixel(0, 0, Color.white);
            tex.SetPixel(1, 0, new Color(0, 0, 0, 0)); // Transparent

            tex.Apply();

            // Save it to your Assets folder
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/DashTexture.png", bytes);
            AssetDatabase.Refresh();

            Debug.Log("Dash Texture created at Assets/DashTexture.png");
        }
    }
}