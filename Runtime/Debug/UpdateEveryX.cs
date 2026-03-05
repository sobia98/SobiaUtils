using UnityEngine;

namespace Sobia.Utils
{
    public class UpdateEveryX : MonoBehaviour
    {
        [SerializeField] private float IntervalX = 2f;
        [SerializeField] private GameObject TargetObject;

        private float Timer = 0f;
        private ILoggableState Provider;

        private void Start()
        {
            if (TargetObject != null)
                Provider = TargetObject.GetComponent<ILoggableState>();
        }

        private void Update()
        {
            Timer += Time.unscaledDeltaTime;

            if (Provider != null && Timer >= IntervalX)
            {
                Provider.LogEveryX();
                Timer = 0f;
            }
        }
    }
}