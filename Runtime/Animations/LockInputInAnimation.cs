using UnityEngine;

namespace Sobia.Utils
{
    public class LockInputInAnimation : StateMachineBehaviour
    {
        [SerializeField] private string MapToDisable = "Player";

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var mapAction = InputManager.Instance.InputActions.FindActionMap(MapToDisable);
            if (mapAction != null)
            {
                mapAction.Disable();
            }
            else
            {
                Debug.LogError($"LockInputBehaviour: Could not find Action Map '{MapToDisable}'");
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            var mapAction = InputManager.Instance.InputActions.FindActionMap(MapToDisable);
            if (mapAction != null)
            {
                mapAction.Enable();
            }
            else
            {
                Debug.LogError($"LockInputBehaviour: Could not find Action Map '{MapToDisable}'");
            }
        }
    }
}