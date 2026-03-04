using UnityEngine;
using TMPro;
using System.Collections;

namespace Sobia.Utils
{
    public class PopupManager : MonoBehaviour
    {
        [SerializeField] private GameObject PopupParent;
        [SerializeField] private float PopupOffsetY = 1.5f;
        [SerializeField] private float PopupFontSize = 6f;
        [SerializeField] private Color PopupFontColor = Color.red;
        [SerializeField] private int PopupSortingOrder = 1000;
        [SerializeField] private float PopupDuration = 2f;
        [SerializeField] private float PopupRisingEffect = 2f;

        public static PopupManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            SobiaUtils.IsAssigned(PopupParent, nameof(PopupParent), gameObject);
        }

        /// <summary>
        /// Displays a popup text above the specified target GameObject.
        /// </summary>
        /// <remarks>The popup text is instantiated as a new GameObject and positioned above the target.
        /// The appearance of the text can be customized through properties such as font size and color. The popup will
        /// be animated and automatically removed after a certain duration.</remarks>
        /// <param name="target">The GameObject above which the popup text will be displayed. Must not be null.</param>
        /// <param name="text">The text content to be displayed in the popup. This string will be rendered as the popup's message.</param>
        public void ShowPopupText(GameObject target, string text)
        {
            if (target == null) return;

            GameObject popupObj = new GameObject("PopupText");
            popupObj.transform.SetParent(PopupParent.transform);
            popupObj.transform.position = target.transform.position + Vector3.up * PopupOffsetY;

            TextMeshPro textMesh = popupObj.AddComponent<TextMeshPro>();
            textMesh.text = text;
            textMesh.fontSize = PopupFontSize;
            textMesh.color = PopupFontColor;
            textMesh.alignment = TextAlignmentOptions.Center;

            Renderer renderer = popupObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = PopupSortingOrder;
            }

            StartCoroutine(AnimatePopupText(popupObj, target.transform));
        }

        /// <summary>
        /// Animates a popup text object by moving it upward and fading it out over a specified duration.
        /// </summary>
        /// <remarks>The animation duration and rising effect are controlled by the PopupDuration and
        /// PopupRisingEffect fields. The popup text is destroyed after the animation completes.</remarks>
        /// <param name="popupObj">The GameObject representing the popup text to be animated. Must not be null.</param>
        /// <param name="targetTransform">The Transform that determines the target position for the rising effect. If null, the popup text rises at a
        /// constant speed.</param>
        /// <returns>An enumerator that performs the animation over time when used in a coroutine.</returns>
        private IEnumerator AnimatePopupText(GameObject popupObj, Transform targetTransform)
        {
            TextMeshPro textMesh = popupObj.GetComponent<TextMeshPro>();
            float elapsed = 0f;
            var targetFixedPosition = popupObj.transform.position;

            float verticalOffset = -1f;

            while (elapsed < PopupDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / PopupDuration;

                if (targetTransform != null)
                {
                    float risingEffect = progress * PopupRisingEffect;
                    popupObj.transform.position = targetFixedPosition + Vector3.up * (verticalOffset + risingEffect);
                }
                else
                {
                    popupObj.transform.position += Vector3.up * Time.deltaTime * 2.0f;
                }

                if (textMesh != null)
                {
                    Color color = textMesh.color;
                    color.a = 1f - progress;
                    textMesh.color = color;
                }

                yield return null;
            }

            Destroy(popupObj);
        }
    }
}