using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConnectionRenderer : MonoBehaviour
{
    private const float LINE_WIDTH = 0.05f;

    private LineRenderer lineRenderer;
    private Transform startTransform;
    private Transform endTransform;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void Initialize(Transform startTransform, Transform endTransform)
    {
        this.startTransform = startTransform;
        this.endTransform = endTransform;

        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = false;
        lineRenderer.startWidth = LINE_WIDTH;
        lineRenderer.endWidth = LINE_WIDTH;

        var shader = Shader.Find("Sprites/Default");
        if (shader == null)
            shader = Shader.Find("Standard");

        if (lineRenderer.material == null)
            lineRenderer.material = new Material(shader);

        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;

        UpdateLinePositions();
    }

    private void LateUpdate()
    {
        if (startTransform == null || endTransform == null || lineRenderer == null)
            return;

        UpdateLinePositions();
    }

    private void UpdateLinePositions()
    {
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
