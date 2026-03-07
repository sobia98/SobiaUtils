using System.Collections;
using UnityEngine;

namespace Sobia.SigmaboyProject
{
    public class GirlNPC : MonoBehaviour
    {
        public enum GirlTier
        { Ugly, Normal, Average, Hot, Baddie }

        private GirlTier tier;
        public int heartValue;
        public float catchProbability;
        public bool isCatched = false;

        private SpriteRenderer sr;
        private bool isRejecting = false;
        private float actualMoveSpeed = 0f; // Store the actual move speed

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        // Call this from SessionManager when spawning
        public void Initialize(GirlTier newTier)
        {
            tier = newTier;

            // 1. Get Base Stats for this Tier
            // Using a helper function keeps the main logic clean
            TierStats stats = GetStatsForTier(tier);

            // 2. Apply Global Upgrades
            // Formula: Base Tier Value * Global Multiplier
            float globalHeartMult = DataManager.Instance.heartReturn.GetValue();
            heartValue = Mathf.RoundToInt(stats.baseHeartValue * globalHeartMult);

            // Formula: Base Tier Chance + Global Bonus (capped at 100%)
            float globalProbBonus = DataManager.Instance.probability.GetValue();
            catchProbability = Mathf.Min(stats.baseCatchChance + globalProbBonus, GameConstants.CATCH_PROBABILITY_CAP);

            // Formula: Base Tier Speed * Global Speed Multiplier
            // If "girlMovementSpeed" is Decremental (1.0 -> 0.5), they get slower.
            float globalSpeedMult = DataManager.Instance.girlMovementSpeed.GetValue();
            // Ensure speed doesn't hit 0 or go negative
            globalSpeedMult = Mathf.Max(GameConstants.MIN_GLOBAL_SPEED_MULTIPLIER, globalSpeedMult);

            float finalSpeed = stats.baseSpeedMultiplier * globalSpeedMult;
            actualMoveSpeed = finalSpeed;

            // 3. Visuals
            if (ColorUtility.TryParseHtmlString(stats.hexColor, out Color c))
            {
                sr.color = c;
            }

            // 4. Movement
            GirlWander wander = GetComponent<GirlWander>();
            if (wander != null)
            {
                float rangeX = DataManager.Instance.spawnAreaSizeX.GetValue();
                float rangeY = DataManager.Instance.spawnAreaSizeY.GetValue();
                wander.SetupWander(finalSpeed, new Vector2(rangeX, rangeY));
            }
        }

        private struct TierStats
        {
            public int baseHeartValue;
            public float baseCatchChance;
            public float baseSpeedMultiplier;
            public string hexColor;
        }

        private TierStats GetStatsForTier(GirlTier t)
        {
            switch (t)
            {
                case GirlTier.Ugly:
                    return new TierStats { baseHeartValue = GameConstants.UGLY_HEART, baseCatchChance = GameConstants.UGLY_CHANCE, baseSpeedMultiplier = GameConstants.UGLY_SPEED, hexColor = GameConstants.UGLY_COLOR };

                case GirlTier.Normal:
                    return new TierStats { baseHeartValue = GameConstants.NORMAL_HEART, baseCatchChance = GameConstants.NORMAL_CHANCE, baseSpeedMultiplier = GameConstants.NORMAL_SPEED, hexColor = GameConstants.NORMAL_COLOR };

                case GirlTier.Average:
                    return new TierStats { baseHeartValue = GameConstants.AVERAGE_HEART, baseCatchChance = GameConstants.AVERAGE_CHANCE, baseSpeedMultiplier = GameConstants.AVERAGE_SPEED, hexColor = GameConstants.AVERAGE_COLOR };

                case GirlTier.Hot:
                    return new TierStats { baseHeartValue = GameConstants.HOT_HEART, baseCatchChance = GameConstants.HOT_CHANCE, baseSpeedMultiplier = GameConstants.HOT_SPEED, hexColor = GameConstants.HOT_COLOR };

                case GirlTier.Baddie:
                    return new TierStats { baseHeartValue = GameConstants.BADDIE_HEART, baseCatchChance = GameConstants.BADDIE_CHANCE, baseSpeedMultiplier = GameConstants.BADDIE_SPEED, hexColor = GameConstants.BADDIE_COLOR };

                default:
                    return new TierStats { baseHeartValue = GameConstants.UGLY_HEART, baseCatchChance = GameConstants.CATCH_PROBABILITY_CAP, baseSpeedMultiplier = GameConstants.UGLY_SPEED, hexColor = GameConstants.NORMAL_COLOR };
            }
        }

        // Called when the girl says "no" - handles the reject sequence
        public void Reject(Vector3 playerPosition)
        {
            if (isRejecting) return; // Prevent multiple reject calls
            isRejecting = true;
            StartCoroutine(RejectSequence(playerPosition, showNoText: true));
        }

        // Called when the girl is eliminated (out of range)
        public void Eliminate(Vector3 playerPosition)
        {
            if (isRejecting) return; // Prevent multiple reject calls
            isRejecting = true;
            StartCoroutine(RejectSequence(playerPosition, showNoText: false));
        }

        private IEnumerator RejectSequence(Vector3 playerPosition, bool showNoText)
        {
            // Disable wandering behavior during rejection
            GirlWander wander = GetComponent<GirlWander>();
            if (wander != null)
            {
                wander.enabled = false;
            }

            // Disable FireEffect if present
            FireEffect fireEffect = GetComponent<FireEffect>();
            if (fireEffect != null)
            {
                fireEffect.enabled = false;
            }

            // Step 1: Show "NO!" popup (this is handled by SessionManager, but we wait a bit)
            if (showNoText)
            {
                yield return new WaitForSeconds(GameConstants.REJECT_NO_TEXT_PAUSE_SECONDS); // Brief pause to show the "NO!"
            }
            else
            {
                yield return new WaitForSeconds(GameConstants.ELIMINATE_NO_TEXT_PAUSE_SECONDS); // Short pause before running away
            }

            // Step 2: Run away from the player
            // Calculate direction away from player
            Vector3 direction = (transform.position - playerPosition).normalized;
            // If too close to player, pick a random direction
            if (direction.magnitude < GameConstants.DIRECTION_TOO_CLOSE_EPSILON)
            {
                float angle = Random.Range(0f, GameConstants.RUN_AWAY_ANGLE_MAX_DEGREES) * Mathf.Deg2Rad;
                direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
            }

            // Calculate a point far away in the run direction
            Vector3 runAwayPoint = transform.position + direction * GameConstants.RUN_AWAY_POINT_DISTANCE;

            // Run away for a longer duration
            float runDuration = GameConstants.RUN_DURATION_SECONDS;
            float elapsed = 0f;
            float runSpeed = actualMoveSpeed * GameConstants.RUN_SPEED_MULTIPLIER; // Run faster than normal (use actual move speed)

            while (elapsed < runDuration)
            {
                elapsed += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, runAwayPoint, runSpeed * Time.deltaTime);

                // If we've reached the point, extend it further
                if (Vector3.Distance(transform.position, runAwayPoint) < GameConstants.DIRECTION_TOO_CLOSE_EPSILON)
                {
                    runAwayPoint = transform.position + direction * GameConstants.RUN_AWAY_POINT_DISTANCE;
                }

                yield return null;
            }

            // Step 3: Fade out
            float fadeDuration = GameConstants.FADE_DURATION_SECONDS;
            float fadeElapsed = 0f;
            Color originalColor = sr.color;

            while (fadeElapsed < fadeDuration)
            {
                fadeElapsed += Time.deltaTime;
                float alpha = 1f - (fadeElapsed / fadeDuration);
                sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }

            // Step 4: Destroy the GameObject (SessionManager will handle list cleanup)
            Destroy(gameObject);
        }
    }
}