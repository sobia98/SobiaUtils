using Unity.Netcode;
using UnityEngine;

namespace Sobia.Utils
{
    /// <summary>
    /// Assign to a Button
    /// will be used to trigger Events for the Caller with Interact
    /// </summary>
    public class ButtonController : NetworkBehaviour, IInteractable
    {
        [SerializeField] private string TriggerName = "ButtonPressed";
        [SerializeField] private GameObject PuzzleReceiver;
        [SerializeField] private Animator Animator;

        private void Awake()
        {
            SobiaUtils.IsAssigned(PuzzleReceiver, nameof(PuzzleReceiver), gameObject);
            SobiaUtils.IsAssigned(Animator, nameof(Animator), gameObject);
        }

        public void Interact()
        {
            TriggerAnimationButtonRpc();
        }

        // Animation Event
        public void ExecuteTrigger()
        {
            if (!IsServer) return;
            if (PuzzleReceiver.TryGetComponent(out IPuzzleTrigger trigger))
            {
                trigger.OnTriggerActivated(GlobalConstants.NON_RELEVANT_USER);
            }
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void TriggerAnimationButtonRpc()
        {
            Animator.SetTrigger(TriggerName);
        }
    }
}