using Sobia.Utils;
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sobia.LaserProject
{
    public class PlayerMovementHandler : NetworkBehaviour
    {
        [SerializeField] private Transform CameraTransform; // Assigned in OnNetworkSpawn for the local player
        [SerializeField] private bool ShouldFaceMoveDirection = false;
        [SerializeField] private float Gravity = -9.8f;

        private CharacterController CharacterController;
        private Vector2 MoveInput;
        private Vector3 Velocity;

        private void Awake()
        {
            CharacterController = GetComponent<CharacterController>();
        }

        private void Start()
        {
            SobiaUtils.IsAssigned(CharacterController, nameof(CharacterController), gameObject);
            SobiaUtils.IsAssigned(CameraTransform, nameof(CameraTransform), gameObject);
        }

        private void Update()
        {
            if (!IsOwner) return;

            Vector3 forward = CameraTransform.forward;
            Vector3 right = CameraTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = forward * MoveInput.y + right * MoveInput.x;
            CharacterController.Move(moveDirection * GameManager.Instance.PlayerMovementSettings.Value.Speed * Time.deltaTime);

            if (ShouldFaceMoveDirection && moveDirection.sqrMagnitude > 0.001f)
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
            }

            Velocity.y += Gravity * Time.deltaTime;
            CharacterController.Move(Velocity * Time.deltaTime);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // The camera should only follow the player instance belonging to this client.
            if (IsOwner)
            {
                CinemachineCamera vcam = FindFirstObjectByType<CinemachineCamera>();

                if (vcam != null)
                {
                    vcam.Follow = transform;
                    CameraTransform = vcam.transform;
                }
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
            if (context.phase == InputActionPhase.Started)
            {
                //AudioManager.Instance.PlayWalkingSound();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                //AudioManager.Instance.StopWalkingSound();
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && CharacterController.isGrounded)
            {
                Velocity.y = Mathf.Sqrt(GameManager.Instance.PlayerMovementSettings.Value.JumpHeight * -2f * Gravity);
                //AudioManager.Instance.PlayJumpClip();
            }
        }

        public void MoveToStartClient()
        {
            Vector3 startPosition = new Vector3(0f, 1f, 0f);
            CharacterController.enabled = false;
            transform.position = startPosition;
            CharacterController.enabled = true;
        }
    }
}