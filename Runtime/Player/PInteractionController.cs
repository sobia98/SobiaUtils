using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sobia.Utils
{
    public class PInteractionController : NetworkBehaviour
    {
        [Header("Assign References")]
        [SerializeField] private GameObject RayCastRoot;

        [Header("Interaction Settings")]
        [SerializeField] private Vector3 SpawnPosition = new Vector3(65f, -14f, -120f);

        [SerializeField] private float DelayButtonPress = 0.75f;
        [SerializeField] private float DelayBackflip = 0.75f;
        [SerializeField] private float InteractionRange = 3f;
        [SerializeField] private LayerMask InteractableLayer;

        [Header("Actions Map")]
        [SerializeField] private string PressButtonActionName = "PressingButton";

        [SerializeField] private string BackflipActionName = "Backflip";

        private int AnimIDPressingButton;
        private int AnimIDBackflip;

        private IInteractable CurrentInteractable;
        private IInteractable PendingInteractable;
        private TextMeshProUGUI InteractionText;
        private Animator Animator;
        private PController PController;
        private CharacterController CharacterController;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            PController = GetComponent<PController>();
            CharacterController = GetComponent<CharacterController>();

            SobiaUtils.IsAssigned(RayCastRoot, nameof(RayCastRoot), gameObject);
            SobiaUtils.IsAssigned(Animator, nameof(Animator), gameObject);
            SobiaUtils.IsAssigned(PController, nameof(PController), gameObject);
            SobiaUtils.IsAssigned(CharacterController, nameof(CharacterController), gameObject);
        }

        private void Start()
        {
            AnimIDPressingButton = Animator.StringToHash("PressingButton");
            AnimIDBackflip = Animator.StringToHash("Backflip");

            //temporrary solution to find inactive UI element and give feedback
            TextMeshProUGUI[] allUI = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();

            foreach (var ui in allUI)
            {
                if (ui.name == "Interaction")
                {
                    InteractionText = ui;
                    break;
                }
            }
        }

        private void Update()
        {
            if (!IsOwner) return;

            HandleInteractionDetection();
        }

        private void HandleInteractionDetection()
        {
            Ray ray = new Ray(RayCastRoot.transform.position, RayCastRoot.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, InteractionRange, InteractableLayer))
            {
                if (hit.collider.TryGetComponent(out IInteractable interactable))
                {
                    if (CurrentInteractable != interactable)
                    {
                        CurrentInteractable = interactable;
                        InteractionText.gameObject.SetActive(true);
                    }
                    return;
                }
            }

            if (CurrentInteractable != null)
            {
                CurrentInteractable = null;
                InteractionText.gameObject.SetActive(false);
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            // The camera should only follow the player instance belonging to this client.
            if (IsOwner)
            {
                // Lock and hide cursor when player spawns
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                // Set spawn position using coroutine to handle timing and CharacterController
                StartCoroutine(SetSpawnPositionCoroutine());
            }
        }

        private IEnumerator SetSpawnPositionCoroutine()
        {
            // Wait one frame to ensure NetworkObject are fully initialized
            yield return null;
            CharacterController.enabled = false;
            transform.position = SpawnPosition;
            // Wait one more frame to ensure position is set
            yield return null;
            CharacterController.enabled = true;
        }

        private void ButtonPressed(InputAction.CallbackContext context)
        {
            if (!IsOwner || !PController.Grounded || CurrentInteractable == null) return;

            // Store the reference for the Animation Event to use, needed
            PendingInteractable = CurrentInteractable;

            StartCoroutine(PlayButtonPressAnimationWithDelay(DelayButtonPress));
        }

        private IEnumerator PlayButtonPressAnimationWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            Animator.SetTrigger(AnimIDPressingButton);
        }

        // THIS is called by the Animation Event at the exact frame the hand hits the button
        public void ExecuteInteractionEvent()
        {
            if (PendingInteractable != null)
            {
                PendingInteractable.Interact();
                PendingInteractable = null; // Clear it
            }
        }

        private void BackflipPressed(InputAction.CallbackContext context)
        {
            if (!IsOwner || !PController.Grounded) return;

            StartCoroutine(PlayBackflipAnimationWithDelay(DelayBackflip));
        }

        private IEnumerator PlayBackflipAnimationWithDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            Animator.SetTrigger(AnimIDBackflip);
        }

        // THIS is called by the Animation Event at the exact frame the Feet tocuhes the floor (Backflip)
        public void ExecuteBackflipTouchGround()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.5f);
            foreach (Collider hit in hitColliders)
            {
                //Do Somethingl, tell someone player did backflip
            }
        }

        public void MoveToStartClient()
        {
            CharacterController.enabled = false;
            transform.position = SpawnPosition;
            CharacterController.enabled = true;
        }

        public void SetCheckpoint(Vector3 newPoint)
        {
            SpawnPosition = newPoint;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Button"))
            {
                if (other.TryGetComponent(out IInteractable interactable))
                {
                    CurrentInteractable = interactable;
                    // **Optional:** Display a UI prompt (e.g., "Press [E]")
                }
            }
            else if (other.gameObject.CompareTag("KillZone"))
            {
                MoveToStartClient();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Button"))
            {
                if (other.TryGetComponent(out IInteractable interactable) && interactable == CurrentInteractable)
                {
                    CurrentInteractable = null;
                    // **Optional:** Hide the UI prompt
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (RayCastRoot == null) return;

            Gizmos.color = (CurrentInteractable != null) ? Color.green : Color.red;

            Vector3 direction = RayCastRoot.transform.forward * InteractionRange;
            Vector3 startPosition = RayCastRoot.transform.position;

            Gizmos.DrawRay(startPosition, direction);
            Gizmos.DrawWireSphere(startPosition + direction, 0.05f);
        }

        private void OnEnable()
        {
            var pressButtonName = "Player/" + PressButtonActionName;
            var pressButtonAction = InputManager.Instance.InputActions.FindAction(pressButtonName);
            if (pressButtonAction != null)
            {
                pressButtonAction?.Enable();
                pressButtonAction.performed += ButtonPressed;
            }
            else
            {
                Debug.Log("Action in Input Asset Not Found!");
            }

            var backflipName = "Player/" + BackflipActionName;
            var backflipAction = InputManager.Instance.InputActions.FindAction(backflipName);
            if (backflipAction != null)
            {
                backflipAction?.Enable();
                backflipAction.performed += BackflipPressed;
            }
            else
            {
                Debug.Log("Action in Input Asset Not Found!");
            }
        }

        private void OnDisable()
        {
            var pressButtonName = "Player/" + PressButtonActionName;
            var pressButtonAction = InputManager.Instance.InputActions.FindAction(pressButtonName);
            if (pressButtonAction != null)
            {
                pressButtonAction?.Disable();
                pressButtonAction.performed -= ButtonPressed;
            }
            else
            {
                Debug.Log("Action in Input Asset Not Found!");
            }

            var backflipName = "Player/" + BackflipActionName;
            var backflipAction = InputManager.Instance.InputActions.FindAction(backflipName);
            if (backflipAction != null)
            {
                backflipAction?.Disable();
                backflipAction.performed -= BackflipPressed;
            }
            else
            {
                Debug.Log("Action in Input Asset Not Found!");
            }
        }
    }
}