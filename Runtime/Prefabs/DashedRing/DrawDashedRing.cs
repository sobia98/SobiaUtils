using UnityEngine;

namespace Sobia.Utils
{
    public class DrawCircle : MonoBehaviour
    {
        public LineRenderer lineRenderer;
        public int subdivisions = 10;
        public float radius = 2.0f;
        public float dashSpeed = 0.5f;

        private void Update()
        {
            float angleStep = 2f * Mathf.PI / subdivisions;
            lineRenderer.positionCount = subdivisions;

            //rotate dashes
            float offset = Time.time * dashSpeed;
            lineRenderer.material.mainTextureOffset = new Vector2(offset, 0);

            for (int i = 0; i < subdivisions; i++)
            {
                float angle = i * angleStep;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                lineRenderer.SetPosition(i, new Vector3(x, z, 0f));
            }
        }
    }
}