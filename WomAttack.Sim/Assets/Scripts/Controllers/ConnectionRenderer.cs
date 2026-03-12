using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConnectionRenderer : MonoBehaviour
{
    private const float LINE_WIDTH = 0.05f;

    private LineRenderer lineRenderer;

    public void Initialize(Transform startTransform, Transform endTransform)
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = LINE_WIDTH;
        lineRenderer.endWidth = LINE_WIDTH;

        var shader = Shader.Find("Sprites/Default");
        if (shader == null)
            shader = Shader.Find("Standard");
        lineRenderer.material = new Material(shader);
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
