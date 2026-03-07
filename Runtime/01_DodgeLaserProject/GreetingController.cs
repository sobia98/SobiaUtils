using Sobia.Utils;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sobia.LaserProject
{
    public class GreetingController : NetworkBehaviour
    {
        [SerializeField] private float PromptDuration = 5f; // How long the prompt stays visible
        [SerializeField] private float ShowInterval = 30f; // How often the prompt shows up
        [SerializeField] private float CooldownTime = 30f; // Cooldown after a successful greet
        [SerializeField] private TextMeshProUGUI PromptText;

        private InputAction GreetingAction;

        private bool IsPromptShowing = false;
        private float Timer;
        private Coroutine HidePromptRoutine;

        private void Awake()
        {
            GreetingAction = InputManager.Instance.InputActions.FindAction("UI/Greeting");

            if (GreetingAction == null)
            {
                Debug.LogError("Greeting action not found in InputActionAsset.");
                return;
            }

            SobiaUtils.IsAssigned(PromptText, nameof(PromptText), gameObject);
        }

        //need to enable/disable the action to work (by default disabled)
        private void OnEnable()
        {
            GreetingAction?.Enable();
            GreetingAction.performed += OnGreet;
        }

        private void OnDisable()
        {
            GreetingAction?.Disable();
            GreetingAction.performed -= OnGreet;
        }

        public override void OnNetworkSpawn()
        {
            //because the attached gameobject is UI Manager and not in Onwership to anyone
            //IsClient runs on all clients
            if (IsClient)
            {
                Timer = ShowInterval;
            }
        }

        private void Update()
        {
            if (!IsClient) return;

            if (!IsPromptShowing)
            {
                Timer -= Time.deltaTime;
                if (Timer <= 0f)
                {
                    ShowPrompt();
                }
            }
        }

        public void OnGreet(InputAction.CallbackContext context)
        {
            if (IsPromptShowing && context.performed)
            {
                HidePrompt();
                GreetServerRpc();

                Timer = CooldownTime;
            }
        }

        private void ShowPrompt()
        {
            IsPromptShowing = true;
            PromptText.gameObject.SetActive(true);
            HidePromptRoutine = StartCoroutine(HidePromptAfterDelay(PromptDuration));
        }

        private void HidePrompt()
        {
            IsPromptShowing = false;
            PromptText.gameObject.SetActive(false);
            if (HidePromptRoutine != null)
            {
                StopCoroutine(HidePromptRoutine);
                HidePromptRoutine = null;
            }
        }

        private IEnumerator HidePromptAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (IsPromptShowing)
            {
                HidePrompt();
                Timer = ShowInterval;
            }
        }

        // --- Networking Section ---

        // This method runs on the server when called by a client
        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void GreetServerRpc()
        {
            PlayGreetingSoundClientRpc();
        }

        // This method runs on ALL clients after being called by the server
        [ClientRpc]
        private void PlayGreetingSoundClientRpc()
        {
            //AudioManager.Instance.PlayGreetingClip();
        }
    }
}