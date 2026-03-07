using UnityEngine;

namespace Sobia.SigmaboyProject
{
    public class Pulse : MonoBehaviour
    {
        [Header("Growth Settings")]
        public float baseSize = 5.0f;

        public float targetGrowth = 1.0f; // How much it adds to the base
        public float speed = 2.0f;

        private float currentSize;

        private void Start()
        {
            // Start at the base size
            currentSize = baseSize;
            transform.localScale = new Vector3(currentSize, currentSize, 1f);
        }

        private void Update()
        {
            float maxSize = baseSize + targetGrowth;

            // Only increase if we haven't reached the max yet
            if (currentSize < maxSize)
            {
                // Move currentSize towards maxSize at a steady speed
                currentSize = Mathf.MoveTowards(currentSize, maxSize, speed * Time.deltaTime);

                // Apply the new scale
                transform.localScale = new Vector3(currentSize, currentSize, 1f);
            }
        }
    }
}