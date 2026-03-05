using UnityEngine;
using UnityEngine.EventSystems;

namespace Sobia.Utils
{
    public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private string Header;
        [SerializeField] private string Content;

        public void OnPointerEnter(PointerEventData eventData)
        {
            TooltipManager.Instance.Show(Content, Header);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            TooltipManager.Instance.Hide();
        }
    }
}