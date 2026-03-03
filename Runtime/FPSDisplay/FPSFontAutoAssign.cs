using UnityEngine;
using TMPro;

[ExecuteAlways]
public class FPSFontAutoAssign : MonoBehaviour
{
    private TextMeshProUGUI TextMeshPro;

    private void Awake()
    {
        TextMeshPro = GetComponent<TextMeshProUGUI>();
        TMP_FontAsset defaultFont = TMP_Settings.defaultFontAsset;
        if (defaultFont != null)
        {
            TextMeshPro.font = defaultFont;
        }
    }
}