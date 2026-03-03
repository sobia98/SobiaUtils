using UnityEngine;

namespace Sobia.Utils
{
    public class UpdateEveryX : MonoBehaviour
    {
        public float IntervalX = 2f;
        public GameObject TargetObject;

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