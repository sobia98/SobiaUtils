using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Sobia.Utils
{
    public class TooltipManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI HeaderField;
        [SerializeField] private TextMeshProUGUI ContentField;
        [SerializeField] private LayoutElement LayoutElement;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private int CharacterLimit = 250;

        public static TooltipManager Instance;

        private void Awake()
        {
            CheckReferences();
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            rectTransform = GetComponent<RectTransform>();
            gameObject.SetActive(false);
        }

        private void CheckReferences()
        {
            SobiaUtils.IsAssigned(HeaderField, nameof(HeaderField), gameObject);
            SobiaUtils.IsAssigned(ContentField, nameof(ContentField), gameObject);
            SobiaUtils.IsAssigned(LayoutElement, nameof(LayoutElement), gameObject);
            SobiaUtils.IsAssigned(rectTransform, nameof(rectTransform), gameObject);
        }

        private void Update()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            // Pivot adjustments to prevent tooltip from being under the cursor
            float pivotX = mousePosition.x / Screen.width;
            float pivotY = mousePosition.y / Screen.height;
            rectTransform.pivot = new Vector2(pivotX, pivotY);

            transform.position = mousePosition;
        }

        public void Show(string content, string header = "")
        {
            HeaderField.gameObject.SetActive(!string.IsNullOrEmpty(header));
            HeaderField.text = header;
            ContentField.text = content;

            // Auto-size adjustment
            int headerLength = HeaderField.text.Length;
            int contentLength = ContentField.text.Length;
            LayoutElement.enabled = (headerLength > CharacterLimit || contentLength > CharacterLimit);

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}