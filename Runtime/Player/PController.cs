using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

namespace Sobia.Utils
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class PController : NetworkBehaviour
    {
        [Header("Player")]
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

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        [SerializeField] private float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        [SerializeField] private float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        [SerializeField] private float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded { get; private set; } = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset { get; private set; } = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius { get; private set; } = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        [SerializeField] private LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        [SerializeField] private GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        [SerializeField] private float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        [SerializeField] private float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        [SerializeField] private float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        [SerializeField] private bool LockCameraPosition = false;

        // private fields
        // cinemachine
        private float CinemachineTargetYaw;

        private float CinemachineTargetPitch;

        // player
        private float Speed;

        private float AnimationBlend;
        private float TargetRotation = 0.0f;
        private float RotationVelocity;
        private float VerticalVelocity;
        private float TerminalVelocity = 53.0f;

        // timeout deltatime
        private float JumpTimeoutDelta;

        private float FallTimeoutDelta;

        // animation IDs
        private int AnimIDSpeed;

        private int AnimIDGrounded;
        private int AnimIDJump;
        private int AnimIDFreeFall;
        private int AnimIDMotionSpeed;
        private int _animIDPressingButton;

        private PlayerInput PlayerInput;
        private Animator Animator;
        private CharacterController Controller;
        private StarterAssetsInputs Input;
        private GameObject MainCamera;

        private const float Threshold = 0.01f;

        private bool HasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return PlayerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private void Awake()
        {
            if (MainCamera is null)
            {
                MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            CinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            HasAnimator = TryGetComponent(out Animator); // in case we use Animator
            Controller = GetComponent<CharacterController>();
            Input = GetComponent<StarterAssetsInputs>();
            PlayerInput = GetComponent<PlayerInput>();

            AssignAnimationIDs();

            // reset our timeouts on start
            JumpTimeoutDelta = JumpTimeout;
            FallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            if (!IsOwner) return;

            // Don't update movement if controller is disabled (e.g., during spawn positioning)
            if (Controller == null || !Controller.enabled) return;

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                FallTimeoutDelta = FallTimeout;

                if (HasAnimator)
                {
                    Animator.SetBool(AnimIDJump, false);
                    Animator.SetBool(AnimIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (VerticalVelocity < 0.0f)
                {
                    VerticalVelocity = -2f;
                }

                // Jump
                if (Input.Jump && JumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    VerticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (HasAnimator)
                    {
                        Animator.SetBool(AnimIDJump, true);
                    }
                }

                // jump timeout
                if (JumpTimeoutDelta >= 0.0f)
                {
                    JumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                JumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (FallTimeoutDelta >= 0.0f)
                {
                    FallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (HasAnimator)
                    {
                        Animator.SetBool(AnimIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                Input.Jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (VerticalVelocity < TerminalVelocity)
            {
                VerticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            if (HasAnimator)
            {
                Animator.SetBool(AnimIDGrounded, Grounded);
            }
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = Input.Sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (Input.Move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(Controller.velocity.x, 0.0f, Controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = Input.AnalogMovement ? Input.Move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                Speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                Speed = Mathf.Round(Speed * 1000f) / 1000f;
            }
            else
            {
                Speed = targetSpeed;
            }

            AnimationBlend = Mathf.Lerp(AnimationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (AnimationBlend < 0.01f) AnimationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(Input.Move.x, 0.0f, Input.Move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (Input.Move != Vector2.zero)
            {
                TargetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  MainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetRotation, ref RotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, TargetRotation, 0.0f) * Vector3.forward;

            // move the player
            Controller.Move(targetDirection.normalized * (Speed * Time.deltaTime) +
                             new Vector3(0.0f, VerticalVelocity, 0.0f) * Time.deltaTime);

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
            // if there is an input and camera position is not fixed
            if (Input.Look.sqrMagnitude >= Threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                CinemachineTargetYaw += Input.Look.x * deltaTimeMultiplier;
                CinemachineTargetPitch += Input.Look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            CinemachineTargetYaw = ClampAngle(CinemachineTargetYaw, float.MinValue, float.MaxValue);
            CinemachineTargetPitch = ClampAngle(CinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(CinemachineTargetPitch + CameraAngleOverride,
                CinemachineTargetYaw, 0.0f);
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
                }
            }

            if (!IsOwner)
            {
                if (TryGetComponent<StarterAssetsInputs>(out StarterAssetsInputs inputs))
                {
                    inputs.enabled = false;
                }

                if (TryGetComponent<PlayerInput>(out PlayerInput playerInput))
                {
                    playerInput.enabled = false;
                }
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }
    }
}