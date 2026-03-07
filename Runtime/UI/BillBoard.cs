using UnityEngine;

namespace Sobia.Utils
{
    public class Billboard : MonoBehaviour
    {
        private Transform MainCameraTransform;

        private void Start()
        {
            if (Camera.main != null)
            {
                MainCameraTransform = Camera.main.transform;
            }
            else
            {
                // Fallback for scenes without a Main Camera tag
                enabled = false;
            }
        }

        private void LateUpdate()
        {
            if (MainCameraTransform == null) return;

            // Force the Canvas to face the camera, ignoring vertical tilt (X-axis)
            // This gives a clean, readable name tag.
            Vector3 directionToCamera = transform.position - MainCameraTransform.position;
            Quaternion rotation = Quaternion.LookRotation(directionToCamera);

            // Apply the rotation, but keep X and Z rotation at 0 (flat billboard)
            transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y, 0f);
        }
    }
}