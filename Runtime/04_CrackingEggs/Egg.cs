using DG.Tweening;
using Sobia.Utils;
using System.Collections;
using UnityEngine;

namespace Sobia.CrackingEggs
{
    public class Egg : MonoBehaviour
    {
        public EggTypeData EggTypeData;
        public float CurrentHp;
        public Vector3 originalPosition;

        public Sprite Egg_crack_1,
            Egg_crack_2,
            Egg_crack_3,
            Egg_crack_4,
            Egg_crack_5,
            Egg_crack_6;

        private SpriteRenderer SpriteRenderer;
        private CapsuleCollider2D CapsuleCollider2D;

        [SerializeField] private Animator Animator;
        [SerializeField] private float ShakeTime = 0.1f;
        [SerializeField] private float shakeIntensity = 0.1f;

        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            CapsuleCollider2D = GetComponent<CapsuleCollider2D>();

            if (SpriteRenderer is null)
            {
                Debug.LogError("SpriteRenderer component missing on Egg object.");
            }

            if (CapsuleCollider2D is null)
            {
                Debug.LogError("CapsuleCollider2D component missing on Egg object.");
            }
        }

        private void Start()
        {
            if (EggTypeData != null)
            {
                CurrentHp = EggTypeData.BaseHp;
                SpriteRenderer.sprite = EggTypeData.EggSprite;
            }

            Animator = GetComponent<Animator>();
            if (Animator is null)
            {
                Debug.LogError("Animator component missing on Egg object.");
                return;
            }

            Animator.enabled = false;
        }

        private IEnumerator Shake(float shakeTime)
        {
            transform.localPosition = originalPosition + (Vector3)Random.insideUnitCircle * shakeIntensity;
            yield return new WaitForSeconds(shakeTime);
            transform.localPosition = originalPosition;
        }

        public void TakeDamage(float damage)
        {
            CurrentHp -= damage;
            UpdateEggSprite();

            if (CurrentHp <= 0)
            {
                GetComponent<CapsuleCollider2D>().enabled = false;
                //AudioManager.Instance.PlayEggCrack();
                StartCoroutine(CrackRoutine());
            }
            else
            {
                StartCoroutine(Shake(ShakeTime));
            }
        }

        private void UpdateEggSprite()
        {
            float healthPercentage = CurrentHp / EggTypeData.BaseHp;

            // Switch logic based on 6 stages (approx 16.6% per stage)
            if (healthPercentage <= 0.16f) { SpriteRenderer.sprite = Egg_crack_6; }
            else if (healthPercentage <= 0.33f) { SpriteRenderer.sprite = Egg_crack_5; }
            else if (healthPercentage <= 0.50f) { SpriteRenderer.sprite = Egg_crack_4; }
            else if (healthPercentage <= 0.66f) { SpriteRenderer.sprite = Egg_crack_3; }
            else if (healthPercentage <= 0.83f) { SpriteRenderer.sprite = Egg_crack_2; }
            else { SpriteRenderer.sprite = Egg_crack_1; }
        }

        private IEnumerator CrackRoutine()
        {
            //shake the egg
            yield return transform.DOShakePosition(3f, 0.08f, 10, 90).WaitForCompletion();
            Animator.enabled = true;
            Animator.SetTrigger("EggBreak");
        }

        private void OnHatchComplete()
        {
            GetComponent<ChickenController>().enabled = true;
        }

        public void SetEggVisibility(bool isVisible)
        {
            SpriteRenderer.enabled = isVisible;
            CapsuleCollider2D.enabled = isVisible;
        }
    }
}