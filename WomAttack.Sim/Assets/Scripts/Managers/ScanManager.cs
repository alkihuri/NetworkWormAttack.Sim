using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScanManager : MonoBehaviour
{
    void Awake()
    {
        ServiceLocator.Register(this);
    }

    public IEnumerator ExecuteScan()
    {
        var nodeManager = ServiceLocator.Get<NodeManager>();

        // Step 1: Highlight all PCs with golden color
        var pcs = nodeManager.GetAllPCs();
        yield return StartCoroutine(HighlightAllPCs(pcs));

        // Step 2: Pulse animation through network lines
        yield return StartCoroutine(PulseNetworkLines(nodeManager));

        // Step 3: Find infected (Red) PC
        var infectedPC = nodeManager.FindInfectedPC();

        // Step 4: Restore PC colors and shake infected PC
        yield return StartCoroutine(RestorePCColors(pcs));

        if (infectedPC != null)
        {
            yield return StartCoroutine(ShakeInfectedPC(infectedPC));
        }
    }

    private IEnumerator HighlightAllPCs(List<NetworkNodeController> pcs)
    {
        Color goldColor = new Color(1f, 0.84f, 0f);

        foreach (var pc in pcs)
        {
            var renderer = pc.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material.DOColor(goldColor, 0.3f);
            }
        }

        yield return new WaitForSeconds(0.5f);
    }

    private IEnumerator PulseNetworkLines(NodeManager nodeManager)
    {
        var connections = nodeManager.GetAllConnections();
        var layers = nodeManager.GetLayers();

        Color pulseColor = Color.yellow;
        Color normalColor = Color.white;

        // Pulse layer by layer from root down
        for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
        {
            // Find connections that connect to this layer's nodes
            foreach (var connection in connections)
            {
                var lineRenderer = connection.GetLineRenderer();
                if (lineRenderer == null) continue;

                Vector3 endPos = lineRenderer.GetPosition(1);

                foreach (var node in layers[layerIndex])
                {
                    if (Vector3.Distance(endPos, node.transform.position) < 0.1f)
                    {
                        connection.SetColor(pulseColor);
                    }
                }
            }

            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(0.5f);

        // Reset line colors
        foreach (var connection in connections)
        {
            connection.SetColor(normalColor);
        }
    }

    private IEnumerator RestorePCColors(List<NetworkNodeController> pcs)
    {
        foreach (var pc in pcs)
        {
            Color originalColor = NetworkNodeController.GetUnityColor(pc.currentColor);
            var renderer = pc.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                renderer.material.DOColor(originalColor, 0.3f);
            }
        }

        yield return new WaitForSeconds(0.4f);
    }

    private IEnumerator ShakeInfectedPC(NetworkNodeController infectedPC)
    {
        infectedPC.transform.DOShakeScale(1f, 0.3f, 10, 90f);
        yield return new WaitForSeconds(1.2f);
    }
}
