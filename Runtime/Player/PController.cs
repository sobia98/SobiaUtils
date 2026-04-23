using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

namespace Sobia.Utils
{
    [RequireComponent(typeof(CharacterController))]
    public class PController : MonoBehaviour // Changed from NetworkBehaviour
    {
        [Header("Player")]
        [SerializeField] private bool ThirdPerson = true;

        [SerializeField] private float MoveSpeed = 2.0f;
        [SerializeField] private float SprintSpeed = 5.335f;

        [Tooltip("The lower the faster")]
        [Range(0.0f, 0.3f)]
        [SerializeField] private float RotationSmoothTime = 0.12f;

        [SerializeField] private float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Header("Audio")]
        [SerializeField] private AudioClip LandingAudioClip;

        [SerializeField] private AudioClip[] FootstepAudioClips;
        [Range(0, 1)][SerializeField] private float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Header("Jumping")]
        [SerializeField] private float JumpHeight = 1.2f;

        [SerializeField] private float Gravity = -15.0f;
        [SerializeField] private float JumpTimeout = 0.50f;
        [SerializeField] private float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool Grounded { get; private set; } = true;

        public float GroundedOffset { get; private set; } = -0.14f;
        public float GroundedRadius { get; private set; } = 0.28f;
        [SerializeField] private LayerMask GroundLayers;

        [Header("Cinemachine")]
        [SerializeField] private GameObject CinemachineCamera;

        [SerializeField] private float TopClamp = 70.0f;
        [SerializeField] private float BottomClamp = -30.0f;
        [SerializeField] private float CameraAngleOverride = 0.0f;
        [SerializeField] private bool LockCameraPosition = false;

        // private fields
        private float CinemachineTargetYaw;

        private float CinemachineTargetPitch;
        private float Speed;
        private float AnimationBlend;
        private float TargetRotation = 0.0f;
        private float RotationVelocity;
        private float VerticalVelocity;
        private float TerminalVelocity = 53.0f;
        private float JumpTimeoutDelta;
        private float FallTimeoutDelta;

        // animation IDs
        private int AnimIDSpeed;

        private int AnimIDGrounded;
        private int AnimIDJump;
        private int AnimIDFreeFall;
        private int AnimIDMotionSpeed;

        private Animator Animator;
        private CharacterController Controller;
        private StarterAssetsInputs Input;
        private GameObject MainCamera;

        private const float Threshold = 0.01f;
        private bool HasAnimator;
        private bool IsPaused = false;

        private bool IsCurrentDeviceMouse
        {
            get
            {
                if (InputSystem.devices.Count > 0)
                {
                    var lastDevice = InputSystem.GetDevice<Pointer>();
                    if (lastDevice != null && lastDevice.wasUpdatedThisFrame) return true;
                    return Mouse.current != null && Mouse.current.wasUpdatedThisFrame;
                }
                return false;
            }
        }

        private void Awake()
        {
            if (MainCamera == null) // Standard Unity null check
            {
                MainCamera = GameObject.FindGameObjectWithTag("MainCamera");

                if (MainCamera == null)
                {
                    Debug.LogError("PController: No GameObject with 'MainCamera' tag found in the scene!"); //
                }
            }
        }

        private void OnEnable() // Replaced OnNetworkSpawn
        {
            GameEvents.OnTogglePause += HandlePause;
        }

        private void OnDisable() // Replaced OnNetworkDespawn
        {
            GameEvents.OnTogglePause -= HandlePause;
        }

        private void Start()
        {
            CinemachineTargetYaw = CinemachineCamera.transform.rotation.eulerAngles.y;

            HasAnimator = TryGetComponent(out Animator);
            Controller = GetComponent<CharacterController>();
            Input = GetComponent<StarterAssetsInputs>();

            AssignAnimationIDs();

            JumpTimeoutDelta = JumpTimeout;
            FallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            // Removed IsOwner check
            if (Controller == null || !Controller.enabled) return;

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            if (IsPaused) return; // Removed IsOwner check
            CameraRotation();
        }

        private void HandlePause(bool isPaused)
        {
            IsPaused = isPaused;
            if (IsPaused)
            {
                Input.Look = Vector2.zero;
                Input.Move = Vector2.zero;
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                FallTimeoutDelta = FallTimeout;

                if (HasAnimator)
                {
                    Animator.SetBool(AnimIDJump, false);
                    Animator.SetBool(AnimIDFreeFall, false);
                }

                if (VerticalVelocity < 0.0f)
                {
                    VerticalVelocity = -2f;
                }

                if (Input.Jump && JumpTimeoutDelta <= 0.0f)
                {
                    VerticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    if (HasAnimator) Animator.SetBool(AnimIDJump, true);
                }

                if (JumpTimeoutDelta >= 0.0f)
                {
                    JumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                JumpTimeoutDelta = JumpTimeout;
                if (FallTimeoutDelta >= 0.0f)
                {
                    FallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (HasAnimator) Animator.SetBool(AnimIDFreeFall, true);
                }

                Input.Jump = false;
            }

            if (VerticalVelocity < TerminalVelocity)
            {
                VerticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

            if (HasAnimator) Animator.SetBool(AnimIDGrounded, Grounded);
        }

        private void Move()
        {
            float targetSpeed = Input.Sprint ? SprintSpeed : MoveSpeed;
            if (Input.Move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(Controller.velocity.x, 0.0f, Controller.velocity.z).magnitude;
            float speedOffset = 0.1f;
            float inputMagnitude = Input.AnalogMovement ? Input.Move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                Speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                Speed = Mathf.Round(Speed * 1000f) / 1000f;
            }
            else
            {
                Speed = targetSpeed;
            }

            AnimationBlend = Mathf.Lerp(AnimationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (AnimationBlend < 0.01f) AnimationBlend = 0f;

            Vector3 inputDirection = new Vector3(Input.Move.x, 0.0f, Input.Move.y).normalized;

            if (Input.Move != Vector2.zero)
            {
                TargetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + MainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetRotation, ref RotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, TargetRotation, 0.0f) * Vector3.forward;

            Controller.Move(targetDirection.normalized * (Speed * Time.deltaTime) + new Vector3(0.0f, VerticalVelocity, 0.0f) * Time.deltaTime);

            if (HasAnimator)
            {
                Animator.SetFloat(AnimIDSpeed, AnimationBlend);
                Animator.SetFloat(AnimIDMotionSpeed, inputMagnitude);
            }
        }

        private void AssignAnimationIDs()
        {
            AnimIDSpeed = Animator.StringToHash("Speed");
            AnimIDGrounded = Animator.StringToHash("Grounded");
            AnimIDJump = Animator.StringToHash("Jump");
            AnimIDFreeFall = Animator.StringToHash("FreeFall");
            AnimIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void CameraRotation()
        {
            if (Input.Look.sqrMagnitude >= Threshold && !LockCameraPosition)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                CinemachineTargetYaw += Input.Look.x * deltaTimeMultiplier;
                CinemachineTargetPitch += Input.Look.y * deltaTimeMultiplier;
            }

            CinemachineTargetYaw = ClampAngle(CinemachineTargetYaw, float.MinValue, float.MaxValue);
            CinemachineTargetPitch = ClampAngle(CinemachineTargetPitch, BottomClamp, TopClamp);

            CinemachineCamera.transform.rotation = Quaternion.Euler(CinemachineTargetPitch + CameraAngleOverride, CinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(Controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(Controller.center), FootstepAudioVolume);
            }
        }
    }
}