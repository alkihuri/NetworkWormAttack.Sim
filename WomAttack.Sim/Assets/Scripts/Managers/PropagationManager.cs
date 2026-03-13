using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropagationManager : MonoBehaviour
{
    void Awake()
    {
        ServiceLocator.Register(this);
    }

    public IEnumerator ExecutePropagate()
    {
        var nodeManager = ServiceLocator.Get<NodeManager>();
        var allPCs = nodeManager.GetAllPCs();

        var transitions = new List<(NetworkNodeController node, NodeColor newColor)>();

        foreach (var pc in allPCs)
        {
            switch (pc.currentColor)
            {
                case NodeColor.Green:
                    transitions.Add((pc, NodeColor.Yellow));
                    break;
                case NodeColor.Yellow:
                    transitions.Add((pc, NodeColor.Red));
                    break;
                case NodeColor.Red:
                case NodeColor.Black:
                    // Red and black nodes are not affected by propagation
                    break;
            }
        }

        SimLog.Write($"Propagating worm to {transitions.Count} node(s)...");

        foreach (var (node, newColor) in transitions)
        {
            SimLog.Write($"{node.id}: {node.currentColor} → {newColor}");
            yield return StartCoroutine(node.ChangeColor(newColor));
        }

        SimLog.Write("Checking parent nodes for full compromise...");
        yield return StartCoroutine(CheckAllParentNodes(nodeManager));
    }

    private IEnumerator CheckAllParentNodes(NodeManager nodeManager)
    {
        var allNodes = nodeManager.GetAllNodes();

        for (int i = allNodes.Count - 1; i >= 0; i--)
        {
            var node = allNodes[i];
            if (node.type == NodeType.PC) continue;
            if (node.currentColor == NodeColor.Black) continue;
            if (node.children.Count == 0) continue;

            bool allChildrenBlack = node.children.All(child => child.currentColor == NodeColor.Black);

            if (allChildrenBlack)
            {
                SimLog.Write($"{node.id}: all children compromised — node taken over.");
                yield return StartCoroutine(node.ChangeColor(NodeColor.Black));
            }
        }
    }
}
