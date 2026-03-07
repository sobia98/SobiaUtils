using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sobia.SigmaboyProject
{
    public class SessionManager : MonoBehaviour
    {
        [Header("Heart Effects")]
        public AudioSource heartAudioSource; // Assign an AudioSource here

        public AudioClip heartPopSound;

        [Header("Spawn Settings")]
        public GameObject girlPrefab;

        // Note: spawn area size is controlled by DataManager's spawnAreaSizeX/Y upgrades.
        // Kept for inspector compatibility but no longer used.
        public Vector2 spawnAreaSize = new Vector2(30, 20); // (legacy)

        private List<GameObject> activeGirls = new List<GameObject>();
        private List<GirlNPC> rejectingGirls = new List<GirlNPC>(); // Track girls currently rejecting
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI stageText;
        public TextMeshProUGUI totalHeartText;
        public Button continueButton;
        public Button upgradeButton;
        public GameObject UpgradePanel;
        public TextMeshProUGUI totalBaddiesCounter;

        public float stageDuration = 2.0f; // default; gameplay tuning can be done in Inspector
        private int currentStage = 1;
        private bool canPlacePlayer = false; // Controls whether player can be placed

        private void Start()
        {
            continueButton.gameObject.SetActive(false);
            SpawnGirls();
            UpdateTotalLovesUI();
            StartNewSession();
        }

        public void Update()
        {
            totalBaddiesCounter.text = $"Total Baddies: {activeGirls.Count}";
        }

        public void UpdateTotalLovesUI()
        {
            totalHeartText.text = $"♥♥♥: {DataManager.Instance.totalHearts}";
            totalHeartText.fontSize = GameConstants.TOTAL_HEARTS_FONT_SIZE;
            totalHeartText.color = GameConstants.TOTAL_HEARTS_COLOR;
        }

        public void StartNewSession()
        {
            currentStage = 1;
            UpgradePanel.SetActive(false);
            continueButton.gameObject.SetActive(false);
            upgradeButton.gameObject.SetActive(false);
            SpawnGirls();
            StartCoroutine(GameFlowRoutine());
        }

        public void ToggleUpgrade()
        {
            UpgradePanel.SetActive(true);
        }

        private void EndSession()
        {
            DestroyPlayer();

            int heartsGained = 0;
            foreach (GameObject girlObj in activeGirls)
            {
                var girlnpc = girlObj.GetComponent<GirlNPC>();
                if (girlnpc.isCatched)
                {
                    heartsGained += girlnpc.heartValue * 2;
                }
                else
                {
                    heartsGained += girlnpc.heartValue;
                }
            }

            if (heartsGained >= GameConstants.FINISHED_GAME_HEARTS)
            {
                //ButtonsManager.Instance.finishGameBtn.interactable = true;
            }

            DataManager.Instance.totalHearts += heartsGained;
            //ButtonsManager.Instance.CheckOnButtons();
            UpdateTotalLovesUI();

            timerText.text = "Session Complete!";
            stageText.text = $"Gained {heartsGained} Hearts!";

            continueButton.gameObject.SetActive(true);
            upgradeButton.gameObject.SetActive(true);
        }

        public void RestartGame()
        {
            // Clean up any remaining girls from the previous end-state
            foreach (var g in activeGirls) if (g != null) Destroy(g);
            activeGirls.Clear();

            StartNewSession();
        }

        public void SpawnGirls()
        {
            foreach (var girl in activeGirls) if (girl != null) Destroy(girl);
            activeGirls.Clear();

            int amountToSpawn = (int)DataManager.Instance.spawn.GetValue();
            float[] weights = DataManager.Instance.GetSpawnWeights(); // [Ugly, Average, Baddie]

            // 1. Calculate exact counts based on weights
            int baddieCount = Mathf.RoundToInt(amountToSpawn * weights[2]);
            int averageCount = Mathf.RoundToInt(amountToSpawn * weights[1]);

            // 2. The remainder goes to 'Ugly' to ensure we hit the exact amountToSpawn
            int uglyCount = amountToSpawn - (baddieCount + averageCount);

            // 3. Create a list of assignments to shuffle (optional, but prevents spawning all baddies first)
            List<GirlNPC.GirlTier> spawnList = new List<GirlNPC.GirlTier>();
            for (int i = 0; i < baddieCount; i++) spawnList.Add(GirlNPC.GirlTier.Baddie);
            for (int i = 0; i < averageCount; i++) spawnList.Add(GirlNPC.GirlTier.Average);
            for (int i = 0; i < uglyCount; i++) spawnList.Add(GirlNPC.GirlTier.Ugly);

            // 4. Shuffle the list so they don't appear in order
            for (int i = 0; i < spawnList.Count; i++)
            {
                GirlNPC.GirlTier temp = spawnList[i];
                int randomIndex = Random.Range(i, spawnList.Count);
                spawnList[i] = spawnList[randomIndex];
                spawnList[randomIndex] = temp;
            }

            // 5. Spawn according to the list
            for (int i = 0; i < spawnList.Count; i++)
            {
                Vector3 randomPos = new Vector3(
                    Random.Range(-DataManager.Instance.spawnAreaSizeX.GetValue() / 2, DataManager.Instance.spawnAreaSizeX.GetValue() / 2),
                    Random.Range(-DataManager.Instance.spawnAreaSizeY.GetValue() / 2, DataManager.Instance.spawnAreaSizeY.GetValue() / 2),
                    0
                );

                GameObject newGirl = Instantiate(girlPrefab, randomPos, Quaternion.identity);
                GirlNPC girlScript = newGirl.GetComponent<GirlNPC>();

                girlScript.Initialize(spawnList[i]);
                activeGirls.Add(newGirl);
            }
        }

        private IEnumerator TransitionRoutine()
        {
            // 1. Disable player placement during shuffling
            canPlacePlayer = false;

            // 2. Destroy the player so they have to reposition/click again
            DestroyPlayer();

            timerText.text = "Shuffling...";
            yield return new WaitForSeconds(GameConstants.SHUFFLE_WAIT_SECONDS); // Short pause to see who survived

            // 3. Reposition the survivors
            foreach (GameObject girl in activeGirls)
            {
                girl.transform.position = new Vector3(
                    Random.Range(-DataManager.Instance.spawnAreaSizeX.GetValue() / 2, DataManager.Instance.spawnAreaSizeX.GetValue() / 2),
                    Random.Range(-DataManager.Instance.spawnAreaSizeY.GetValue() / 2, DataManager.Instance.spawnAreaSizeY.GetValue() / 2),
                    0
                );
            }

            // 4. Re-enable player placement when shuffling is done (will be enabled when timer starts)
        }

        // Public method to check if player can be placed
        public bool CanPlacePlayer()
        {
            return canPlacePlayer;
        }

        // Helper method to clean up the player
        private void DestroyPlayer()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) Destroy(player);
        }

        private IEnumerator GameFlowRoutine()
        {
            while (currentStage <= GameConstants.MAX_STAGES)
            {
                stageText.text = $"STAGE {currentStage}";

                canPlacePlayer = true;
                timerText.text = "Place your player";
                yield return new WaitUntil(() => GameObject.FindWithTag("Player") != null);

                float timeLeft = stageDuration;
                while (timeLeft > 0)
                {
                    timeLeft -= Time.deltaTime;
                    timerText.text = $"Time: {timeLeft:F1}s";
                    yield return null;
                }

                // Timer hit 0 - disable player placement
                canPlacePlayer = false;

                yield return StartCoroutine(FilterSurvivors());
                yield return StartCoroutine(WaitForRejectionsToComplete());

                if (currentStage < GameConstants.MAX_STAGES && activeGirls.Count > 0)
                {
                    currentStage++;
                    yield return StartCoroutine(TransitionRoutine());
                }
                else
                {
                    break;
                }
            }

            EndSession();
            canPlacePlayer = false;
        }

        private IEnumerator FilterSurvivors()
        {
            AuraDetection playerAura = FindFirstObjectByType<AuraDetection>();
            if (playerAura == null)
            {
                // No player was placed this stage:
                // treat it as a complete failure (0 matches).
                // Destroy all girls and clear the list so no hearts are awarded
                // and the session cannot progress to the next stage.
                for (int i = activeGirls.Count - 1; i >= 0; i--)
                {
                    if (activeGirls[i] != null)
                    {
                        Destroy(activeGirls[i]);
                    }
                }
                activeGirls.Clear();
                yield break;
            }

            float radius = playerAura.AuraTransform.localScale.x / 2f;
            Vector3 playerPos = playerAura.transform.position;

            rejectingGirls.Clear();

            // We go from the end of the list to the beginning so we can safely remove items
            for (int i = activeGirls.Count - 1; i >= 0; i--)
            {
                if (activeGirls[i] == null) continue;

                GirlNPC girl = activeGirls[i].GetComponent<GirlNPC>();
                float distance = Vector3.Distance(playerPos, girl.transform.position);

                if (distance <= radius) //TODO: fix for responsivess
                {
                    // SUCCESS
                    girl.GetComponent<SpriteRenderer>().color = Color.yellow;

                    float diceRoll = Random.value;
                    if (currentStage == GameConstants.MAX_STAGES && diceRoll <= girl.catchProbability)
                    {
                        girl.isCatched = true;
                        ShowHeartPopup(activeGirls[i], true);
                    }
                    else
                    {
                        ShowHeartPopup(activeGirls[i], false);
                    }

                    // --- DELAY HERE ---
                    // This makes the hearts pop one by one
                    float dynamicDelay = GameConstants.HEART_POP_DELAY_NUMERATOR / activeGirls.Count;
                    yield return new WaitForSeconds(Mathf.Clamp(dynamicDelay, GameConstants.HEART_POP_DELAY_MIN, GameConstants.HEART_POP_DELAY_MAX));
                }
                else
                {
                    // OUT OF RANGE
                    ShowPopupText(activeGirls[i], "NO!");
                    girl.Reject(playerPos);
                    rejectingGirls.Add(girl);
                    activeGirls.RemoveAt(i);
                    float dynamicDelay = GameConstants.HEART_POP_DELAY_NUMERATOR / activeGirls.Count;
                    yield return new WaitForSeconds(Mathf.Clamp(dynamicDelay, GameConstants.HEART_POP_DELAY_MIN, GameConstants.HEART_POP_DELAY_MAX));
                }
            }

            yield return null;
        }

        private IEnumerator WaitForRejectionsToComplete()
        {
            // Wait until all rejecting girls have finished their animation
            while (rejectingGirls.Count > 0)
            {
                // Remove any null entries (girls that have been destroyed)
                for (int i = rejectingGirls.Count - 1; i >= 0; i--)
                {
                    if (rejectingGirls[i] == null)
                    {
                        rejectingGirls.RemoveAt(i);
                    }
                }
                yield return null;
            }
        }

        // Show popup text above a GameObject
        private void ShowPopupText(GameObject target, string text)
        {
            if (target == null) return;

            GameObject popupObj = new GameObject("PopupText");
            // Initial position
            popupObj.transform.position = target.transform.position + Vector3.up * GameConstants.POPUP_WORLD_Y_OFFSET;

            TextMesh textMesh = popupObj.AddComponent<TextMesh>();
            textMesh.text = text;
            textMesh.fontSize = GameConstants.POPUP_FONT_SIZE;
            textMesh.color = Color.red;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;

            Renderer renderer = popupObj.GetComponent<Renderer>();
            if (renderer != null) renderer.sortingOrder = GameConstants.POPUP_SORTING_ORDER;

            // Pass the target.transform so the coroutine can track it
            StartCoroutine(AnimatePopupText(popupObj, target.transform));
        }

        private IEnumerator AnimatePopupText(GameObject popupObj, Transform targetTransform)
        {
            TextMesh textMesh = popupObj.GetComponent<TextMesh>();
            float duration = GameConstants.NO_TEXT_DURATION_SECONDS;
            float elapsed = 0f;

            // The vertical offset we want the text to maintain/increase over the target
            float verticalOffset = GameConstants.NO_TEXT_VERTICAL_OFFSET;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;

                if (targetTransform != null)
                {
                    // Calculate new position: Target's current pos + starting offset + rising animation
                    float risingEffect = progress * GameConstants.NO_TEXT_RISE_UNITS;
                    popupObj.transform.position = targetTransform.position + Vector3.up * (verticalOffset + risingEffect);
                }
                else
                {
                    // If the girl is destroyed/removed, just keep moving the text up from its last spot
                    popupObj.transform.position += Vector3.up * Time.deltaTime * GameConstants.NO_TEXT_RISE_UNITS;
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

        // Show heart popup above a GameObject
        private void ShowHeartPopup(GameObject target, bool doubleHearts)
        {
            if (target == null) return;

            if (heartAudioSource != null && heartPopSound != null)
            {
                // Random pitch makes the pop sounds feel like a shower of hearts
                heartAudioSource.pitch = Random.Range(GameConstants.HEART_POP_PITCH_MIN, GameConstants.HEART_POP_PITCH_MAX);
                heartAudioSource.PlayOneShot(heartPopSound, GameConstants.HEART_POP_SOUND_VOLUME);

                // Reset pitch for other sounds
                heartAudioSource.pitch = GameConstants.DEFAULT_AUDIO_PITCH;
            }

            // Create a new GameObject for the heart popup
            GameObject popupObj = new GameObject("HeartPopup");
            popupObj.transform.position = target.transform.position + Vector3.up * GameConstants.POPUP_WORLD_Y_OFFSET; // Position above the girl

            // Add TextMesh component
            TextMesh textMesh = popupObj.AddComponent<TextMesh>();
            if (doubleHearts)
            {
                textMesh.text = "♥♥"; // Double Heart character
            }
            else
            {
                textMesh.text = "♥"; // Heart character
            }
            textMesh.fontSize = GameConstants.POPUP_FONT_SIZE;
            textMesh.color = GameConstants.HEART_POPUP_COLOR;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;

            // Make sure it renders on top
            Renderer renderer = popupObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = GameConstants.POPUP_SORTING_ORDER;
            }

            // Start the popup animation coroutine
            StartCoroutine(AnimateHeartPopup(popupObj));
        }

        private IEnumerator AnimateHeartPopup(GameObject popupObj)
        {
            TextMesh textMesh = popupObj.GetComponent<TextMesh>();
            if (textMesh == null) yield break;

            Vector3 startPos = popupObj.transform.position;
            float duration = GameConstants.HEART_POPUP_DURATION_SECONDS;
            float elapsed = 0f;

            // 1. Randomize rotation so it looks more organic
            // Random.value > 0.5f makes it spin either left or right
            float rotationDirection = Random.value > 0.5f ? 1f : -1f;
            float rotationSpeed = Random.Range(100f, 250f) * rotationDirection;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / duration;

                // Move upward
                popupObj.transform.position = startPos + Vector3.up * (progress * GameConstants.HEART_POPUP_RISE_UNITS);

                // 2. APPLY ROTATION
                // We rotate on the Z axis (0, 0, Z) because TextMesh exists in a 3D world space
                popupObj.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

                // Fade out
                Color color = textMesh.color;
                color.a = 1f - progress;
                textMesh.color = color;

                yield return null;
            }

            // Clean up
            Destroy(popupObj);
        }

        // Draw the spawn box in the editor for easy tuning
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            if (DataManager.Instance != null)
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(DataManager.Instance.spawnAreaSizeX.GetValue(), DataManager.Instance.spawnAreaSizeY.GetValue(), 0));
        }
    }
}