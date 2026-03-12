using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConnectionRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public void Initialize(Transform startTransform, Transform endTransform)
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;

        lineRenderer.SetPosition(0, startTransform.position);
        lineRenderer.SetPosition(1, endTransform.position);
    }

    public void SetColor(Color color)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }

    public LineRenderer GetLineRenderer()
    {
        return lineRenderer;
    }
}
