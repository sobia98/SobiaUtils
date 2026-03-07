using UnityEngine;

using Unity.Cinemachine;
using UnityEngine.InputSystem;
using Sobia.Utils;

namespace Sobia.CODProject
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [SerializeField] private float ZoomSpeed = 2f;
        [SerializeField] private float ZoomLerpSpeed = 10f;
        [SerializeField] private float MinDistance = 3f;
        [SerializeField] private float MaxDistance = 15f;

        private InputAction MouseZoomAction;

        private CinemachineCamera Cam;
        private CinemachineOrbitalFollow Orbital;
        private Vector2 ScrollDelta;

        private float TargetZoom;
        private float CurrentZoom;

        // made by a youtuber; is stable and works good for basic games and third person games

        private void Awake()
        {
            Cam = GetComponent<CinemachineCamera>();
            Orbital = Cam.GetComponent<CinemachineOrbitalFollow>();
        }

        private void Start()
        {
            SobiaUtils.IsAssigned(Cam, nameof(Cam), gameObject);
            SobiaUtils.IsAssigned(Orbital, nameof(Orbital), gameObject);

            MouseZoomAction = InputManager.Instance.InputActions.FindAction("CameraControls/MouseZoom");
            if (MouseZoomAction != null)
            {
                Debug.LogError("CameraControls/MouseZoom Input Action not found");
                return;
            }

            MouseZoomAction.Enable();
            MouseZoomAction.performed += HandleMouseScroll;

            TargetZoom = CurrentZoom = Orbital.Radius;
        }

        private void HandleMouseScroll(InputAction.CallbackContext context)
        {
            ScrollDelta = context.ReadValue<Vector2>();
        }

        private void Update()
        {
            if (ScrollDelta.y != 0)
            {
                if (Orbital != null)
                {
                    TargetZoom = Mathf.Clamp(Orbital.Radius - ScrollDelta.y * ZoomSpeed, MinDistance, MaxDistance);
                    ScrollDelta = Vector2.zero;
                }
            }

            CurrentZoom = Mathf.Lerp(CurrentZoom, TargetZoom, Time.deltaTime * ZoomLerpSpeed);
            Orbital.Radius = CurrentZoom;
        }
    }
}