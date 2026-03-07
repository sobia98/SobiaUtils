using Sobia.Utils;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Sobia.CrackingEggs
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private LayerMask eggLayer;
        public int eggsCollected = 0;
        [SerializeField] private float DamageCooldown = 0.5f;
        private GameObject ParentOfTexts;

        private Animator Animator;

        public PlayerModel playerModel;

        private WaitForSeconds DamageTickWait;

        public static event Action<int> OnEggCollected;

        public int BloodPoolMax = 100;

        private Collider2D[] hitColliders;

        public static PlayerController Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Animator = GetComponentInChildren<Animator>();

            if (Animator is null)
            {
                Debug.LogError("Animator component not found in children of PlayerController.");
            }
        }

        private void Start()
        {
            DamageCooldown = playerModel.CrackSpeed.CurrentValue;
            DamageTickWait = new WaitForSeconds(DamageCooldown);
        }

        private void OnEnable()
        {
            StartCoroutine(DamageTickRoutine());
            ParentOfTexts = GameObject.Find("ParentOfTexts");
        }

        private IEnumerator DamageTickRoutine()
        {
            while (true)
            {
                if (SessionManager.Instance.CurrentGameState == GameState.Start)
                {
                    ApplyDamageToZone();
                }

                yield return DamageTickWait;
            }
        }

        private void ApplyDamageToZone()
        {
            float dynamicRadius = playerModel.CrackRadius.CurrentValue;
            hitColliders = Physics2D.OverlapCircleAll(transform.position, dynamicRadius, eggLayer);
            if (hitColliders.Length > 0)
            {
                Animator.SetTrigger("Attacking");
            }
        }

        public void TriggerDamage()
        {
            //AudioManager.Instance.PlayEggDamage();
            foreach (var col in hitColliders)
            {
                if (col.TryGetComponent<ChickenController>(out ChickenController chickenController))
                {
                    if (chickenController.enabled)
                    {
                        chickenController.Dead = true;
                        col.GetComponent<Animator>().SetTrigger("Death");
                        col.GetComponent<CapsuleCollider2D>().enabled = false;
                    }
                    else if (col.TryGetComponent<Egg>(out Egg egg))
                    {
                        egg.TakeDamage(playerModel.CrackDamage.CurrentValue);
                        //ShowPopupText(egg.gameObject, playerModel.CrackDamage.CurrentValue.ToString());
                    }
                }
            }
        }

        public void CollectEgg(int eggValue)
        {
            eggsCollected += eggValue;
            OnEggCollected?.Invoke(eggsCollected);

            if (eggsCollected > BloodPoolMax)
            {
                eggsCollected = 0;
            }
        }

        // Show popup text above a GameObject
        private void ShowPopupText(GameObject target, string text)
        {
            if (target == null) return;

            GameObject popupObj = new GameObject("PopupText");
            ParentOfTexts = GameObject.Find("ParentOfTexts");
            popupObj.transform.SetParent(ParentOfTexts.transform);
            // Initial position
            popupObj.transform.position = target.transform.position + Vector3.up * 1.5f;

            TextMeshPro textMesh = popupObj.AddComponent<TextMeshPro>();
            textMesh.text = text;
            textMesh.fontSize = 6;
            textMesh.color = Color.red;
            textMesh.alignment = TextAlignmentOptions.Center;

            Renderer renderer = popupObj.GetComponent<Renderer>();
            if (renderer != null) renderer.sortingOrder = 100;

            // Pass the target.transform so the coroutine can track it
            StartCoroutine(AnimatePopupText(popupObj, target.transform));
        }

        private IEnumerator AnimatePopupText(GameObject popupObj, Transform targetTransform)
        {
            TextMeshPro textMesh = popupObj.GetComponent<TextMeshPro>();
            float duration = 2f;
            float elapsed = 0f;
            var targetFixedPosition = popupObj.transform.position;

            float verticalOffset = -1f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;

                if (targetTransform != null)
                {
                    float risingEffect = progress * 2.0f;
                    popupObj.transform.position = targetFixedPosition + Vector3.up * (verticalOffset + risingEffect);
                }
                else
                {
                    popupObj.transform.position += Vector3.up * Time.deltaTime * 2.0f;
                }

                // Fade out
                if (textMesh != null)
                {
                    Color color = textMesh.color;
                    color.a = 1f - progress;
                    textMesh.color = color;
                }

                yield return null;
            }

            Destroy(popupObj);
        }
    }
}