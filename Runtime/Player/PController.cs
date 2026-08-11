using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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
        [SerializeField] private LayerMask PaintableLayer;

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

        [Header("Audio")]
        public AudioSource JumpAndLandAudioSource;

        [Header("OnJump")]
        [SerializeField] private AudioClip[] OnJumpClips;

        [Range(0f, 1f)]
        [SerializeField] private float OnJumpVolume = 0.5f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnJumpPitch = 0.9f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnJumpPitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnJumpCooldown = 0.2f;

        [Header("OnLand")]
        [SerializeField] private AudioClip[] OnLandClips;

        [Range(0f, 1f)]
        [SerializeField] private float OnLandVolume = 0.5f;

        [Range(0.5f, 2f)][SerializeField] private float MinOnLandPitch = 0.9f;
        [Range(0.5f, 2f)][SerializeField] private float MaxOnLandPitch = 1.05f;
        [Range(0.05f, 1.0f)][SerializeField] private float OnLandCooldown = 0.2f;

        [Header("Footstep Audio Settings")]
        [SerializeField] private AudioSource FootstepAudioSource;

        [SerializeField] private AudioClip WalkingClip;

        [Range(0f, 1f)]
        [SerializeField] private float MaxFootstepVolume = 1.0f;

        [Range(0f, 5f)]
        [SerializeField] private float FadeInSpeed = 2.0f; // Slider from 0 to 5

        [Range(0f, 5f)]
        [SerializeField] private float FadeOutSpeed = 4.0f; // Slider from 0 to 5

        [Header("Pitch & Speed Settings")]
        [Tooltip("Base speed/pitch when walking.")]
        [Range(0.1f, 3f)]
        [SerializeField] private float WalkPitch = 1.0f;

        [Tooltip("Speed/pitch when sprinting.")]
        [Range(0.1f, 3f)]
        [SerializeField] private float SprintPitch = 2.0f;

        [Tooltip("How fast the pitch transitions between walking and sprinting.")]
        [Range(1f, 30f)]
        [SerializeField] private float PitchTransitionSpeed = 10.0f;

        private bool _wasGrounded;

        public float CurrentSpeed => Speed;

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

            Animator.fireEvents = false; // to stop sending animation events
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

                // JUMP TRIGGER
                if (Input.Jump && JumpTimeoutDelta <= 0.0f)
                {
                    // 1. Accumulate speed on jump
                    JumpAndLandAudioSource.pitch = Random.Range(MinOnJumpPitch, MaxOnJumpPitch);
                    if (!JumpAndLandAudioSource.isPlaying)
                    {
                        JumpAndLandAudioSource.PlayOneShot(OnJumpClips[Random.Range(0, OnJumpClips.Length)], OnJumpVolume);
                    }
                    Speed += 0.5f;

                    // 2. Cap the speed at 35
                    if (Speed > 35.0f) Speed = 35.0f;

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
            }

            if (VerticalVelocity < TerminalVelocity)
            {
                VerticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void GroundedCheck()
        {
            LayerMask combinedGroundMask = GroundLayers | PaintableLayer;

            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, combinedGroundMask, QueryTriggerInteraction.Ignore);

            // LANDED TRIGGER: Grounded just became true, but wasn't last frame
            if (Grounded && !_wasGrounded)
            {
                PlayLandAudio();
            }

            _wasGrounded = Grounded; // Store state for next frame

            if (HasAnimator) Animator.SetBool(AnimIDGrounded, Grounded);
        }

        private void PlayLandAudio()
        {
            if (OnLandClips == null || OnLandClips.Length == 0 || JumpAndLandAudioSource == null) return;

            JumpAndLandAudioSource.pitch = Random.Range(MinOnLandPitch, MaxOnLandPitch);
            if (!JumpAndLandAudioSource.isPlaying)
            {
                JumpAndLandAudioSource.PlayOneShot(OnLandClips[Random.Range(0, OnLandClips.Length)], OnLandVolume);
            }
        }

        private void Move()
        {
            // Determine the base speed we should be at
            float targetBaseSpeed = Input.Sprint ? SprintSpeed : MoveSpeed;
            if (Input.Move == Vector2.zero) targetBaseSpeed = 0.0f;

            // SPRINT TAKEOVER: If we are sprinting and our current speed is less than SprintSpeed,
            // boost us up to the SprintSpeed immediately.
            if (Input.Sprint && Speed < SprintSpeed && Input.Move != Vector2.zero)
            {
                Speed = SprintSpeed;
            }

            // GROUND DECAY: If we are on the ground and NOT jumping, bleed speed back to targetBaseSpeed
            if (Grounded && !Input.Jump)
            {
                // This ensures that if we stop jumping, we eventually slow down to normal walking/running
                Speed = Mathf.Lerp(Speed, targetBaseSpeed, Time.deltaTime * SpeedChangeRate);
            }

            // Animation handling
            AnimationBlend = Mathf.Lerp(AnimationBlend, targetBaseSpeed, Time.deltaTime * SpeedChangeRate);
            if (AnimationBlend < 0.01f) AnimationBlend = 0f;

            Vector3 inputDirection = new Vector3(Input.Move.x, 0.0f, Input.Move.y).normalized;

            if (Input.Move != Vector2.zero)
            {
                TargetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + MainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetRotation, ref RotationVelocity, RotationSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, TargetRotation, 0.0f) * Vector3.forward;

            // Apply movement
            float moveSpeed = (Input.Move == Vector2.zero) ? 0f : Speed;

            Controller.Move(targetDirection.normalized * (moveSpeed * Time.deltaTime) +
                             new Vector3(0.0f, VerticalVelocity, 0.0f) * Time.deltaTime);

            if (HasAnimator)
            {
                Animator.SetFloat(AnimIDSpeed, AnimationBlend);
                Animator.SetFloat(AnimIDMotionSpeed, Input.AnalogMovement ? Input.Move.magnitude : 1f);
            }

            HandleFootstepAudio();
        }

        private void HandleFootstepAudio()
        {
            if (FootstepAudioSource == null || WalkingClip == null) return;

            // Determine if the player should be making movement sounds
            bool isMovingOnGround = Grounded && Input.Move != Vector2.zero;

            // Target volume is Max Volume if moving on ground, 0 if idle/airborne
            float targetVolume = isMovingOnGround ? MaxFootstepVolume : 0.0f;

            // 1. Ensure clip is assigned and set to loop
            if (FootstepAudioSource.clip != WalkingClip)
            {
                FootstepAudioSource.clip = WalkingClip;
                FootstepAudioSource.loop = true;
            }

            // 2. Start playing at a RANDOM timestamp when starting movement
            if (isMovingOnGround && !FootstepAudioSource.isPlaying)
            {
                // Pick a random starting time within the audio clip
                FootstepAudioSource.time = Random.Range(0f, WalkingClip.length);
                FootstepAudioSource.Play();
            }

            // 3. Handle Walking vs. Sprinting Pitch
            float targetPitch = Input.Sprint ? SprintPitch : WalkPitch;
            FootstepAudioSource.pitch = Mathf.Lerp(
                FootstepAudioSource.pitch,
                targetPitch,
                Time.deltaTime * PitchTransitionSpeed
            );

            // 4. Smoothly transition volume
            float currentFadeSpeed = isMovingOnGround ? FadeInSpeed : FadeOutSpeed;
            FootstepAudioSource.volume = Mathf.MoveTowards(
                FootstepAudioSource.volume,
                targetVolume,
                currentFadeSpeed * Time.deltaTime
            );

            // 5. Stop playback once fully faded out
            if (FootstepAudioSource.volume <= 0.001f && FootstepAudioSource.isPlaying)
            {
                FootstepAudioSource.Stop();
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

        private void OnLand(AnimationEvent animationEvent)
        {
            JumpAndLandAudioSource.pitch = Random.Range(MinOnLandPitch, MaxOnLandPitch);
            if (!JumpAndLandAudioSource.isPlaying)
            {
                JumpAndLandAudioSource.PlayOneShot(OnLandClips[Random.Range(0, OnLandClips.Length)], OnLandVolume);
            }
        }
    }
}