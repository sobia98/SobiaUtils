using UnityEngine;
using UnityEngine.EventSystems;

namespace Sobia.Utils
{
    public class UISoundElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float UpdateEveryX = 0.15f;
        private float LastPlayTime = 0f;

        public void OnValueChanged()
        {
            if (Time.unscaledTime - LastPlayTime >= UpdateEveryX)
            {
                LastPlayTime = Time.unscaledTime;
                AudioManager.Instance.PlayUIOnValueChangedSound();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            AudioManager.Instance.PlayUIOnHoverSound();
        }

        public void OnClickSound()
        {
            AudioManager.Instance.PlayUIOnClickSound();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }
    }
}