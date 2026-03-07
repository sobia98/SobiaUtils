using UnityEngine;

namespace Sobia.SigmaboyProject
{
    public class GirlWander : MonoBehaviour
    {
        private Vector2 targetPoint;
        private float speed;
        private Vector2 bounds;
        private bool isWandering = false;

        public void SetupWander(float npcSpeed, Vector2 areaSize)
        {
            speed = npcSpeed;
            bounds = areaSize / 2f;
            PickNewTarget();
            isWandering = true;
        }

        private void Update()
        {
            if (!isWandering) return;

            // Move toward the target
            transform.position = Vector2.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

            // If reached target, pick a new one
            if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
            {
                PickNewTarget();
            }
        }

        private void PickNewTarget()
        {
            targetPoint = new Vector2(
                Random.Range(-bounds.x, bounds.x),
                Random.Range(-bounds.y, bounds.y)
            );
        }

        private bool isPanicking = false;

        public void SetPanic(bool panic)
        {
            if (panic && !isPanicking)
            {
                isPanicking = true;
                speed /= 3f; // Speed down when inside the aura!
                PickNewTarget(); // Change direction immediately
            }
            else if (!panic && isPanicking)
            {
                isPanicking = false;
                speed *= 3f;
            }
        }
    }
}