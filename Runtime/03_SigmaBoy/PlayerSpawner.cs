namespace Sobia.SigmaboyProject
{
    using UnityEngine;
    using UnityEngine.InputSystem; // Essential for New Input System

    public class PlayerSpawner : MonoBehaviour
    {
        [Header("Settings")]
        public GameObject playerPrefab;

        private GameObject currentInstance;
        private Camera mainCam;
        private SessionManager sessionManager;

        private void Awake()
        {
            mainCam = Camera.main;
            sessionManager = FindFirstObjectByType<SessionManager>();
        }

        private void Update()
        {
            // Check if player placement is allowed
            if (sessionManager != null && !sessionManager.CanPlacePlayer())
            {
                return; // Don't allow placement during timer end or shuffling
            }

            // New Input System way to check for left-click this frame
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                SpawnPlayerAtMouse();
            }
        }

        private void SpawnPlayerAtMouse()
        {
            // 1. Read mouse position as a Vector2 (Screen Space)
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

            // 2. Convert to World Space
            // Note: For 2D, Z should be the distance from the camera to the plane (usually 10)
            Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10f));

            worldPos.z = 0; // Lock to 2D plane

            // 3. Spawn or Move
            if (currentInstance == null)
            {
                currentInstance = Instantiate(playerPrefab, worldPos, Quaternion.identity);
            }
            else
            {
                currentInstance.transform.position = worldPos;
            }
        }
    }
}