using UnityEngine;
using TMPro;

[ExecuteAlways]
public class FPSFontAutoAssign : MonoBehaviour
{
    private TextMeshPro TextMeshPro;

    private void Awake()
    {
        TextMeshPro = GetComponent<TextMeshPro>();
        Debug.Log("Test");

        TMP_FontAsset defaultFont = TMP_Settings.defaultFontAsset;
        if (defaultFont != null)
        {
            TextMeshPro.font = defaultFont;
        }
    }
}