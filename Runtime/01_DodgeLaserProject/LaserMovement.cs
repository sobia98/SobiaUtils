using UnityEngine;

namespace Sobia.LaserProject
{
    public class LaserMovement : MonoBehaviour
    {
        [SerializeField] private float TravelDistance = 20f;
        [SerializeField] private float TravelTime = 4f;

        private float StartPosition;
        private float TimeElapsed;

        private bool IsMovingOnX;

        private void Start()
        {
            if (Mathf.Approximately(transform.localEulerAngles.y, 90f) || Mathf.Approximately(transform.localEulerAngles.y, 270f))
            {
                IsMovingOnX = true;
                StartPosition = transform.position.x - (TravelDistance / 2f);
            }
            else
            {
                IsMovingOnX = false;
                StartPosition = transform.position.z - (TravelDistance / 2f);
            }

            TimeElapsed = 0f;
        }

        private void Update()
        {
            TimeElapsed += Time.deltaTime;

            float sinValue = Mathf.Sin(TimeElapsed * (2 * Mathf.PI / TravelTime));
            float percentage = (sinValue + 1f) / 2f;

            float newPosition = StartPosition + (percentage * TravelDistance);

            if (IsMovingOnX)
            {
                transform.position = new Vector3(
                    newPosition,
                    transform.position.y,
                    transform.position.z
                );
            }
            else
            {
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    newPosition
                );
            }
        }
    }
}