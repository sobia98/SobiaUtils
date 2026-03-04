using UnityEngine;
using UnityEngine.InputSystem;

namespace Sobia.Utils
{
    /// <summary>
    /// need to assign the c# generated class or the .inputactions
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public InputActionAsset InputActions;
        public static InputManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            SobiaUtils.IsAssigned(InputActions, nameof(InputActions), gameObject);
        }

        private void OnEnable()
        {
            InputActions.Enable();
        }

        private void OnDisable()
        {
            InputActions.Disable();
        }

        public void DisableInput() => InputActions.Disable();

        public void EnableInput() => InputActions.Enable();
    }
}