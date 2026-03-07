using DG.Tweening;
using Sobia.Utils;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Sobia.CrackingEggs
{
    public enum HelperState
    {
        ROAMING,
        APPROACHING,
        CRACKING,
        NONE
    }

    public class FindNextEgg : MonoBehaviour
    {
        [SerializeField] private LayerMask eggLayer;
        [SerializeField] private float DamageRadius = 3f;
        [SerializeField] private float DetectionRadius = 3f;
        [SerializeField] private float DamageValue = 2f;
        [SerializeField] private float DamageCooldown = 0.5f;
        [SerializeField] private float EveryXUpdateTimer = 3f;
        [SerializeField] private float TargetOffsetX = 1.0f; // Adjust this distance to fit your sprite size
        [SerializeField] private float TargetOffsetY = 0.2f; // Adjust this distance to fit your sprite size
        [SerializeField] private LayerMask BloodPoolLayer;

        private Coroutine CurrentCoroutine;
        private HelperState CurrentGameState;
        private HelperState NextGameState;
        private float UpdateTimer = 0f;

        private Transform CurrentEggTarget;
        private WaitForSeconds DamageTickWait;

        private float LastX;
        private Animator Animator;
        private bool AttackingTarget = false;

        public void Start()
        {
            DamageTickWait = new WaitForSeconds(DamageCooldown);

            CurrentGameState = HelperState.NONE;
            NextGameState = HelperState.ROAMING;

            LastX = transform.position.x;
            Animator = GetComponent<Animator>();
        }

        public void Update()
        {
            CheckMovingDirection();
            UpdateTimer += Time.deltaTime;
            if (UpdateTimer >= EveryXUpdateTimer)
            {
                UpdateTimer = 0f;
                if (CurrentGameState != NextGameState)
                {
                    TransitionTo(NextGameState);
                }
            }
        }

        private void CheckMovingDirection()
        {
            float currentX = transform.position.x;

            if (currentX > LastX)
            {
                // Moving Right
                Animator.SetBool("HelperMovementRight", true);
                Animator.SetBool("HelperMovementLeft", false);
            }
            else if (currentX < LastX)
            {
                // Moving Left
                Animator.SetBool("HelperMovementRight", false);
                Animator.SetBool("HelperMovementLeft", true);
            }

            LastX = currentX; // Important: Update the record for the next frame
        }

        public void TransitionTo(HelperState newState)
        {
            transform.DOKill();
            CurrentGameState = newState;

            switch (newState)
            {
                case HelperState.APPROACHING:
                    HandleApproachingState();
                    break;

                case HelperState.ROAMING:
                    HandleRoamingState();
                    break;

                case HelperState.CRACKING:
                    HandleCrackingState();
                    break;

                case HelperState.NONE:
                    break;
            }
        }

        private void HandleRoamingState()
        {
            Vector2 spawnPoint = GetRandomPointInPolygon();

            float checkTimer = 0;
            float checkInterval = 1f;

            float time = UnityEngine.Random.Range(3f, 6f);
            transform.DOMove(spawnPoint, time).SetEase(Ease.InOutSine).OnUpdate(()
                =>
            {
                checkTimer += Time.deltaTime;

                if (checkTimer >= checkInterval)
                {
                    checkTimer = 0;
                    Collider2D hitCollider = Physics2D.OverlapCircle(transform.position, DetectionRadius, LayerMask.GetMask("Egg"));
                    if (hitCollider != null && hitCollider.transform.TryGetComponent<ChickenController>(out ChickenController chickenController) && !chickenController.enabled) // dont chase chickens
                    {
                        CurrentEggTarget = hitCollider.transform;
                        transform.DOKill(false);
                        NextGameState = HelperState.APPROACHING;
                    }
                }
            }).OnComplete(
                () =>
                {
                    //if no target was found
                    CurrentGameState = HelperState.NONE;
                    NextGameState = HelperState.ROAMING;
                });
        }

        private Vector2 GetRandomPointInPolygon()
        {
            Bounds bounds = SpawnerManager.Instance.Spawnarea.bounds;
            Vector2 point;

            //check for walls
            Vector2 currentPos2D = new Vector2(transform.position.x, transform.position.y);
            float moveDistance = 10f;
            RaycastHit2D hit;

            do
            {
                float x = Random.Range(bounds.min.x, bounds.max.x);
                float y = Random.Range(bounds.min.y, bounds.max.y);
                point = new Vector2(x, y);

                //check for walls
                Vector2 heading = point - currentPos2D;
                Vector2 direction = heading.normalized;
                hit = Physics2D.Raycast(transform.position, direction, moveDistance, BloodPoolLayer);
            }
            while (!SpawnerManager.Instance.Spawnarea.OverlapPoint(point) || SpawnerManager.Instance.BloodPool.OverlapPoint(point)); // If outside the shape, try again

            return point;
        }

        private void HandleApproachingState()
        {
            if (CurrentEggTarget == null) return;

            Vector3 targetWithOffset = new Vector3(
        CurrentEggTarget.position.x - TargetOffsetX,
        CurrentEggTarget.position.y + TargetOffsetY,
        CurrentEggTarget.position.z
    );

            float time = UnityEngine.Random.Range(1f, 2f);
            transform.DOMove(targetWithOffset, time).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                NextGameState = HelperState.CRACKING;
            });
        }

        private void HandleCrackingState()
        {
            if (CurrentCoroutine != null)
            {
                StopCoroutine(CurrentCoroutine);
                CurrentCoroutine = null;
            }

            AttackingTarget = true;
            CurrentCoroutine = StartCoroutine(DamageTickRoutine());
            Animator.SetBool("HelperMovementRight", true);
            Animator.SetBool("HelperMovementLeft", false);
        }

        private IEnumerator DamageTickRoutine()
        {
            while (AttackingTarget)
            {
                if (SessionManager.Instance.CurrentGameState == GameState.Start)
                {
                    Animator.SetTrigger("HelperAttack");
                }

                yield return DamageTickWait;
            }
        }

        //called in animation event
        private void ApplyDamageToZone()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, DamageRadius, eggLayer);

            //important, if egg is destoryed on arrive
            if (hitColliders.Length == 0)
            {
                if (CurrentCoroutine != null)
                {
                    StopCoroutine(CurrentCoroutine);
                    CurrentCoroutine = null;
                }

                NextGameState = HelperState.ROAMING;
                AttackingTarget = false;
            }
            else
            {
                //AudioManager.Instance.PlayEggDamage();

                foreach (var col in hitColliders)
                {
                    if (col.TryGetComponent<Egg>(out Egg egg) && col.TryGetComponent<ChickenController>(out ChickenController chickenController))
                    {
                        if (chickenController.enabled) continue; // dont attack chicken
                        egg.TakeDamage(DamageValue);
                    }
                }

                // avoid playing one more attack animation (all eggs are destoryed), no need to check again
                hitColliders = Physics2D.OverlapCircleAll(transform.position, DamageRadius, eggLayer);
                if (hitColliders.Length == 0)
                {
                    if (CurrentCoroutine != null)
                    {
                        StopCoroutine(CurrentCoroutine);
                        CurrentCoroutine = null;
                    }

                    NextGameState = HelperState.ROAMING;
                    AttackingTarget = false;
                }
            }
        }

        private void OnDrawGizmos()
        {
            // 1. Set the color of the circle
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, DamageRadius);
            // 2. Draw a wireframe circle (Outline only)
            // transform.position = center, 2f = radius
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, DetectionRadius);
        }
    }
}