using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sobia.Utils
{
    public class UISoundElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public float UpdateEveryX = 0.15f;
        private float LastPlayTime;

        private void Start()
        {
        }

        public void OnValueChanged(bool value)
        {
            if (Time.unscaledTime - LastPlayTime >= UpdateEveryX)
            {
                LastPlayTime = Time.unscaledTime;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
        }
    }
}