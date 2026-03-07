using Sobia.SigmaboyProject;
using System.Collections;
using UnityEngine;

namespace Sobia.CrackingEggs
{
    public class MoonMovement : MonoBehaviour
    {
        [SerializeField] private Transform StartPosition;
        [SerializeField] private Transform TargetPosition;

        private float TotalTime;
        private Coroutine MoonRoutine;

        private void ResetPosition()
        {
            TotalTime = SessionManager.Instance.GameDuration;
            transform.position = StartPosition.position;
        }

        public void MoveMoon()
        {
            if (MoonRoutine != null)
            {
                StopCoroutine(MoonRoutine);
            }

            ResetPosition();
            MoonRoutine = StartCoroutine(LerpMoon());
        }

        private IEnumerator LerpMoon()
        {
            float elapsedTimer = 0f;

            while (elapsedTimer < TotalTime)
            {
                elapsedTimer += Time.deltaTime;
                float percentage = elapsedTimer / TotalTime;

                transform.position = Vector3.Lerp(StartPosition.position, TargetPosition.position, percentage);

                yield return null;
            }

            transform.position = TargetPosition.position;
        }
    }
}