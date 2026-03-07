using Sobia.Utils;
using System.Collections;
using UnityEngine;

namespace Sobia.CODProject
{
    public class PlatformRotator : MonoBehaviour, IActivatable
    {
        [Header("Timing Settings")]
        [SerializeField] private float rotationDuration = 0.25f;

        [SerializeField] private float startDelay = 0.0f;
        [SerializeField] private float activationTime = 6.0f;

        private Coroutine _activeRoutine;
        private bool _isActivated = false;

        public void Activate()
        {
            if (_isActivated) return;

            if (_activeRoutine != null) StopCoroutine(_activeRoutine);
            _activeRoutine = StartCoroutine(ActivationSequence());
        }

        public void Deactivate()
        {
            if (!_isActivated) return;

            if (_activeRoutine != null) StopCoroutine(_activeRoutine);
            _activeRoutine = StartCoroutine(RotateTo(-90f));
            _isActivated = false;
        }

        private IEnumerator ActivationSequence()
        {
            _isActivated = true;

            // 1. Initial Delay
            if (startDelay > 0) yield return new WaitForSeconds(startDelay);

            // 2. Rotate Up (0 degrees)
            yield return StartCoroutine(RotateTo(0f));

            // 3. Stay Flat for the duration
            yield return new WaitForSeconds(activationTime);

            // 4. Automatically Deactivate
            yield return StartCoroutine(RotateTo(-90f));

            _isActivated = false;
            _activeRoutine = null;
        }

        private IEnumerator RotateTo(float targetX)
        {
            Quaternion startRotation = transform.rotation;
            Vector3 targetEuler = new Vector3(targetX, transform.eulerAngles.y, transform.eulerAngles.z);
            Quaternion endRotation = Quaternion.Euler(targetEuler);

            float elapsed = 0;
            while (elapsed < rotationDuration)
            {
                elapsed += Time.deltaTime;
                float percent = Mathf.SmoothStep(0, 1, elapsed / rotationDuration);
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, percent);
                yield return null;
            }

            transform.rotation = endRotation;
        }

        // Add this to your platform script to see distances in the Scene View
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            // Draws a 5-unit circle to show the "Danger Zone" distance
            Gizmos.DrawWireSphere(transform.position, 5f);
        }
    }
}