using Sobia.Utils;
using System.Collections;
using UnityEngine;

namespace Sobia.CrackingEggs
{
    public class SpawnerManager : MonoBehaviour
    {
        [SerializeField] private GameObject EggPrefab;
        [SerializeField] private EggTypeData ChickenEggData;
        [SerializeField] private Transform EggUICounterPosition;
        [SerializeField] private Transform ParentOfEggs;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float spawnRate = 1.0f;

        [Header("Spawn Area & Scale")]
        public PolygonCollider2D Spawnarea;

        public Transform PoolArea;
        public Transform DropPool;

        public Collider2D BloodPool;
        public float MinScale = 0.8f;
        public float MaxScale = 1.5f;

        [Header("Egg spawn from door")]
        [SerializeField] private Transform[] doors; // Drag your door transforms here

        [SerializeField] private float rollDuration = 1.5f; // How long the roll takes
        [SerializeField] private Vector3 startScale = new Vector3(0.3f, 0.3f, 1f);
        [SerializeField] private Vector3 finalScale = new Vector3(1.5f, 1.5f, 1.5f);

        private float distanceFromCamera = 10f;
        private float nextSpawnTime;
        private bool isSpawning = true;

        public static SpawnerManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            ValidateAssigned();
        }

        private void Update()
        {
            if (SessionManager.Instance.CurrentGameState != GameState.Start) return;

            if (Time.time >= nextSpawnTime)
            {
                if (!isSpawning) return;
                Transform randomDoor = doors[Random.Range(0, doors.Length)];
                randomDoor.GetComponent<Animator>().SetTrigger("Spawn");
                nextSpawnTime = Time.time + spawnRate;
            }
        }

        private void OnEnable()
        {
            SessionManager.OnGameEnd += StopSpawning;
            SessionManager.OnGameStart += StartSpawning;
        }

        private void OnDisable()
        {
            SessionManager.OnGameEnd -= StopSpawning;
            SessionManager.OnGameStart -= StartSpawning;
        }

        public void SpawnEgg(Transform randomDoor)
        {
            Vector2 targetPos = GetRandomPointInPolygon();

            GameObject newEgg = Instantiate(EggPrefab, randomDoor.position, Quaternion.identity, ParentOfEggs);
            Vector3 eggUICounterPosition = mainCamera.ScreenToWorldPoint(new Vector3(EggUICounterPosition.position.x, EggUICounterPosition.position.y, distanceFromCamera));
            newEgg.GetComponent<Egg>().EggTypeData = ChickenEggData;
            newEgg.GetComponent<Egg>().originalPosition = targetPos;
            newEgg.GetComponent<CapsuleCollider2D>().enabled = false;

            newEgg.transform.localScale = startScale;
            //newEgg.transform.position = targetPos;
            StartCoroutine(RollEggRoutine(newEgg.transform, targetPos));
        }

        private IEnumerator RollEggRoutine(Transform eggTransform, Vector3 destination)
        {
            float elapsed = 0f;
            Vector3 startPos = eggTransform.position;

            Bounds bounds = SpawnerManager.Instance.Spawnarea.bounds;
            float top = bounds.max.y;
            float bottom = bounds.min.y;

            while (elapsed < rollDuration)
            {
                elapsed += Time.deltaTime;
                float percent = elapsed / rollDuration;

                // Move the egg
                eggTransform.position = Vector3.Lerp(startPos, destination, percent);

                // --- NEW DEPTH SCALING LOGIC ---
                float yPos = eggTransform.position.y;
                // Calculate t based on Y position (0 at top, 1 at bottom)
                float t = Mathf.InverseLerp(top, bottom, yPos);

                // Calculate the scale based on the manager's global settings
                float currentScale = Mathf.Lerp(SpawnerManager.Instance.MinScale, SpawnerManager.Instance.MaxScale, t);
                eggTransform.localScale = new Vector3(currentScale, currentScale, 1f);
                // -------------------------------

                eggTransform.Rotate(0, 0, -500 * Time.deltaTime);

                yield return null;
            }

            eggTransform.position = destination;

            // Final scale check to ensure it's perfect at the destination
            float finalT = Mathf.InverseLerp(top, bottom, destination.y);
            float finalScaleValue = Mathf.Lerp(SpawnerManager.Instance.MinScale, SpawnerManager.Instance.MaxScale, finalT);
            eggTransform.localScale = new Vector3(finalScaleValue, finalScaleValue, 1f);

            eggTransform.rotation = Quaternion.identity;
            eggTransform.GetComponent<CapsuleCollider2D>().enabled = true;
        }

        private Vector2 GetRandomPointInPolygon()
        {
            Bounds bounds = Spawnarea.bounds;
            Vector2 point;

            do
            {
                float x = Random.Range(bounds.min.x, bounds.max.x);
                float y = Random.Range(bounds.min.y, bounds.max.y);
                point = new Vector2(x, y);
            }
            while (!Spawnarea.OverlapPoint(point) || BloodPool.OverlapPoint(point)); // If outside the shape, try again

            return point;
        }

        public void StopSpawning()
        {
            isSpawning = false;
        }

        public void StartSpawning()
        {
            isSpawning = true;
        }

        private void ValidateAssigned()
        {
            SobiaUtils.IsAssigned(EggPrefab, nameof(EggPrefab), gameObject);
            SobiaUtils.IsAssigned(ChickenEggData, nameof(ChickenEggData), gameObject);
            SobiaUtils.IsAssigned(EggUICounterPosition, nameof(EggUICounterPosition), gameObject);
            SobiaUtils.IsAssigned(ParentOfEggs, nameof(ParentOfEggs), gameObject);
            SobiaUtils.IsAssigned(mainCamera, nameof(mainCamera), gameObject);
        }
    }
}