using Sobia.Utils;
using UnityEngine;

namespace Sobia.SigmaboyProject
{
    public class FireEffect : MonoBehaviour
    {
        private SpriteRenderer SpriteRenderer;
        private Color OriginalColor;
        private bool WasUpdated;
        private float TransitionSpeed = 0.75f;
        [SerializeField] private float GlowEffect = 1f;

        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            SobiaUtils.IsAssigned(SpriteRenderer, nameof(SpriteRenderer), gameObject);
            OriginalColor = SpriteRenderer.color;
        }

        public void Refresh() => WasUpdated = true;

        private void LateUpdate()
        {
            if (WasUpdated)
            {
                // Lower number (2f) makes the glow slower than before (10f)
                float pulse = Mathf.PingPong(Time.time * TransitionSpeed, GlowEffect);

                // Lerp between your base color and yellow
                SpriteRenderer.color = Color.Lerp(OriginalColor, Color.yellow, pulse);

                WasUpdated = false;
            }
            else
            {
                SpriteRenderer.color = OriginalColor;
                Destroy(this);
            }
        }
    }
}