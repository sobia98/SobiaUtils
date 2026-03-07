using Sobia.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Sobia.SigmaboyProject
{
    public class AuraDetection : MonoBehaviour
    {
        [Header("Aura Sounds")]
        public AudioSource AuraAudioSource;

        public AudioClip GirlEnterSound;
        public Transform AuraTransform;
        public LayerMask NpcLayer;

        private Pulse AuraPulseScript;

        private List<GirlWander> CurrentlyPanicked = new List<GirlWander>();

        private float Timer = 0.0f;
        [SerializeField] private float UpdateInterval = 1.0f;

        private void Start()
        {
            AuraPulseScript = GetComponentInChildren<Pulse>();

            SobiaUtils.IsAssigned(AuraPulseScript, nameof(AuraPulseScript), gameObject);
            SobiaUtils.IsAssigned(AuraTransform, nameof(AuraTransform), gameObject);
            SobiaUtils.IsAssigned(GirlEnterSound, nameof(GirlEnterSound), gameObject);
            SobiaUtils.IsAssigned(AuraAudioSource, nameof(AuraAudioSource), gameObject);

            float currentRadius = DataManager.Instance.radius.GetValue();
            AuraPulseScript.baseSize = currentRadius;
        }

        private void Update()
        {
            Timer += Time.deltaTime;

            if (Timer >= UpdateInterval)
            {
                DetectGirls();
                Timer = 0.0f;
            }
        }

        private void DetectGirls()
        {
            float dynamicRadius = AuraTransform.localScale.x / 2f; // very expensive
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, dynamicRadius, NpcLayer);

            List<GirlWander> girlsInRange = new List<GirlWander>();
            bool someoneNewEntered = false;

            foreach (var col in hitColliders)
            {
                if (col.TryGetComponent(out GirlWander wander))
                {
                    girlsInRange.Add(wander);
                    if (!CurrentlyPanicked.Contains(wander))
                    {
                        wander.SetPanic(true);
                        CurrentlyPanicked.Add(wander);
                        someoneNewEntered = true;
                    }

                    // Fire logic
                    if (!col.TryGetComponent(out FireEffect fire))
                        fire = col.gameObject.AddComponent<FireEffect>();
                    fire.Refresh();
                }
            }

            if (someoneNewEntered)
            {
                AuraAudioSource.PlayOneShot(GirlEnterSound);
            }

            // Remove and reset girls who are no longer in range
            for (int i = CurrentlyPanicked.Count - 1; i >= 0; i--)
            {
                if (!girlsInRange.Contains(CurrentlyPanicked[i]))
                {
                    CurrentlyPanicked[i].SetPanic(false);
                    CurrentlyPanicked.RemoveAt(i);
                }
            }
        }
    }
}