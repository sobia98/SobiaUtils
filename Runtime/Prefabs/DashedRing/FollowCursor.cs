using UnityEngine;

#if ENABLE_INPUT_SYSTEM

using UnityEngine.InputSystem;

#endif

namespace Sobia.Utils
{
    public class FollowMouse2D : MonoBehaviour
    {
        private void Update()
        {
#if ENABLE_INPUT_SYSTEM //new one
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            Vector3 mousePosition = new Vector3(screenPosition.x, screenPosition.y, 10f);
            transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
#else
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f;
            transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
#endif
        }
    }
}