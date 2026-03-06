using UnityEngine;

namespace Sobia.Utils
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] private Color ActiveColor = Color.green;
        [SerializeField] private Color InactiveColor = Color.red;
        [SerializeField] private bool IsActive = false;

        [SerializeField] private Renderer ChildRenderer;

        private void Awake()
        {
            ChildRenderer = GetComponent<Renderer>();

            SobiaUtils.IsAssigned(ChildRenderer, nameof(ChildRenderer), gameObject);
        }

        private void Start()
        {
            ChildRenderer.material.color = InactiveColor;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsActive) return;

            if (other.TryGetComponent(out PInteractionController player))
            {
                player.SetCheckpoint(transform.position);
                ChildRenderer.material.color = ActiveColor;
            }
        }
    }
}