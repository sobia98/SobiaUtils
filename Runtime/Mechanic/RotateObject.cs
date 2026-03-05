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

        private Coroutine ActiveRoutine;
        private bool IsActivated = false;
        private Quaternion originalRotation;

        private void Awake()
        {
            originalRotation = transform.rotation;
        }

        private void Start()
        {
            Activate();
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
            if (IsActivated) return;

            if (ActiveRoutine != null)
            {
                StopCoroutine(ActiveRoutine);
            }
            if (RotationAxisDirection == AxisDirection.Plus) // just go the opposite
            {
                ActiveRoutine = StartCoroutine(RotateTo(RotationDegree * GlobalConstants.NEGATIVE_ONE));
            }
            else
            {
                ActiveRoutine = StartCoroutine(RotateTo(RotationDegree * GlobalConstants.NEGATIVE_ONE));
            }
            IsActivated = false;
            ActiveRoutine = null;
        }

        private IEnumerator ActivationSequence()
        {
            IsActivated = true;

            // 1. Initial Delay
            if (StartDelay > 0) yield return new WaitForSeconds(StartDelay);

            float rotationDegree;
            if (RotationAxisDirection == AxisDirection.Plus)
            {
                rotationDegree = RotationDegree * GlobalConstants.POSITIVE_ONE;
            }
            else
            {
                rotationDegree = RotationDegree * GlobalConstants.NEGATIVE_ONE;
            }

            //First rotation
            yield return StartCoroutine(RotateTo(rotationDegree));

            //Activation Time
            yield return new WaitForSeconds(ObjectActivationTime);

            IsActivated = false;
            ActiveRoutine = null;
            Deactivate();
        }

        private IEnumerator RotateTo(float targetDirection)
        {
            Vector3 targetEuler = Vector3.zero;
            if (RotationAxis == Axis.X)
            {
                targetEuler = new Vector3(targetDirection, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            else if (RotationAxis == Axis.Y)
            {
                targetEuler = new Vector3(transform.eulerAngles.x, targetDirection, transform.eulerAngles.z);
            }
            else if (RotationAxis == Axis.Z)
            {
                targetEuler = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, targetDirection);
            }

            Quaternion startRotation = transform.rotation;
            Quaternion endRotation = Quaternion.Euler(targetEuler);

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