using System.Collections;
using UnityEngine;

namespace Sobia.Utils
{
    public class RotateObject : MonoBehaviour, IActivatable
    {
        public enum Axis
        { X, Y, Z }

        public enum AxisDirection
        { Plus, Minus }

        [SerializeField] private Axis RotationAxis = Axis.X;
        [SerializeField] private AxisDirection RotationAxisDirection = AxisDirection.Minus;
        [SerializeField] private float RotationDuration = 0.25f;
        [SerializeField] private float RotationDegree = 90f;
        [SerializeField] private float StartDelay = 0.0f;
        [SerializeField] private float ObjectActivationTime = 6.0f;
        [SerializeField] private float GizmosRadius = 4.0f;
        [SerializeField] private bool IsGizmos = false;
        [SerializeField] private bool RotationBack = true;

        private Coroutine ActiveRoutine;
        private bool IsActivated = false;
        private Quaternion originalRotation;

        private void Awake()
        {
            originalRotation = transform.rotation;
        }

        public void Activate()
        {
            if (IsActivated) return;

            if (ActiveRoutine != null)
            {
                StopCoroutine(ActiveRoutine);
            }

            ActiveRoutine = StartCoroutine(ActivationSequence());
        }

        public void Deactivate()
        {
            if (ActiveRoutine != null)
            {
                StopCoroutine(ActiveRoutine);
            }

            ActiveRoutine = StartCoroutine(RotateToOriginal());
            IsActivated = false;
        }

        private IEnumerator ActivationSequence()
        {
            IsActivated = true;

            if (StartDelay > 0) yield return new WaitForSeconds(StartDelay);

            float rotationDegree;
            if (RotationAxisDirection == AxisDirection.Plus)
            {
                rotationDegree = RotationDegree;
            }
            else
            {
                rotationDegree = -RotationDegree;
            }

            yield return StartCoroutine(RotateTo(rotationDegree));

            yield return new WaitForSeconds(ObjectActivationTime);

            if (RotationBack)
            {
                Deactivate();
            }
        }

        private IEnumerator RotateTo(float targetOffset)
        {
            Vector3 targetEuler = originalRotation.eulerAngles;
            if (RotationAxis == Axis.X)
            {
                targetEuler.x += targetOffset;
            }
            else if (RotationAxis == Axis.Y)
            {
                targetEuler.y += targetOffset;
            }
            else if (RotationAxis == Axis.Z)
            {
                targetEuler.z += targetOffset;
            }

            yield return StartCoroutine(PerformRotation(Quaternion.Euler(targetEuler)));
        }

        private IEnumerator RotateToOriginal()
        {
            yield return StartCoroutine(PerformRotation(originalRotation));
            ActiveRoutine = null;
        }

        private IEnumerator PerformRotation(Quaternion endRotation)
        {
            Quaternion startRotation = transform.rotation;
            float elapsed = 0;
            while (elapsed < RotationDuration)
            {
                elapsed += Time.deltaTime;
                float percent = Mathf.SmoothStep(0, 1, elapsed / RotationDuration);
                transform.rotation = Quaternion.Slerp(startRotation, endRotation, percent);
                yield return null;
            }
            transform.rotation = endRotation;
        }

        private void OnDrawGizmosSelected()
        {
            if (IsGizmos)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, GizmosRadius);
            }
        }
    }
}