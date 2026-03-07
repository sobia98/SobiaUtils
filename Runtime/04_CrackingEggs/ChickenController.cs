using DG.Tweening;
using Sobia.Utils;
using System.Collections;
using UnityEngine;

namespace Sobia.CrackingEggs
{
    public class ChickenController : MonoBehaviour
    {
        [SerializeField] private float EveryXUpdateTimer = 3f;
        [SerializeField] private SpriteRenderer BloodSpriteRenderer;
        [SerializeField] private float ChickenDeathFade = 2f;
        [SerializeField] private Transform BloodTransform;
        [SerializeField] private float DurationTimeToUI = 2f;
        [SerializeField] private float MoveToUIStrength = 1.5f;
        [SerializeField] private LayerMask BloodPoolLayer;

        private bool TargetPositionReached = true;
        private float UpdateTimer = 0f;
        private Animator Animator;
        private Vector2 CurrentTargetPosition;

        public bool Dead = false;

        private void Awake()
        {
            Animator = GetComponent<Animator>();

            if (Animator is null)
            {
                Debug.LogError("Animator component missing on Egg object.");
                return;
            }
        }

        private void OnEnable()
        {
            GetComponent<CapsuleCollider2D>().enabled = true;
        }

        private void Update()
        {
            UpdateTimer += Time.deltaTime;
            if (UpdateTimer >= EveryXUpdateTimer)
            {
                UpdateTimer = 0f;
                if (TargetPositionReached && !Dead)
                {
                    HandleRoamingState();
                }
            }
        }

        private void HandleRoamingState()
        {
            Bounds bounds = SpawnerManager.Instance.Spawnarea.bounds;
            TargetPositionReached = false;
            CurrentTargetPosition = GetRandomPointInPolygon();
            OnHatchComplete();
            float time = Random.Range(2f, 4f);

            float checkTimer = 0;
            float checkInterval = 0.05f;
            transform.DOMove(CurrentTargetPosition, time).SetEase(Ease.InOutSine).OnUpdate(() =>
            {
                checkTimer += Time.deltaTime;
                if (Dead) { transform.DOKill(); return; }

                if (checkTimer >= checkInterval)
                {
                    checkTimer = 0;
                    float yPos = transform.position.y;
                    float top = bounds.max.y;
                    float bottom = bounds.min.y;

                    // InverseLerp gives us a 0-1 value based on position
                    float t = Mathf.InverseLerp(top, bottom, yPos);

                    // 2. Lerp between your scale values
                    float currentScale = Mathf.Lerp(SpawnerManager.Instance.MinScale, SpawnerManager.Instance.MaxScale, t);
                    transform.localScale = new Vector3(currentScale, currentScale, 1f);
                }
            }).
                OnComplete(() =>
                {
                    TargetPositionReached = true;
                })
                ;
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
            while (hit.collider != null || !SpawnerManager.Instance.Spawnarea.OverlapPoint(point) || SpawnerManager.Instance.BloodPool.OverlapPoint(point)); // If outside the shape, try again

            return point;
        }

        public void OnHatchComplete()
        {
            Vector2 direction = (CurrentTargetPosition - (Vector2)transform.position);
            Animator.SetFloat("MoveX", direction.x);
            Animator.SetFloat("MoveY", direction.y);
        }

        private void OnBloodFlow()
        {
            //AudioManager.Instance.PlaySqueeze();
        }

        private IEnumerator OnChickenDead()
        {
            bool isLeft = transform.position.x > SpawnerManager.Instance.PoolArea.position.x;
            SpriteRenderer spriteRendererChicken = GetComponent<SpriteRenderer>();

            BloodSpriteRenderer.enabled = true;
            Animator.SetTrigger(isLeft ? "BloodStartLeft" : "BloodStart");

            // 2. Start Movement (Arc to UI/Mid point)
            Vector3 midPos = new Vector3(BloodTransform.position.x, BloodTransform.position.y + 5, 0f);
            yield return BloodTransform.DOJump(midPos, MoveToUIStrength, 1, DurationTimeToUI).SetEase(Ease.OutQuad).WaitForCompletion();

            // 4. End Movement (Arc to Blood Pool)
            Animator.SetTrigger(isLeft ? "BloodEndLeft" : "BloodEnd");
            Vector3 finalPos = SpawnerManager.Instance.BloodPool.transform.position;
            yield return BloodTransform.DOJump(finalPos, MoveToUIStrength, 1, DurationTimeToUI).SetEase(Ease.InBack).WaitForCompletion();
            BloodSpriteRenderer.enabled = false;

            // Rewards
            var rewardValue = GetComponent<Egg>().EggTypeData.RewardValue;
            PlayerController.Instance.CollectEgg(rewardValue);
            spriteRendererChicken.DOFade(0, ChickenDeathFade).OnComplete(() => Destroy(gameObject));
        }
    }
}